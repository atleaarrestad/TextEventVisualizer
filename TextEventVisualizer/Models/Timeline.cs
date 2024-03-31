using TextEventVisualizer.Models.Request;

namespace TextEventVisualizer.Models
{
    public class Timeline
    {
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public List<TimelineChunk> TimelineChunks { get; set; } = [];
        public int TimelineRequestId { get; set; } // foreign key, set by entity framework
        public TimelineRequest TimelineRequest { get; set; } // navigation property, set by entity framework
        public int Id { get; set; } // Primary key, set by entity framework
    }

    public class TimelineChunk
    {
        public int ArticleId { get; set; }
        public List<Event> Events { get; set; } = [];
        public int Id { get; set; } // Primary key, set by entity framework
        public int TimelineId { get; set; } // foreign key, set by entity framework
        public Timeline Timeline { get; set; } // navigation property, set by entity framework 
    }
}
