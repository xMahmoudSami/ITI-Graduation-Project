using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Inventory_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Services
{
    public class AIService : IAIService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AIService(
            ApplicationDbContext context,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // =====================================================
        // MAIN AI METHOD
        // =====================================================
        public async Task<string> AskAsync(string userQuery)
        {
            // Get API key from configuration
            var apiKey = _configuration["Groq:ApiKey"]?.Trim();

            // If configuration key is invalid, use environment variable
            if (!IsValidApiKey(apiKey))
            {
                apiKey = Environment
                    .GetEnvironmentVariable("GROQ_API_KEY")
                    ?.Trim();
            }

            // Final validation
            if (!IsValidApiKey(apiKey))
            {
                return "Groq API key is not configured correctly.";
            }

            Console.WriteLine("STEP 1 - Start AskAsync");

            // Get relevant data from database
            var databaseContext = await BuildDatabaseContextAsync(userQuery);

            Console.WriteLine("STEP 2 - Database context prepared");

            // =====================================================
            // AI SYSTEM PROMPT
            // =====================================================
            var systemPrompt = $"""
                You are an AI Inventory Management Assistant.

                Answer the user's question using ONLY the database information provided below.

                IMPORTANT RULES:
                1. Never invent products, suppliers, quantities, prices, dates, customers, purchases, or sales.
                2. Use exact values from the database context.
                3. If a specific product is not listed in the database, say:
                   "No, that product is not listed in the inventory database."
                4. If the requested information is unavailable, say:
                   "The requested information is not available in the inventory database."
                5. Do not confuse low stock with out of stock.
                6. "Out of stock" means StockQuantity = 0.
                7. "Low stock" means StockQuantity > 0 and StockQuantity <= LowStockThreshold.
                8. Use database calculations directly when they are already provided.
                9. Do not make unsupported assumptions.
                10. Keep answers concise and directly related to the user's question.

                RESPONSE FORMATTING RULES:
                11. Make the response easy to read.
                12. Start with a short clear heading when appropriate.
                13. Use bullet points for lists.
                14. Put important numbers clearly next to the product name.
                15. Do not write one long paragraph when listing multiple items.
                16. For inventory items, prefer this format:

                   ### Low Stock Products

                   • Product Name
                     Stock: X
                     Threshold: Y

                17. If suppliers are requested, use:

                   • Product Name
                     Supplier: Supplier Name
                     Contract Price: X
                     Lead Time: Y days

                18. At the end, add a short useful summary when appropriate.
                19. Do not use unnecessary explanations.

                You can answer questions about:
                - Products
                - Categories
                - Stock
                - Low stock
                - Out of stock
                - Restocking
                - Suppliers
                - Supplier prices
                - Lead times
                - Purchases
                - Purchase amounts
                - Sales
                - Customers
                - Best-selling products
                - Inventory analytics

                DATABASE CONTEXT:
                {databaseContext}
                """;

            // =====================================================
            // GROQ REQUEST
            // =====================================================
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
                max_completion_tokens = 400,
                include_reasoning = false,
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.groq.com/openai/v1/chat/completions");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    apiKey);

            request.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            Console.WriteLine("STEP 3 - Before Groq request");

            var response = await _httpClient.SendAsync(request);

            Console.WriteLine("STEP 4 - After Groq request");

            var responseJson =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Groq Error: {response.StatusCode}");
                Console.WriteLine(responseJson);

                return $"Groq API error: {response.StatusCode}";
            }

            using var document =
                JsonDocument.Parse(responseJson);

            var aiResponse = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            var finalResponse =
                aiResponse?.Trim() ?? "No response generated.";

            // =====================================================
            // SAVE CHAT LOG
            // =====================================================
            var chatLog = new AIChatLog
            {
                UserQuery = userQuery,
                AIResponse = finalResponse,
                CreatedAt = DateTime.Now
            };

            _context.AIChatLogs.Add(chatLog);

            await _context.SaveChangesAsync();

            Console.WriteLine("STEP 5 - Response saved");

            return finalResponse;
        }

        // =====================================================
        // VALIDATE GROQ API KEY
        // =====================================================
        private static bool IsValidApiKey(string? apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return false;
            }

            if (!apiKey.StartsWith("gsk_", StringComparison.Ordinal))
            {
                return false;
            }

            // Authorization header values must contain ASCII characters only
            return apiKey.All(c => c <= 127);
        }

        // =====================================================
        // BUILD DATABASE CONTEXT
        // =====================================================
        private async Task<string> BuildDatabaseContextAsync(string userQuery)
        {
            var query = userQuery.ToLower().Trim();

            // =====================================================
            // 1. SALES
            // =====================================================
            if (ContainsAny(
                query,
                "sales",
                "sale",
                "sold",
                "revenue",
                "selling",
                "customer",
                "customers",
                "sales amount",
                "total sales",
                "best selling",
                "top selling",
                "most sold"))
            {
                var totalSales = await _context.Sales
                    .AsNoTracking()
                    .SumAsync(s => (decimal?)s.TotalAmount) ?? 0;

                var salesByProduct = await _context.SaleItems
                    .AsNoTracking()
                    .Where(si => si.Product != null)
                    .GroupBy(si => new
                    {
                        si.ProductID,
                        ProductName = si.Product!.ProductName,
                        SKU = si.Product!.SKU
                    })
                    .Select(g => new
                    {
                        g.Key.ProductID,
                        g.Key.ProductName,
                        g.Key.SKU,
                        QuantitySold = g.Sum(x => x.Quantity),
                        SalesAmount = g.Sum(x => x.Quantity * x.UnitPrice)
                    })
                    .OrderByDescending(x => x.QuantitySold)
                    .ToListAsync();

                var sales = await _context.Sales
                    .AsNoTracking()
                    .Select(s => new
                    {
                        s.SaleID,
                        s.SaleDate,
                        s.CustomerInfo,
                        s.TotalAmount
                    })
                    .OrderBy(s => s.SaleDate)
                    .ToListAsync();

                var saleItems = await _context.SaleItems
                    .AsNoTracking()
                    .Where(si => si.Product != null && si.Sale != null)
                    .Select(si => new
                    {
                        si.SaleItemID,
                        si.SaleID,
                        SaleDate = si.Sale!.SaleDate,
                        CustomerInfo = si.Sale!.CustomerInfo,
                        ProductName = si.Product!.ProductName,
                        SKU = si.Product!.SKU,
                        si.Quantity,
                        si.UnitPrice,
                        Total = si.Quantity * si.UnitPrice
                    })
                    .ToListAsync();

                return Serialize(new
                {
                    DataType = "Sales Analytics",
                    TotalSales = totalSales,
                    SalesTransactions = sales,
                    SalesByProduct = salesByProduct,
                    SaleItems = saleItems
                });
            }

            // =====================================================
            // 2. PURCHASES
            // =====================================================
            if (ContainsAny(
                query,
                "purchase",
                "purchases",
                "spent",
                "spend",
                "buy",
                "bought",
                "purchase amount",
                "total purchases"))
            {
                var totalPurchases = await _context.Purchases
                    .AsNoTracking()
                    .SumAsync(p => (decimal?)p.TotalAmount) ?? 0;

                var purchasesBySupplier = await _context.Purchases
                    .AsNoTracking()
                    .Where(p => p.Supplier != null)
                    .GroupBy(p => new
                    {
                        p.SupplierID,
                        SupplierName = p.Supplier!.SupplierName
                    })
                    .Select(g => new
                    {
                        g.Key.SupplierID,
                        g.Key.SupplierName,
                        TotalPurchaseAmount = g.Sum(x => x.TotalAmount)
                    })
                    .OrderByDescending(x => x.TotalPurchaseAmount)
                    .ToListAsync();

                var purchases = await _context.Purchases
                    .AsNoTracking()
                    .Where(p => p.Supplier != null)
                    .Select(p => new
                    {
                        p.PurchaseID,
                        p.PurchaseDate,
                        SupplierName = p.Supplier!.SupplierName,
                        p.TotalAmount
                    })
                    .OrderBy(p => p.PurchaseDate)
                    .ToListAsync();

                var purchaseItems = await _context.PurchaseItems
                    .AsNoTracking()
                    .Where(pi => pi.Product != null && pi.Purchase != null)
                    .Select(pi => new
                    {
                        pi.PurchaseItemID,
                        pi.PurchaseID,
                        PurchaseDate = pi.Purchase!.PurchaseDate,
                        SupplierName = pi.Purchase!.Supplier!.SupplierName,
                        ProductName = pi.Product!.ProductName,
                        SKU = pi.Product!.SKU,
                        pi.Quantity,
                        pi.UnitCost,
                        Total = pi.Quantity * pi.UnitCost
                    })
                    .ToListAsync();

                return Serialize(new
                {
                    DataType = "Purchase Analytics",
                    TotalPurchases = totalPurchases,
                    PurchasesBySupplier = purchasesBySupplier,
                    PurchaseTransactions = purchases,
                    PurchaseItems = purchaseItems
                });
            }

            // =====================================================
            // 3. LOW STOCK / OUT OF STOCK / RESTOCK
            // =====================================================
            if (ContainsAny(
                query,
                "low stock",
                "low-stock",
                "currently low",
                "running low",
                "low on stock",
                "low inventory",
                "low quantity",
                "stock is low",
                "items are low",
                "products are low",
                "restock",
                "re-stock",
                "reorder",
                "re-order",
                "need restock",
                "needs restocking",
                "should restock",
                "should be restocked",
                "out of stock",
                "out-of-stock"))
            {
                // -------------------------------------------------
                // OUT OF STOCK
                // -------------------------------------------------
                if (query.Contains("out of stock") ||
                    query.Contains("out-of-stock"))
                {
                    var outOfStockProducts = await _context.Products
                        .AsNoTracking()
                        .Where(p => p.StockQuantity == 0)
                        .Select(p => new
                        {
                            p.ProductID,
                            p.ProductName,
                            p.SKU,
                            p.StockQuantity,
                            p.LowStockThreshold
                        })
                        .OrderBy(p => p.ProductName)
                        .ToListAsync();

                    return Serialize(new
                    {
                        DataType = "Out of Stock Analysis",
                        OutOfStockCount = outOfStockProducts.Count,
                        OutOfStockProducts = outOfStockProducts
                    });
                }

                // -------------------------------------------------
                // LOW STOCK
                // Stock > 0 AND Stock <= Threshold
                // -------------------------------------------------
                var lowStockProducts = await _context.Products
                    .AsNoTracking()
                    .Where(p =>
                        p.StockQuantity > 0 &&
                        p.StockQuantity <= p.LowStockThreshold)
                    .Select(p => new
                    {
                        p.ProductID,
                        p.ProductName,
                        p.SKU,
                        p.StockQuantity,
                        p.LowStockThreshold,
                        p.UnitPrice,
                        CategoryName = p.Category != null
                            ? p.Category.CategoryName
                            : null
                    })
                    .OrderBy(p => p.StockQuantity)
                    .ToListAsync();

                var lowStockProductIds = lowStockProducts
                    .Select(p => p.ProductID)
                    .ToList();

                // -------------------------------------------------
                // SUPPLIERS FOR LOW STOCK PRODUCTS
                // -------------------------------------------------
                var suppliersForLowStock = await _context.SupplierProducts
                    .AsNoTracking()
                    .Where(sp =>
                        sp.Product != null &&
                        sp.Supplier != null &&
                        lowStockProductIds.Contains(sp.ProductID))
                    .Select(sp => new
                    {
                        ProductName = sp.Product!.ProductName,
                        SKU = sp.Product!.SKU,
                        SupplierName = sp.Supplier!.SupplierName,
                        sp.ContractPrice,
                        sp.LeadTimeDays
                    })
                    .OrderBy(x => x.ProductName)
                    .ThenBy(x => x.ContractPrice)
                    .ToListAsync();

                return Serialize(new
                {
                    DataType = "Low Stock and Restock Analysis",
                    LowStockCount = lowStockProducts.Count,
                    LowStockProducts = lowStockProducts,
                    AvailableSuppliers = suppliersForLowStock
                });
            }

            // =====================================================
            // 4. SUPPLIERS
            // =====================================================
            if (ContainsAny(
                query,
                "supplier",
                "suppliers",
                "provide",
                "provides",
                "provided by",
                "vendor",
                "vendors"))
            {
                var suppliers = await _context.Suppliers
                    .AsNoTracking()
                    .Select(s => new
                    {
                        s.SupplierID,
                        s.SupplierName,
                        s.ContactName,
                        s.Phone,
                        s.Email,
                        s.Address
                    })
                    .ToListAsync();

                var supplierProducts = await _context.SupplierProducts
                    .AsNoTracking()
                    .Where(sp =>
                        sp.Supplier != null &&
                        sp.Product != null)
                    .Select(sp => new
                    {
                        sp.SupplierProductID,
                        SupplierName = sp.Supplier!.SupplierName,
                        SupplierID = sp.SupplierID,
                        ProductName = sp.Product!.ProductName,
                        ProductID = sp.ProductID,
                        ProductSKU = sp.Product!.SKU,
                        sp.SupplierSKU,
                        sp.ContractPrice,
                        sp.LeadTimeDays
                    })
                    .OrderBy(sp => sp.ProductName)
                    .ThenBy(sp => sp.ContractPrice)
                    .ToListAsync();

                return Serialize(new
                {
                    DataType = "Supplier Information",
                    Suppliers = suppliers,
                    SupplierProducts = supplierProducts
                });
            }

            // =====================================================
            // 5. PRODUCTS / CATEGORIES / STOCK
            // =====================================================
            if (ContainsAny(
                query,
                "product",
                "products",
                "stock",
                "inventory",
                "category",
                "categories",
                "price",
                "prices",
                "how many products"))
            {
                var categories = await _context.Categories
                    .AsNoTracking()
                    .Select(c => new
                    {
                        c.CategoryID,
                        c.CategoryName,
                        c.Description
                    })
                    .ToListAsync();

                var products = await _context.Products
                    .AsNoTracking()
                    .Select(p => new
                    {
                        p.ProductID,
                        p.SKU,
                        p.ProductName,
                        CategoryName = p.Category != null
                            ? p.Category.CategoryName
                            : null,
                        p.UnitPrice,
                        p.StockQuantity,
                        p.LowStockThreshold
                    })
                    .OrderBy(p => p.ProductName)
                    .ToListAsync();

                var totalProducts = products.Count;

                var totalStockUnits = products
                    .Sum(p => p.StockQuantity);

                return Serialize(new
                {
                    DataType = "Product and Inventory Information",
                    TotalProducts = totalProducts,
                    TotalStockUnits = totalStockUnits,
                    Categories = categories,
                    Products = products
                });
            }

            // =====================================================
            // 6. GENERAL INVENTORY OVERVIEW
            // =====================================================
            var allCategories = await _context.Categories
                .AsNoTracking()
                .Select(c => new
                {
                    c.CategoryID,
                    c.CategoryName,
                    c.Description
                })
                .ToListAsync();

            var allProducts = await _context.Products
                .AsNoTracking()
                .Select(p => new
                {
                    p.ProductID,
                    p.SKU,
                    p.ProductName,
                    CategoryName = p.Category != null
                        ? p.Category.CategoryName
                        : null,
                    p.UnitPrice,
                    p.StockQuantity,
                    p.LowStockThreshold
                })
                .ToListAsync();

            var allSuppliers = await _context.Suppliers
                .AsNoTracking()
                .Select(s => new
                {
                    s.SupplierID,
                    s.SupplierName,
                    s.ContactName,
                    s.Email,
                    s.Phone
                })
                .ToListAsync();

            return Serialize(new
            {
                DataType = "General Inventory Overview",
                Categories = allCategories,
                Products = allProducts,
                Suppliers = allSuppliers
            });
        }

        // =====================================================
        // CHECK IF QUERY CONTAINS ANY KEYWORD
        // =====================================================
        private static bool ContainsAny(
            string query,
            params string[] keywords)
        {
            return keywords.Any(query.Contains);
        }

        // =====================================================
        // SERIALIZE DATABASE DATA
        // =====================================================
        private static string Serialize(object data)
        {
            return JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions
                {
                    WriteIndented = false
                });
        }
    }
}