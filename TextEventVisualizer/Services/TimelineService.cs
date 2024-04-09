using TextEventVisualizer.Models.Request;
using TextEventVisualizer.Services;
using TextEventVisualizer.Models;
using System;
using TextEventVisualizer.Repositories;
using System.Diagnostics;

namespace TextEventVisualizer.Services
{
    public class TimelineService : ITimelineService
    {
        private readonly IEmbeddingService embeddingService;
        private readonly ILargeLanguageModelService largeLanguageModelService;
        private readonly ITimelineRepository timelineRepository;
        private readonly ITimelineCreationTrackerService timelineCreationTrackerService;
        public TimelineService(IEmbeddingService embeddingService,
            ILargeLanguageModelService largeLanguageModelService,
            ITimelineRepository timelineRepository,
            ITimelineCreationTrackerService operationTrackerService)
        {
            this.embeddingService = embeddingService;
            this.largeLanguageModelService = largeLanguageModelService;
            this.timelineRepository = timelineRepository;
            this.timelineCreationTrackerService = operationTrackerService;
        }

        public Task<int> AddTimeline(Timeline timeline)
        {
            timeline.CreationDate = DateTime.Now.ToUniversalTime();
            return timelineRepository.AddTimeline(timeline);
        }

        public Task<Timeline?> GetTimeline(int Id)
        {
            return timelineRepository.GetTimeline(Id);
        }

        public async Task<Timeline> GenerateTimeline(TimelineRequest timelineRequest)
        {
            timelineCreationTrackerService.ClearMessages();
            timelineCreationTrackerService.StartTracking();
            timelineCreationTrackerService.Name = timelineRequest.Name;
            timelineCreationTrackerService.AddMessage("Started new timeline generation");
            timelineCreationTrackerService.Status = Status.InProgess;

            var queryRequest = new EmbeddingQueryRequest()
            {
                Category = timelineRequest.Category,
                Distance = timelineRequest.MaxArticleClusterSearchDistance,
                Limit = timelineRequest.MaxArticleCount,
                NegativeBias = timelineRequest.ArticleClusterSearchNegativeBias,
                PositiveBias = timelineRequest.ArticleClusterSearchPositiveBias,
                Prompts = timelineRequest.ArticleClusterSearch
            };

            timelineCreationTrackerService.AddMessage("Querying for embeddings");

            var embeddingQueryResult = await embeddingService.QueryDataAsync(queryRequest);
            if (embeddingQueryResult.Count > 0)
            {
                timelineCreationTrackerService.AddMessage("Successfully queried vector database");
            }
            else
            {
                timelineCreationTrackerService.AddMessage("No results from vector database query");
                timelineCreationTrackerService.Status = Status.Failed;
                timelineCreationTrackerService.EndTracking();
            }


            var timeline = new Timeline();
            timeline.TimelineRequest = timelineRequest;
            timeline.Name = timelineRequest.Name;

            foreach (var (embedding, index) in embeddingQueryResult.Select((value, index) => (value, index)))
            {
                timelineCreationTrackerService.AddMessage($"Embedding {index + 1}/{embeddingQueryResult.Count} Extracting {timelineRequest.DesiredEventCountForEachArticle} events");
                var events = await largeLanguageModelService.ExtractEventsFromText(embedding.content, timelineRequest.DesiredEventCountForEachArticle);
                if (events.Count > 0)
                {
                    timeline.TimelineChunks.Add(new TimelineChunk()
                    {
                        Events = events,
                        ArticleId = embedding.originalId,
                    });
                    timelineCreationTrackerService.AddMessage($"Embedding {index + 1}/{embeddingQueryResult.Count} Found {events.Count} events");
                }
                else
                {
                    timelineCreationTrackerService.AddMessage($"No valid events from embedding {index + 1}/{embeddingQueryResult.Count}. Skipping this one");
                }
            }
            timelineCreationTrackerService.Status = Status.Completed;
            timelineCreationTrackerService.AddMessage("Completed");
            timelineCreationTrackerService.EndTracking();
            return timeline;
        }

        public Task<List<TimelineBriefInfo>> GetAllTimelinesBriefInfoAsync()
        {
            return timelineRepository.GetAllTimelinesBriefInfoAsync();
        }
    }
}
