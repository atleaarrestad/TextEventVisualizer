using System.ComponentModel.DataAnnotations;
namespace TextEventVisualizer.Models.Request
{
    public class TimelineRequest
    {
        [Required(ErrorMessage = "The timeline name is required.")]
        public string Name { get; set; } = "Example timeline";
        public string ArticleClusterSearch { get; set; } = "covid19, corona virus";
        public float MaxArticleClusterSearchDistance { get; set; } = 1.0f;
        public float MaxDistanceDeltaForArticles { get; set; } = 0.5f;
        public Bias ArticleClusterSearchPositiveBias { get; set; } = new() { Concepts = "Pandemic", Force = .5f };
        public Bias ArticleClusterSearchNegativeBias { get; set; } = new() { Concepts = "Politics", Force = .5f };
        public int MaxEventCountForEachArticle { get; set; } = 3;
        public int DesiredEventCountForEachArticle { get; set; } = 3;
        public int MaxArticleCount { get; set; } = 6;
        public EmbeddingCategory Category { get; set; } = EmbeddingCategory.Article;
        public int Id { get; set; } // set by entity framework
        public Timeline Timeline { get; set; } // set by entity framework, navigation property 
    }
}