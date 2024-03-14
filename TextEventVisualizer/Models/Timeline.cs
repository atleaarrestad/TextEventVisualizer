namespace TextEventVisualizer.Models
{
    public class Timeline
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public List<TimelineChunk> TimelineChunks { get; set; }
    }

    public class TimelineChunk
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public List<Event> Events { get; set; }
    }
}
