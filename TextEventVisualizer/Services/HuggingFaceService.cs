using System.Text;
using System.Text.Json;

namespace TextEventVisualizer.Services
{
    public class HuggingFaceService : IHuggingFaceService
    {
        private readonly HttpClient client;
        private readonly string apiKey = "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"; //yes this is not safe, but this key is not critical. can implement better solution later
        private readonly string apiUrl = "https://api-inference.huggingface.co/models/facebook/bart-large-cnn";
        public HuggingFaceService(IHttpClientFactory clientFactory)
        {
            client = clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }
        public async Task<string> SummarizeText(string input)
        {
            var requestData = new
            {
                inputs = input,
            };

            string jsonString = JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var summaries = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(responseBody);
                if (summaries != null && summaries.Count > 0 && summaries[0].ContainsKey("summary_text"))
                {
                    return summaries[0]["summary_text"];
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Message :{0} ", e.Message);
                throw;
            }

        }
    }
}
