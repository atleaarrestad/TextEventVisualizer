using Microsoft.EntityFrameworkCore;
using TextEventVisualizer.Data;
using TextEventVisualizer.Models;

namespace TextEventVisualizer.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Article> GetArticleAsync(int id)
        {
            return await _context.Articles.FindAsync(id);
        }

        public async Task AddArticleAsync(Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateArticleAsync(Article article)
        {
            _context.Entry(article).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteArticleAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
            }
        }

        public Task<int> GetArticlesCountAsync(bool? scraped = null, string? category = null, DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Articles.AsQueryable();

            query = query.Where(article => !article.UrlDoesntExistAnymore);

            if (scraped.HasValue)
            {
                query = query.Where(article => article.HasBeenScraped == scraped.Value);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(article => article.Category == category);
            }

            if (from.HasValue)
            {
                query = query.Where(article => article.Date >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(article => article.Date <= to.Value);
            }

            return query.CountAsync();
        }

        public Task<List<Article>> GetArticlesAsync(bool? scraped = null, string? category = null, DateTime? from = null, DateTime? to = null, int? limit = null)
        {
            var query = _context.Articles.AsQueryable();

            query = query.Where(article => !article.UrlDoesntExistAnymore);

            if (scraped.HasValue)
            {
                query = query.Where(article => article.HasBeenScraped == scraped.Value);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(article => article.Category == category);
            }

            if (from.HasValue)
            {
                query = query.Where(article => article.Date >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(article => article.Date <= to.Value);
            }

            if (limit.HasValue && limit.Value > 0)
            {
                query = query.Take(limit.Value);
            }

            return query.ToListAsync();
        }

        public Task AddArticleBatchAsync(List<Article> articles)
        {
            foreach (var article in articles)
            {
                _context.Articles.Add(article);
            }
            return _context.SaveChangesAsync();
        }

        public Task<List<string>> GetUniqueCategories()
        {
            return _context.Articles
                .Select(a => a.Category)
                .Distinct()
                .ToListAsync();
        }
    }

}
