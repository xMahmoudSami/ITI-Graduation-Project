using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .ToListAsync();

            return View(purchases);
        }
        public async Task<IActionResult> Details(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(p => p.PurchaseID == id);

            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.PurchaseID == id);

            if (purchase == null)
            {
                return NotFound();
            }

            foreach (var item in purchase.PurchaseItems)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductID == item.ProductID);

                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                }
            }

            _context.Purchases.Remove(purchase);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.PurchaseID == id);

            if (purchase == null)
            {
                return NotFound();
            }

            var model = new PurchaseViewModel
            {
                SupplierID = purchase.SupplierID,
                PurchaseDate = purchase.PurchaseDate,

                Items = purchase.PurchaseItems.Select(item => new PurchaseItemViewModel
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost
                }).ToList()
            };

            ViewBag.Suppliers = _context.Suppliers.ToList();
            ViewBag.Products = _context.Products.ToList();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers = _context.Suppliers.ToList();
                ViewBag.Products = _context.Products.ToList();

                return View(model);
            }
            var duplicateProducts = model.Items
    .GroupBy(x => x.ProductID)
    .Where(x => x.Count() > 1)
    .ToList();

            if (duplicateProducts.Any())
            {
                ModelState.AddModelError("", "You cannot add the same product more than once.");

                ViewBag.Suppliers = _context.Suppliers.ToList();
                ViewBag.Products = _context.Products.ToList();

                return View(model);
            }
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.PurchaseID == id);

            if (purchase == null)
            {
                return NotFound();
            }

            // Return old quantities to stock
            foreach (var oldItem in purchase.PurchaseItems)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductID == oldItem.ProductID);

                if (product != null)
                {
                    product.StockQuantity -= oldItem.Quantity;
                }
            }

            // Remove old purchase items
            _context.PurchaseItems.RemoveRange(purchase.PurchaseItems);

            // Update purchase information
            purchase.SupplierID = model.SupplierID;
            purchase.PurchaseDate = model.PurchaseDate;

            decimal totalAmount = 0;

            foreach (var item in model.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductID == item.ProductID);

                if (product == null)
                {
                    ModelState.AddModelError("", "Product not found.");

                    ViewBag.Suppliers = _context.Suppliers.ToList();
                    ViewBag.Products = _context.Products.ToList();

                    return View(model);
                }

                product.StockQuantity += item.Quantity;

                totalAmount += item.Quantity * item.UnitCost;

                var purchaseItem = new PurchaseItem
                {
                    PurchaseID = purchase.PurchaseID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost
                };

                _context.PurchaseItems.Add(purchaseItem);
            }

            purchase.TotalAmount = totalAmount;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            ViewBag.Suppliers = _context.Suppliers.ToList();
            ViewBag.Products = _context.Products.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers = _context.Suppliers.ToList();
                ViewBag.Products = _context.Products.ToList();

                return View(model);
            }
            var duplicateProducts = model.Items
                .GroupBy(x => x.ProductID)
                .Where(x => x.Count() > 1)
                .ToList();

            if (duplicateProducts.Any())
            {
                ModelState.AddModelError("", "You cannot add the same product more than once.");

                ViewBag.Suppliers = _context.Suppliers.ToList();
                ViewBag.Products = _context.Products.ToList();

                return View(model);
            }
            if (model.Items == null || model.Items.Count == 0)
            {
                ModelState.AddModelError("", "At least one product is required.");

                ViewBag.Suppliers = _context.Suppliers.ToList();
                ViewBag.Products = _context.Products.ToList();

                return View(model);
            }
            decimal totalAmount = 0;

            foreach (var item in model.Items)
            {
                totalAmount += item.Quantity * item.UnitCost;
            }

            var purchase = new Purchase
            {
                SupplierID = model.SupplierID,
                PurchaseDate = model.PurchaseDate,
                TotalAmount = totalAmount
            };

            _context.Purchases.Add(purchase);

            await _context.SaveChangesAsync();

            foreach (var item in model.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductID == item.ProductID);

                if (product == null)
                {
                    ModelState.AddModelError("", "Product not found.");

                    ViewBag.Suppliers = _context.Suppliers.ToList();
                    ViewBag.Products = _context.Products.ToList();

                    return View(model);
                }

                product.StockQuantity += item.Quantity;

                var purchaseItem = new PurchaseItem
                {
                    PurchaseID = purchase.PurchaseID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost
                };

                _context.PurchaseItems.Add(purchaseItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}