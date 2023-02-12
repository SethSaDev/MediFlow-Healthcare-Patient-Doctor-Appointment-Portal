using MediatR;
using MediFlow.Domain.Entities;

namespace MediFlow.Domain.Events;

public class AppointmentCreatedEvent : INotification
{
    public Appointment Appointment { get; }
    
    public AppointmentCreatedEvent(Appointment appointment)
    {
        Appointment = appointment;
    }
}