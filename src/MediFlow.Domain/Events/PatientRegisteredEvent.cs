using MediatR;
using MediFlow.Domain.Entities;

namespace MediFlow.Domain.Events;

public class PatientRegisteredEvent : INotification
{
    public Patient Patient { get; }
    
    public PatientRegisteredEvent(Patient patient)
    {
        Patient = patient;
    }
}