using TextEventVisualizer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Diagnostics;

namespace TextEventVisualizer.Services
{
    /// <summary>
    /// Provides services to extract and manage articles from JSON data files.
    /// </summary>
    public class JsonService : IJsonService
    {
        private readonly IArticleService articleService;

        /// <summary>
        /// Delegate for logging messages during processing.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public delegate Task LoggerDelegate(string message);

        /// <summary>
        /// Initializes a new instance of the JsonService class.
        /// </summary>
        /// <param name="articleService">Service used for article data operations.</param>
        public JsonService(IArticleService articleService)
        {
            this.articleService = articleService;
        }

        /// <summary>
        /// Asynchronously extracts articles from a JSON file and adds them to the database in batches.
        /// </summary>
        /// <param name="loggerDelegate">A delegate to log progress and errors.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Validates if the given article has all necessary information filled.
        /// </summary>
        /// <param name="article">The article to validate.</param>
        /// <returns>True if the article is valid, otherwise false.</returns>
        private bool IsArticleValid(Article article)
        {
            return
                   !string.IsNullOrWhiteSpace(article.Headline)
                && !string.IsNullOrWhiteSpace(article.WebUrl)
                && !string.IsNullOrWhiteSpace(article.Category)
                && !string.IsNullOrWhiteSpace(article.Description);
        }

        /// <summary>
        /// Converts a date string from American EDT to UTC.
        /// </summary>
        /// <param name="dateString">The date string to convert.</param>
        /// <returns>The UTC date if conversion is successful, otherwise null.</returns>
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
