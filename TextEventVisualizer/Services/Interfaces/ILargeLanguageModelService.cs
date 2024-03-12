namespace TextEventVisualizer.Services
{
    public interface ILargeLanguageModelService
    {
        Task<string> Ask(string prompt);
        Task<string> ExtractEventsFromText(string text);
    }
}
