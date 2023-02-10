using MediFlow.Domain.Common;

namespace MediFlow.Domain.Entities;

public class WorkingHours : BaseEntity
{
    public Guid DoctorId { get; set; }
    public int DayOfWeek { get; set; } // 0 = Sunday, 6 = Saturday
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SlotDuration { get; set; } = 30; // minutes
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual Doctor Doctor { get; set; } = null!;
}