namespace TextEventVisualizer.Models.Request
{
    public class TimelineRequest
    {
        public string ArticleClusterSearch { get; set; } = string.Empty;
        public float MaxArticleClusterSearchDistance { get; set; } = 1.0f;
        public float MaxDistanceDeltaForArticles { get; set; } = 0.5f;
        public string ArticleClusterSearchPositiveBias { get; set; } = string.Empty;
        public float ArticleClusterSearchPositiveBiasWeight { get; set; } = 0.5f;
        public string ArticleClusterSearchNegativeBias { get; set; } = string.Empty;
        public float ArticleClusterSearchNegativeBiasWeight { get; set; } = 0.5f;
        public int MaxEventCountForEachArticle { get; set; } = 3;
        public int DesiredEventCountForEachArticle { get; set; } = 3;
        public int MaxArticleCount { get; set; } = 10;
        


        public TimelineRequest(string articleClusterSearch)
        {
            ArticleClusterSearch = articleClusterSearch;
        }
    }
}
