using Microsoft.Extensions.Logging;
using TimetableApp.DataModels.Models;

namespace TimetableApp.DataModels.DataAccess;

/// <summary>
/// Class that handles mongo DAL functions for Term.
/// </summary>
public class MongoTermData : ITermData
{
    private readonly IDbConnection _db;
    private readonly IMongoCollection<Term> _terms;
    private readonly ILogger<MongoTermData> _logger;

    /// <summary>
    /// Instantiates a new instance of the Mongo Term data class.
    /// </summary>
    /// <param name="db"></param>
    public MongoTermData(IDbConnection db, ILogger<MongoTermData> logger)
    {
        _db = db;
        _terms = db.TermCollection;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new term in the database.
    /// </summary>
    /// <param name="term">Term object to be added to the db</param>
    /// <returns></returns>
    public async Task CreateAsync(Term term)
    {
        try
        {
            await _terms.InsertOneAsync(term);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to create term for user {UserId}", term.AuditInformation.CreatedById);
        }
       
    }

    /// <summary>
    /// Retrienve a term by its id.
    /// </summary>
    /// <param name="id">Id for the term to be retrieved</param>
    /// <returns>Term object</returns>
    public async Task<Term?> GetByIdAsync(string id)
    {
        try
        {
            var results = await _terms.FindAsync(t => t.Id == id);

            return results.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get term with Id of {TermId}", id);
            return null;
        }
       
    }

    /// <summary>
    /// Retrieve all terms in the database.
    /// </summary>
    /// <returns>List of Term Objects</returns>
    public async Task<List<Term>> GetAllAsync()
    {
        try
        {
            var results = await _terms.FindAsync(term => term.AuditInformation.IsArchived == false);

            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get terms");
            return null;
        }
        
    }

    /// <summary>
    /// Retrieve all terms in the database.
    /// </summary>
    /// <returns>List of Term Objects</returns>
    public async Task<List<Term>> GetAllWithArchivedAsync()
    {
        try
        {
            var results = await _terms.FindAsync(_ => true);

            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get archived terms");
            return null;
        }
        
    }

    /// <summary>
    /// Update a term in the database.
    /// </summary>
    /// <param name="term">The updated Term Object</param>
    /// <returns>Task</returns>
    public Task UpdateAsync(Term term)
    {
        try
        {
            var filter = Builders<Term>.Filter.Eq("Id", term.Id);
            return _terms.ReplaceOneAsync(filter, term, new ReplaceOptions { IsUpsert = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update term with Id of {TermId}", term.Id);
            return null;
        }
       
    }
}
