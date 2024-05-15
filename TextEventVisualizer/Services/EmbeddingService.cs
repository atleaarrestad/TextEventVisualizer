using System.Text;
using System.Text.Json;
using TextEventVisualizer.Models;
using TextEventVisualizer.Models.Request;
using TextEventVisualizer.Models.Response;


namespace TextEventVisualizer.Services
{
    /// <summary>
    /// Service for managing embeddings, providing methods to interact with the Weaviate vector database.
    /// </summary>
    public class EmbeddingService : IEmbeddingService
    {
        private readonly HttpClient client;
        private readonly IArticleService ArticleService;
        private readonly string weaviateEndpoint = "http://localhost:8080/v1";

        /// <summary>
        /// Initializes a new instance of the EmbeddingService with specified HTTP client factory and article service.
        /// </summary>
        /// <param name="httpClientFactory">The factory to create HTTP client instances.</param>
        /// <param name="articleService">The service handling articles.</param>
        public EmbeddingService(IHttpClientFactory httpClientFactory, IArticleService articleService)
        {
            client = httpClientFactory.CreateClient();
            ArticleService = articleService;
        }

        /// <summary>
        /// Sets up the schema for embeddings in the Weaviate database asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation of creating a schema.</returns>
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

        /// <summary>
        /// Checks if the Embedding schema exists in the Weaviate database asynchronously.
        /// </summary>
        /// <returns>A boolean indicating the existence of the schema.</returns>
        public async Task<bool> SchemaExist()
        {
            try
            {
                //this will check to see if the Embedding schema exist, but will only return OK if data is also stored in this schema.
                HttpResponseMessage response = await client.GetAsync($"{weaviateEndpoint}/schema/Embedding");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Inserts embedding data asynchronously based on the provided text, original ID, and category.
        /// </summary>
        /// <param name="text">The content text to be embedded.</param>
        /// <param name="originalId">The original identifier of the content.</param>
        /// <param name="category">The category of the embedding.</param>
        /// <returns>A boolean indicating whether the insertion was successful.</returns>
        public async Task<bool> InsertDataAsync(string text, int originalId, EmbeddingCategory category)
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
            try
            {
                var response = await client.PostAsync($"{weaviateEndpoint}/objects", content);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return true;
            }
            catch (Exception)
            {
                return false;
            } 
        }


        /// <summary>
        /// Checks asynchronously if an article exists in the database by original ID and category.
        /// </summary>
        /// <param name="originalId">The original identifier of the article.</param>
        /// <param name="category">The category of the embedding.</param>
        /// <returns>A boolean indicating whether the article exists.</returns>
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

        /// <summary>
        /// Queries embedding data asynchronously based on specified criteria.
        /// </summary>
        /// <param name="request">The parameters defining the query, including biases, prompts, and category filtering.</param>
        /// <returns>A list of embeddings that match the query criteria.</returns>
        public async Task<List<Embedding>> QueryDataAsync(EmbeddingQueryRequest request)
        {
            string graphqlQuery = $@"
            {{
                ""query"": ""{{
                    Get {{
                        Embedding(
                            limit: {request.Limit},
                            nearText: {{
                                concepts: [\""{request.Prompts}\""],
                                distance: {request.Distance},
                                moveTo: {{
                                    concepts: [\""{request.PositiveBias.Concepts}\""],
                                    force: {request.PositiveBias.Force}
                                }},
                                moveAwayFrom: {{
                                    concepts: [\""{request.NegativeBias.Concepts}\""],
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
            try
            {
                var response = await client.PostAsync($"{weaviateEndpoint}/graphql", content);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"GraphQL query failed. Status: {response.StatusCode}. Response body: {error}");
                    return new();
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var embeddingQueryResult = JsonSerializer.Deserialize<EmbeddingQueryResponse>(responseBody);
                return embeddingQueryResult?.data?.Get.Embedding ?? new();

            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
                return new();
            }
        }

        /// <summary>
        /// Retrieves the count of embeddings in a specified category asynchronously.
        /// </summary>
        /// <param name="category">The category of embeddings to count.</param>
        /// <returns>The count of embedding entries within the specified category.</returns>
        public async Task<int> GetEmbeddingEntriesCountInCategory(EmbeddingCategory category)
        {
            var query = new
            {
                query = $@"{{
                Aggregate {{
                    Embedding(where: {{operator: Equal, path: [""category""], valueNumber: {(int)category}}}) {{
                        meta {{
                            count
                        }}
                    }}
                }}
            }}"
            };

            var queryJson = JsonSerializer.Serialize(query);
            var content = new StringContent(queryJson, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync($"{weaviateEndpoint}/graphql", content);
                string responseBody = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    var root = doc.RootElement;
                    var count = root.GetProperty("data").GetProperty("Aggregate").GetProperty("Embedding")[0].GetProperty("meta").GetProperty("count").GetInt32();
                    return count;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught: {e.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Checks if the Weaviate endpoint is reachable asynchronously.
        /// </summary>
        /// <returns>A boolean indicating whether the Weaviate endpoint is responsive.</returns>
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

        /// <summary>
        /// Returns the API endpoint used by the EmbeddingService.
        /// </summary>
        /// <returns>The URL of the Weaviate endpoint.</returns>
        public string GetAPIEndpoint()
        {
            return weaviateEndpoint;
        }
    }
}
