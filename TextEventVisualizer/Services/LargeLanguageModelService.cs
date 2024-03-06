
using Newtonsoft.Json;
using System.Text;

namespace TextEventVisualizer.Services
{
    public class LargeLanguageModelService : ILargeLanguageModelService
    {
        private readonly HttpClient client;
        public LargeLanguageModelService(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(1000);
        }
        public async Task<string> Ask(string prompt)
        {
            var payload = new
            {
                model = "llama2",
                prompt = prompt,
                stream = false
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:11434/api/generate", content);

                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();
                var parsedResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                string result = parsedResponse.response;
                return result;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error sending request: {e.Message}");
                return null;
            }
        }
    }
}
