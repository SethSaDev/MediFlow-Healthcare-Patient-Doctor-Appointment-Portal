namespace MediFlow.Domain.Enums;

public enum QueueStatus
{
    Waiting = 1,
    Called = 2,
    InConsultation = 3,
    Completed = 4,
    Cancelled = 5,
    NoShow = 6
}