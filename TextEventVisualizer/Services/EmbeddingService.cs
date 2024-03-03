using System.Net.Http;
using System;
using System.Text;
using System.Text.Json;
using TextEventVisualizer.Models;
using TextEventVisualizer.Models.Request;
using TextEventVisualizer.Models.Response;


namespace TextEventVisualizer.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly HttpClient client;
        private readonly IArticleService ArticleService;
        private readonly string weaviateEndpoint = "http://localhost:8080/v1";
        public EmbeddingService(IHttpClientFactory httpClientFactory, IArticleService articleService)
        {
            client = httpClientFactory.CreateClient();
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
                        dataType = new[] { "int" },
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
                HttpResponseMessage response = await client.PostAsync($"{weaviateEndpoint}/schema", content);
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

        public async Task InsertDataAsync(string text, int originalId, EmbeddingCategory category)
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

            var response = await client.PostAsync($"{weaviateEndpoint}/objects", content);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> ArticleExistsAsync(int originalId, EmbeddingCategory category)
        {
            string graphqlQuery = $@"
            {{
                ""query"": ""{{
                    Get {{
                        Embedding(
                            where: {{
                                operator: And,
                                operands: [
                                    {{
                                        path: [\""originalId\""],
                                        operator: Equal,
                                        valueNumber: {originalId}
                                    }},
                                    {{
                                        path: [\""category\""],
                                        operator: Equal,
                                        valueNumber: {(int)category}
                                    }}
                                ]
                            }},
                            limit: 1
                        ) {{
                            originalId
                        }}
                    }}
                }}""
            }}".Replace("\r", "").Replace("\n", " ").Replace("    ", "");

            var content = new StringContent(graphqlQuery, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{weaviateEndpoint}/graphql", content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"GraphQL query failed. Status: {response.StatusCode}. Response body: {await response.Content.ReadAsStringAsync()}");
                    return false;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody.Contains("originalId");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
                return false;
            }
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
                                valueNumber: {(int)request.Category}
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
                var response = await client.PostAsync($"{weaviateEndpoint}/graphql", content);
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

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

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
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return "";
        }

        public async Task<bool> Ping()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(weaviateEndpoint);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (HttpRequestException e)
            {
                return false;
            }
        }

        public string GetAPIEndpoint()
        {
            return weaviateEndpoint;
        }
    }
}
