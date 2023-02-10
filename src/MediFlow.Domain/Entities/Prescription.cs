using MediFlow.Domain.Common;

namespace MediFlow.Domain.Entities;

public class Prescription : BaseEntity
{
    public Guid? AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? Duration { get; set; }
    public string? Instructions { get; set; }
    public int Refills { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime PrescribedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
    // Navigation properties
    public virtual Appointment? Appointment { get; set; }
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
}