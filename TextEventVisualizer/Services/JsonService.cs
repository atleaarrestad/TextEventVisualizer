using TextEventVisualizer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Diagnostics;

namespace TextEventVisualizer.Services
{
    public class JsonService : IJsonService
    {
        private readonly IArticleService articleService;
        public delegate Task LoggerDelegate(string message);

        public JsonService(IArticleService articleService)
        {
            this.articleService = articleService;
        }

        public async Task ExtractArticlesFromJsonFile(LoggerDelegate loggerDelegate)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            string jsonFilePath = "Data/news_articles.json";
            List<Article> articlesBuffer = new();
            int batchSize = 2500;
            using (StreamReader file = File.OpenText(jsonFilePath))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    try
                    {
                        JObject obj = JObject.Parse(line);

                        var date = ConvertAmericanEdtToUtc((obj["date"] ?? "").ToString());
                        if (date == null)
                            continue;

                        Article currentArticle = new Article
                        {
                            WebUrl = (obj["link"] ?? "").ToString(),
                            Headline = (obj["headline"] ?? "").ToString(),
                            Category = (obj["category"] ?? "").ToString(),
                            Description = (obj["short_description"] ?? "").ToString(),
                            Authors = (obj["authors"] ?? "").ToString(),
                            Date = date.Value,
                            Content = string.Empty,
                            HasBeenScraped = false,
                        };

                        if (IsArticleValid(currentArticle))
                            articlesBuffer.Add(currentArticle);

                        if (articlesBuffer.Count >= batchSize)
                        {
                            await articleService.AddArticleBatchAsync(articlesBuffer);
                            loggerDelegate?.Invoke($"added {batchSize} articles");
                            articlesBuffer.Clear();
                        }
                    }
                    catch (JsonReaderException ex)
                    {
                        loggerDelegate?.Invoke($"JSON Parsing Error: {ex.Message}");
                    }
                }

                if (articlesBuffer.Any())
                {
                    await articleService.AddArticleBatchAsync(articlesBuffer);
                    loggerDelegate?.Invoke($"added {articlesBuffer.Count} articles");
                }
                stopWatch.Stop();
                loggerDelegate?.Invoke($"Finished adding skeleton articles, elapsed time: {stopWatch.Elapsed}");
            }
        }

        private bool IsArticleValid(Article article)
        {
            return
                   !string.IsNullOrWhiteSpace(article.Headline)
                && !string.IsNullOrWhiteSpace(article.WebUrl)
                && !string.IsNullOrWhiteSpace(article.Category)
                && !string.IsNullOrWhiteSpace(article.Description);
        }

        private DateTime? ConvertAmericanEdtToUtc(string dateString)
        {
            DateTime edtDate;
            bool parseSuccess = DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out edtDate);

            if (!parseSuccess)
            {
                return null;
            }

            TimeZoneInfo edtZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime utcDate = TimeZoneInfo.ConvertTimeToUtc(edtDate, edtZone);

            return utcDate;
        }
    }
}
