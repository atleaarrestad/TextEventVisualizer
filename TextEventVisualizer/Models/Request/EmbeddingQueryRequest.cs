namespace TextEventVisualizer.Models.Request
{
    public class EmbeddingQueryRequest
    {
        public string Prompts { get; set; } = string.Empty;
        public float Distance { get; set; } = 0.0f;
        public int Limit { get; set; } = 20;
        public Bias PositiveBias { get; set; } = new();
        public Bias NegativeBias { get; set; } = new();
        public EmbeddingCategory Category { get; set; } = EmbeddingCategory.Article;
    }
}
