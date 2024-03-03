namespace TextEventVisualizer.Models.Response
{
    public class EmbeddingQueryResponse
    {
        public Data? data { get; set; }
    }

    public class Additional
    {
        public double certainty { get; set; }
        public double distance { get; set; }
    }

    public class Embedding
    {
        public Additional _additional { get; set; }
        public EmbeddingCategory category { get; set; }
        public int originalId { get; set; }
        public string content { get; set; }
    }

    public class Data
    {
        public Get Get { get; set; }
    }

    public class Get
    {
        public List<Embedding> Embedding { get; set; }
    }
}
