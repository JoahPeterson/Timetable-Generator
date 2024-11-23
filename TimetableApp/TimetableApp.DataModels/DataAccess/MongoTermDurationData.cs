using Microsoft.Extensions.Logging;
using TimetableApp.DataModels.Models;

namespace TimetableApp.DataModels.DataAccess;

/// <summary>
/// Class that handles mongo DAL functions for Term.
/// </summary>
public class MongoTermDurationData : ITermDurationData
{
    private readonly IDbConnection _db;
    private readonly IMongoCollection<TermDuration> _termDurations;
    private readonly ILogger<MongoTermDurationData> _logger;

    /// <summary>
    /// Instantiates a new instance of the Mongo Term data class.
    /// </summary>
    /// <param name="db"></param>
    public MongoTermDurationData(IDbConnection db, ILogger<MongoTermDurationData> logger)
    {
        _db = db;
        _termDurations = db.TermDurationCollection;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new term duration in the database.
    /// </summary>
    /// <param name="term">Term object to be added to the db</param>
    /// <returns></returns>
    public async Task CreateAsync(TermDuration termDuration)
    {
        try
        {
            await _termDurations.InsertOneAsync(termDuration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create term duration for user {UserId}", termDuration.AuditInformation.CreatedById);
        }
       
    }

    /// <summary>
    /// Retrienve a term durations by its id.
    /// </summary>
    /// <param name="id">Id for the term to be retrieved</param>
    /// <returns>Term Duration object</returns>
    public async Task<TermDuration?> GetByIdAsync(string id)
    {
        try
        {
            var results = await _termDurations.FindAsync(t => t.Id == id);

            return results.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get term duration with Id of {TermDurationId}", id);
            return null;
        }
    }

    /// <summary>
    /// Retrieve all term durations in the database.
    /// </summary>
    /// <returns>List of Term Duration Objects</returns>
    public async Task<List<TermDuration>> GetAsync()
    {
        try
        {
            var results = await _termDurations.FindAsync(_ => true);

            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get term durations");
            return null;
        }
        
    }

    /// <summary>
    /// Update a term duration in the database.
    /// </summary>
    /// <param name="term">The updated Term Object</param>
    /// <returns>Task</returns>
    public Task UpdateAsync(TermDuration termDuration)
    {
        try
        {
            var filter = Builders<TermDuration>.Filter.Eq("Id", termDuration.Id);
            return _termDurations.ReplaceOneAsync(filter, termDuration, new ReplaceOptions { IsUpsert = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update term duration with Id of {TermDurationId}", termDuration.Id);
            return null;
        }
        
    }
}
