using Microsoft.EntityFrameworkCore;
using TextEventVisualizer.Data.Models;
using TextEventVisualizer.Data;

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

        public async Task<IEnumerable<Article>> GetAllArticlesAsync()
        {
            return await _context.Articles.ToListAsync();
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
    }

}
