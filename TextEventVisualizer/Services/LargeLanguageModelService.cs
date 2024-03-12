
using Newtonsoft.Json;
using System.Text;
using TextEventVisualizer.Models;

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

        public Task<string> ExtractEventsFromText(string text)
        {
            var prompt = $@"Analyze the following text and extract the most important events along with their associated dates or times. Format the response strictly as a JSON array, without any additional text, prefatory remarks, or concluding notes. If a date is not available for an event, leave it empty. Here is the text to analyze:

            {text}

            Expected JSON response format:

            [
                {{
                ""event"": ""[Event description here]"",
                ""timestamp"": ""[Event time here]""
                }}
            ]

            The response should be a JSON array, beginning and ending with the array brackets, and containing no additional text outside of this format.";

            return Ask(prompt) ;
        }
    }
}
