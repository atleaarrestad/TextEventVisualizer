using TextEventVisualizer.Models.Request;
using TextEventVisualizer.Services;
using TextEventVisualizer.Models;
using System;
using TextEventVisualizer.Repositories;

namespace TextEventVisualizer.Services
{
    public class TimelineService : ITimelineService
    {
        private readonly IEmbeddingService embeddingService;
        private readonly ILargeLanguageModelService largeLanguageModelService;
        private readonly ITimelineRepository timelineRepository;
        public TimelineService(IEmbeddingService embeddingService, ILargeLanguageModelService largeLanguageModelService, ITimelineRepository timelineRepository)
        {
            this.embeddingService = embeddingService;
            this.largeLanguageModelService = largeLanguageModelService;
            this.timelineRepository = timelineRepository;
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
            var queryRequest = new EmbeddingQueryRequest()
            {
                Category = timelineRequest.Category,
                Distance = timelineRequest.MaxArticleClusterSearchDistance,
                Limit = timelineRequest.MaxArticleCount,
                NegativeBias = timelineRequest.ArticleClusterSearchNegativeBias,
                PositiveBias = timelineRequest.ArticleClusterSearchPositiveBias,
                Prompts = timelineRequest.ArticleClusterSearch.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList()
            };

            var embeddingQueryResult = await embeddingService.QueryDataAsync(queryRequest);
            var timeline = new Timeline();
            timeline.TimelineRequest = timelineRequest;

            foreach (var embedding in embeddingQueryResult)
            {
                var events = await largeLanguageModelService.ExtractEventsFromText(embedding.content, timelineRequest.DesiredEventCountForEachArticle);
                if (events.Count > 0)
                {
                    timeline.TimelineChunks.Add(new TimelineChunk()
                    {
                        Events = events,
                        ArticleId = embedding.originalId,
                    });
                }
            }
            return timeline;
        }
    }
}
