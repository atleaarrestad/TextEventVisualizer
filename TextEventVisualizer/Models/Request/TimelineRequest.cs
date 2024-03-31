namespace TextEventVisualizer.Models.Request
{
    public class TimelineRequest
    {
        public int Id { get; set; } // set by entity framework
        public Timeline Timeline { get; set; } // set by entity framework, navigation property 
        public string ArticleClusterSearch { get; set; } = string.Empty;
        public float MaxArticleClusterSearchDistance { get; set; } = 1.0f;
        public float MaxDistanceDeltaForArticles { get; set; } = 0.5f;
        public Bias? ArticleClusterSearchPositiveBias { get; set; } = null;
        public Bias? ArticleClusterSearchNegativeBias { get; set; } = null;
        public int MaxEventCountForEachArticle { get; set; } = 3;
        public int DesiredEventCountForEachArticle { get; set; } = 3;
        public int MaxArticleCount { get; set; } = 10;
        public EmbeddingCategory Category { get; set; } = EmbeddingCategory.Article;



        public TimelineRequest(string articleClusterSearch)
        {
            ArticleClusterSearch = articleClusterSearch;
        }
    }
}
