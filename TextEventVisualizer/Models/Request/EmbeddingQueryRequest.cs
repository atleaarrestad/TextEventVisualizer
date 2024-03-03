namespace TextEventVisualizer.Models.Request
{
    public class EmbeddingQueryRequest
    {
        public List<string> Prompts { get; set; }
        public float Distance { get; set; } = 0.0f;
        public int Limit { get; set; }
        public Bias PositiveBias { get; set; }
        public Bias NegativeBias { get; set; }
        public EmbeddingCategory Category { get; set; }

        public EmbeddingQueryRequest()
        {
            Prompts = new();
            PositiveBias = new();
            NegativeBias = new();
            Limit = 20;
            Category = EmbeddingCategory.Article;
        }
    }
}
