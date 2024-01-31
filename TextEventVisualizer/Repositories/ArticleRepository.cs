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

        public async Task<List<Article>> GetUnscrapedArticlesAsync()
        {
            return await _context.Articles.Where(article => !article.HasBeenScraped).ToListAsync();
        }
        public async Task<List<Article>> GetScrapedArticlesAsync()
        {
            return await _context.Articles.Where(article => article.HasBeenScraped).ToListAsync();
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

        public async Task<int> GetUnscrapedArticlesCountAsync()
        {
            return await _context.Articles.CountAsync(article => !article.HasBeenScraped);
        }

        public async Task<int> GetScrapedArticlesCountAsync()
        {
            return await _context.Articles.CountAsync(article => article.HasBeenScraped);
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
