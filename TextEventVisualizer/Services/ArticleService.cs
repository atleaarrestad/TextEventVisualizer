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

        public Task<List<Article>> GetUnscrapedArticlesAsync()
        {
            return ArticleRepository.GetUnscrapedArticlesAsync();
        }
        public Task<int> GetUnscrapedArticlesCountAsync()
        {
            return ArticleRepository.GetUnscrapedArticlesCountAsync();
        }

        public Task<int> GetUnscrapedArticlesCountAsync(string category, DateTime from, DateTime to)
        {
            return ArticleRepository.GetUnscrapedArticlesCountAsync(category, from, to);
        }

        public Task<List<Article>> GetScrapedArticlesAsync()
        {
            return ArticleRepository.GetScrapedArticlesAsync();
        }
        public Task<int> GetScrapedArticlesCountAsync()
        {
            return ArticleRepository.GetScrapedArticlesCountAsync();
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
