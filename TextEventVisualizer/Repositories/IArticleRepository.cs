using TextEventVisualizer.Models;

namespace TextEventVisualizer.Repositories
{
    public interface IArticleRepository
    {
        Task<Article> GetArticleAsync(int id);
        Task<int> GetArticlesCountAsync(bool? scraped = null, string? category = null, DateTime? from = null, DateTime? to = null);
        Task<List<Article>> GetArticlesAsync(bool? scraped = null, string? category = null, DateTime? from = null, DateTime? to = null, int? limit = null);
        Task AddArticleBatchAsync(List<Article> articles);
        Task AddArticleAsync(Article article);
        Task UpdateArticleAsync(Article article);
        Task DeleteArticleAsync(int id);
        Task<List<string>> GetUniqueCategories();
    }
}
