namespace MediFlow.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendEmailWithTemplateAsync(string to, string templateId, object templateData, CancellationToken cancellationToken = default);
}