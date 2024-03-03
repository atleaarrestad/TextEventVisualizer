namespace TextEventVisualizer.Services
{
    public interface IHuggingFaceService
    {
        Task<string> SummarizeText(string input);
    }
}
