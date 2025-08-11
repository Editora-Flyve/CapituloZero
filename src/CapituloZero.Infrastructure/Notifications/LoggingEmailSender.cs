using CapituloZero.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace CapituloZero.Infrastructure.Notifications;

internal sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[Email] To: {To} | Subject: {Subject} | Body: {Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
