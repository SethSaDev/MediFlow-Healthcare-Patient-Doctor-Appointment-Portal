using MediFlow.Domain.Common;

namespace MediFlow.Domain.Entities;

public class ConsultationNote : BaseAuditableEntity
{
    public Guid AppointmentId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid PatientId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string? Diagnosis { get; set; }
    public string? Recommendations { get; set; }
    public DateOnly? FollowUpDate { get; set; }
    public bool IsConfidential { get; set; }
    
    // Navigation properties
    public virtual Appointment Appointment { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual Patient Patient { get; set; } = null!;
}