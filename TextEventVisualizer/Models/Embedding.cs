namespace TextEventVisualizer.Models
{
    public class Embedding
    {
        public string ArticleId { get; set; }
        public string Content { get; set; }

        public static Embedding FromArticle(Article article)
        {
            return new()
            {
                ArticleId = article.Id.ToString(),
                Content = article.Content,
  
            };
        }
    }

    

}
