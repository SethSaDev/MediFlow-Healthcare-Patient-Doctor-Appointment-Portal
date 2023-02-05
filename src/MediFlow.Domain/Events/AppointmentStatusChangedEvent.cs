using MediatR;
using MediFlow.Domain.Entities;
using MediFlow.Domain.Enums;

namespace MediFlow.Domain.Events;

public class AppointmentStatusChangedEvent : INotification
{
    public Appointment Appointment { get; }
    public AppointmentStatus OldStatus { get; }
    public AppointmentStatus NewStatus { get; }
    
    public AppointmentStatusChangedEvent(Appointment appointment, AppointmentStatus oldStatus, AppointmentStatus newStatus)
    {
        Appointment = appointment;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}