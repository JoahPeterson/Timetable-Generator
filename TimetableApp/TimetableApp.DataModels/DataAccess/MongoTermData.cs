namespace TimetableApp.DataModels.DataAccess;

/// <summary>
/// Class that handles mongo DAL functions for Term.
/// </summary>
public class MongoTermData : ITermData
{
    private readonly IDbConnection _db;
    private readonly IMongoCollection<Term> _terms;

    /// <summary>
    /// Instantiates a new instance of the Mongo Term data class.
    /// </summary>
    /// <param name="db"></param>
    public MongoTermData(IDbConnection db)
    {
        _db = db;
        _terms = db.TermCollection;
    }

    /// <summary>
    /// Creates a new term in the database.
    /// </summary>
    /// <param name="term">Term object to be added to the db</param>
    /// <returns></returns>
    public async Task CreateAsync(Term term)
    {
        await _terms.InsertOneAsync(term);
    }

    /// <summary>
    /// Retrienve a term by its id.
    /// </summary>
    /// <param name="id">Id for the term to be retrieved</param>
    /// <returns>Term object</returns>
    public async Task<Term?> GetByIdAsync(string id)
    {
        var results = await _terms.FindAsync(t => t.Id == id);

        return results.FirstOrDefault();
    }

    /// <summary>
    /// Retrieve all terms in the database.
    /// </summary>
    /// <returns>List of Term Objects</returns>
    public async Task<List<Term>> GetAsync()
    {
        var results = await _terms.FindAsync(_ => true);

        return results.ToList();
    }

    /// <summary>
    /// Update a term in the database.
    /// </summary>
    /// <param name="term">The updated Term Object</param>
    /// <returns>Task</returns>
    public Task UpdateAsync(Term term)
    {
        var filter = Builders<Term>.Filter.Eq("Id", term.Id);
        return _terms.ReplaceOneAsync(filter, term, new ReplaceOptions { IsUpsert = true });
    }
}
