using TextEventVisualizer.Models;

namespace TextEventVisualizer.Repositories
{
    public interface IArticleRepository
    {
        Task<Article> GetArticleAsync(int id);
        Task<List<Article>> GetUnscrapedArticlesAsync();
        Task<int> GetUnscrapedArticlesCountAsync();
        Task<List<Article>> GetScrapedArticlesAsync();
        Task<int> GetScrapedArticlesCountAsync();
        Task AddArticleAsync(Article article);
        Task UpdateArticleAsync(Article article);
        Task DeleteArticleAsync(int id);
    }
}
