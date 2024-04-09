using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TextEventVisualizer.Models;

namespace TextEventVisualizer.Services
{
    public interface ITimelineCreationTrackerService
    {
        event Action OnChanged;
        event Action OnCompleted;
        Status Status { get; set; }
        string Name { get; set; }
        void AddMessage(string message);
        void ClearMessages();
        ReadOnlyCollection<string> GetMessages();
        void StartTracking();
        void EndTracking();

    }
}
