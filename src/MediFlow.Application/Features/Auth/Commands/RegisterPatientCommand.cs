using MediatR;
using MediFlow.Application.Common.Interfaces;

namespace MediFlow.Application.Features.Auth.Commands;

public class RegisterPatientCommand : IRequest<Guid>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
}

public class RegisterPatientCommandHandler : IRequestHandler<RegisterPatientCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;
    
    public RegisterPatientCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IEmailService emailService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _emailService = emailService;
    }
    
    public async Task<Guid> Handle(RegisterPatientCommand request, CancellationToken cancellationToken)
    {
        // Check if email exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            
        if (existingUser != null)
            throw new ConflictException("Email already registered");
            
        // Create user
        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };
        
        _context.Users.Add(user);
        
        // Assign Patient role
        var patientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Patient", cancellationToken);
        if (patientRole != null)
        {
            _context.Set<UserRole>().Add(new UserRole
            {
                UserId = user.Id,
                RoleId = patientRole.Id,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = _currentUserService.UserId
            });
        }
        
        // Create patient record
        var patient = new Domain.Entities.Patient
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            MedicalRecordNumber = GenerateMedicalRecordNumber(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };
        
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Send welcome email
        await _emailService.SendEmailAsync(request.Email, "Welcome to MediFlow", 
            $"Welcome {request.FirstName}! Your account has been created successfully.");
            
        return patient.Id;
    }
    
    private string GenerateMedicalRecordNumber()
    {
        return $"MRN{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }
}