using TextEventVisualizer.Models;
using TextEventVisualizer.Models.Request;

namespace TextEventVisualizer.Services
{
    public interface ITimelineService
    {
        Task<Timeline> GenerateTimeline(TimelineRequest timelineRequest);
        Task<int> AddTimeline(Timeline timeline);
        Task<Timeline?> GetTimeline(int id);
    }
}
