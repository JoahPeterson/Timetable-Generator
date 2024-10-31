namespace TimetableApp.DataModels.DataAccess;

/// <summary>
/// Class the handles the Mongo DAL functions for TaskType
/// </summary>
public class MongoWorkUnitData : IWorkUnitData
{
    private readonly IDbConnection _db;
    private readonly IMongoCollection<Course> _courses;
    private readonly ICourseData _courseData;
    private readonly IMongoCollection<WorkUnit> _workUnits;

    /// <summary>
    /// Intantiates a new instance of the Mongo TaskType data class.
    /// </summary>
    /// <param name="db">Instance of a Mongo DB Connection</param>
    public MongoWorkUnitData(IDbConnection db, ICourseData courseData)
    {
        _db = db;
        _courseData = courseData;
        _workUnits = db.WorkUnitCollection;
        _courses = db.CourseCollection;
    }

    /// <summary>
    /// Deletes a work unit from mongo db
    /// </summary>
    /// <param name="workUnit">The work unit to delete</param>
    public async Task DeleteAsync(string Id)
    {
        var filter = Builders<WorkUnit>.Filter.Eq("_id", new ObjectId(Id));
        await _workUnits.DeleteOneAsync(filter);
    }

    /// <summary>
    /// Create a WorkUnit  in the database and upate the Course's WorkUnits  list.
    /// Created in a transaction so we can rollback if the user update fails.
    /// </summary>
    /// <param name="workUnit">Work Unit to be added.</param>
    /// <returns>Task</returns>
    public async Task CreateAsync(WorkUnit workUnit)
    {
        var client = _db.Client;

        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var workUnitsInTransaction = db.GetCollection<WorkUnit>(_db.WorkUnitCollectionName);
            await workUnitsInTransaction.InsertOneAsync(session, workUnit);

            var coursesInTransaction = db.GetCollection<Course>(_db.CourseTypeCollectionName);
            var course = _courses.Find(c => c.Id == workUnit.CourseId).FirstOrDefault();

            if (course == null)
            {
                throw new InvalidOperationException($"Course with Id {workUnit.CourseId} not found.");
            }

            course.WorkUnits.Add(workUnit);
            await coursesInTransaction.ReplaceOneAsync(session, c => c.Id == workUnit.CourseId, course);

            await session.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Gets a list of Work Units for a user.
    /// </summary>
    /// <param name="createdById">The id value from the user in MongoDB</param>
    /// <returns>List of TaskTypes</returns>
    public async Task<List<WorkUnit>> GetUsersAsync(string createdById)
    {
        var results = await _workUnits.FindAsync(workUnit => workUnit.AuditInformation.CreatedById == createdById);

        return results.ToList();
    }

    /// <summary>
    /// Gets a list of all Work Units in the database, excluding archived Work Units.
    /// </summary>
    /// <returns>List of NON archived Work Units</returns>
    public async Task<List<WorkUnit>> GetAsync()
    {
        var results = await _workUnits.FindAsync(workUnit => workUnit.AuditInformation.IsArchived == false);

        return results.ToList();
    }

    /// <summary>
    /// Gets a Work Unit by its ID.
    /// </summary>
    /// <param name="id">Id value of the task type.</param>
    /// <returns>Work Unit by Id or null.</returns>
    public async Task<WorkUnit?> GetByIdAsync(string id)
    {
        var result = await _workUnits.FindAsync(c => c.Id == id);

        return result.FirstOrDefault();
    }

    /// <summary>
    /// Updates a Workunit in the database and updates the Course's WorkUnits list.
    /// </summary>
    /// <param name="WorkUnit">Work Unit to be updated</param>
    /// <returns>Task</returns>
    public async Task UpdateAsync(WorkUnit workUnit)
    {
        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var workUnitsInTransation = db.GetCollection<WorkUnit>(_db.WorkUnitCollectionName);

            await workUnitsInTransation.ReplaceOneAsync(
                session,
                wUnit => wUnit.Id == workUnit.Id,
                workUnit
            );

            var coursesInTransaction = db.GetCollection<Course>(_db.CourseCollectionName);

            // Get the user from the Audit Info
            var courseFilters = Builders<Course>.Filter.Eq(course => course.Id, workUnit.CourseId);
            var course = await coursesInTransaction
                .Find(session, courseFilters)
                .FirstOrDefaultAsync();

            if (course != null)
            {
                // Get the index of the TaskType in the list so we can replace it.
                var index = course.WorkUnits.FindIndex(wUnit=> wUnit.Id == workUnit.Id);

                if (index != -1)
                {
                    course.WorkUnits[index] = workUnit;
                }
                else
                {
                    // Optional: Handle the case where the TaskType is not found in the user's list
                    // For example, you might want to add it or log a warning
                }

                await coursesInTransaction.ReplaceOneAsync(
                    session,
                    course => course.Id == workUnit.CourseId,
                    course
                );
            }
            else
            {
                throw new Exception($"Course with ID {workUnit.CourseId} not found.");
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
