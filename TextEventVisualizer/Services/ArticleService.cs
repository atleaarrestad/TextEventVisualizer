using TextEventVisualizer.Data.Models;
using TextEventVisualizer.Repositories;

namespace TextEventVisualizer.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository ArticleRepository;
        public ArticleService(IArticleRepository articleRepository)
        {
            ArticleRepository = articleRepository;
        }

        public Task AddArticleAsync(Article article)
        {
            return ArticleRepository.AddArticleAsync(article);
        }

        public Task DeleteArticleAsync(int id)
        {
            return ArticleRepository.DeleteArticleAsync(id);
        }

        public Task<IEnumerable<Article>> GetAllArticlesAsync()
        {
            return ArticleRepository.GetAllArticlesAsync();
        }

        public Task<Article> GetArticleAsync(int id)
        {
            return ArticleRepository.GetArticleAsync(id);
        }

        public Task UpdateArticleAsync(Article article)
        {
            return ArticleRepository.UpdateArticleAsync(article);
        }
    }
}
