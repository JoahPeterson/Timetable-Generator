namespace Timetable.ExcelApi.Services
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;
    using TimetableApp.DataModels.DataAccess;
    using TimetableApp.DataModels.Models;

    public class TimetableLogger : ILogger
    {

        private readonly ILogData _logData;

        public TimetableLogger(ILogData logData)
        {
            _logData = logData;
        }

        /// <summary>
        /// Begins a logical operation scope. 
        /// Not implemented for this logger, returns null. (need to have to implement interface)
        /// </summary>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        /// <summary>
        /// Always returns true to ensure all log levels are processed.
        /// </summary>
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <summary>
        /// Logs a message by creating a Log and adding it to the database.
        /// </summary>
        /// <param name="logLevel">The log level of the message</param>
        /// <param name="exception">Optional exception associated with the log</param>
        /// <param name="state">The state object for the log message</param>
        /// <param name="exception">Optional exception associated with the log</param>
        /// <param name="formatter">Function to format the log message</param>
        public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
        {
            // Format the log message using the provided formatter
            var message = formatter(state, exception);

            // Create a new Log with details of the log entry
            var log = new Log
            {
                LogLevel = logLevel.ToString(),
                Message = message,
                CallStack = exception?.StackTrace,
                TimeStamp = DateTime.Now
            };

            // Asynchronously save the log to the database
            _ = _logData.CreateAsync(log);
        }


    }

    /// <summary>
    /// Provider for creating TimetableLogger instances.
    /// Implements ILoggerProvider for integration with .NET logging system.
    /// </summary>
    public class TimetableLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// Data access layer for storing logs.
        /// </summary>
        private readonly ILogData _logData;

        /// <summary>
        /// Initializes a new instance of the TimetableLoggerProvider.
        /// </summary>
        /// <param name="logData">Interface for log data persistence</param>
        public TimetableLoggerProvider(ILogData logData)
        {
            _logData = logData;
        }

        /// <summary>
        /// Creates a new TimetableLogger instance.
        /// </summary>
        public ILogger CreateLogger(string categoryName) =>
            new TimetableLogger(_logData);

        public void Dispose() { }
    }

    /// <summary>
    /// Extension methods for adding TimetableLogger to the logging configuration.
    /// </summary>
    public static class TimetableLoggerExtensions
    {
        /// <summary>
        /// Adds TimetableLogger to the logging builder.
        /// </summary>
        /// <param name="loggingBuilder">The logging builder to configure</param>
        /// <param name="logData">Data access layer for logs</param>
        public static ILoggingBuilder AddTimetableLogger(
            this ILoggingBuilder loggingBuilder,
            ILogData logData)
        {
            loggingBuilder.AddProvider(new TimetableLoggerProvider(logData));
            return loggingBuilder;
        }
    }
}
