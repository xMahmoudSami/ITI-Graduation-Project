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
            var databaseContext =
                await BuildDatabaseContextAsync(userQuery);

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
                5. If the user asks about dead stock and no dead stock products exist,
                   say:
                   "No dead stock products were found in the inventory database."
                6. Do not confuse low stock with out of stock.
                7. "Out of stock" means StockQuantity = 0.
                8. "Low stock" means StockQuantity > 0 and StockQuantity <= LowStockThreshold.
                9. "Dead stock" means StockQuantity > 0 and UnitsSold = 0.
                10. Use database calculations directly when they are already provided.
                11. Do not make unsupported assumptions.
                12. Keep answers concise and directly related to the user's question.

                RESPONSE FORMATTING RULES:
                13. Make the response easy to read.
                14. Start with a short clear heading when appropriate.
                15. Use bullet points for lists.
                16. Put important numbers clearly next to the product name.
                17. Do not write one long paragraph when listing multiple items.

                18. For low stock questions, prefer:

                    ### Low Stock Products

                    • Product Name
                      Stock: X
                      Threshold: Y

                19. If suppliers are requested, prefer:

                    • Product Name
                      Supplier: Supplier Name
                      Contract Price: X
                      Lead Time: Y days

                20. For analytics or summaries, organize the answer using:
                    - Short headings
                    - Bullet points
                    - Clear numbers
                    - Short explanations

                21. For inventory intelligence questions, organize the response into
                    clear sections such as:
                    - Top Selling Products
                    - Fast-Moving Products
                    - Slow-Moving Products
                    - Products With No Sales
                    - Demand Analysis
                    - Category Demand
                    - Stock Valuation
                    - Total Inventory Value

                22. Explain why a product is considered fast-moving or slow-moving
                    using the calculated SalesVelocity when it is provided.

                23. Do not create or invent sales metrics that are not provided
                    in the database context.

                24. Show calculated values directly when available.

                25. For stock risk questions, clearly show:
                    - Product
                    - Current Stock
                    - Units Sold
                    - Sales Velocity
                    - Days of Inventory Remaining
                    - Risk Level
                    - Restock Priority

                26. Explain stock risk using the calculated values provided
                    by the database.

                27. Treat Critical, High, Medium, and Low as calculated
                    system risk levels.

                28. Clearly identify dead stock as products that have
                    current stock but no recorded sales.

                29. If a requested analysis has zero matching products,
                    clearly state that no matching products were found.
                    Do not say that the information is unavailable.

                30. For smart restock recommendations, clearly show:
                    - Product
                    - Current Stock
                    - Units Sold
                    - Sales Velocity
                    - Projected Demand
                    - Recommended Reorder Quantity
                    - Priority
                    - Reason

                31. Never invent a reorder quantity.
                    Use the RecommendedReorderQuantity calculated by the system.

                32. Clearly distinguish between:
                    - Immediate
                    - High
                    - Planned
                    - Monitor
                    - No Immediate Action

                33. If no products require restocking, say:
                    "No products currently require restocking based on the available inventory data."

                34. At the end, provide a short useful summary when appropriate.
                35. Do not add unnecessary explanations.

                You can answer questions about:
                - Products
                - Categories
                - Stock
                - Low stock
                - Out of stock
                - Restocking
                - Smart restock recommendations
                - Recommended reorder quantity
                - Reorder quantity
                - Restock planning
                - Replenishment
                - Products requiring restock
                - Restock priority
                - Restock urgency
                - Expected stock coverage
                - Projected demand
                - Suppliers
                - Supplier prices
                - Lead times
                - Purchases
                - Purchase amounts
                - Sales
                - Customers
                - Best-selling products
                - Inventory analytics
                - Inventory value
                - Stock value
                - Average product price
                - Stock by category
                - Inventory summary
                - Top selling products
                - Fast-moving products
                - Slow-moving products
                - Products with no sales
                - Sales velocity
                - Demand analysis
                - Category demand
                - Stock valuation
                - Total inventory valuation
                - Inventory performance
                - Stock risk analysis
                - Stockout risk
                - Days of inventory remaining
                - Stock coverage
                - Demand versus stock
                - Projected stock
                - Dead stock
                - Restock priority
                - Restock urgency

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

            var response =
                await _httpClient.SendAsync(request);

            Console.WriteLine("STEP 4 - After Groq request");

            var responseJson =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(
                    $"Groq Error: {response.StatusCode}");

                Console.WriteLine(responseJson);

                return $"Groq API error: {response.StatusCode}";
            }

            using var document =
                JsonDocument.Parse(responseJson);

            var aiResponse =
                document.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

            var finalResponse =
                aiResponse?.Trim()
                ?? "No response generated.";

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

            if (!apiKey.StartsWith(
                    "gsk_",
                    StringComparison.Ordinal))
            {
                return false;
            }

            // Authorization header values must contain ASCII characters only
            return apiKey.All(c => c <= 127);
        }

        // =====================================================
        // BUILD DATABASE CONTEXT
        // =====================================================
        private async Task<string> BuildDatabaseContextAsync(
            string userQuery)
        {
            var query =
                userQuery.ToLower().Trim();

            // =====================================================
            // 1. AI INVENTORY DEMAND & SALES INTELLIGENCE
            // =====================================================
            if (ContainsAny(
                query,
                "top selling",
                "top-selling",
                "best selling",
                "best-selling",
                "most sold",
                "fast moving",
                "fast-moving",
                "slow moving",
                "slow-moving",
                "no sales",
                "never sold",
                "sales velocity",
                "selling speed",
                "demand analysis",
                "demand",
                "product demand",
                "category demand",
                "stock valuation",
                "inventory valuation",
                "total valuation",
                "total inventory value",
                "inventory value",
                "stock value",
                "inventory intelligence",
                "inventory performance"))
            {
                // -------------------------------------------------
                // GET PRODUCTS
                // -------------------------------------------------
                var products =
                    await _context.Products
                        .AsNoTracking()
                        .Select(p => new
                        {
                            p.ProductID,
                            p.ProductName,
                            p.SKU,
                            p.UnitPrice,
                            p.StockQuantity,
                            p.LowStockThreshold,

                            CategoryName =
                                p.Category != null
                                    ? p.Category.CategoryName
                                    : null
                        })
                        .ToListAsync();

                // -------------------------------------------------
                // GET SALES PERIOD
                // -------------------------------------------------
                var saleDates =
                    await _context.Sales
                        .AsNoTracking()
                        .Select(s => s.SaleDate)
                        .ToListAsync();

                int salesPeriodDays = 1;

                DateTime? firstSaleDate = null;
                DateTime? lastSaleDate = null;

                if (saleDates.Count > 0)
                {
                    firstSaleDate =
                        saleDates.Min();

                    lastSaleDate =
                        saleDates.Max();

                    salesPeriodDays =
                        Math.Max(
                            1,
                            (
                                lastSaleDate.Value.Date -
                                firstSaleDate.Value.Date
                            ).Days + 1);
                }

                // -------------------------------------------------
                // GET SALES BY PRODUCT
                // -------------------------------------------------
                var salesByProduct =
                    await _context.SaleItems
                        .AsNoTracking()
                        .Where(
                            si =>
                                si.Product != null)
                        .GroupBy(
                            si =>
                                si.ProductID)
                        .Select(
                            g => new
                            {
                                ProductID =
                                    g.Key,

                                UnitsSold =
                                    g.Sum(
                                        x =>
                                            x.Quantity),

                                SalesAmount =
                                    g.Sum(
                                        x =>
                                            x.Quantity *
                                            x.UnitPrice)
                            })
                        .ToListAsync();

                // -------------------------------------------------
                // PREPARE SALES LOOKUP
                // -------------------------------------------------
                var salesLookup =
                    salesByProduct
                        .ToDictionary(
                            x => x.ProductID,
                            x => new
                            {
                                x.UnitsSold,
                                x.SalesAmount
                            });

                // -------------------------------------------------
                // BUILD PRODUCT INTELLIGENCE
                // -------------------------------------------------
                var productIntelligence =
                    products
                        .Select(
                            p =>
                            {
                                var hasSales =
                                    salesLookup.TryGetValue(
                                        p.ProductID,
                                        out var sales);

                                var unitsSold =
                                    hasSales
                                        ? sales.UnitsSold
                                        : 0;

                                var salesAmount =
                                    hasSales
                                        ? sales.SalesAmount
                                        : 0;

                                var salesVelocity =
                                    Math.Round(
                                        (decimal)unitsSold /
                                        salesPeriodDays,
                                        2);

                                var stockValue =
                                    p.UnitPrice *
                                    p.StockQuantity;

                                return new
                                {
                                    p.ProductID,
                                    p.ProductName,
                                    p.SKU,
                                    p.CategoryName,
                                    p.UnitPrice,
                                    p.StockQuantity,
                                    p.LowStockThreshold,

                                    UnitsSold =
                                        unitsSold,

                                    SalesAmount =
                                        salesAmount,

                                    SalesVelocity =
                                        salesVelocity,

                                    StockValue =
                                        stockValue
                                };
                            })
                        .ToList();

                // -------------------------------------------------
                // AVERAGE SALES VELOCITY
                // -------------------------------------------------
                var productsWithSales =
                    productIntelligence
                        .Where(
                            p =>
                                p.UnitsSold > 0)
                        .ToList();

                var averageSalesVelocity =
                    productsWithSales.Count > 0
                        ? productsWithSales
                            .Average(
                                p =>
                                    p.SalesVelocity)
                        : 0;

                // -------------------------------------------------
                // FAST MOVING PRODUCTS
                // -------------------------------------------------
                var fastMovingProducts =
                    productIntelligence
                        .Where(
                            p =>
                                p.UnitsSold > 0 &&
                                p.SalesVelocity >=
                                averageSalesVelocity)
                        .OrderByDescending(
                            p =>
                                p.SalesVelocity)
                        .ToList();

                // -------------------------------------------------
                // SLOW MOVING PRODUCTS
                // -------------------------------------------------
                var slowMovingProducts =
                    productIntelligence
                        .Where(
                            p =>
                                p.UnitsSold > 0 &&
                                p.SalesVelocity <
                                averageSalesVelocity)
                        .OrderBy(
                            p =>
                                p.SalesVelocity)
                        .ToList();

                // -------------------------------------------------
                // PRODUCTS WITH NO SALES
                // -------------------------------------------------
                var noSalesProducts =
                    productIntelligence
                        .Where(
                            p =>
                                p.UnitsSold == 0)
                        .OrderByDescending(
                            p =>
                                p.StockQuantity)
                        .ToList();

                // -------------------------------------------------
                // TOP SELLING PRODUCTS
                // -------------------------------------------------
                var topSellingProducts =
                    productIntelligence
                        .OrderByDescending(
                            p =>
                                p.UnitsSold)
                        .ThenByDescending(
                            p =>
                                p.SalesAmount)
                        .Take(5)
                        .ToList();

                // -------------------------------------------------
                // CATEGORY DEMAND
                // -------------------------------------------------
                var categoryDemand =
                    productIntelligence
                        .GroupBy(
                            p =>
                                p.CategoryName ??
                                "Uncategorized")
                        .Select(
                            g => new
                            {
                                Category =
                                    g.Key,

                                ProductCount =
                                    g.Count(),

                                UnitsSold =
                                    g.Sum(
                                        p =>
                                            p.UnitsSold),

                                SalesAmount =
                                    g.Sum(
                                        p =>
                                            p.SalesAmount),

                                TotalStockUnits =
                                    g.Sum(
                                        p =>
                                            p.StockQuantity),

                                InventoryValue =
                                    g.Sum(
                                        p =>
                                            p.StockValue)
                            })
                        .OrderByDescending(
                            x =>
                                x.UnitsSold)
                        .ToList();

                // -------------------------------------------------
                // STOCK VALUATION
                // -------------------------------------------------
                var stockValuation =
                    productIntelligence
                        .OrderByDescending(
                            p =>
                                p.StockValue)
                        .Select(
                            p => new
                            {
                                p.ProductName,
                                p.SKU,
                                p.UnitPrice,
                                p.StockQuantity,

                                StockValue =
                                    p.StockValue
                            })
                        .ToList();

                // -------------------------------------------------
                // TOTAL INVENTORY VALUATION
                // -------------------------------------------------
                var totalInventoryValue =
                    productIntelligence
                        .Sum(
                            p =>
                                p.StockValue);

                // -------------------------------------------------
                // TOTAL UNITS SOLD
                // -------------------------------------------------
                var totalUnitsSold =
                    productIntelligence
                        .Sum(
                            p =>
                                p.UnitsSold);

                return Serialize(new
                {
                    DataType =
                        "AI Inventory Demand and Sales Intelligence",

                    SalesPeriod = new
                    {
                        StartDate =
                            firstSaleDate,

                        EndDate =
                            lastSaleDate,

                        Days =
                            salesPeriodDays
                    },

                    OverallMetrics = new
                    {
                        TotalProducts =
                            productIntelligence.Count,

                        TotalUnitsSold =
                            totalUnitsSold,

                        AverageDailySales =
                            Math.Round(
                                (decimal)totalUnitsSold /
                                salesPeriodDays,
                                2),

                        AverageSalesVelocity =
                            Math.Round(
                                averageSalesVelocity,
                                2),

                        TotalInventoryValue =
                            totalInventoryValue
                    },

                    TopSellingProducts =
                        topSellingProducts,

                    FastMovingProducts =
                        fastMovingProducts,

                    SlowMovingProducts =
                        slowMovingProducts,

                    ProductsWithNoSales =
                        noSalesProducts,

                    CategoryDemand =
                        categoryDemand,

                    StockValuation =
                        stockValuation,

                    ProductIntelligence =
                        productIntelligence
                });
            }

            // =====================================================
            // 2. STOCK RISK ANALYSIS
            // =====================================================
            if (ContainsAny(
                query,
                "stock risk",
                "stockout risk",
                "stock-out risk",
                "risk of stockout",
                "risk of stock-out",
                "stockout",
                "stock-out",
                "products at risk",
                "products at risk of running out",
                "running out",
                "run out of stock",
                "days remaining",
                "days of inventory",
                "inventory remaining",
                "stock coverage",
                "stock coverage days",
                "demand vs stock",
                "demand versus stock",
                "projected stock",
                "stock forecast",
                "forecast stock",
                "dead stock",
                "unsold stock",
                "restock priority",
                "restock urgency",
                "restock priority analysis"))
            {
                // -------------------------------------------------
                // GET PRODUCTS
                // -------------------------------------------------
                var products =
                    await _context.Products
                        .AsNoTracking()
                        .Select(p => new
                        {
                            p.ProductID,
                            p.ProductName,
                            p.SKU,
                            p.UnitPrice,
                            p.StockQuantity,
                            p.LowStockThreshold,

                            CategoryName =
                                p.Category != null
                                    ? p.Category.CategoryName
                                    : null
                        })
                        .ToListAsync();

                // -------------------------------------------------
                // GET SALES PERIOD
                // -------------------------------------------------
                var saleDates =
                    await _context.Sales
                        .AsNoTracking()
                        .Select(s => s.SaleDate)
                        .ToListAsync();

                int salesPeriodDays = 1;

                DateTime? firstSaleDate = null;
                DateTime? lastSaleDate = null;

                if (saleDates.Count > 0)
                {
                    firstSaleDate =
                        saleDates.Min();

                    lastSaleDate =
                        saleDates.Max();

                    salesPeriodDays =
                        Math.Max(
                            1,
                            (
                                lastSaleDate.Value.Date -
                                firstSaleDate.Value.Date
                            ).Days + 1);
                }

                // -------------------------------------------------
                // GET SALES BY PRODUCT
                // -------------------------------------------------
                var salesByProduct =
                    await _context.SaleItems
                        .AsNoTracking()
                        .Where(
                            si =>
                                si.Product != null)
                        .GroupBy(
                            si =>
                                si.ProductID)
                        .Select(
                            g => new
                            {
                                ProductID =
                                    g.Key,

                                UnitsSold =
                                    g.Sum(
                                        x =>
                                            x.Quantity),

                                SalesAmount =
                                    g.Sum(
                                        x =>
                                            x.Quantity *
                                            x.UnitPrice)
                            })
                        .ToListAsync();

                var salesLookup =
                    salesByProduct
                        .ToDictionary(
                            x => x.ProductID,
                            x => new
                            {
                                x.UnitsSold,
                                x.SalesAmount
                            });

                // -------------------------------------------------
                // BUILD STOCK RISK ANALYSIS
                // -------------------------------------------------
                var stockRiskAnalysis =
                    products
                        .Select(
                            p =>
                            {
                                var hasSales =
                                    salesLookup.TryGetValue(
                                        p.ProductID,
                                        out var sales);

                                var unitsSold =
                                    hasSales
                                        ? sales.UnitsSold
                                        : 0;

                                var salesAmount =
                                    hasSales
                                        ? sales.SalesAmount
                                        : 0;

                                // Average units sold per day
                                var salesVelocity =
                                    Math.Round(
                                        (decimal)unitsSold /
                                        salesPeriodDays,
                                        2);

                                // -------------------------------------------------
                                // DAYS OF INVENTORY REMAINING
                                // -------------------------------------------------
                                decimal?
                                    daysOfInventoryRemaining =
                                        null;

                                if (salesVelocity > 0)
                                {
                                    daysOfInventoryRemaining =
                                        Math.Round(
                                            p.StockQuantity /
                                            salesVelocity,
                                            1);
                                }

                                // -------------------------------------------------
                                // DEMAND VS CURRENT STOCK
                                // -------------------------------------------------
                                var demandVsStockRatio =
                                    p.StockQuantity > 0
                                        ? Math.Round(
                                            (decimal)unitsSold /
                                            p.StockQuantity,
                                            2)
                                        : unitsSold > 0
                                            ? decimal.MaxValue
                                            : 0;

                                // -------------------------------------------------
                                // PROJECTED STOCK AFTER 7 DAYS
                                // -------------------------------------------------
                                var projectedStockAfter7Days =
                                    Math.Max(
                                        0m,
                                        Math.Round(
                                            p.StockQuantity -
                                            (salesVelocity * 7),
                                            1));

                                // -------------------------------------------------
                                // RISK LEVEL
                                // -------------------------------------------------
                                string riskLevel;

                                if (p.StockQuantity == 0)
                                {
                                    riskLevel =
                                        "Critical";
                                }
                                else if (
                                    salesVelocity > 0 &&
                                    daysOfInventoryRemaining <= 7)
                                {
                                    riskLevel =
                                        "High";
                                }
                                else if (
                                    p.StockQuantity <=
                                    p.LowStockThreshold ||
                                    (
                                        salesVelocity > 0 &&
                                        daysOfInventoryRemaining <= 14
                                    ))
                                {
                                    riskLevel =
                                        "Medium";
                                }
                                else
                                {
                                    riskLevel =
                                        "Low";
                                }

                                // -------------------------------------------------
                                // RESTOCK PRIORITY
                                // -------------------------------------------------
                                string restockPriority;

                                if (p.StockQuantity == 0)
                                {
                                    restockPriority =
                                        "Immediate";
                                }
                                else if (
                                    salesVelocity > 0 &&
                                    daysOfInventoryRemaining <= 7)
                                {
                                    restockPriority =
                                        "High";
                                }
                                else if (
                                    p.StockQuantity <=
                                    p.LowStockThreshold ||
                                    (
                                        salesVelocity > 0 &&
                                        daysOfInventoryRemaining <= 14
                                    ))
                                {
                                    restockPriority =
                                        "Planned";
                                }
                                else if (
                                    salesVelocity > 0)
                                {
                                    restockPriority =
                                        "Monitor";
                                }
                                else
                                {
                                    restockPriority =
                                        "No Immediate Action";
                                }

                                // -------------------------------------------------
                                // DEAD STOCK
                                // -------------------------------------------------
                                var isDeadStock =
                                    p.StockQuantity > 0 &&
                                    unitsSold == 0;

                                return new
                                {
                                    p.ProductID,
                                    p.ProductName,
                                    p.SKU,
                                    p.CategoryName,
                                    p.UnitPrice,

                                    CurrentStock =
                                        p.StockQuantity,

                                    LowStockThreshold =
                                        p.LowStockThreshold,

                                    UnitsSold =
                                        unitsSold,

                                    SalesAmount =
                                        salesAmount,

                                    SalesVelocity =
                                        salesVelocity,

                                    DaysOfInventoryRemaining =
                                        daysOfInventoryRemaining,

                                    DemandVsStockRatio =
                                        demandVsStockRatio,

                                    ProjectedStockAfter7Days =
                                        projectedStockAfter7Days,

                                    RiskLevel =
                                        riskLevel,

                                    RestockPriority =
                                        restockPriority,

                                    IsDeadStock =
                                        isDeadStock
                                };
                            })
                        .ToList();

                // -------------------------------------------------
                // HIGH RISK PRODUCTS
                // -------------------------------------------------
                var highRiskProducts =
                    stockRiskAnalysis
                        .Where(
                            p =>
                                p.RiskLevel ==
                                    "Critical" ||
                                p.RiskLevel ==
                                    "High")
                        .OrderByDescending(
                            p =>
                                p.RiskLevel ==
                                "Critical")
                        .ThenBy(
                            p =>
                                p.DaysOfInventoryRemaining ??
                                decimal.MaxValue)
                        .ToList();

                // -------------------------------------------------
                // RESTOCK PRIORITY PRODUCTS
                // -------------------------------------------------
                var restockPriorityProducts =
                    stockRiskAnalysis
                        .Where(
                            p =>
                                p.RestockPriority ==
                                    "Immediate" ||
                                p.RestockPriority ==
                                    "High" ||
                                p.RestockPriority ==
                                    "Planned")
                        .OrderBy(
                            p =>
                                p.RestockPriority ==
                                    "Immediate" ? 1 :
                                p.RestockPriority ==
                                    "High" ? 2 : 3)
                        .ThenBy(
                            p =>
                                p.DaysOfInventoryRemaining ??
                                decimal.MaxValue)
                        .ToList();

                // -------------------------------------------------
                // DEAD STOCK
                // -------------------------------------------------
                var deadStockProducts =
                    stockRiskAnalysis
                        .Where(
                            p =>
                                p.IsDeadStock)
                        .OrderByDescending(
                            p =>
                                p.CurrentStock)
                        .ToList();

                // -------------------------------------------------
                // DIRECT DEAD STOCK HANDLING
                // -------------------------------------------------
                if (
                    query.Contains("dead stock") ||
                    query.Contains("unsold stock"))
                {
                    return Serialize(new
                    {
                        DataType =
                            "Dead Stock Analysis",

                        DeadStockCount =
                            deadStockProducts.Count,

                        DeadStockProducts =
                            deadStockProducts
                    });
                }

                // -------------------------------------------------
                // LIMITED COVERAGE PRODUCTS
                // -------------------------------------------------
                var limitedCoverageProducts =
                    stockRiskAnalysis
                        .Where(
                            p =>
                                p.DaysOfInventoryRemaining
                                    .HasValue &&
                                p.DaysOfInventoryRemaining <= 14)
                        .OrderBy(
                            p =>
                                p.DaysOfInventoryRemaining)
                        .ToList();

                return Serialize(new
                {
                    DataType =
                        "Stock Risk Analysis",

                    AnalysisPeriod = new
                    {
                        StartDate =
                            firstSaleDate,

                        EndDate =
                            lastSaleDate,

                        Days =
                            salesPeriodDays
                    },

                    Summary = new
                    {
                        TotalProducts =
                            stockRiskAnalysis.Count,

                        CriticalProducts =
                            stockRiskAnalysis.Count(
                                p =>
                                    p.RiskLevel ==
                                    "Critical"),

                        HighRiskProducts =
                            stockRiskAnalysis.Count(
                                p =>
                                    p.RiskLevel ==
                                    "High"),

                        MediumRiskProducts =
                            stockRiskAnalysis.Count(
                                p =>
                                    p.RiskLevel ==
                                    "Medium"),

                        LowRiskProducts =
                            stockRiskAnalysis.Count(
                                p =>
                                    p.RiskLevel ==
                                    "Low"),

                        DeadStockProducts =
                            deadStockProducts.Count
                    },

                    HighRiskProducts =
                        highRiskProducts,

                    RestockPriorityProducts =
                        restockPriorityProducts,

                    LimitedCoverageProducts =
                        limitedCoverageProducts,

                    DeadStockProducts =
                        deadStockProducts,

                    ProductRiskAnalysis =
                        stockRiskAnalysis
                });
            }

            // =====================================================
            // 3. SMART RESTOCK RECOMMENDATION
            // =====================================================
            if (ContainsAny(
                query,
                "smart restock",
                "restock recommendation",
                "restocking recommendation",
                "reorder recommendation",
                "recommended reorder",
                "recommended order",
                "how much should we order",
                "how many should we order",
                "how many units should we order",
                "what should we restock",
                "what should we reorder",
                "which products should we restock",
                "which products need restocking",
                "products that need restocking",
                "restock quantity",
                "reorder quantity",
                "suggested order quantity",
                "suggested reorder quantity",
                "restock plan",
                "replenishment",
                "replenishment recommendation"))
            {
                // -------------------------------------------------
                // PLANNING SETTINGS
                // -------------------------------------------------
                const int planningHorizonDays = 14;

                // -------------------------------------------------
                // GET PRODUCTS
                // -------------------------------------------------
                var products =
                    await _context.Products
                        .AsNoTracking()
                        .Select(p => new
                        {
                            p.ProductID,
                            p.ProductName,
                            p.SKU,
                            p.UnitPrice,
                            p.StockQuantity,
                            p.LowStockThreshold,

                            CategoryName =
                                p.Category != null
                                    ? p.Category.CategoryName
                                    : null
                        })
                        .ToListAsync();

                // -------------------------------------------------
                // GET SALES PERIOD
                // -------------------------------------------------
                var saleDates =
                    await _context.Sales
                        .AsNoTracking()
                        .Select(s => s.SaleDate)
                        .ToListAsync();

                int salesPeriodDays = 1;

                DateTime? firstSaleDate = null;
                DateTime? lastSaleDate = null;

                if (saleDates.Count > 0)
                {
                    firstSaleDate =
                        saleDates.Min();

                    lastSaleDate =
                        saleDates.Max();

                    salesPeriodDays =
                        Math.Max(
                            1,
                            (
                                lastSaleDate.Value.Date -
                                firstSaleDate.Value.Date
                            ).Days + 1);
                }

                // -------------------------------------------------
                // GET SALES BY PRODUCT
                // -------------------------------------------------
                var salesByProduct =
                    await _context.SaleItems
                        .AsNoTracking()
                        .Where(
                            si =>
                                si.Product != null)
                        .GroupBy(
                            si =>
                                si.ProductID)
                        .Select(
                            g => new
                            {
                                ProductID =
                                    g.Key,

                                UnitsSold =
                                    g.Sum(
                                        x =>
                                            x.Quantity),

                                SalesAmount =
                                    g.Sum(
                                        x =>
                                            x.Quantity *
                                            x.UnitPrice)
                            })
                        .ToListAsync();

                // -------------------------------------------------
                // PREPARE SALES LOOKUP
                // -------------------------------------------------
                var salesLookup =
                    salesByProduct
                        .ToDictionary(
                            x => x.ProductID,
                            x => new
                            {
                                x.UnitsSold,
                                x.SalesAmount
                            });

                // -------------------------------------------------
                // BUILD SMART RESTOCK ANALYSIS
                // -------------------------------------------------
                var restockAnalysis =
                    products
                        .Select(
                            p =>
                            {
                                var hasSales =
                                    salesLookup.TryGetValue(
                                        p.ProductID,
                                        out var sales);

                                var unitsSold =
                                    hasSales
                                        ? sales.UnitsSold
                                        : 0;

                                var salesAmount =
                                    hasSales
                                        ? sales.SalesAmount
                                        : 0;

                                // Average daily demand
                                var salesVelocity =
                                    Math.Round(
                                        (decimal)unitsSold /
                                        salesPeriodDays,
                                        2);

                                // -------------------------------------------------
                                // PROJECTED DEMAND
                                // -------------------------------------------------
                                var projectedDemand =
                                    (int)Math.Ceiling(
                                        salesVelocity *
                                        planningHorizonDays);

                                // -------------------------------------------------
                                // TARGET STOCK
                                // -------------------------------------------------
                                var thresholdTarget =
                                    p.LowStockThreshold * 2;

                                var targetStock =
                                    Math.Max(
                                        thresholdTarget,
                                        projectedDemand);

                                // -------------------------------------------------
                                // RECOMMENDED REORDER QUANTITY
                                // -------------------------------------------------
                                var recommendedReorderQuantity =
                                    Math.Max(
                                        0,
                                        targetStock -
                                        p.StockQuantity);

                                // -------------------------------------------------
                                // SHOULD RESTOCK?
                                // -------------------------------------------------
                                bool shouldRestock;

                                if (
                                    p.StockQuantity == 0)
                                {
                                    shouldRestock = true;
                                }
                                else if (
                                    p.StockQuantity <=
                                    p.LowStockThreshold)
                                {
                                    shouldRestock = true;
                                }
                                else if (
                                    salesVelocity > 0 &&
                                    p.StockQuantity <
                                    projectedDemand)
                                {
                                    shouldRestock = true;
                                }
                                else
                                {
                                    shouldRestock = false;
                                }

                                // -------------------------------------------------
                                // PRIORITY
                                // -------------------------------------------------
                                string priority;

                                decimal?
                                    expectedCoverageDays =
                                        null;

                                if (salesVelocity > 0)
                                {
                                    expectedCoverageDays =
                                        Math.Round(
                                            p.StockQuantity /
                                            salesVelocity,
                                            1);
                                }

                                if (
                                    p.StockQuantity == 0)
                                {
                                    priority =
                                        "Immediate";
                                }
                                else if (
                                    shouldRestock &&
                                    salesVelocity > 0 &&
                                    expectedCoverageDays <= 7)
                                {
                                    priority =
                                        "High";
                                }
                                else if (
                                    shouldRestock)
                                {
                                    priority =
                                        "Planned";
                                }
                                else if (
                                    salesVelocity > 0)
                                {
                                    priority =
                                        "Monitor";
                                }
                                else
                                {
                                    priority =
                                        "No Immediate Action";
                                }

                                // -------------------------------------------------
                                // REASON
                                // -------------------------------------------------
                                string reason;

                                if (
                                    p.StockQuantity == 0)
                                {
                                    reason =
                                        "Product is currently out of stock.";
                                }
                                else if (
                                    p.StockQuantity <=
                                        p.LowStockThreshold &&
                                    salesVelocity > 0)
                                {
                                    reason =
                                        "Current stock is below the defined threshold and the product has recorded demand.";
                                }
                                else if (
                                    p.StockQuantity <=
                                    p.LowStockThreshold)
                                {
                                    reason =
                                        "Current stock is below the defined threshold.";
                                }
                                else if (
                                    salesVelocity > 0 &&
                                    p.StockQuantity <
                                    projectedDemand)
                                {
                                    reason =
                                        "Projected demand for the planning period is higher than the current stock.";
                                }
                                else if (
                                    unitsSold == 0)
                                {
                                    reason =
                                        "No recorded sales were found for this product.";
                                }
                                else
                                {
                                    reason =
                                        "Current inventory is sufficient for the planning period.";
                                }

                                return new
                                {
                                    p.ProductID,
                                    p.ProductName,
                                    p.SKU,
                                    p.CategoryName,
                                    p.UnitPrice,

                                    CurrentStock =
                                        p.StockQuantity,

                                    LowStockThreshold =
                                        p.LowStockThreshold,

                                    UnitsSold =
                                        unitsSold,

                                    SalesAmount =
                                        salesAmount,

                                    SalesVelocity =
                                        salesVelocity,

                                    PlanningHorizonDays =
                                        planningHorizonDays,

                                    ProjectedDemand =
                                        projectedDemand,

                                    TargetStock =
                                        targetStock,

                                    RecommendedReorderQuantity =
                                        recommendedReorderQuantity,

                                    ExpectedCoverageDays =
                                        expectedCoverageDays,

                                    ShouldRestock =
                                        shouldRestock,

                                    Priority =
                                        priority,

                                    Reason =
                                        reason
                                };
                            })
                        .ToList();

                // -------------------------------------------------
                // PRODUCTS THAT SHOULD BE RESTOCKED
                // -------------------------------------------------
                var productsToRestock =
                    restockAnalysis
                        .Where(
                            p =>
                                p.ShouldRestock &&
                                p.RecommendedReorderQuantity > 0)
                        .OrderBy(
                            p =>
                                p.Priority ==
                                    "Immediate" ? 1 :
                                p.Priority ==
                                    "High" ? 2 :
                                p.Priority ==
                                    "Planned" ? 3 : 4)
                        .ThenBy(
                            p =>
                                p.ExpectedCoverageDays ??
                                decimal.MaxValue)
                        .ToList();

                // -------------------------------------------------
                // PRODUCTS TO MONITOR
                // -------------------------------------------------
                var productsToMonitor =
                    restockAnalysis
                        .Where(
                            p =>
                                !p.ShouldRestock ||
                                p.RecommendedReorderQuantity == 0)
                        .OrderByDescending(
                            p =>
                                p.UnitsSold)
                        .ToList();

                // -------------------------------------------------
                // TOTAL RECOMMENDED UNITS
                // -------------------------------------------------
                var totalRecommendedUnits =
                    productsToRestock
                        .Sum(
                            p =>
                                p.RecommendedReorderQuantity);

                return Serialize(new
                {
                    DataType =
                        "Smart Restock Recommendation",

                    PlanningSettings = new
                    {
                        PlanningHorizonDays =
                            planningHorizonDays
                    },

                    Summary = new
                    {
                        ProductsRequiringRestock =
                            productsToRestock.Count,

                        ProductsToMonitor =
                            productsToMonitor.Count,

                        TotalRecommendedUnits =
                            totalRecommendedUnits
                    },

                    ProductsToRestock =
                        productsToRestock,

                    ProductsToMonitor =
                        productsToMonitor,

                    FullRestockAnalysis =
                        restockAnalysis
                });
            }

            // =====================================================
            // 4. SALES
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
                var totalSales =
                    await _context.Sales
                        .AsNoTracking()
                        .SumAsync(
                            s =>
                                (decimal?)
                                    s.TotalAmount)
                        ?? 0;

                var salesByProduct =
                    await _context.SaleItems
                        .AsNoTracking()
                        .Where(
                            si =>
                                si.Product != null)
                        .GroupBy(
                            si =>
                                new
                                {
                                    si.ProductID,

                                    ProductName =
                                        si.Product!
                                            .ProductName,

                                    SKU =
                                        si.Product!
                                            .SKU
                                })
                        .Select(
                            g => new
                            {
                                g.Key.ProductID,

                                g.Key.ProductName,

                                g.Key.SKU,

                                QuantitySold =
                                    g.Sum(
                                        x =>
                                            x.Quantity),

                                SalesAmount =
                                    g.Sum(
                                        x =>
                                            x.Quantity *
                                            x.UnitPrice)
                            })
                        .OrderByDescending(
                            x =>
                                x.QuantitySold)
                        .ToListAsync();

                var sales =
                    await _context.Sales
                        .AsNoTracking()
                        .Select(
                            s => new
                            {
                                s.SaleID,
                                s.SaleDate,
                                s.CustomerInfo,
                                s.TotalAmount
                            })
                        .OrderBy(
                            s =>
                                s.SaleDate)
                        .ToListAsync();

                var saleItems =
                    await _context.SaleItems
                        .AsNoTracking()
                        .Where(
                            si =>
                                si.Product != null &&
                                si.Sale != null)
                        .Select(
                            si => new
                            {
                                si.SaleItemID,
                                si.SaleID,

                                SaleDate =
                                    si.Sale!
                                        .SaleDate,

                                CustomerInfo =
                                    si.Sale!
                                        .CustomerInfo,

                                ProductName =
                                    si.Product!
                                        .ProductName,

                                SKU =
                                    si.Product!
                                        .SKU,

                                si.Quantity,
                                si.UnitPrice,

                                Total =
                                    si.Quantity *
                                    si.UnitPrice
                            })
                        .ToListAsync();

                return Serialize(new
                {
                    DataType =
                        "Sales Analytics",

                    TotalSales =
                        totalSales,

                    SalesTransactions =
                        sales,

                    SalesByProduct =
                        salesByProduct,

                    SaleItems =
                        saleItems
                });
            }

            // =====================================================
            // 5. PURCHASES
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
                var totalPurchases =
                    await _context.Purchases
                        .AsNoTracking()
                        .SumAsync(
                            p =>
                                (decimal?)
                                    p.TotalAmount)
                        ?? 0;

                var purchasesBySupplier =
                    await _context.Purchases
                        .AsNoTracking()
                        .Where(
                            p =>
                                p.Supplier != null)
                        .GroupBy(
                            p =>
                                new
                                {
                                    p.SupplierID,

                                    SupplierName =
                                        p.Supplier!
                                            .SupplierName
                                })
                        .Select(
                            g => new
                            {
                                g.Key.SupplierID,
                                g.Key.SupplierName,

                                TotalPurchaseAmount =
                                    g.Sum(
                                        x =>
                                            x.TotalAmount)
                            })
                        .OrderByDescending(
                            x =>
                                x.TotalPurchaseAmount)
                        .ToListAsync();

                var purchases =
                    await _context.Purchases
                        .AsNoTracking()
                        .Where(
                            p =>
                                p.Supplier != null)
                        .Select(
                            p => new
                            {
                                p.PurchaseID,
                                p.PurchaseDate,

                                SupplierName =
                                    p.Supplier!
                                        .SupplierName,

                                p.TotalAmount
                            })
                        .OrderBy(
                            p =>
                                p.PurchaseDate)
                        .ToListAsync();

                var purchaseItems =
                    await _context.PurchaseItems
                        .AsNoTracking()
                        .Where(
                            pi =>
                                pi.Product != null &&
                                pi.Purchase != null)
                        .Select(
                            pi => new
                            {
                                pi.PurchaseItemID,
                                pi.PurchaseID,

                                PurchaseDate =
                                    pi.Purchase!
                                        .PurchaseDate,

                                SupplierName =
                                    pi.Purchase!
                                        .Supplier!
                                        .SupplierName,

                                ProductName =
                                    pi.Product!
                                        .ProductName,

                                SKU =
                                    pi.Product!.SKU,

                                pi.Quantity,
                                pi.UnitCost,

                                Total =
                                    pi.Quantity *
                                    pi.UnitCost
                            })
                        .ToListAsync();

                return Serialize(new
                {
                    DataType =
                        "Purchase Analytics",

                    TotalPurchases =
                        totalPurchases,

                    PurchasesBySupplier =
                        purchasesBySupplier,

                    PurchaseTransactions =
                        purchases,

                    PurchaseItems =
                        purchaseItems
                });
            }

            // =====================================================
            // 6. LOW STOCK / OUT OF STOCK / RESTOCK
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
                if (
                    query.Contains("out of stock") ||
                    query.Contains("out-of-stock"))
                {
                    var outOfStockProducts =
                        await _context.Products
                            .AsNoTracking()
                            .Where(
                                p =>
                                    p.StockQuantity == 0)
                            .Select(
                                p => new
                                {
                                    p.ProductID,
                                    p.ProductName,
                                    p.SKU,
                                    p.StockQuantity,
                                    p.LowStockThreshold
                                })
                            .OrderBy(
                                p =>
                                    p.ProductName)
                            .ToListAsync();

                    return Serialize(new
                    {
                        DataType =
                            "Out of Stock Analysis",

                        OutOfStockCount =
                            outOfStockProducts.Count,

                        OutOfStockProducts =
                            outOfStockProducts
                    });
                }

                // -------------------------------------------------
                // LOW STOCK
                // -------------------------------------------------
                var lowStockProducts =
                    await _context.Products
                        .AsNoTracking()
                        .Where(
                            p =>
                                p.StockQuantity > 0 &&
                                p.StockQuantity <=
                                p.LowStockThreshold)
                        .Select(
                            p => new
                            {
                                p.ProductID,
                                p.ProductName,
                                p.SKU,
                                p.StockQuantity,
                                p.LowStockThreshold,
                                p.UnitPrice,

                                CategoryName =
                                    p.Category != null
                                        ? p.Category.CategoryName
                                        : null
                            })
                        .OrderBy(
                            p =>
                                p.StockQuantity)
                        .ToListAsync();

                var lowStockProductIds =
                    lowStockProducts
                        .Select(
                            p =>
                                p.ProductID)
                        .ToList();

                // -------------------------------------------------
                // SUPPLIERS FOR LOW STOCK PRODUCTS
                // -------------------------------------------------
                var suppliersForLowStock =
                    await _context.SupplierProducts
                        .AsNoTracking()
                        .Where(
                            sp =>
                                sp.Product != null &&
                                sp.Supplier != null &&
                                lowStockProductIds
                                    .Contains(
                                        sp.ProductID))
                        .Select(
                            sp => new
                            {
                                ProductName =
                                    sp.Product!
                                        .ProductName,

                                SKU =
                                    sp.Product!
                                        .SKU,

                                SupplierName =
                                    sp.Supplier!
                                        .SupplierName,

                                sp.ContractPrice,
                                sp.LeadTimeDays
                            })
                        .OrderBy(
                            x =>
                                x.ProductName)
                        .ThenBy(
                            x =>
                                x.ContractPrice)
                        .ToListAsync();

                return Serialize(new
                {
                    DataType =
                        "Low Stock and Restock Analysis",

                    LowStockCount =
                        lowStockProducts.Count,

                    LowStockProducts =
                        lowStockProducts,

                    AvailableSuppliers =
                        suppliersForLowStock
                });
            }

            // =====================================================
            // 7. INVENTORY ANALYTICS
            // =====================================================
            if (ContainsAny(
                query,
                "analytics",
                "analysis",
                "inventory summary",
                "inventory overview",
                "stock summary",
                "stock value",
                "inventory value",
                "total inventory value",
                "average price",
                "average product price",
                "highest stock",
                "lowest stock",
                "highest inventory value",
                "category stock",
                "stock by category",
                "inventory statistics",
                "inventory stats",
                "stock statistics",
                "stock stats",
                "how many products",
                "how many categories"))
            {
                var products =
                    await _context.Products
                        .AsNoTracking()
                        .Select(
                            p => new
                            {
                                p.ProductID,
                                p.ProductName,
                                p.SKU,
                                p.UnitPrice,
                                p.StockQuantity,
                                p.LowStockThreshold,

                                CategoryName =
                                    p.Category != null
                                        ? p.Category.CategoryName
                                        : null
                            })
                        .ToListAsync();

                var totalProducts =
                    products.Count;

                var totalCategories =
                    await _context.Categories
                        .AsNoTracking()
                        .CountAsync();

                var totalStockUnits =
                    products.Sum(
                        p =>
                            p.StockQuantity);

                var totalInventoryValue =
                    products.Sum(
                        p =>
                            p.UnitPrice *
                            p.StockQuantity);

                var averageProductPrice =
                    products.Count > 0
                        ? products.Average(
                            p =>
                                p.UnitPrice)
                        : 0;

                var lowStockCount =
                    products.Count(
                        p =>
                            p.StockQuantity > 0 &&
                            p.StockQuantity <=
                            p.LowStockThreshold);

                var outOfStockCount =
                    products.Count(
                        p =>
                            p.StockQuantity == 0);

                var stockByCategory =
                    products
                        .GroupBy(
                            p =>
                                p.CategoryName ??
                                "Uncategorized")
                        .Select(
                            g => new
                            {
                                Category =
                                    g.Key,

                                ProductCount =
                                    g.Count(),

                                TotalStockUnits =
                                    g.Sum(
                                        p =>
                                            p.StockQuantity),

                                InventoryValue =
                                    g.Sum(
                                        p =>
                                            p.UnitPrice *
                                            p.StockQuantity)
                            })
                        .OrderByDescending(
                            x =>
                                x.TotalStockUnits)
                        .ToList();

                var highestStockProduct =
                    products
                        .OrderByDescending(
                            p =>
                                p.StockQuantity)
                        .Select(
                            p => new
                            {
                                p.ProductName,
                                p.SKU,
                                p.StockQuantity
                            })
                        .FirstOrDefault();

                var highestValueProduct =
                    products
                        .OrderByDescending(
                            p =>
                                p.UnitPrice *
                                p.StockQuantity)
                        .Select(
                            p => new
                            {
                                p.ProductName,
                                p.SKU,
                                p.StockQuantity,
                                p.UnitPrice,

                                InventoryValue =
                                    p.UnitPrice *
                                    p.StockQuantity
                            })
                        .FirstOrDefault();

                return Serialize(new
                {
                    DataType =
                        "Inventory Analytics",

                    Summary = new
                    {
                        TotalProducts =
                            totalProducts,

                        TotalCategories =
                            totalCategories,

                        TotalStockUnits =
                            totalStockUnits,

                        TotalInventoryValue =
                            totalInventoryValue,

                        AverageProductPrice =
                            averageProductPrice,

                        LowStockProducts =
                            lowStockCount,

                        OutOfStockProducts =
                            outOfStockCount
                    },

                    HighestStockProduct =
                        highestStockProduct,

                    HighestInventoryValueProduct =
                        highestValueProduct,

                    StockByCategory =
                        stockByCategory
                });
            }

            // =====================================================
            // 8. SUPPLIERS
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
                var suppliers =
                    await _context.Suppliers
                        .AsNoTracking()
                        .Select(
                            s => new
                            {
                                s.SupplierID,
                                s.SupplierName,
                                s.ContactName,
                                s.Phone,
                                s.Email,
                                s.Address
                            })
                        .ToListAsync();

                var supplierProducts =
                    await _context.SupplierProducts
                        .AsNoTracking()
                        .Where(
                            sp =>
                                sp.Supplier != null &&
                                sp.Product != null)
                        .Select(
                            sp => new
                            {
                                sp.SupplierProductID,

                                SupplierName =
                                    sp.Supplier!
                                        .SupplierName,

                                SupplierID =
                                    sp.SupplierID,

                                ProductName =
                                    sp.Product!
                                        .ProductName,

                                ProductID =
                                    sp.ProductID,

                                ProductSKU =
                                    sp.Product!
                                        .SKU,

                                sp.SupplierSKU,

                                sp.ContractPrice,

                                sp.LeadTimeDays
                            })
                        .OrderBy(
                            sp =>
                                sp.ProductName)
                        .ThenBy(
                            sp =>
                                sp.ContractPrice)
                        .ToListAsync();

                return Serialize(new
                {
                    DataType =
                        "Supplier Information",

                    Suppliers =
                        suppliers,

                    SupplierProducts =
                        supplierProducts
                });
            }

            // =====================================================
            // 9. PRODUCTS / CATEGORIES / STOCK
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
                "prices"))
            {
                var categories =
                    await _context.Categories
                        .AsNoTracking()
                        .Select(
                            c => new
                            {
                                c.CategoryID,
                                c.CategoryName,
                                c.Description
                            })
                        .ToListAsync();

                var products =
                    await _context.Products
                        .AsNoTracking()
                        .Select(
                            p => new
                            {
                                p.ProductID,
                                p.SKU,
                                p.ProductName,

                                CategoryName =
                                    p.Category != null
                                        ? p.Category.CategoryName
                                        : null,

                                p.UnitPrice,
                                p.StockQuantity,
                                p.LowStockThreshold
                            })
                        .OrderBy(
                            p =>
                                p.ProductName)
                        .ToListAsync();

                var totalProducts =
                    products.Count;

                var totalStockUnits =
                    products.Sum(
                        p =>
                            p.StockQuantity);

                return Serialize(new
                {
                    DataType =
                        "Product and Inventory Information",

                    TotalProducts =
                        totalProducts,

                    TotalStockUnits =
                        totalStockUnits,

                    Categories =
                        categories,

                    Products =
                        products
                });
            }

            // =====================================================
            // 10. GENERAL INVENTORY OVERVIEW
            // =====================================================
            var allCategories =
                await _context.Categories
                    .AsNoTracking()
                    .Select(
                        c => new
                        {
                            c.CategoryID,
                            c.CategoryName,
                            c.Description
                        })
                    .ToListAsync();

            var allProducts =
                await _context.Products
                    .AsNoTracking()
                    .Select(
                        p => new
                        {
                            p.ProductID,
                            p.SKU,
                            p.ProductName,

                            CategoryName =
                                p.Category != null
                                    ? p.Category.CategoryName
                                    : null,

                            p.UnitPrice,
                            p.StockQuantity,
                            p.LowStockThreshold
                        })
                    .ToListAsync();

            var allSuppliers =
                await _context.Suppliers
                    .AsNoTracking()
                    .Select(
                        s => new
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
                DataType =
                    "General Inventory Overview",

                Categories =
                    allCategories,

                Products =
                    allProducts,

                Suppliers =
                    allSuppliers
            });
        }

        // =====================================================
        // CHECK IF QUERY CONTAINS ANY KEYWORD
        // =====================================================
        private static bool ContainsAny(
            string query,
            params string[] keywords)
        {
            return keywords.Any(
                query.Contains);
        }

        // =====================================================
        // SERIALIZE DATABASE DATA
        // =====================================================
        private static string Serialize(
            object data)
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