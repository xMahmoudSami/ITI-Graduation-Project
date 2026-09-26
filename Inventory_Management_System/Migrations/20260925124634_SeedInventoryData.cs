using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    public partial class SeedInventoryData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================
            // Categories
            // =========================
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Electronics')
                    INSERT INTO Categories (CategoryName, Description)
                    VALUES ('Electronics', NULL);

                IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Computer Accessories')
                    INSERT INTO Categories (CategoryName, Description)
                    VALUES ('Computer Accessories', NULL);

                IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Office Supplies')
                    INSERT INTO Categories (CategoryName, Description)
                    VALUES ('Office Supplies', NULL);

                IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Networking')
                    INSERT INTO Categories (CategoryName, Description)
                    VALUES ('Networking', NULL);

                IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Storage')
                    INSERT INTO Categories (CategoryName, Description)
                    VALUES ('Storage', NULL);
                """);

            // =========================
            // Suppliers
            // =========================
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE Email = 'ahmed@techworld.com')
                    INSERT INTO Suppliers
                        (SupplierName, ContactName, Phone, Email, Address)
                    VALUES
                        ('Tech World Egypt', 'Ahmed Hassan', '01012345678',
                         'ahmed@techworld.com', 'Nasr City, Cairo');

                IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE Email = 'mona@smartoffice.com')
                    INSERT INTO Suppliers
                        (SupplierName, ContactName, Phone, Email, Address)
                    VALUES
                        ('Smart Office Supplies', 'Mona Ali', '01123456789',
                         'mona@smartoffice.com', 'Heliopolis, Cairo');

                IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE Email = 'omar@cns.com')
                    INSERT INTO Suppliers
                        (SupplierName, ContactName, Phone, Email, Address)
                    VALUES
                        ('Cairo Networking Solutions', 'Omar Mahmoud', '01234567890',
                         'omar@cns.com', 'Maadi, Cairo');

                IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE Email = 'sara@digitalstore.com')
                    INSERT INTO Suppliers
                        (SupplierName, ContactName, Phone, Email, Address)
                    VALUES
                        ('Digital Store Egypt', 'Sara Mohamed', '01098765432',
                         'sara@digitalstore.com', 'Dokki, Giza');

                IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                    INSERT INTO Suppliers
                        (SupplierName, ContactName, Phone, Email, Address)
                    VALUES
                        ('Future Tech Distribution', 'Youssef Adel', '01198765432',
                         'youssef@futuretech.com', '6th of October, Giza');
                """);

            // =========================
            // Products
            // =========================
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU = 'LAP-001')
                    INSERT INTO Products
                        (SKU, ProductName, CategoryID, UnitPrice, StockQuantity, LowStockThreshold)
                    VALUES
                        ('LAP-001', 'Dell Inspiron 15 Laptop',
                         (SELECT CategoryID FROM Categories WHERE CategoryName = 'Electronics'),
                         25000, 8, 5);

                IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU = 'MON-001')
                    INSERT INTO Products
                        (SKU, ProductName, CategoryID, UnitPrice, StockQuantity, LowStockThreshold)
                    VALUES
                        ('MON-001', 'Samsung 24 Inch Monitor',
                         (SELECT CategoryID FROM Categories WHERE CategoryName = 'Electronics'),
                         7000, 3, 5);

                IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU = 'MOU-001')
                    INSERT INTO Products
                        (SKU, ProductName, CategoryID, UnitPrice, StockQuantity, LowStockThreshold)
                    VALUES
                        ('MOU-001', 'Logitech Wireless Mouse',
                         (SELECT CategoryID FROM Categories WHERE CategoryName = 'Computer Accessories'),
                         650, 20, 5);

                IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU = 'KEY-001')
                    INSERT INTO Products
                        (SKU, ProductName, CategoryID, UnitPrice, StockQuantity, LowStockThreshold)
                    VALUES
                        ('KEY-001', 'Mechanical Keyboard',
                         (SELECT CategoryID FROM Categories WHERE CategoryName = 'Computer Accessories'),
                         1800, 4, 5);

                IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU = 'PEN-001')
                    INSERT INTO Products
                        (SKU, ProductName, CategoryID, UnitPrice, StockQuantity, LowStockThreshold)
                    VALUES
                        ('PEN-001', 'Ballpoint Pens Pack',
                         (SELECT CategoryID FROM Categories WHERE CategoryName = 'Office Supplies'),
                         120, 50, 10);

                IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU = 'PAP-001')
                    INSERT INTO Products
                        (SKU, ProductName, CategoryID, UnitPrice, StockQuantity, LowStockThreshold)
                    VALUES
                        ('PAP-001', 'A4 Paper Ream',
                         (SELECT CategoryID FROM Categories WHERE CategoryName = 'Office Supplies'),
                         220, 6, 10);

                IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU = 'RTR-001')
                    INSERT INTO Products
                        (SKU, ProductName, CategoryID, UnitPrice, StockQuantity, LowStockThreshold)
                    VALUES
                        ('RTR-001', 'TP-Link WiFi Router',
                         (SELECT CategoryID FROM Categories WHERE CategoryName = 'Networking'),
                         2200, 7, 3);

                IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU = 'CAB-001')
                    INSERT INTO Products
                        (SKU, ProductName, CategoryID, UnitPrice, StockQuantity, LowStockThreshold)
                    VALUES
                        ('CAB-001', 'Ethernet Cable 5m',
                         (SELECT CategoryID FROM Categories WHERE CategoryName = 'Networking'),
                         150, 30, 10);

                IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU = 'SSD-001')
                    INSERT INTO Products
                        (SKU, ProductName, CategoryID, UnitPrice, StockQuantity, LowStockThreshold)
                    VALUES
                        ('SSD-001', 'Samsung 1TB SSD',
                         (SELECT CategoryID FROM Categories WHERE CategoryName = 'Storage'),
                         3500, 2, 3);

                IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU = 'USB-001')
                    INSERT INTO Products
                        (SKU, ProductName, CategoryID, UnitPrice, StockQuantity, LowStockThreshold)
                    VALUES
                        ('USB-001', 'Kingston 64GB USB Flash Drive',
                         (SELECT CategoryID FROM Categories WHERE CategoryName = 'Storage'),
                         450, 25, 5);
                """);

            // =========================
            // SupplierProducts
            // =========================
            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'LAP-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'LAP-001'),
                        'TW-LAP-001', 23500, 5
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'MON-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'MON-001'),
                        'TW-MON-001', 6500, 4
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'MOU-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'MOU-001'),
                        'TW-MOU-001', 500, 3
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'RTR-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'RTR-001'),
                        'TW-RTR-001', 1900, 5
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'PEN-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'PEN-001'),
                        'SOS-PEN-001', 90, 2
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'PAP-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'PAP-001'),
                        'SOS-PAP-001', 170, 2
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'USB-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'USB-001'),
                        'SOS-USB-001', 350, 4
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'omar@cns.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'RTR-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'omar@cns.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'RTR-001'),
                        'CNS-RTR-001', 1850, 3
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'omar@cns.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'CAB-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'omar@cns.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'CAB-001'),
                        'CNS-CAB-001', 100, 2
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'LAP-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'LAP-001'),
                        'DS-LAP-001', 23200, 6
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'KEY-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'KEY-001'),
                        'DS-KEY-001', 1500, 4
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'SSD-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'SSD-001'),
                        'DS-SSD-001', 3000, 5
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'MON-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'MON-001'),
                        'FT-MON-001', 6400, 5
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'MOU-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'MOU-001'),
                        'FT-MOU-001', 480, 3
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'KEY-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'KEY-001'),
                        'FT-KEY-001', 1450, 4
                    );

                IF NOT EXISTS (
                    SELECT 1 FROM SupplierProducts
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                    AND ProductID = (SELECT ProductID FROM Products WHERE SKU = 'SSD-001')
                )
                INSERT INTO SupplierProducts
                    (SupplierID, ProductID, SupplierSKU, ContractPrice, LeadTimeDays)
                VALUES
                    (
                        (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com'),
                        (SELECT ProductID FROM Products WHERE SKU = 'SSD-001'),
                        'FT-SSD-001', 2950, 5
                    );
                """);

            // =========================
            // Purchases
            // =========================
            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1 FROM Purchases
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com')
                    AND PurchaseDate = '2026-09-10'
                )
                INSERT INTO Purchases (SupplierID, PurchaseDate, TotalAmount)
                VALUES (
                    (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com'),
                    '2026-09-10',
                    53500
                );

                IF NOT EXISTS (
                    SELECT 1 FROM Purchases
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com')
                    AND PurchaseDate = '2026-09-12'
                )
                INSERT INTO Purchases (SupplierID, PurchaseDate, TotalAmount)
                VALUES (
                    (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com'),
                    '2026-09-12',
                    4350
                );

                IF NOT EXISTS (
                    SELECT 1 FROM Purchases
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'omar@cns.com')
                    AND PurchaseDate = '2026-09-15'
                )
                INSERT INTO Purchases (SupplierID, PurchaseDate, TotalAmount)
                VALUES (
                    (SELECT SupplierID FROM Suppliers WHERE Email = 'omar@cns.com'),
                    '2026-09-15',
                    6200
                );

                IF NOT EXISTS (
                    SELECT 1 FROM Purchases
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com')
                    AND PurchaseDate = '2026-09-18'
                )
                INSERT INTO Purchases (SupplierID, PurchaseDate, TotalAmount)
                VALUES (
                    (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com'),
                    '2026-09-18',
                    35200
                );

                IF NOT EXISTS (
                    SELECT 1 FROM Purchases
                    WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                    AND PurchaseDate = '2026-09-20'
                )
                INSERT INTO Purchases (SupplierID, PurchaseDate, TotalAmount)
                VALUES (
                    (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com'),
                    '2026-09-20',
                    23450
                );
                """);

            // =========================
            // PurchaseItems
            // =========================
            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com')
                    AND p.PurchaseDate = '2026-09-10'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'LAP-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com')
                     AND PurchaseDate = '2026-09-10'),
                    (SELECT ProductID FROM Products WHERE SKU = 'LAP-001'),
                    2, 23500
                );

                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com')
                    AND p.PurchaseDate = '2026-09-10'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'MON-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'ahmed@techworld.com')
                     AND PurchaseDate = '2026-09-10'),
                    (SELECT ProductID FROM Products WHERE SKU = 'MON-001'),
                    1, 6500
                );

                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com')
                    AND p.PurchaseDate = '2026-09-12'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'PEN-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com')
                     AND PurchaseDate = '2026-09-12'),
                    (SELECT ProductID FROM Products WHERE SKU = 'PEN-001'),
                    10, 90
                );

                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com')
                    AND p.PurchaseDate = '2026-09-12'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'PAP-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com')
                     AND PurchaseDate = '2026-09-12'),
                    (SELECT ProductID FROM Products WHERE SKU = 'PAP-001'),
                    10, 170
                );

                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com')
                    AND p.PurchaseDate = '2026-09-12'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'USB-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'mona@smartoffice.com')
                     AND PurchaseDate = '2026-09-12'),
                    (SELECT ProductID FROM Products WHERE SKU = 'USB-001'),
                    5, 350
                );

                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'omar@cns.com')
                    AND p.PurchaseDate = '2026-09-15'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'RTR-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'omar@cns.com')
                     AND PurchaseDate = '2026-09-15'),
                    (SELECT ProductID FROM Products WHERE SKU = 'RTR-001'),
                    2, 1850
                );

                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'omar@cns.com')
                    AND p.PurchaseDate = '2026-09-15'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'CAB-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'omar@cns.com')
                     AND PurchaseDate = '2026-09-15'),
                    (SELECT ProductID FROM Products WHERE SKU = 'CAB-001'),
                    25, 100
                );

                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com')
                    AND p.PurchaseDate = '2026-09-18'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'LAP-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com')
                     AND PurchaseDate = '2026-09-18'),
                    (SELECT ProductID FROM Products WHERE SKU = 'LAP-001'),
                    1, 23200
                );

                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com')
                    AND p.PurchaseDate = '2026-09-18'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'KEY-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com')
                     AND PurchaseDate = '2026-09-18'),
                    (SELECT ProductID FROM Products WHERE SKU = 'KEY-001'),
                    2, 1500
                );

                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com')
                    AND p.PurchaseDate = '2026-09-18'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'SSD-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'sara@digitalstore.com')
                     AND PurchaseDate = '2026-09-18'),
                    (SELECT ProductID FROM Products WHERE SKU = 'SSD-001'),
                    3, 3000
                );

                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                    AND p.PurchaseDate = '2026-09-20'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'MON-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                     AND PurchaseDate = '2026-09-20'),
                    (SELECT ProductID FROM Products WHERE SKU = 'MON-001'),
                    2, 6400
                );

                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                    AND p.PurchaseDate = '2026-09-20'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'MOU-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                     AND PurchaseDate = '2026-09-20'),
                    (SELECT ProductID FROM Products WHERE SKU = 'MOU-001'),
                    10, 480
                );

                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                    AND p.PurchaseDate = '2026-09-20'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'KEY-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                     AND PurchaseDate = '2026-09-20'),
                    (SELECT ProductID FROM Products WHERE SKU = 'KEY-001'),
                    2, 1450
                );

                IF NOT EXISTS (
                    SELECT 1 FROM PurchaseItems pi
                    JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                    WHERE p.SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                    AND p.PurchaseDate = '2026-09-20'
                    AND pi.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'SSD-001')
                )
                INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitCost)
                VALUES (
                    (SELECT PurchaseID FROM Purchases
                     WHERE SupplierID = (SELECT SupplierID FROM Suppliers WHERE Email = 'youssef@futuretech.com')
                     AND PurchaseDate = '2026-09-20'),
                    (SELECT ProductID FROM Products WHERE SKU = 'SSD-001'),
                    1, 2950
                );
                """);

            // =========================
            // Sales
            // =========================
            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1 FROM Sales
                    WHERE SaleDate = '2026-09-11' AND CustomerInfo = 'Customer 001'
                )
                INSERT INTO Sales (SaleDate, TotalAmount, CustomerInfo)
                VALUES ('2026-09-11', 26300, 'Customer 001');

                IF NOT EXISTS (
                    SELECT 1 FROM Sales
                    WHERE SaleDate = '2026-09-13' AND CustomerInfo = 'Customer 002'
                )
                INSERT INTO Sales (SaleDate, TotalAmount, CustomerInfo)
                VALUES ('2026-09-13', 2100, 'Customer 002');

                IF NOT EXISTS (
                    SELECT 1 FROM Sales
                    WHERE SaleDate = '2026-09-16' AND CustomerInfo = 'Customer 003'
                )
                INSERT INTO Sales (SaleDate, TotalAmount, CustomerInfo)
                VALUES ('2026-09-16', 8800, 'Customer 003');

                IF NOT EXISTS (
                    SELECT 1 FROM Sales
                    WHERE SaleDate = '2026-09-19' AND CustomerInfo = 'Customer 004'
                )
                INSERT INTO Sales (SaleDate, TotalAmount, CustomerInfo)
                VALUES ('2026-09-19', 3130, 'Customer 004');

                IF NOT EXISTS (
                    SELECT 1 FROM Sales
                    WHERE SaleDate = '2026-09-21' AND CustomerInfo = 'Customer 005'
                )
                INSERT INTO Sales (SaleDate, TotalAmount, CustomerInfo)
                VALUES ('2026-09-21', 7900, 'Customer 005');

                IF NOT EXISTS (
                    SELECT 1 FROM Sales
                    WHERE SaleDate = '2026-09-23' AND CustomerInfo = 'Customer 006'
                )
                INSERT INTO Sales (SaleDate, TotalAmount, CustomerInfo)
                VALUES ('2026-09-23', 3900, 'Customer 006');
                """);

            // =========================
            // SaleItems
            // =========================
            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1 FROM SaleItems si
                    JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate = '2026-09-11'
                      AND s.CustomerInfo = 'Customer 001'
                      AND si.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'LAP-001')
                )
                INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice)
                VALUES (
                    (SELECT SaleID FROM Sales
                     WHERE SaleDate = '2026-09-11' AND CustomerInfo = 'Customer 001'),
                    (SELECT ProductID FROM Products WHERE SKU = 'LAP-001'),
                    1, 25000
                );

                IF NOT EXISTS (
                    SELECT 1 FROM SaleItems si
                    JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate = '2026-09-11'
                      AND s.CustomerInfo = 'Customer 001'
                      AND si.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'MOU-001')
                )
                INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice)
                VALUES (
                    (SELECT SaleID FROM Sales
                     WHERE SaleDate = '2026-09-11' AND CustomerInfo = 'Customer 001'),
                    (SELECT ProductID FROM Products WHERE SKU = 'MOU-001'),
                    2, 650
                );

                IF NOT EXISTS (
                    SELECT 1 FROM SaleItems si
                    JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate = '2026-09-13'
                      AND s.CustomerInfo = 'Customer 002'
                      AND si.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'PEN-001')
                )
                INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice)
                VALUES (
                    (SELECT SaleID FROM Sales
                     WHERE SaleDate = '2026-09-13' AND CustomerInfo = 'Customer 002'),
                    (SELECT ProductID FROM Products WHERE SKU = 'PEN-001'),
                    5, 120
                );

                IF NOT EXISTS (
                    SELECT 1 FROM SaleItems si
                    JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate = '2026-09-13'
                      AND s.CustomerInfo = 'Customer 002'
                      AND si.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'CAB-001')
                )
                INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice)
                VALUES (
                    (SELECT SaleID FROM Sales
                     WHERE SaleDate = '2026-09-13' AND CustomerInfo = 'Customer 002'),
                    (SELECT ProductID FROM Products WHERE SKU = 'CAB-001'),
                    10, 150
                );

                IF NOT EXISTS (
                    SELECT 1 FROM SaleItems si
                    JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate = '2026-09-16'
                      AND s.CustomerInfo = 'Customer 003'
                      AND si.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'MON-001')
                )
                INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice)
                VALUES (
                    (SELECT SaleID FROM Sales
                     WHERE SaleDate = '2026-09-16' AND CustomerInfo = 'Customer 003'),
                    (SELECT ProductID FROM Products WHERE SKU = 'MON-001'),
                    1, 7000
                );

                IF NOT EXISTS (
                    SELECT 1 FROM SaleItems si
                    JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate = '2026-09-16'
                      AND s.CustomerInfo = 'Customer 003'
                      AND si.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'KEY-001')
                )
                INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice)
                VALUES (
                    (SELECT SaleID FROM Sales
                     WHERE SaleDate = '2026-09-16' AND CustomerInfo = 'Customer 003'),
                    (SELECT ProductID FROM Products WHERE SKU = 'KEY-001'),
                    1, 1800
                );

                IF NOT EXISTS (
                    SELECT 1 FROM SaleItems si
                    JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate = '2026-09-19'
                      AND s.CustomerInfo = 'Customer 004'
                      AND si.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'PAP-001')
                )
                INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice)
                VALUES (
                    (SELECT SaleID FROM Sales
                     WHERE SaleDate = '2026-09-19' AND CustomerInfo = 'Customer 004'),
                    (SELECT ProductID FROM Products WHERE SKU = 'PAP-001'),
                    4, 220
                );

                IF NOT EXISTS (
                    SELECT 1 FROM SaleItems si
                    JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate = '2026-09-19'
                      AND s.CustomerInfo = 'Customer 004'
                      AND si.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'USB-001')
                )
                INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice)
                VALUES (
                    (SELECT SaleID FROM Sales
                     WHERE SaleDate = '2026-09-19' AND CustomerInfo = 'Customer 004'),
                    (SELECT ProductID FROM Products WHERE SKU = 'USB-001'),
                    5, 450
                );

                IF NOT EXISTS (
                    SELECT 1 FROM SaleItems si
                    JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate = '2026-09-21'
                      AND s.CustomerInfo = 'Customer 005'
                      AND si.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'RTR-001')
                )
                INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice)
                VALUES (
                    (SELECT SaleID FROM Sales
                     WHERE SaleDate = '2026-09-21' AND CustomerInfo = 'Customer 005'),
                    (SELECT ProductID FROM Products WHERE SKU = 'RTR-001'),
                    2, 2200
                );

                IF NOT EXISTS (
                    SELECT 1 FROM SaleItems si
                    JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate = '2026-09-21'
                      AND s.CustomerInfo = 'Customer 005'
                      AND si.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'SSD-001')
                )
                INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice)
                VALUES (
                    (SELECT SaleID FROM Sales
                     WHERE SaleDate = '2026-09-21' AND CustomerInfo = 'Customer 005'),
                    (SELECT ProductID FROM Products WHERE SKU = 'SSD-001'),
                    1, 3500
                );

                IF NOT EXISTS (
                    SELECT 1 FROM SaleItems si
                    JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate = '2026-09-23'
                      AND s.CustomerInfo = 'Customer 006'
                      AND si.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'MOU-001')
                )
                INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice)
                VALUES (
                    (SELECT SaleID FROM Sales
                     WHERE SaleDate = '2026-09-23' AND CustomerInfo = 'Customer 006'),
                    (SELECT ProductID FROM Products WHERE SKU = 'MOU-001'),
                    3, 650
                );

                IF NOT EXISTS (
                    SELECT 1 FROM SaleItems si
                    JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate = '2026-09-23'
                      AND s.CustomerInfo = 'Customer 006'
                      AND si.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'CAB-001')
                )
                INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice)
                VALUES (
                    (SELECT SaleID FROM Sales
                     WHERE SaleDate = '2026-09-23' AND CustomerInfo = 'Customer 006'),
                    (SELECT ProductID FROM Products WHERE SKU = 'CAB-001'),
                    5, 150
                );

                IF NOT EXISTS (
                    SELECT 1 FROM SaleItems si
                    JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate = '2026-09-23'
                      AND s.CustomerInfo = 'Customer 006'
                      AND si.ProductID = (SELECT ProductID FROM Products WHERE SKU = 'PEN-001')
                )
                INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice)
                VALUES (
                    (SELECT SaleID FROM Sales
                     WHERE SaleDate = '2026-09-23' AND CustomerInfo = 'Customer 006'),
                    (SELECT ProductID FROM Products WHERE SKU = 'PEN-001'),
                    10, 120
                );
                """);
            // =========================
            // Recalculate totals
            // =========================
            migrationBuilder.Sql("""
    UPDATE p
    SET TotalAmount =
        (
            SELECT COALESCE(SUM(pi.Quantity * pi.UnitCost), 0)
            FROM PurchaseItems pi
            WHERE pi.PurchaseID = p.PurchaseID
        )
    FROM Purchases p
    WHERE EXISTS
        (
            SELECT 1
            FROM PurchaseItems pi
            WHERE pi.PurchaseID = p.PurchaseID
        );

    UPDATE s
    SET TotalAmount =
        (
            SELECT COALESCE(SUM(si.Quantity * si.UnitPrice), 0)
            FROM SaleItems si
            WHERE si.SaleID = s.SaleID
        )
    FROM Sales s
    WHERE EXISTS
        (
            SELECT 1
            FROM SaleItems si
            WHERE si.SaleID = s.SaleID
        );
    """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}