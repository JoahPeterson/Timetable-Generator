namespace TimetableApp.DataModels.DataAccess;

/// <summary>
/// Class the handles the Mongo DAL functions for TaskType
/// </summary>
public class MongoTaskTypeData : ITaskTypeData
{
    private readonly IMongoCollection<TaskType> _taskTypes;

    /// <summary>
    /// Intantiates a new instance of the Mongo TaskType data class.
    /// </summary>
    /// <param name="db">Instance of a Mongo DB Connection</param>
    public MongoTaskTypeData(IDbConnection db)
    {
        _taskTypes = db.TaskTypeCollection;
    }

    public Task ArchiveTaskTypeAsync(string id)
    {
//TODO: Should this should should probably archive the task type as if it was deleted
// if this it would break previous documents?
        throw new NotImplementedException();
    }

    public Task CreateTaskTypeAsync(TaskType type)
    {
        return _taskTypes.InsertOneAsync(type);
    }

    public async Task<TaskType?> GetTaskTypeByIdAsync(string id)
    {
        var results = await _taskTypes.FindAsync(tt => tt.Id == id);

        return results.FirstOrDefault();
    }

    public async Task<List<TaskType>> GetTaskTypesAsync()
    {
        var results = await _taskTypes.FindAsync(tt => tt.AuditInformation.IsArchived == false);

        return results.ToList();
    }

    public async Task UpdateTaskTypeAsync(TaskType type)
    {
        await _taskTypes.ReplaceOneAsync(tt => tt.Id == type.Id, type);
    }
}
