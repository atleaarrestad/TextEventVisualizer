using System.Collections.ObjectModel;
using System.Diagnostics;
using TextEventVisualizer.Models;

namespace TextEventVisualizer.Services
{
    public class TimelineCreationTrackerService : ITimelineCreationTrackerService
    {
        private List<string> _messages = new();
        private string _name = "Empty";
        private Status _status = Status.NotStarted;
        private Stopwatch _stopwatch = new();

        public event Action OnChanged;
        public event Action OnCompleted;

        public void AddMessage(string message)
        {
            var elapsedTime = _stopwatch.Elapsed;
            var formattedMessage = $"(Elapsed: {elapsedTime.ToString(@"hh\:mm\:ss")}) {message}";
            _messages.Add(formattedMessage);
            OnChanged?.Invoke();
        }
        public ReadOnlyCollection<string> GetMessages()
        {
            return _messages.AsReadOnly();
        }
        public void ClearMessages()
        {
            _messages.Clear();
            OnChanged?.Invoke();
        }
        public void StartTracking()
        {
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        public void EndTracking()
        {
            _stopwatch.Stop();
        }

        public Status Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnChanged?.Invoke();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnChanged?.Invoke();
                }
            }
        }
    }

}
