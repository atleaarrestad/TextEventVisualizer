using TextEventVisualizer.Models;
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

        public Task<List<Article>> GetArticlesAsync(bool? scraped = null, string? category = null, DateTime? from = null, DateTime? to = null)
        {
            return ArticleRepository.GetArticlesAsync(scraped, category, from, to);
        }
        
        public Task<int> GetArticlesCountAsync(bool? scraped = null, string? category = null, DateTime? from = null, DateTime? to = null)
        {
            return ArticleRepository.GetArticlesCountAsync(scraped, category, from, to);
        }

        public Task<Article> GetArticleAsync(int id)
        {
            return ArticleRepository.GetArticleAsync(id);
        }

        public Task UpdateArticleAsync(Article article)
        {
            return ArticleRepository.UpdateArticleAsync(article);
        }

        public Task AddArticleBatchAsync(List<Article> articles)
        {
            return ArticleRepository.AddArticleBatchAsync(articles);
        }

        public Task<List<string>> GetUniqueCategories()
        {
            return ArticleRepository.GetUniqueCategories();
        }
    }
}
