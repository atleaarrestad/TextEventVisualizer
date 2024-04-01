namespace TextEventVisualizer.Models
{
    public class Event
    {
        public string Content { get; set; }
        public string Timestamp { get; set; }
        public int Id { get; set; } // Primary key, set by entity framework
        public int TimelineChunkId { get; set; } // foreign key, set by entity framework
        public TimelineChunk TimelineChunk { get; set; } // navigation property, set by entity framework 
    }
}
