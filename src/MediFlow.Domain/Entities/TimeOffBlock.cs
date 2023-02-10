using MediFlow.Domain.Common;

namespace MediFlow.Domain.Entities;

public class TimeOffBlock : BaseEntity
{
    public Guid DoctorId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string? Reason { get; set; }
    public bool IsApproved { get; set; } = true;
    
    // Navigation properties
    public virtual Doctor Doctor { get; set; } = null!;
}