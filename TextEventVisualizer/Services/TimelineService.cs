using TextEventVisualizer.Models.Request;
using TextEventVisualizer.Models;
using TextEventVisualizer.Repositories;
using System.Diagnostics;
using System.Collections.ObjectModel;
using TextEventVisualizer.Migrations;

namespace TextEventVisualizer.Services
{
    /// <summary>
    /// Service for managing and generating timelines based on text data analysis.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the TimelineService with dependency injection.
        /// </summary>
        /// <param name="serviceProvider">A service provider for resolving dependencies.</param>
        public TimelineService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Adds a new timeline to the database asynchronously.
        /// </summary>
        /// <param name="timeline">The timeline to add.</param>
        /// <returns>The ID of the added timeline.</returns>
        public async Task<int> AddTimeline(Timeline timeline)
        {
            using var scope = serviceProvider.CreateScope();
            var timelineRepository = scope.ServiceProvider.GetRequiredService<ITimelineRepository>();
            timeline.CreationDate = DateTime.UtcNow;
            return await timelineRepository.AddTimeline(timeline);
        }

        /// <summary>
        /// Retrieves a timeline by its ID asynchronously.
        /// </summary>
        /// <param name="Id">The ID of the timeline to retrieve.</param>
        /// <returns>The requested timeline if found, otherwise null.</returns>
        public async Task<Timeline?> GetTimeline(int Id)
        {
            using var scope = serviceProvider.CreateScope();
            var timelineRepository = scope.ServiceProvider.GetRequiredService<ITimelineRepository>();
            return await timelineRepository.GetTimeline(Id);
        }

        /// <summary>
        /// Generates a timeline based on a specified request asynchronously.
        /// </summary>
        /// <param name="timelineRequest">The parameters defining how the timeline should be generated.</param>
        /// <returns>The generated timeline if successful, otherwise null.</returns>
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
                    if (events.Count > timelineRequest.MaxEventCountForEachArticle)
                    {
                        events.RemoveRange(timelineRequest.MaxEventCountForEachArticle, events.Count - timelineRequest.MaxEventCountForEachArticle);
                    }

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

        /// <summary>
        /// Retrieves brief information for all timelines stored in the database asynchronously.
        /// </summary>
        /// <returns>A list of TimelineBriefInfo objects containing brief details of each timeline.</returns>
        public async Task<List<TimelineBriefInfo>> GetAllTimelinesBriefInfoAsync()
        {
            using var scope = serviceProvider.CreateScope();
            var timelineRepository = scope.ServiceProvider.GetRequiredService<ITimelineRepository>();
            return await timelineRepository.GetAllTimelinesBriefInfoAsync();
        }

        /// <summary>
        /// Adds a progress message to the timeline generation log.
        /// </summary>
        /// <param name="message">The message to add to the log.</param>
        public void AddProgressMessage(string message)
        {
            var elapsedTime = _stopwatch.Elapsed;
            var formattedMessage = $"(Elapsed: {elapsedTime.ToString(@"hh\:mm\:ss")}) {message}";
            _timelineGenerationMessages.Add(formattedMessage);
            OnTimelineGenerationMessagesUpdated?.Invoke();
        }

        /// <summary>
        /// Retrieves all logged messages for the timeline generation process as a read-only collection.
        /// </summary>
        /// <returns>A read-only collection of progress messages logged during timeline generation.</returns>
        public ReadOnlyCollection<string> GetTimelineGenerationMessages()
        {
            return _timelineGenerationMessages.AsReadOnly();
        }

        /// <summary>
        /// Clears all logged progress messages for the timeline generation.
        /// </summary>
        public void ClearMessages()
        {
            _timelineGenerationMessages.Clear();
            OnTimelineGenerationMessagesUpdated?.Invoke();
        }
    }
}
