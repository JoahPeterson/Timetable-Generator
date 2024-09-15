namespace TimetableApp.DataModels.DataAccess;

/// <summary>
/// Class that handles mongo DAL functions for Term.
/// </summary>
public class MongoTermDurationData : ITermDurationData
{
    private readonly IDbConnection _db;
    private readonly IMongoCollection<TermDuration> _termDurations;

    /// <summary>
    /// Instantiates a new instance of the Mongo Term data class.
    /// </summary>
    /// <param name="db"></param>
    public MongoTermDurationData(IDbConnection db)
    {
        _db = db;
        _termDurations = db.TermDurationCollection;
    }

    /// <summary>
    /// Creates a new term duration in the database.
    /// </summary>
    /// <param name="term">Term object to be added to the db</param>
    /// <returns></returns>
    public async Task CreateAsync(TermDuration termDuration)
    {
        await _termDurations.InsertOneAsync(termDuration);
    }

    /// <summary>
    /// Retrienve a term durations by its id.
    /// </summary>
    /// <param name="id">Id for the term to be retrieved</param>
    /// <returns>Term Duration object</returns>
    public async Task<TermDuration?> GetByIdAsync(string id)
    {
        var results = await _termDurations.FindAsync(t => t.Id == id);

        return results.FirstOrDefault();
    }

    /// <summary>
    /// Retrieve all term durations in the database.
    /// </summary>
    /// <returns>List of Term Duration Objects</returns>
    public async Task<List<TermDuration>> GetAsync()
    {
        var results = await _termDurations.FindAsync(_ => true);

        return results.ToList();
    }

    /// <summary>
    /// Update a term duration in the database.
    /// </summary>
    /// <param name="term">The updated Term Object</param>
    /// <returns>Task</returns>
    public Task UpdateAsync(TermDuration termDuration)
    {
        var filter = Builders<TermDuration>.Filter.Eq("Id", termDuration.Id);
        return _termDurations.ReplaceOneAsync(filter, termDuration, new ReplaceOptions { IsUpsert = true });
    }
}
