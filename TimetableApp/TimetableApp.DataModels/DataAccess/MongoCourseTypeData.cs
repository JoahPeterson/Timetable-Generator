﻿namespace TimetableApp.DataModels.DataAccess;

/// <summary>
/// Class that handles mongo DAL functions for Term.
/// </summary>
public class MongoCourseTypeData : ICourseTypeData
{
    private readonly IDbConnection _db;
    private readonly IMongoCollection<CourseType> _courseTypes;

    /// <summary>
    /// Instantiates a new instance of the Mongo Course Type data class.
    /// </summary>
    /// <param name="db"></param>
    public MongoCourseTypeData(IDbConnection db)
    {
        _db = db;
        _courseTypes = db.CourseTypeCollection;
    }

    /// <summary>
    /// Creates a new course type in the database.
    /// </summary>
    /// <param name="term">Course Type object to be added to the db</param>
    /// <returns>Task</returns>
    public async Task CreateAsync(CourseType courseType)
    {
        await _courseTypes.InsertOneAsync(courseType);
    }

    /// <summary>
    /// Retrienve a Course Types by its id.
    /// </summary>
    /// <param name="id">Id for the course to be retrieved</param>
    /// <returns>Course Type object</returns>
    public async Task<CourseType?> GetByIdAsync(string id)
    {
        var results = await _courseTypes.FindAsync(t => t.Id == id);

        return results.FirstOrDefault();
    }

    /// <summary>
    /// Retrieve all Course Types in the database.
    /// </summary>
    /// <returns>List of Course Type Objects</returns>
    public async Task<List<CourseType>> GetAsync()
    {
        var results = await _courseTypes.FindAsync(_ => true);

        return results.ToList();
    }

    /// <summary>
    /// Update a course type in the database.
    /// </summary>
    /// <param name="term">The updated CourseType object</param>
    /// <returns>Task</returns>
    public Task UpdateAsync(CourseType courseType)
    {
        var filter = Builders<CourseType>.Filter.Eq("Id", courseType.Id);
        return _courseTypes.ReplaceOneAsync(filter, courseType, new ReplaceOptions { IsUpsert = true });
    }
}
