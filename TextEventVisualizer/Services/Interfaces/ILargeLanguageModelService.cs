using TextEventVisualizer.Models;

namespace TextEventVisualizer.Services
{
    public interface ILargeLanguageModelService
    {
        Task<string> Ask(string prompt);
        Task<List<Event>> ExtractEventsFromText(string text, int desiredEventCount = 3);
    }
}
