﻿namespace TextEventVisualizer.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string WebUrl { get; set; }
        public string Headline { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public List<string> Authors { get; set; }
        public DateTime Date { get; set; }
        public bool HasBeenScraped { get; set; }
    }

}