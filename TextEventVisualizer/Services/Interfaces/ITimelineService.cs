using TextEventVisualizer.Models.Request;

namespace TextEventVisualizer.Services.Interfaces
{
    public interface ITimelineService
    {
        Task<int> GenerateTimeline(TimelineRequest timelineRequest);
    }
}
