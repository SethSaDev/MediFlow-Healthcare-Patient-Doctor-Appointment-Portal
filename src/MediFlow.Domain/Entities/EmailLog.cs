using MediFlow.Domain.Common;

namespace MediFlow.Domain.Entities;

public class EmailLog : BaseEntity
{
    public string ToEmail { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? Status { get; set; }
    public string? SendgridMessageId { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? AppointmentId { get; set; }
    public DateTime SentAt { get; set; }
    
    // Navigation properties
    public virtual Appointment? Appointment { get; set; }
}