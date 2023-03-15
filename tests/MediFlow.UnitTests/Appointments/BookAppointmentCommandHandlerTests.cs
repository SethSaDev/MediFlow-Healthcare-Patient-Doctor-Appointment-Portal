using MediFlow.Application.Common.Exceptions;
using MediFlow.Application.Common.Interfaces;
using MediFlow.Application.Features.Appointments.Commands;
using MediFlow.Domain.Entities;
using MediFlow.Domain.Enums;
using Moq;
using Xunit;

namespace MediFlow.UnitTests.Appointments;

public class BookAppointmentCommandHandlerTests
{
    private static readonly DateOnly Day = new(2026, 3, 10);

    private readonly TestDbContext _db = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IEmailService> _email = new();
    private readonly Mock<ISmsService> _sms = new();
    private readonly Guid _doctorId;

    public BookAppointmentCommandHandlerTests()
    {
        var patientUser = new User { Id = Guid.NewGuid(), Email = "pat@example.org", FirstName = "Pat", LastName = "Lee" };
        var doctorUser = new User { Id = Guid.NewGuid(), Email = "dr@example.org", FirstName = "Ana", LastName = "Ruiz" };
        var doctor = new Doctor { Id = Guid.NewGuid(), UserId = doctorUser.Id, LicenseNumber = "NY-123", IsAvailable = true };

        _db.Users.AddRange(patientUser, doctorUser);
        _db.Patients.Add(new Patient { Id = Guid.NewGuid(), UserId = patientUser.Id });
        _db.Doctors.Add(doctor);
        _db.SaveChanges();
        _db.ChangeTracker.Clear();

        _doctorId = doctor.Id;
        _currentUser.SetupGet(c => c.UserId).Returns(patientUser.Id);
    }

    private BookAppointmentCommandHandler CreateHandler() =>
        new(_db, _currentUser.Object, _email.Object, _sms.Object);

    private BookAppointmentCommand Booking(int hour, int minute = 0) => new()
    {
        DoctorId = _doctorId,
        AppointmentDate = Day,
        StartTime = new TimeOnly(hour, minute),
        Reason = "Follow-up"
    };

    [Fact]
    public async Task Books_a_30_minute_slot_and_sends_a_confirmation_email()
    {
        var id = await CreateHandler().Handle(Booking(9), CancellationToken.None);

        var appointment = _db.Appointments.Single(a => a.Id == id);
        Assert.Equal(new TimeOnly(9, 30), appointment.EndTime);
        Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
        Assert.Single(_db.AuditLogs);
        _email.Verify(e => e.SendEmailAsync("pat@example.org", "Appointment Confirmed",
            It.Is<string>(body => body.Contains("Dr. Ana Ruiz")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Rejects_a_slot_that_overlaps_an_existing_appointment()
    {
        await CreateHandler().Handle(Booking(9), CancellationToken.None);

        await Assert.ThrowsAsync<ConflictException>(() =>
            CreateHandler().Handle(Booking(9, 15), CancellationToken.None));
    }

    [Fact]
    public async Task Allows_a_slot_that_starts_when_the_previous_one_ends()
    {
        await CreateHandler().Handle(Booking(9), CancellationToken.None);

        var second = await CreateHandler().Handle(Booking(9, 30), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, second);
    }

    [Fact]
    public async Task Rejects_booking_while_the_doctor_is_on_leave()
    {
        _db.TimeOffBlocks.Add(new TimeOffBlock { Id = Guid.NewGuid(), DoctorId = _doctorId, StartDate = Day.AddDays(-1), EndDate = Day.AddDays(1) });
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<ConflictException>(() =>
            CreateHandler().Handle(Booking(11), CancellationToken.None));
    }

    [Fact]
    public async Task Throws_not_found_when_the_doctor_is_unavailable()
    {
        var doctor = _db.Doctors.Single();
        doctor.IsAvailable = false;
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateHandler().Handle(Booking(9), CancellationToken.None));
    }
}
