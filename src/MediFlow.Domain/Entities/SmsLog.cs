using MediFlow.Domain.Common;

namespace MediFlow.Domain.Entities;

public class SmsLog : BaseEntity
{
    public string ToPhone { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Status { get; set; }
    public string? TwilioSid { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? AppointmentId { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    
    // Navigation properties
    public virtual Appointment? Appointment { get; set; }
}