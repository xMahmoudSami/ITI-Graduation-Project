using System.Text;
using System.Text.Json;
using Inventory_Management_System.Models;

namespace Inventory_Management_System.Services
{
    public class AIService : IAIService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AIService(ApplicationDbContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> AskAsync(string userQuery)
        {
            var apiKey = _configuration["Groq:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "Groq API key is not configured.";
            }

            var products = _context.Products
                .Select(p => new
                {
                    p.ProductName,
                    p.StockQuantity,
                    p.LowStockThreshold,
                    p.UnitPrice
                })
                .ToList();

            var inventoryData = JsonSerializer.Serialize(products);

            var systemPrompt = $"""
                You are an inventory management assistant.

                Answer the user's question using ONLY the inventory data provided below.

                Do not invent or assume any product, quantity, price, or other information.

                If the requested information is not available in the inventory data,
                clearly say that the information is not available.

                Inventory Data:
                {inventoryData}
                """;

            var requestBody = new
            {
                model = "openai/gpt-oss-20b",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = systemPrompt
                    },
                    new
                    {
                        role = "user",
                        content = userQuery
                    }
                },
                temperature = 0.2,
                max_completion_tokens = 500,
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.groq.com/openai/v1/chat/completions");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    apiKey);

            request.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);

            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"Groq API error: {response.StatusCode}";
            }

            using var document = JsonDocument.Parse(responseJson);

            var aiResponse = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            var chatLog = new AIChatLog
            {
                UserQuery = userQuery,
                AIResponse = aiResponse ?? "No response generated.",
                CreatedAt = DateTime.Now
            };

            _context.AIChatLogs.Add(chatLog);
            await _context.SaveChangesAsync();

            return aiResponse ?? "No response generated.";
        }
    }
}
