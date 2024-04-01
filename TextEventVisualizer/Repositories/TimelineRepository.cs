using Microsoft.EntityFrameworkCore;
using TextEventVisualizer.Data;
using TextEventVisualizer.Models;

namespace TextEventVisualizer.Repositories
{
    public class TimelineRepository : ITimelineRepository
    {
        private readonly ApplicationDbContext _context;
        public TimelineRepository( ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> AddTimeline(Timeline timeline)
        {
            _context.Add(timeline);
            await _context.SaveChangesAsync();
            return timeline.Id;
        }

        public Task<List<TimelineBriefInfo>> GetAllTimelinesBriefInfoAsync()
        {
            return _context.Timelines
                .Select(timeline => new TimelineBriefInfo(timeline.Name, timeline.Id))
                .ToListAsync();
        }

        public Task<Timeline?> GetTimeline(int id)
        {
            return _context.Timelines
                .Include(timeline => timeline.TimelineChunks)
                    .ThenInclude(chunk => chunk.Events)
                .Include(timeline => timeline.TimelineChunks)
                    .ThenInclude(chunk => chunk.Article)
                .FirstOrDefaultAsync(timeline => timeline.Id == id);
        }


    }
}
