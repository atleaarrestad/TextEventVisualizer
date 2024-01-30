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
        public Task<List<Article>> GetScrapedArticlesAsync()
        {
            return ArticleRepository.GetScrapedArticlesAsync();
        }

        public Task<Article> GetArticleAsync(int id)
        {
            return ArticleRepository.GetArticleAsync(id);
        }

        public Task UpdateArticleAsync(Article article)
        {
            return ArticleRepository.UpdateArticleAsync(article);
        }

        public async Task AddDummyArticles()
        {
            await ArticleRepository.AddArticleAsync(new Article
            {
                WebUrl = "https://www.huffpost.com/entry/hurricane-fiona-barrels-toward-turks-and-caicos-islands_n_63298597e4b0ed991abcacf9",
                Headline = "Fiona Barrels Toward Turks And Caicos Islands As Category 3 Hurricane",
                Category = "WORLD NEWS",
                Description = "The Turks and Caicos Islands government imposed a curfew as the intensifying storm kept dropping copious rain over the Dominican Republic and Puerto Rico.",
                Authors = new() { "nica Coto" },
                Date = new DateTime(2022, 9, 20),
                HasBeenScraped = false,
                Title = "dummy 1",
                Content = ""
            });
            await ArticleRepository.AddArticleAsync(new Article
            {
                WebUrl = "https://www.huffpost.com/entry/kash-patel-claim-life-in-danger-fbi-mar-a-lago-affidavit_n_630ac58ae4b07744a2f79726",
                Headline = "Kash Patel Says His Life's In Danger Because He's In FBI Affidavit",
                Category = "POLITICS",
                Description = "In a message posted on Truth Social, Patel called it a \\u201cvicious attack from DOJ/FBI.\\",
                Authors = new() { "Mary Papenfuss" },
                Date = new DateTime(2022, 8, 28),
                HasBeenScraped = false,
                Title = "dummy 2",
                Content = ""
            });


            return;
        }
    }
}
