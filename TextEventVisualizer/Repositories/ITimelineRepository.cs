using TextEventVisualizer.Models;

namespace TextEventVisualizer.Repositories
{
    public interface ITimelineRepository
    {
        Task<int> AddTimeline(Timeline timeline);
        Task<Timeline?> GetTimeline(int id);
    }
}
