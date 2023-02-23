namespace MediFlow.Application.Common.Security;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AuthorizeAttribute : Attribute
{
    public string? Roles { get; set; }
    public string? Policy { get; set; }
}