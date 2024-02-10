namespace TextEventVisualizer.Models.Response
{

    public class Additional
    {
        public double certainty { get; set; }
        public double distance { get; set; }
    }

    public class Article
    {
        public Additional _additional { get; set; }
        public string originalId { get; set; }
        public string textContent { get; set; }
    }

    public class Data
    {
        public Get Get { get; set; }
    }

    public class Get
    {
        public List<Article> Article { get; set; }
    }

    public class GraphQLResponse
    {
        public Data data { get; set; }
    }

}
