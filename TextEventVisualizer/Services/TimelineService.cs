using TextEventVisualizer.Models.Request;
using TextEventVisualizer.Models;
using TextEventVisualizer.Repositories;
using System.Diagnostics;
using System.Collections.ObjectModel;
using TextEventVisualizer.Migrations;

namespace TextEventVisualizer.Services
{
    public class TimelineService : ITimelineService
    {
        // requires serviceprovider to resolve dependencies because this service is a singleton
        private readonly IServiceProvider serviceProvider;

        // For status updates while creating timeline
        public event Action OnTimelineGenerationStarted;
        public event Action OnTimelineGenerationMessagesUpdated;
        public event Action OnTimelineGenerationCompleted;
        private Stopwatch _stopwatch = new();
        private List<string> _timelineGenerationMessages = new();
        public Status TimelineGenerationStatus { get; set; }
        public string TimelineGenerationName { get; set; }

        public TimelineService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<int> AddTimeline(Timeline timeline)
        {
            using var scope = serviceProvider.CreateScope();
            var timelineRepository = scope.ServiceProvider.GetRequiredService<ITimelineRepository>();
            timeline.CreationDate = DateTime.UtcNow;
            return await timelineRepository.AddTimeline(timeline);
        }

        public async Task<Timeline?> GetTimeline(int Id)
        {
            using var scope = serviceProvider.CreateScope();
            var timelineRepository = scope.ServiceProvider.GetRequiredService<ITimelineRepository>();
            return await timelineRepository.GetTimeline(Id);
        }

        public async Task<Timeline?> GenerateTimeline(TimelineRequest timelineRequest)
        {
            ClearMessages();
            _stopwatch.Reset();
            _stopwatch.Start();
            TimelineGenerationName = timelineRequest.Name;
            TimelineGenerationStatus = Status.InProgess;
            AddProgressMessage("Started new timeline generation");
            OnTimelineGenerationStarted?.Invoke();

            var queryRequest = new EmbeddingQueryRequest()
            {
                Category = timelineRequest.Category,
                Distance = timelineRequest.MaxArticleClusterSearchDistance,
                Limit = timelineRequest.MaxArticleCount,
                NegativeBias = timelineRequest.ArticleClusterSearchNegativeBias,
                PositiveBias = timelineRequest.ArticleClusterSearchPositiveBias,
                Prompts = timelineRequest.ArticleClusterSearch
            };

            using var scope = serviceProvider.CreateScope();
            var timelineRepository = scope.ServiceProvider.GetRequiredService<ITimelineRepository>();
            var embeddingService = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();
            var largeLanguageModelService = scope.ServiceProvider.GetRequiredService<ILargeLanguageModelService>();

            AddProgressMessage("Querying for embeddings");
            var embeddingQueryResult = await embeddingService.QueryDataAsync(queryRequest);

            if (embeddingQueryResult.Count > 0)
            {
                AddProgressMessage("Successfully queried vector database");
            }
            else
            {
                AddProgressMessage("No results from vector database query");
                AddProgressMessage("Not able to create timeline");
                TimelineGenerationStatus = Status.Failed;
                _stopwatch.Stop();
                OnTimelineGenerationCompleted?.Invoke();
                return null;
            }

            var timeline = new Timeline();
            timeline.TimelineRequest = timelineRequest;
            timeline.Name = timelineRequest.Name;

            foreach (var (embedding, index) in embeddingQueryResult.Select((value, index) => (value, index)))
            {
                AddProgressMessage($"Embedding {index + 1}/{embeddingQueryResult.Count} Extracting {timelineRequest.DesiredEventCountForEachArticle} events");

                List<Event> events;
                try
                {
                    events = await largeLanguageModelService.ExtractEventsFromText(embedding.content, timelineRequest.DesiredEventCountForEachArticle);
                }
                catch
                {
                    events = [];
                }

                if (events.Count > 0)
                {
                    timeline.TimelineChunks.Add(new TimelineChunk()
                    {
                        Events = events,
                        ArticleId = embedding.originalId,
                    });
                    AddProgressMessage($"Embedding {index + 1}/{embeddingQueryResult.Count} Found {events.Count} events");
                }
                else
                {
                    AddProgressMessage($"No valid events from embedding {index + 1}/{embeddingQueryResult.Count}. Skipping this one");
                }
            }
            TimelineGenerationStatus = Status.Completed;
            AddProgressMessage("Completed");
            _stopwatch.Stop();
            OnTimelineGenerationCompleted?.Invoke();
            return timeline;
        }

        public async Task<List<TimelineBriefInfo>> GetAllTimelinesBriefInfoAsync()
        {
            using var scope = serviceProvider.CreateScope();
            var timelineRepository = scope.ServiceProvider.GetRequiredService<ITimelineRepository>();
            return await timelineRepository.GetAllTimelinesBriefInfoAsync();
        }

        public void AddProgressMessage(string message)
        {
            var elapsedTime = _stopwatch.Elapsed;
            var formattedMessage = $"(Elapsed: {elapsedTime.ToString(@"hh\:mm\:ss")}) {message}";
            _timelineGenerationMessages.Add(formattedMessage);
            OnTimelineGenerationMessagesUpdated?.Invoke();
        }
        public ReadOnlyCollection<string> GetTimelineGenerationMessages()
        {
            return _timelineGenerationMessages.AsReadOnly();
        }
        public void ClearMessages()
        {
            _timelineGenerationMessages.Clear();
            OnTimelineGenerationMessagesUpdated?.Invoke();
        }
    }
}
