namespace MediFlow.Domain.Entities;

public class DoctorSpecialty
{
    public Guid DoctorId { get; set; }
    public Guid SpecialtyId { get; set; }
    
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual Specialty Specialty { get; set; } = null!;
}