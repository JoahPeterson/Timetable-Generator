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

        /// <summary>
        /// Instantiates a new instance of the Mongo WorkTask data class.
        /// </summary>
        /// <param name="db"></param>
        public MongoWorkTaskData(IDbConnection db)
        {
            _db = db;
            _tasks = db.WorkTaskCollection;
        }

        /// <summary>
        /// Creates a new WorkTask in the database.
        /// </summary>
        /// <param name="task">WorkTask object to be added to the db</param>
        /// <returns></returns>
        public async Task CreateAsync(WorkTask task)
        {
            await _tasks.InsertOneAsync(task);
        }

        /// <summary>
        /// Retrienve a WorkTask by its id.
        /// </summary>
        /// <param name="id">Id for the WorkTask to be retrieved</param>
        /// <returns>WorkTask object</returns>
        public async Task<WorkTask?> GetByIdAsync(string id)
        {
            var results = await _tasks.FindAsync(t => t.Id == id);

            return results.FirstOrDefault();
        }

        /// <summary>
        /// Retrieve all WorkTasks in the database.
        /// </summary>
        /// <returns>List of WorkTask Objects</returns>
        public async Task<List<WorkTask>> GetAsync()
        {
            var results = await _tasks.FindAsync(_ => true);

            return results.ToList();
        }

        /// <summary>
        /// Gets a list of WorkTasks for a user.
        /// </summary>
        /// <param name="createdById">The id value from the user in MongoDB</param>
        /// <returns>List of WorkTasks</returns>
        public async Task<List<WorkTask>> GetUsersWorkTasksAsync(string createdById)
        {
            var results = await _tasks.FindAsync(tt => tt.AuditInformation.CreatedById == createdById);

            return results.ToList();
        }

        /// <summary>
        /// Update a WorkTask in the database.
        /// </summary>
        /// <param name="task">The updated WorkTask Object</param>
        /// <returns>WorkTask</returns>
        public Task UpdateAsync(WorkTask task)
        {
            var filter = Builders<WorkTask>.Filter.Eq("Id", task.Id);
            return _tasks.ReplaceOneAsync(filter, task, new ReplaceOptions { IsUpsert = true });
        }
    }
}
