using MediFlow.Domain.Common;

namespace MediFlow.Domain.ValueObjects;

public class EmailAddress : ValueObject
{
    public string Value { get; private set; }
    
    private EmailAddress(string value)
    {
        Value = value;
    }
    
    public static Result<EmailAddress> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<EmailAddress>.Failure("Email cannot be empty");
            
        if (!email.Contains("@") || !email.Contains("."))
            return Result<EmailAddress>.Failure("Invalid email format");
            
        return Result<EmailAddress>.Success(new EmailAddress(email));
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value.ToLowerInvariant();
    }
}