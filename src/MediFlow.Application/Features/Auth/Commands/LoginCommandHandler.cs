using MediatR;
using Microsoft.EntityFrameworkCore;
using MediFlow.Application.Common.Interfaces;
using MediFlow.Application.Common.Exceptions;
using MediFlow.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace MediFlow.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;
    
    public LoginCommandHandler(
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        ICurrentUserService currentUserService,
        IEmailService emailService)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _currentUserService = currentUserService;
        _emailService = emailService;
    }
    
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password");
            
        if (!user.IsActive)
            throw new UnauthorizedException("Account is deactivated");
            
        // Check for 2FA
        if (user.IsTwoFactorEnabled && string.IsNullOrEmpty(request.TwoFactorCode))
        {
            // Generate and send 2FA code
            var code = GenerateTwoFactorCode();
            user.TwoFactorSecret = HashTwoFactorCode(code);
            await _context.SaveChangesAsync(cancellationToken);
            
            await _emailService.SendEmailAsync(user.Email, "Your 2FA Code", 
                $"Your verification code is: {code}. This code expires in 5 minutes.");
                
            return new LoginResponse { RequiresTwoFactor = true };
        }
        
        // Verify 2FA if enabled
        if (user.IsTwoFactorEnabled && !string.IsNullOrEmpty(request.TwoFactorCode))
        {
            if (!VerifyTwoFactorCode(request.TwoFactorCode, user.TwoFactorSecret))
                throw new UnauthorizedException("Invalid 2FA code");
        }
        
        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        user.LastLoginIp = _currentUserService.IpAddress;
        
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        
        await _context.SaveChangesAsync(cancellationToken);
        
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var accessToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, roles);
        
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            }
        };
    }
    
    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
    
    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
    
    private string GenerateTwoFactorCode()
    {
        return new Random().Next(100000, 999999).ToString();
    }
    
    private string HashTwoFactorCode(string code)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(code);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
    
    private bool VerifyTwoFactorCode(string code, string? hash)
    {
        if (string.IsNullOrEmpty(hash)) return false;
        return HashTwoFactorCode(code) == hash;
    }
}