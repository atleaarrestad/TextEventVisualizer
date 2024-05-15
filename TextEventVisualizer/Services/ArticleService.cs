using TextEventVisualizer.Models;
using TextEventVisualizer.Repositories;

namespace TextEventVisualizer.Services
{
    /// <summary>
    /// Service layer for managing articles. Provides a high-level API to interact with article data.
    /// </summary>
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository ArticleRepository;

        /// <summary>
        /// Initializes a new instance of the ArticleService class.
        /// </summary>
        /// <param name="articleRepository">The repository used for article data operations.</param>
        public ArticleService(IArticleRepository articleRepository)
        {
            ArticleRepository = articleRepository;
        }

        /// <summary>
        /// Adds a new article asynchronously.
        /// </summary>
        /// <param name="article">The article to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task AddArticleAsync(Article article)
        {
            return ArticleRepository.AddArticleAsync(article);
        }

        /// <summary>
        /// Deletes an article by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the article to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task DeleteArticleAsync(int id)
        {
            return ArticleRepository.DeleteArticleAsync(id);
        }

        /// <summary>
        /// Retrieves a list of articles asynchronously based on the specified criteria.
        /// </summary>
        /// <param name="scraped">Filter by whether articles were scraped. Null for no filter.</param>
        /// <param name="category">Filter by category. Null for no filter.</param>
        /// <param name="from">Filter by the starting date of article publication. Null for no filter.</param>
        /// <param name="to">Filter by the ending date of article publication. Null for no filter.</param>
        /// <param name="limit">Limit the number of articles returned. Null for no limit.</param>
        /// <returns>A task that represents the asynchronous read operation, which could return a list of articles.</returns>
        public Task<List<Article>> GetArticlesAsync(bool? scraped = null, string? category = null, DateTime? from = null, DateTime? to = null, int? limit = null)
        {
            return ArticleRepository.GetArticlesAsync(scraped, category, from, to, limit);
        }

        /// <summary>
        /// Counts the articles asynchronously based on specified criteria.
        /// </summary>
        /// <param name="scraped">Filter by whether articles were scraped. Null for no filter.</param>
        /// <param name="category">Filter by category. Null for no filter.</param>
        /// <param name="from">Filter by the starting date of article publication. Null for no filter.</param>
        /// <param name="to">Filter by the ending date of article publication. Null for no filter.</param>
        /// <returns>A task that represents the asynchronous operation, which could return the count of articles.</returns>
        public Task<int> GetArticlesCountAsync(bool? scraped = null, string? category = null, DateTime? from = null, DateTime? to = null)
        {
            return ArticleRepository.GetArticlesCountAsync(scraped, category, from, to);
        }

        /// <summary>
        /// Retrieves a single article by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the article to retrieve.</param>
        /// <returns>A task that represents the asynchronous read operation, which could return the requested article.</returns>
        public Task<Article> GetArticleAsync(int id)
        {
            return ArticleRepository.GetArticleAsync(id);
        }

        /// <summary>
        /// Updates an article asynchronously.
        /// </summary>
        /// <param name="article">The article to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task UpdateArticleAsync(Article article)
        {
            return ArticleRepository.UpdateArticleAsync(article);
        }

        /// <summary>
        /// Adds a batch of articles asynchronously.
        /// </summary>
        /// <param name="articles">The list of articles to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task AddArticleBatchAsync(List<Article> articles)
        {
            return ArticleRepository.AddArticleBatchAsync(articles);
        }

        /// <summary>
        /// Retrieves a list of unique article categories asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation, which could return a list of unique categories.</returns>
        public Task<List<string>> GetUniqueCategories()
        {
            return ArticleRepository.GetUniqueCategories();
        }
    }
}
