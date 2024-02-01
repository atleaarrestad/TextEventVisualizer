using TextEventVisualizer.Models;

namespace TextEventVisualizer.Services
{
    public interface IArticleService
    {
        Task<Article> GetArticleAsync(int id);
        Task<List<Article>> GetUnscrapedArticlesAsync();
        Task<int> GetUnscrapedArticlesCountAsync();
        Task<int> GetUnscrapedArticlesCountAsync(string category, DateTime from, DateTime to);
        Task<List<Article>> GetScrapedArticlesAsync();
        Task<int> GetScrapedArticlesCountAsync();
        Task AddArticleAsync(Article article);
        Task AddArticleBatchAsync(List<Article> articles);
        Task UpdateArticleAsync(Article article);
        Task DeleteArticleAsync(int id);
        Task<List<string>> GetUniqueCategories();

    }
}
