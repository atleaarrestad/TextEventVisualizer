using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TextEventVisualizer.Models;
using TextEventVisualizer.Models.Request;
using TextEventVisualizer.Models.Response;


namespace TextEventVisualizer.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly HttpClient Client;
        private readonly IArticleService ArticleService;
        private readonly string weaviateEndpoint = "http://localhost:8080/v1";
        public EmbeddingService(IArticleService articleService)
        {
            Client = new HttpClient();
            ArticleService = articleService;
        }

        public async Task SetupSchemaAsync()
        {
            var schema = new
            {
                @class = "Embedding",
                description = "A generic class to store various types of embeddings with categorization, original ID, and content",
                properties = new object[]
                {
                    new
                    {
                        name = "category",
                        dataType = new[] { "int" },
                        description = "The numeric category of the embedding (from EmbeddingCategory enum)"
                    },
                    new
                    {
                        name = "originalId",
                        dataType = new[] { "string" },
                        description = "The original ID of the content"
                    },
                    new
                    {
                        name = "content",
                        dataType = new[] { "text" },
                        description = "The content to be stored and vectorized",
                        moduleConfig = new
                        {
                            text2vec_transformers = new { isVector = true }
                        }
                    }
                }
            };

            var jsonRequest = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await Client.PostAsync($"{weaviateEndpoint}/schema", content);
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Schema created successfully.");
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught: {e.Message}");
            }
        }

        public async Task InsertDataAsync(string text, string originalId, EmbeddingCategory category)
        {
            var data = new
            {
                @class = "Embedding",
                properties = new
                {
                    category = (int)category,
                    originalId,
                    content = text
                }
            };

            var jsonRequest = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync($"{weaviateEndpoint}/objects", content);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        public async Task<EmbeddingQueryResponse> QueryDataAsync(EmbeddingQueryRequest request)
        {

            var promptConcepts = string.Join(", ", request.Prompts.Select(p => $"\\\" {p} \\\""));
            var positiveConcepts = string.Join(", ", request.PositiveBias.Concepts.Select(p => $"\\\"{p}\\\""));
            var negativeConcepts = string.Join(", ", request.NegativeBias.Concepts.Select(p => $"\\\"{p}\\\""));

            string graphqlQuery = $@"
            {{
                ""query"": ""{{
                    Get {{
                        Embedding(
                            limit: {request.Limit},
                            nearText: {{
                                concepts: [{promptConcepts}],
                                distance: {request.Distance},
                                moveTo: {{
                                    concepts: [{positiveConcepts}],
                                    force: {request.PositiveBias.Force}
                                }},
                                moveAwayFrom: {{
                                    concepts: [{negativeConcepts}],
                                    force: {request.NegativeBias.Force}
                                }}
                            }},
                            where: {{
                                path: [\""category\""],
                                operator: Equal,
                                valueNumber: 0
                            }}
                        ) {{
                            category
                            originalId
                            content
                            _additional {{
                                certainty
                                distance
                            }}
                        }}
                    }}
                }}""
            }}"
            .Replace("\r", "").Replace("\n", " ").Replace("    ", "");

            Console.WriteLine(graphqlQuery);
            var content = new StringContent(graphqlQuery, Encoding.UTF8, "application/json");
            var embeddingQueryResult = new EmbeddingQueryResponse();
            try
            {
                var response = await Client.PostAsync($"{weaviateEndpoint}/graphql", content);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"GraphQL query failed. Status: {response.StatusCode}. Response body: {error}");
                    return embeddingQueryResult;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                embeddingQueryResult = JsonSerializer.Deserialize<EmbeddingQueryResponse>(responseBody);

            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }

            return embeddingQueryResult ?? new();
        }

        public async Task<string> testHuggingFace(string input)
        {
            string apiKey = "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";
            string apiUrl = "https://api-inference.huggingface.co/models/facebook/bart-large-cnn";

            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestData = new
            {
                inputs = input,
            };

            string jsonString = JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await Client.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return "";
        }
    }
}
