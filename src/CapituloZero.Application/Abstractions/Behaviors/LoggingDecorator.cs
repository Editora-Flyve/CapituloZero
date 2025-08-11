using CapituloZero.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Abstractions.Behaviors;

internal static class LoggingDecorator
{
    private static class Log
    {
        private static readonly Action<ILogger, string, Exception?> Processing =
            LoggerMessage.Define<string>(LogLevel.Information, new EventId(1000, nameof(Processing)), "Processing {Operation}");
        private static readonly Action<ILogger, string, Exception?> Completed =
            LoggerMessage.Define<string>(LogLevel.Information, new EventId(1001, nameof(Completed)), "Completed {Operation}");
        private static readonly Action<ILogger, string, Exception?> CompletedWithError =
            LoggerMessage.Define<string>(LogLevel.Error, new EventId(1002, nameof(CompletedWithError)), "Completed {Operation} with error");

        public static void ProcessingOp(ILogger logger, string operation) => Processing(logger, operation, null);
        public static void CompletedOp(ILogger logger, string operation) => Completed(logger, operation, null);
        public static void CompletedOpWithError(ILogger logger, string operation) => CompletedWithError(logger, operation, null);
    }
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            Log.ProcessingOp(logger, commandName);

            Result<TResponse> result = await innerHandler.Handle(command, cancellationToken).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                Log.CompletedOp(logger, commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    Log.CompletedOpWithError(logger, commandName);
                }
            }

            return result;
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            Log.ProcessingOp(logger, commandName);

            Result result = await innerHandler.Handle(command, cancellationToken).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                Log.CompletedOp(logger, commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    Log.CompletedOpWithError(logger, commandName);
                }
            }

            return result;
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            string queryName = typeof(TQuery).Name;

            Log.ProcessingOp(logger, queryName);

            Result<TResponse> result = await innerHandler.Handle(query, cancellationToken).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                Log.CompletedOp(logger, queryName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    Log.CompletedOpWithError(logger, queryName);
                }
            }

            return result;
        }
    }
}
