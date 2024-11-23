using Microsoft.Extensions.Logging;
using TimetableApp.DataModels.Models;

namespace TimetableApp.DataModels.DataAccess;

/// <summary>
/// Class the handles the Mongo DAL functions for HelpArticle
/// </summary>
public class MongoHelpArticleData : IHelpArticleData
{
    private readonly IDbConnection _db;
    private readonly IMongoCollection<HelpArticle> _helpArticles;
    private readonly ILogger<MongoHelpArticleData> _logger;

    /// <summary>
    /// Instatiates a new instance of the Mongo Help Article data class.
    /// </summary>
    /// <param name="db">Instance of a Mongo DB Connection</param>
    /// <param name="userData"></param>
    public MongoHelpArticleData(IDbConnection db, ILogger<MongoHelpArticleData> logger)
    {
        _db = db;
        _helpArticles = db.HelpArticleCollection;
        _logger = logger;
    }
    /// <summary>
    /// Inserts a HelpArticle in the database.
    /// </summary>
    /// <param name="helpArticle">HelpArticle to be created</param>
    /// <returns>Instance of the Help Article if successfully inserted in </returns>
    public async Task<HelpArticle> CreateAsync(HelpArticle helpArticle)
    {
        var client = _db.Client;

        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);

            var articlesInTransaction = db.GetCollection<HelpArticle>(_db.HelpArticleCollectionName);
            await articlesInTransaction.InsertOneAsync(session, helpArticle);

            await session.CommitTransactionAsync();

            return helpArticle;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create help article for user {UserId}", helpArticle.AuditInformation.CreatedById);
            await session.AbortTransactionAsync();
            return null;
        }
    }

    /// <summary>
    /// Gets all HelpArticles from the database. Excludes archived articles.
    /// </summary>
    /// <returns>List of HelpArticl</returns>
    public async Task<List<HelpArticle>> GetAllAsync()
    {
        try
        {
            return await _db.HelpArticleCollection.Find(hp => hp.AuditInformation.IsArchived == false).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get help articles");
            return null;
        }
    }

    /// <summary>
    /// Gets all HelpArticles from the database. Includes archived articles.
    /// </summary>
    /// <returns>List of HelpArticles</returns>
    public async Task<List<HelpArticle>> GetAllWithArchivedAsync()
    {
        try
        {
            return await _db.HelpArticleCollection.Find(_ => true).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get help articles");
            return null;
        }
    }

    /// <summary>
    /// Gets a single instance of article by its ID.
    /// </summary>
    /// <param name="id">The help article to be fetched from DB</param>
    /// <returns>HelpArticle with matchin ID</returns>
    public async Task<HelpArticle> GetByIdAsync(string id)
    {
        try
        {
            return await _db.HelpArticleCollection.FindAsync(hp => hp.Id == id).Result.FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get help article with Id of {HelpArticleId}", id);
            return null;
        }

    }

    /// <summary>
    /// Updates a HelpArticle in the database.
    /// </summary>
    /// <param name="article">New Version of the help article.</param>
    /// <returns>The newly inserted HelpArticle.</returns>
    public async Task<HelpArticle> UpdateAsync(HelpArticle article)
    {
        try
        {
            // Define the filter to find the HelpArticle by its ID
            var filter = Builders<HelpArticle>.Filter.Eq(a => a.Id, article.Id);

            // Options to return the document after the replacement
            var options = new FindOneAndReplaceOptions<HelpArticle>
            {
                ReturnDocument = ReturnDocument.After // Return the updated document after replacement
            };

            var replacedArticle = await _helpArticles.FindOneAndReplaceAsync(filter, article, options);

            return replacedArticle;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update help article with Id of {HelpArticleId}", article.Id);
            return null;
        }
       
    }
}
