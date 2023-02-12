using MediFlow.Domain.Common;
using MediFlow.Domain.Enums;

namespace MediFlow.Domain.Entities;

public class AppointmentStatusHistory : BaseEntity
{
    public Guid AppointmentId { get; set; }
    public AppointmentStatus? OldStatus { get; set; }
    public AppointmentStatus NewStatus { get; set; }
    public Guid ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Reason { get; set; }
    public string? IpAddress { get; set; }
    
    // Navigation properties
    public virtual Appointment Appointment { get; set; } = null!;
    public virtual User ChangedByUser { get; set; } = null!;
}