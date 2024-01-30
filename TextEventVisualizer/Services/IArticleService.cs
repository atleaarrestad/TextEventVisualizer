using TextEventVisualizer.Models;

namespace TextEventVisualizer.Services
{
    public interface IArticleService
    {
        Task<Article> GetArticleAsync(int id);
        Task<List<Article>> GetUnscrapedArticlesAsync();
        Task<int> GetUnscrapedArticlesCountAsync();
        Task<List<Article>> GetScrapedArticlesAsync();
        Task<int> GetScrapedArticlesCountAsync();
        Task AddArticleAsync(Article article);
        Task UpdateArticleAsync(Article article);
        Task DeleteArticleAsync(int id);
        public Task AddDummyArticles();

    }
}
