using System.Text.RegularExpressions;
using MediFlow.Domain.Common;

namespace MediFlow.Domain.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string Value { get; private set; }
    
    private PhoneNumber(string value)
    {
        Value = value;
    }
    
    public static Result<PhoneNumber> Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result<PhoneNumber>.Success(new PhoneNumber(string.Empty));
            
        var cleaned = Regex.Replace(phoneNumber, @"[^\d+]", "");
        
        if (cleaned.Length < 10 || cleaned.Length > 15)
            return Result<PhoneNumber>.Failure("Invalid phone number length");
            
        return Result<PhoneNumber>.Success(new PhoneNumber(cleaned));
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}