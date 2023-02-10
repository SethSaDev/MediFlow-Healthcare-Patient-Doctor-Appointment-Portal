using MediFlow.Domain.Common;
using MediFlow.Domain.Enums;

namespace MediFlow.Domain.Entities;

public class Appointment : BaseAuditableEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Reason { get; set; }
    public string? Symptoms { get; set; }
    public string? Notes { get; set; }
    public bool IsWalkIn { get; set; }
    public DateTime? CheckedInAt { get; set; }
    public DateTime? ConsultationStartedAt { get; set; }
    public DateTime? ConsultationEndedAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public Guid? RescheduledFrom { get; set; }
    
    // Navigation properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual ICollection<AppointmentStatusHistory> StatusHistory { get; set; } = new List<AppointmentStatusHistory>();
    public virtual ICollection<ConsultationNote> ConsultationNotes { get; set; } = new List<ConsultationNote>();
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public virtual QueueEntry? QueueEntry { get; set; }
}