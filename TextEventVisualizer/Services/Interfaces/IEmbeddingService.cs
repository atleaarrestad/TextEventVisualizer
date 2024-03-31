using TextEventVisualizer.Models;
using TextEventVisualizer.Models.Request;
using TextEventVisualizer.Models.Response;

namespace TextEventVisualizer.Services
{
    public interface IEmbeddingService
    {
        Task SetupSchemaAsync();
        Task<bool> SchemaExist();
        Task<bool> InsertDataAsync(string text, int originalId, EmbeddingCategory category);
        Task<bool> ArticleExistsAsync(int originalId, EmbeddingCategory category);
        Task<List<Embedding>> QueryDataAsync(EmbeddingQueryRequest request);
        Task<bool> Ping();
        string GetAPIEndpoint();
        Task<int> GetEmbeddingEntriesCountInCategory(EmbeddingCategory category);
    }
}
