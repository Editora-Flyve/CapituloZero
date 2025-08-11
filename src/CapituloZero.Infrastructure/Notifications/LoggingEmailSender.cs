using CapituloZero.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace CapituloZero.Infrastructure.Notifications;

internal sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default)
    {
        Log.Email(logger, recipient, subject, body);
        return Task.CompletedTask;
    }

    private static class Log
    {
        private static readonly Action<ILogger, string, string, string, Exception?> EmailDef =
            LoggerMessage.Define<string, string, string>(LogLevel.Information, new EventId(3000, nameof(Email)), "[Email] To: {To} | Subject: {Subject} | Body: {Body}");

    public static void Email(ILogger logger, string to, string subject, string body) => EmailDef(logger, to, subject, body, null);
    }
}
