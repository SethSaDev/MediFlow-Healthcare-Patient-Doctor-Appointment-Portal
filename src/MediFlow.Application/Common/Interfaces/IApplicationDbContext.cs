using Microsoft.EntityFrameworkCore;
using MediFlow.Domain.Entities;

namespace MediFlow.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Patient> Patients { get; }
    DbSet<Doctor> Doctors { get; }
    DbSet<Specialty> Specialties { get; }
    DbSet<Appointment> Appointments { get; }
    DbSet<ConsultationNote> ConsultationNotes { get; }
    DbSet<Prescription> Prescriptions { get; }
    DbSet<MedicalFile> MedicalFiles { get; }
    DbSet<QueueEntry> QueueEntries { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<SystemSetting> SystemSettings { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}