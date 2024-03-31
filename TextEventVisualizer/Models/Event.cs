namespace TextEventVisualizer.Models
{
    public class Event
    {
        public int Id { get; set; } // Primary key, set by entity framework
        public string Content { get; set; }
        public string Timestamp { get; set; }
        public int TimelineChunkId { get; set; } // foreign key, set by entity framework
        public TimelineChunk TimelineChunk { get; set; } // navigation property, set by entity framework 
    }
}
