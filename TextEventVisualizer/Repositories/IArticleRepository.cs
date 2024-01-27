using TextEventVisualizer.Data.Models;

namespace TextEventVisualizer.Repositories
{
    public interface IArticleRepository
    {
        Task<Article> GetArticleAsync(int id);
        Task<IEnumerable<Article>> GetAllArticlesAsync();
        Task AddArticleAsync(Article article);
        Task UpdateArticleAsync(Article article);
        Task DeleteArticleAsync(int id);
    }
}
