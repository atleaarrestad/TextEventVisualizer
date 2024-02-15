using TextEventVisualizer.Models;
using TextEventVisualizer.Models.Request;
using TextEventVisualizer.Models.Response;

namespace TextEventVisualizer.Services
{
    public interface IEmbeddingService
    {
        Task SetupSchemaAsync();
        Task InsertDataAsync(string text, string originalId, EmbeddingCategory category);
        Task<EmbeddingQueryResponse> QueryDataAsync(EmbeddingQueryRequest request);
        Task<string> testHuggingFace(string input);
    }
}
