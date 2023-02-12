using MediFlow.Domain.Common;
using MediFlow.Domain.Enums;

namespace MediFlow.Domain.Entities;

public class QueueEntry : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? AppointmentId { get; set; }
    public int QueueNumber { get; set; }
    public QueueStatus Status { get; set; } = QueueStatus.Waiting;
    public int Position { get; set; }
    public int EstimatedWaitTime { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? CalledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? RemovedAt { get; set; }
    
    // Navigation properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual Appointment? Appointment { get; set; }
}