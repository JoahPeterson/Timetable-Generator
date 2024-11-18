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

        public MongoLogData(IDbConnection db)
        {
            _logs = db.LogCollection;
        }
        public Task CreateAsync(Log log)
        {
            return _logs.InsertOneAsync(log);
        }

        public async Task<List<Log>> GetByUserIdAsync(string loggedInUserId)
        {
            var results = await _logs.FindAsync(l => l.LoggedInUserId == loggedInUserId);
            return results.ToList();
        }

        public async Task<List<Log>> GetLogsAsync()
        {
            var results = await _logs.FindAsync(_ => true);

            return results.ToList();    
        }
    }
}
