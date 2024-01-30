using System.Text;
using HtmlAgilityPack;

namespace TextEventVisualizer.Data
{
    public static class WebScraper
    {

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
