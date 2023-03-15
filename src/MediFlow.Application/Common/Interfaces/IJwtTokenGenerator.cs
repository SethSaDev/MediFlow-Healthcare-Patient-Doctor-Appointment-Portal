namespace MediFlow.Application.Common.Interfaces;

/// <summary>
/// Issues signed access tokens for authenticated users.
/// Implemented in the infrastructure layer so the application core stays free of JWT library details.
/// </summary>
public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, IEnumerable<string> roles);
}
