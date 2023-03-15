using MediFlow.Domain.ValueObjects;
using Xunit;

namespace MediFlow.UnitTests.Domain;

public class EmailAddressTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    public void Create_rejects_empty_or_malformed_input(string input)
    {
        var result = EmailAddress.Create(input);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Equality_ignores_case()
    {
        var a = EmailAddress.Create("Jane.Doe@Clinic.org").Value!;
        var b = EmailAddress.Create("jane.doe@clinic.org").Value!;

        Assert.Equal(a, b);
    }
}

public class PhoneNumberTests
{
    [Fact]
    public void Create_strips_formatting_characters()
    {
        var result = PhoneNumber.Create("+1 (929) 555-0142");

        Assert.True(result.IsSuccess);
        Assert.Equal("+19295550142", result.Value!.Value);
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("1234567890123456")]
    public void Create_rejects_numbers_outside_10_to_15_digits(string input)
    {
        Assert.True(PhoneNumber.Create(input).IsFailure);
    }

    [Fact]
    public void Create_treats_blank_as_an_optional_empty_number()
    {
        var result = PhoneNumber.Create("");

        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, result.Value!.Value);
    }
}
