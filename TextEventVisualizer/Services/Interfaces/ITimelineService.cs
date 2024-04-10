using System.Collections.ObjectModel;
using TextEventVisualizer.Models;
using TextEventVisualizer.Models.Request;

namespace TextEventVisualizer.Services
{
    public interface ITimelineService
    {
        Task<Timeline?> GenerateTimeline(TimelineRequest timelineRequest);
        Task<int> AddTimeline(Timeline timeline);
        Task<Timeline?> GetTimeline(int id);
        Task<List<TimelineBriefInfo>> GetAllTimelinesBriefInfoAsync();


        event Action OnTimelineGenerationStarted;
        event Action OnTimelineGenerationMessagesUpdated;
        event Action OnTimelineGenerationCompleted;
        Status TimelineGenerationStatus { get; set; }
        string TimelineGenerationName { get; set; }
        ReadOnlyCollection<string> GetTimelineGenerationMessages();
        void AddProgressMessage(string message);
        void ClearMessages();
    }
}
