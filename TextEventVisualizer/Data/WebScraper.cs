using System;
using System.Net.Http;
using HtmlAgilityPack;

namespace TextEventVisualizer.Data
{
    public static class WebScraper
    {

        public static async Task<string> Scrape()
        {
            string url = "https://www.huffpost.com/entry/russian-controlled-ukrainian-regions-referendum_n_6329d53ae4b07198f012f023";
            string htmlClassCriteria = "primary-cli cli cli-text ";

            HttpClient httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var desiredNodes = htmlDoc.DocumentNode.SelectNodes($".//div[@class='{htmlClassCriteria}']");
            foreach ( var desiredNode in desiredNodes )
            {
                Console.WriteLine(desiredNode.InnerText);
            }

            return "";
        }

    }
}
