using MediatR;
using MediFlow.Application.Common.Security;
using MediFlow.Domain.Entities;
using MediFlow.Domain.Enums;
using MediFlow.Domain.Events;

namespace MediFlow.Application.Features.Appointments.Commands;

[Authorize(Roles = "Patient")]
public class BookAppointmentCommand : IRequest<Guid>
{
    public Guid DoctorId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public string? Reason { get; set; }
    public string? Symptoms { get; set; }
}

public class BookAppointmentCommandHandler : IRequestHandler<BookAppointmentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    
    public BookAppointmentCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IEmailService emailService,
        ISmsService smsService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _emailService = emailService;
        _smsService = smsService;
    }
    
    public async Task<Guid> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        // Get patient
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken);
            
        if (patient == null)
            throw new NotFoundException(nameof(Patient), _currentUserService.UserId);
            
        // Check if doctor exists and is available
        var doctor = await _context.Doctors
            .Include(d => d.WorkingHours)
            .FirstOrDefaultAsync(d => d.Id == request.DoctorId && d.IsAvailable, cancellationToken);
            
        if (doctor == null)
            throw new NotFoundException(nameof(Doctor), request.DoctorId);
            
        // Check if time slot is available
        var endTime = request.StartTime.Add(TimeSpan.FromMinutes(30));
        var isSlotAvailable = !await _context.Appointments
            .AnyAsync(a => a.DoctorId == request.DoctorId &&
                          a.AppointmentDate == request.AppointmentDate &&
                          a.StartTime < endTime &&
                          a.EndTime > request.StartTime &&
                          a.Status != AppointmentStatus.Cancelled &&
                          a.Status != AppointmentStatus.Completed, cancellationToken);
                          
        if (!isSlotAvailable)
            throw new ConflictException("Time slot is not available");
            
        // Check if doctor is on leave
        var isOnLeave = await _context.TimeOffBlocks
            .AnyAsync(t => t.DoctorId == request.DoctorId &&
                          t.StartDate <= request.AppointmentDate &&
                          t.EndDate >= request.AppointmentDate, cancellationToken);
                          
        if (isOnLeave)
            throw new ConflictException("Doctor is on leave on this date");
            
        // Create appointment
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = patient.Id,
            DoctorId = request.DoctorId,
            AppointmentDate = request.AppointmentDate,
            StartTime = request.StartTime,
            EndTime = endTime,
            Status = AppointmentStatus.Scheduled,
            Reason = request.Reason,
            Symptoms = request.Symptoms,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };
        
        _context.Appointments.Add(appointment);
        
        // Add to audit
        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserService.UserId,
            Action = "Create",
            EntityType = nameof(Appointment),
            EntityId = appointment.Id,
            NewValues = System.Text.Json.JsonSerializer.Serialize(appointment),
            IpAddress = _currentUserService.IpAddress,
            Timestamp = DateTime.UtcNow
        });
        
        await _context.SaveChangesAsync(cancellationToken);
        
        // Send confirmation email
        var user = await _context.Users.FindAsync(patient.UserId);
        if (user != null)
        {
            await _emailService.SendEmailAsync(user.Email, "Appointment Confirmed", 
                $"Your appointment with Dr. {doctor.User.FirstName} {doctor.User.LastName} on {request.AppointmentDate:yyyy-MM-dd} at {request.StartTime} has been confirmed.");
                
            // Send SMS if phone number exists
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                await _smsService.SendSmsAsync(user.PhoneNumber, 
                    $"MediFlow: Appointment confirmed for {request.AppointmentDate:yyyy-MM-dd} at {request.StartTime}");
            }
        }
        
        // Raise domain event
        appointment.AddDomainEvent(new AppointmentCreatedEvent(appointment));
        
        return appointment.Id;
    }
}