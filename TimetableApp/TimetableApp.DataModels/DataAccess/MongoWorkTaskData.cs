using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApp.DataModels.Models;

namespace TimetableApp.DataModels.DataAccess
{
    /// <summary>
    /// Class that handles mongo DAL functions for WorkTask.
    /// </summary>
    public class MongoWorkTaskData : IWorkTaskData
    {
        private readonly IDbConnection _db;
        private readonly IMongoCollection<WorkTask> _tasks;
        private readonly ILogger<MongoWorkTaskData> _logger;

        /// <summary>
        /// Instantiates a new instance of the Mongo WorkTask data class.
        /// </summary>
        /// <param name="db"></param>
        public MongoWorkTaskData(IDbConnection db, ILogger<MongoWorkTaskData> logger)
        {
            _db = db;
            _tasks = db.WorkTaskCollection;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new WorkTask in the database.
        /// </summary>
        /// <param name="task">WorkTask object to be added to the db</param>
        /// <returns></returns>
        public async Task CreateAsync(WorkTask task)
        {
            try
            {
                await _tasks.InsertOneAsync(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create work task for user {UserId}", task.AuditInformation.CreatedById);
            }
            
        }

        /// <summary>
        /// Retrienve a WorkTask by its id.
        /// </summary>
        /// <param name="id">Id for the WorkTask to be retrieved</param>
        /// <returns>WorkTask object</returns>
        public async Task<WorkTask?> GetByIdAsync(string id)
        {
            try
            {
                var results = await _tasks.FindAsync(t => t.Id == id);

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get work task with an Id of {WorkTaskId}", id);
                return null;
            }
            
        }

        /// <summary>
        /// Retrieve all WorkTasks in the database.
        /// </summary>
        /// <returns>List of WorkTask Objects</returns>
        public async Task<List<WorkTask>> GetAsync()
        {
            try
            {
                var results = await _tasks.FindAsync(_ => true);

                return results.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get work tasks");
                return null;
            }
            
        }

        /// <summary>
        /// Gets a list of WorkTasks for a user, excluding archived tasks.
        /// </summary>
        /// <param name="createdById">The id value from the user in MongoDB</param>
        /// <returns>List of WorkTasks</returns>
        public async Task<List<WorkTask>> GetUsersWorkTasksAsync(string createdById)
        {
            try
            {
                // Add a filter to only include non-archived WorkTasks
                var results = await _tasks.FindAsync(tt =>
                    tt.AuditInformation.CreatedById == createdById &&
                    tt.AuditInformation.IsArchived == false);

                return results.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user's work task with an Id of {CreatedById}", createdById);
                return null;
            }
           
        }

        /// <summary>
        /// Update a WorkTask in the database.
        /// </summary>
        /// <param name="task">The updated WorkTask Object</param>
        /// <returns>WorkTask</returns>
        public Task UpdateAsync(WorkTask task)
        {
            try
            {
                var filter = Builders<WorkTask>.Filter.Eq("Id", task.Id);
                return _tasks.ReplaceOneAsync(filter, task, new ReplaceOptions { IsUpsert = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update work task with an Id of {WorkTaskId}", task.Id);
                return null;
            }
           
        }
    }
}
