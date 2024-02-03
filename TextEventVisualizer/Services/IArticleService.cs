using TextEventVisualizer.Models;

namespace TextEventVisualizer.Services
{
    public interface IArticleService
    {
        Task<Article> GetArticleAsync(int id);
        Task<int> GetArticlesCountAsync(bool? scraped = null, string? category = null, DateTime? from = null, DateTime? to = null);
        Task<List<Article>> GetArticlesAsync(bool? scraped = null, string? category = null, DateTime? from = null, DateTime? to = null);
        Task AddArticleAsync(Article article);
        Task AddArticleBatchAsync(List<Article> articles);
        Task UpdateArticleAsync(Article article);
        Task DeleteArticleAsync(int id);
        Task<List<string>> GetUniqueCategories();

    }
}
