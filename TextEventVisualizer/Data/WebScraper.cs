using System.Text;
using HtmlAgilityPack;

namespace TextEventVisualizer.Data
{
    /// <summary>
    /// Static class for web scraping operations.
    /// </summary>
    public static class WebScraper
    {
        /// <summary>
        /// Asynchronously scrapes the specified huffpost URL and extracts the article contents.
        /// </summary>
        /// <param name="webUrl">The URL of the web page to scrape.</param>
        /// <returns>A string containing the article content.</returns>
        public static async Task<string> Scrape(string webUrl)
        {
            string htmlClassCriteria = "primary-cli cli cli-text ";
            HttpClient httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(webUrl);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var articleText = new StringBuilder();
            var desiredNodes = htmlDoc.DocumentNode.SelectNodes($".//div[@class='{htmlClassCriteria}']");
            foreach ( var desiredNode in desiredNodes )
            {
                articleText.AppendLine(desiredNode.InnerText);
            }

            return articleText.ToString();
        }

    }
}
