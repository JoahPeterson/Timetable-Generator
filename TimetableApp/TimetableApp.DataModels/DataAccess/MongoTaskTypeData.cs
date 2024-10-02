namespace TimetableApp.DataModels.DataAccess;

/// <summary>
/// Class the handles the Mongo DAL functions for TaskType
/// </summary>
public class MongoTaskTypeData : ITaskTypeData
{
    private readonly IDbConnection _db;
    private readonly IMongoCollection<TaskType> _taskTypes;
    private readonly IUserData _userData;

    /// <summary>
    /// Intantiates a new instance of the Mongo TaskType data class.
    /// </summary>
    /// <param name="db">Instance of a Mongo DB Connection</param>
    public MongoTaskTypeData(IDbConnection db, IUserData userData)
    {
        _db = db;
        _taskTypes = db.TaskTypeCollection;
        _userData = userData;
    }

    /// <summary>
    /// Create a TaskType in the database and upate the user's TaskTypes list.
    /// Created in a transaction so we can rollback if the user update fails.
    /// </summary>
    /// <param name="type">Task Type to be added.</param>
    /// <returns>Task</returns>
    public async Task CreateTaskTypeAsync(TaskType type)
    {
        var client = _db.Client;

        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var suggestionsInTransaction = db.GetCollection<TaskType>(_db.TaskTypeCollectionName);
            await suggestionsInTransaction.InsertOneAsync(session, type);

            var usersInTransaction = db.GetCollection<User>(_db.UserCollectionName);
            var user = await _userData.GetByIdAsync(type.AuditInformation.CreatedById);
            user.TaskTypes.Add(type);
            await usersInTransaction.ReplaceOneAsync(session, u => u.Id == user.Id, user);

            await session.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Gets a list of TaskTypes for a user.
    /// </summary>
    /// <param name="createdById">The id value from the user in MongoDB</param>
    /// <returns>List of TaskTypes</returns>
    public async Task<List<TaskType>> GetUsersTaskTypesAsync(string createdById)
    {
        var results = await _taskTypes.FindAsync(tt => tt.AuditInformation.CreatedById == createdById && tt.AuditInformation.IsArchived == false);

        return results.ToList();
    }

    /// <summary>
    /// Gets a list of all TaskTypes in the database, excluding archived TaskTypes.
    /// </summary>
    /// <returns>List of NON archived TaskTypes</returns>
    public async Task<List<TaskType>> GetTaskTypesAsync()
    {
        var results = await _taskTypes.FindAsync(tt => tt.AuditInformation.IsArchived == false);

        return results.ToList();
    }

    /// <summary>
    /// Gets a TaskType by its ID.
    /// </summary>
    /// <param name="id">Id value of the task type.</param>
    /// <returns>Task Type by Id or null.</returns>
    public async Task<TaskType?> GetTaskTypeAsync(string id)
    {
        var result = await _taskTypes.FindAsync(tt => tt.Id == id);

        return result.FirstOrDefault();
    }

    /// <summary>
    /// Updates a TaskType in the database and updates the user's TaskTypes list.
    /// </summary>
    /// <param name="type">Task Type to be updated</param>
    /// <returns></returns>
    public async Task UpdateTaskTypeAsync(TaskType type)
    {
        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var typesInTransaction = db.GetCollection<TaskType>(_db.TaskTypeCollectionName);

            await typesInTransaction.ReplaceOneAsync(
                session,
                tt => tt.Id == type.Id,
                type
            );

            var usersInTransaction = db.GetCollection<User>(_db.UserCollectionName);

            // Get the user from the Audit Info
            var userFilter = Builders<User>.Filter.Eq(u => u.Id, type.AuditInformation.CreatedById);
            var user = await usersInTransaction
                .Find(session, userFilter)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                // Get the index of the TaskType in the list so we can replace it.
                var index = user.TaskTypes.FindIndex(utt => utt.Id == type.Id);

                if (index != -1)
                {
                    user.TaskTypes[index] = type;
                }
                else
                {
                    // Optional: Handle the case where the TaskType is not found in the user's list
                    // For example, you might want to add it or log a warning
                }

                await usersInTransaction.ReplaceOneAsync(
                    session,
                    u => u.Id == user.Id,
                    user
                );
            }
            else
            {
                throw new Exception($"User with ID {type.AuditInformation.CreatedById} not found.");
            }

            await session.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            // Log the exception or rethrow it
            throw;
        }
    }

}
