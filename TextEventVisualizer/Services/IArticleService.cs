using TextEventVisualizer.Data.Models;

namespace TextEventVisualizer.Services
{
    public interface IArticleService
    {
        Task<Article> GetArticleAsync(int id);
        Task<IEnumerable<Article>> GetAllArticlesAsync();
        Task AddArticleAsync(Article article);
        Task UpdateArticleAsync(Article article);
        Task DeleteArticleAsync(int id);

    }
}
