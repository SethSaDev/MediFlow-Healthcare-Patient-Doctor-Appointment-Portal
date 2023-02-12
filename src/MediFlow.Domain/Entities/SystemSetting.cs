using MediFlow.Domain.Common;

namespace MediFlow.Domain.Entities;

public class SystemSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? DataType { get; set; }
    public string? Description { get; set; }
}