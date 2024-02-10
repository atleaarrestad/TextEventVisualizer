using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
                @class = "Article",
                description = "A class to store articles with ID and content",
                properties = new Object[]
                {
                new
                {
                    name = "originalId",
                    dataType = new[] { "string" },
                    description = "The original ID of the article"
                },
                new
                {
                    name = "textContent",
                    dataType = new[] { "text" },
                    description = "The content of the article",
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

        public async Task InsertDataAsync(string originalId, string textContent)
        {
            var data = new
            {
                @class = "Article",
                properties = new
                {
                    originalId,
                    textContent
                }
            };

            var jsonRequest = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync($"{weaviateEndpoint}/objects", content);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        public async Task QueryDataAsync(string prompt)
        {
            string graphqlQuery = $@"
            {{
                ""query"": ""{{
                    Get {{
                        Article(nearText: {{
                            concepts: [\""{prompt}\""],
                            moveTo: {{
                                concepts: [\""positive\""],
                                force: 0.85
                            }},
                            moveAwayFrom: {{
                                concepts: [\""negative\""],
                                force: 0.45
                            }}
                        }}) {{
                            originalId
                            textContent
                            _additional {{
                                certainty
                                distance
                            }}
                        }}
                    }}
                }}""
            }}".Replace("\r", "").Replace("\n", " ").Replace("    ", "");

            var content = new StringContent(graphqlQuery, Encoding.UTF8, "application/json");

            try
            {
                var response = await Client.PostAsync($"{weaviateEndpoint}/graphql", content);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"GraphQL query failed. Status: {response.StatusCode}. Response body: {error}");
                    return;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var queryResult = JsonSerializer.Deserialize<GraphQLResponse>(responseBody);

                Console.WriteLine("Query successful. Response:");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
        }




    }
}
