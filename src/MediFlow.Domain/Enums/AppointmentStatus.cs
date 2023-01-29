namespace MediFlow.Domain.Enums;

public enum AppointmentStatus
{
    Scheduled = 1,
    CheckedIn = 2,
    InConsultation = 3,
    Completed = 4,
    Cancelled = 5,
    NoShow = 6,
    Rescheduled = 7
}