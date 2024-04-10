using Newtonsoft.Json;
using Polly;
using System.Text;
using TextEventVisualizer.Extentions;
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
                string responseContent = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
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

        public async Task<List<Event>> ExtractEventsFromText(string text, int desiredEventCount = 3)
        {
            var prompt = $@"Analyze the following text and extract the {desiredEventCount} most important events along with their associated dates or times. Format the response strictly as a JSON array, without any additional text, prefatory remarks, or concluding notes. If a date is not available for an event, leave it empty. Here is the text to analyze:

            {text}

            Expected JSON response format:

            [
                {{
                ""content"": ""[Event content here]"",
                ""timestamp"": ""[Event time here]""
                }}
            ]

            The response should be a JSON array, beginning and ending with the array brackets, and containing no additional text outside of this format.";

            var policy = Policy
                .Handle<Exception>(ex => IsTransientException(ex))
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromMilliseconds(250));

            return await policy.ExecuteAsync(async () =>
            {
                var result = await Ask(prompt);

                int startIndex = result.IndexOf('[');
                int endIndex = result.LastIndexOf(']');

                if (startIndex == -1 || endIndex == -1 || endIndex < startIndex)
                {
                    throw new InvalidOperationException("Valid JSON not found in the response string.");
                }

                var json = result.Substring(startIndex, endIndex - startIndex + 1);
                json = json.RemoveInvalidCharactersForJSON();
                var events = JsonConvert.DeserializeObject<List<Event>>(json);

                if (events.Count > desiredEventCount)
                {
                    events.RemoveRange(3, events.Count - 3);
                }
                return events;
            });
        }
        private static bool IsTransientException(Exception ex)
        {
            return ex is InvalidOperationException;
        }
    }
}
