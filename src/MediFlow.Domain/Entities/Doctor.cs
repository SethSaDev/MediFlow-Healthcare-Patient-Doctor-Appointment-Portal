using MediFlow.Domain.Common;

namespace MediFlow.Domain.Entities;

public class Doctor : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public Guid? SpecialtyId { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Qualification { get; set; }
    public int YearsOfExperience { get; set; }
    public decimal ConsultationFee { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Specialty? Specialty { get; set; }
    public virtual ICollection<DoctorSpecialty> DoctorSpecialties { get; set; } = new List<DoctorSpecialty>();
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<WorkingHours> WorkingHours { get; set; } = new List<WorkingHours>();
    public virtual ICollection<TimeOffBlock> TimeOffBlocks { get; set; } = new List<TimeOffBlock>();
    public virtual ICollection<ConsultationNote> ConsultationNotes { get; set; } = new List<ConsultationNote>();
}