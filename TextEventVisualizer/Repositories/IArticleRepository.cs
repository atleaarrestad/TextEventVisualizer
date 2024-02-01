using TextEventVisualizer.Models;

namespace TextEventVisualizer.Repositories
{
    public interface IArticleRepository
    {
        Task<Article> GetArticleAsync(int id);
        Task<List<Article>> GetUnscrapedArticlesAsync();
        Task<int> GetUnscrapedArticlesCountAsync();
        Task<int> GetUnscrapedArticlesCountAsync(string category, DateTime from, DateTime to);
        Task<List<Article>> GetScrapedArticlesAsync();
        Task<int> GetScrapedArticlesCountAsync();
        Task AddArticleBatchAsync(List<Article> articles);
        Task AddArticleAsync(Article article);
        Task UpdateArticleAsync(Article article);
        Task DeleteArticleAsync(int id);
        Task<List<string>> GetUniqueCategories();
    }
}
