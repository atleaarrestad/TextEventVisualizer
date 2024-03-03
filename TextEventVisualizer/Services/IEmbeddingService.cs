using TextEventVisualizer.Models;
using TextEventVisualizer.Models.Request;
using TextEventVisualizer.Models.Response;

namespace TextEventVisualizer.Services
{
    public interface IEmbeddingService
    {
        Task SetupSchemaAsync();
        Task InsertDataAsync(string text, int originalId, EmbeddingCategory category);
        Task<bool> ArticleExistsAsync(int originalId, EmbeddingCategory category);
        Task<EmbeddingQueryResponse> QueryDataAsync(EmbeddingQueryRequest request);
        Task<bool> Ping();
        string GetAPIEndpoint();
    }
}
