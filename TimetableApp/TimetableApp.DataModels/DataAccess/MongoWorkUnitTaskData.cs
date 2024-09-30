using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess
{

        /// <summary>
        /// Class that handles mongo DAL functions for WorkUnitTask.
        /// </summary>
        public class MongoWorkUnitTaskData : IWorkUnitTaskData
        {
            private readonly IDbConnection _db;
            private readonly IMongoCollection<WorkUnitTask> _tasks;

            /// <summary>
            /// Instantiates a new instance of the Mongo WorkUnitTask data class.
            /// </summary>
            /// <param name="db"></param>
            public MongoWorkUnitTaskData(IDbConnection db)
            {
                _db = db;
                _tasks = db.WorkUnitTaskCollection;
            }

            /// <summary>
            /// Creates a new WorkUnitTask in the database.
            /// </summary>
            /// <param name="task">WorkUnitTask object to be added to the db</param>
            /// <returns></returns>
            public async Task CreateAsync(WorkUnitTask task)
            {
                await _tasks.InsertOneAsync(task);
            }

            /// <summary>
            /// Retrienve a WorkUnitTask by its id.
            /// </summary>
            /// <param name="id">Id for the WorkUnitTask to be retrieved</param>
            /// <returns>WorkUnitTask object</returns>
            public async Task<WorkUnitTask?> GetByIdAsync(string id)
            {
                var results = await _tasks.FindAsync(t => t.Id == id);

                return results.FirstOrDefault();
            }

            /// <summary>
            /// Retrieve all WorkUnitTasks in the database.
            /// </summary>
            /// <returns>List of WorkUnitTask Objects</returns>
            public async Task<List<WorkUnitTask>> GetAsync()
            {
                var results = await _tasks.FindAsync(_ => true);

                return results.ToList();
            }

            /// <summary>
            /// Gets a list of WorkUnitTasks for a user.
            /// </summary>
            /// <param name="createdById">The id value from the user in MongoDB</param>
            /// <returns>List of WorkUnitTasks</returns>
            public async Task<List<WorkUnitTask>> GetUsersWorkUnitTasksAsync(string createdById)
            {
                var results = await _tasks.FindAsync(tt => tt.AuditInformation.CreatedById == createdById);

                return results.ToList();
            }

            /// <summary>
            /// Update a WorkUnitTask in the database.
            /// </summary>
            /// <param name="task">The updated WorkUnitTask Object</param>
            /// <returns>WorkUnitTask</returns>
            public Task UpdateAsync(WorkUnitTask task)
            {
                var filter = Builders<WorkUnitTask>.Filter.Eq("Id", task.Id);
                return _tasks.ReplaceOneAsync(filter, task, new ReplaceOptions { IsUpsert = true });
            }
        }

