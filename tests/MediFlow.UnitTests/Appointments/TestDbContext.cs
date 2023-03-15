using MediFlow.Application.Common.Interfaces;
using MediFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediFlow.UnitTests.Appointments;

/// <summary>In-memory EF Core context used to exercise application handlers without a database.</summary>
public sealed class TestDbContext : DbContext, IApplicationDbContext
{
    public TestDbContext()
        : base(new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<ConsultationNote> ConsultationNotes => Set<ConsultationNote>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<MedicalFile> MedicalFiles => Set<MedicalFile>();
    public DbSet<QueueEntry> QueueEntries => Set<QueueEntry>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<TimeOffBlock> TimeOffBlocks => Set<TimeOffBlock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
        modelBuilder.Entity<DoctorSpecialty>().HasKey(ds => new { ds.DoctorId, ds.SpecialtyId });
        modelBuilder.Entity<Doctor>().HasOne(d => d.User).WithOne(u => u.Doctor).HasForeignKey<Doctor>(d => d.UserId);
        modelBuilder.Entity<Patient>().HasOne(p => p.User).WithOne(u => u.Patient).HasForeignKey<Patient>(p => p.UserId);

        foreach (var clrType in modelBuilder.Model.GetEntityTypes().Select(e => e.ClrType).ToList())
        {
            if (clrType.GetProperty("DomainEvents") is not null)
                modelBuilder.Entity(clrType).Ignore("DomainEvents");
        }
    }
}
