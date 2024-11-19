namespace Timetable.BlazorUI.Services
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Timetable.BlazorUI.Services;
    using TimetableApp.DataModels.DataAccess;
    using TimetableApp.DataModels.Models;

    public class TimetableLogger : ILogger
    {
        private readonly ILogData _logData;
        private readonly IUserData _userData;
        private readonly CurrentUserService _currentUserService;

        public TimetableLogger(ILogData logData, IUserData userData, CurrentUserService currentUserService)
        {
            _logData = logData;
            _userData = userData;
            _currentUserService = currentUserService;
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
        public async void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
        {
            // Format the log message using the provided formatter
            var message = formatter(state, exception);

            // Get logged-in user 
            var userId = _currentUserService.GetUserId();
            var mongoUser = userId != null ? await _userData.GetUserFromAuthenticationAsync(userId) : null;

            // Create a new Log with details of the log entry
            var log = new Log
            {
                LoggedInUserId = mongoUser?.Id ?? string.Empty,
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
        private readonly ILogData _logData;
        private readonly IUserData _userData;
        private readonly CurrentUserService _currentUserService;

        public TimetableLoggerProvider(ILogData logData, IUserData userData, CurrentUserService currentUserService)
        {
            _logData = logData;
            _userData = userData;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Creates a new TimetableLogger instance.
        /// </summary>
        public ILogger CreateLogger(string categoryName) =>
            new TimetableLogger(_logData, _userData, _currentUserService);

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
        /// <param name="userData">Data access layer for users</param>
        /// <param name="currentUserService">Current user service for getting user details</param>
        public static ILoggingBuilder AddTimetableLogger(
            this ILoggingBuilder loggingBuilder,
            ILogData logData,
            IUserData userData,
            CurrentUserService currentUserService)
        {
            loggingBuilder.AddProvider(new TimetableLoggerProvider(logData, userData, currentUserService));
            return loggingBuilder;
        }
    }
}

