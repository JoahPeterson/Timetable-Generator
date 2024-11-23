using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApp.DataModels.Models;

namespace TimetableApp.DataModels.DataAccess
{
    public class MongoLogData : ILogData
    {
        private readonly IMongoCollection<Log> _logs;
        private readonly ILogger<MongoLogData> _logger;

        public MongoLogData(IDbConnection db, ILogger<MongoLogData> logger)
        {
            _logs = db.LogCollection;
            _logger = logger;
        }
        public Task CreateAsync(Log log)
        {
            try
            {
                return _logs.InsertOneAsync(log);
            }
            catch(Exception ex)
            {
                // Not sure if this one is needed here ¯\_(ツ)_/¯
                _logger.LogError(ex, "Failed to create log");
                return null;
            }
            
        }

        public async Task<List<Log>> GetByUserIdAsync(string loggedInUserId)
        {
            try
            {
                var results = await _logs.FindAsync(l => l.LoggedInUserId == loggedInUserId);
                return results.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get log with a user Id of {UserId}", loggedInUserId);
                return null;
            }
           
        }

        public async Task<List<Log>> GetLogsAsync()
        {
            try
            {
                var results = await _logs.FindAsync(_ => true);

                return results.ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to get logs");
                return null;
            }
        }
    }
}
