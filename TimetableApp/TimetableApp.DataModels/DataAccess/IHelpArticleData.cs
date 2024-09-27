namespace TimetableApp.DataModels.DataAccess;
public interface IHelpArticleData
{
    Task<HelpArticle> CreateAsync(HelpArticle helpArticle);
    Task<List<HelpArticle>> GetAllAsync();
    Task<HelpArticle> GetByIdAsync(string id);
    Task<List<HelpArticle>> GetAllWithArchivedAsync();
    Task<HelpArticle> UpdateAsync(HelpArticle);
}
