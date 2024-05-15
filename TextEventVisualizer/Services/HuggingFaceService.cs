using System.Text;
using System.Text.Json;

namespace TextEventVisualizer.Services
{
    /// <summary>
    /// Service for interacting with the Hugging Face API to perform text summarization.
    /// </summary>
    public class HuggingFaceService : IHuggingFaceService
    {
        private readonly HttpClient client;
        private readonly string apiKey = "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"; //yes this is not safe, but this key is not critical. can implement better solution later
        private readonly string apiUrl = "https://api-inference.huggingface.co/models/facebook/bart-large-cnn";

        /// <summary>
        /// Initializes a new instance of the HuggingFaceService class.
        /// </summary>
        /// <param name="clientFactory">The HTTP client factory used to create HTTP client instances.</param>
        public HuggingFaceService(IHttpClientFactory clientFactory)
        {
            client = clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        /// <summary>
        /// Summarizes the provided text using the Hugging Face API's BART large CNN model.
        /// </summary>
        /// <param name="input">The text to summarize.</param>
        /// <returns>A summarized version of the input text if successful, otherwise an empty string.</returns>
        /// <exception cref="Exception">Throws an exception if the API call fails.</exception>
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
