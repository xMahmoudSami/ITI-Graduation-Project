using System.Text;
using System.Text.Json;
using Inventory_Management_System.Models;

namespace Inventory_Management_System.Services
{
    public class AIService : IAIService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public AIService(
            ApplicationDbContext context,
            HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<string> AskAsync(string userQuery)
        {
            // Get inventory data
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

            var prompt = $"""
                You are an inventory management assistant.

                Answer the user's question using ONLY the inventory data provided below.

                Do not invent or assume any product, quantity, price, or other information.

                If the requested information is not available in the inventory data,
                clearly say that the information is not available.

                Inventory Data:
                {inventoryData}

                User Question:
                {userQuery}

                Give a short, clear and helpful answer.
                """;

            var requestBody = new
            {
                model = "llama3.2:3b",
                prompt = prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "http://localhost:11434/api/generate",
                content);

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(responseJson);

            var aiResponse = document.RootElement
                .GetProperty("response")
                .GetString();

            // Save chat log
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