namespace TextEventVisualizer.Services
{
    public interface IEmbeddingService
    {
        Task SetupSchemaAsync();
        Task InsertDataAsync(string originalId, string textContent);
        Task QueryDataAsync(string prompt);
    }
}
