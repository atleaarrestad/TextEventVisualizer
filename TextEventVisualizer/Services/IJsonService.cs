using static TextEventVisualizer.Services.JsonService;

namespace TextEventVisualizer.Services
{
    public interface IJsonService
    {
        Task ExtractArticlesFromJsonFile(LoggerDelegate logger);
    }
}
