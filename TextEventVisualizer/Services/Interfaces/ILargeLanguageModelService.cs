namespace TextEventVisualizer.Services
{
    public interface ILargeLanguageModelService
    {
        Task<string> Ask(string prompt);
    }
}
