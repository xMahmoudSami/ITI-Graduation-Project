namespace Inventory_Management_System.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Purchases
        public async Task<IActionResult> Index()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                .Select(p => new PurchaseListItemViewModel
                {
                    PurchaseID = p.PurchaseID,
                    PurchaseDate = p.PurchaseDate,
                    SupplierID = p.SupplierID,
                    SupplierName = p.Supplier != null ? p.Supplier.SupplierName : "N/A",
                    TotalAmount = p.TotalAmount,
                    ItemsCount = p.PurchaseItems.Count
                })
                .ToListAsync();

            return View(purchases);
        }

        // GET: /Purchases/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(p => p.PurchaseID == id);

            if (purchase == null) return NotFound();

            var viewModel = new PurchaseDetailsViewModel
            {
                PurchaseID = purchase.PurchaseID,
                PurchaseDate = purchase.PurchaseDate,
                SupplierID = purchase.SupplierID,
                SupplierName = purchase.Supplier?.SupplierName ?? "N/A",
                TotalAmount = purchase.TotalAmount,
                Items = purchase.PurchaseItems.Select(pi => new PurchaseItemDetailsViewModel
                {
                    PurchaseItemID = pi.PurchaseItemID,
                    ProductID = pi.ProductID,
                    ProductName = pi.Product?.ProductName ?? "N/A",
                    SKU = pi.Product?.SKU ?? "N/A",
                    Quantity = pi.Quantity,
                    UnitCost = pi.UnitCost
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: /Purchases/Create
        public async Task<IActionResult> Create()
        {
            var model = new PurchaseFormViewModel
            {
                PurchaseDate = DateTime.Now
            };

            await PopulateSelectListsAsync(model);

            return View(model);
        }

        // POST: /Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync(model);
                return View(model);
            }

            if (model.Items == null || !model.Items.Any())
            {
                ModelState.AddModelError("", "At least one product is required.");
                await PopulateSelectListsAsync(model);
                return View(model);
            }

            var duplicateProducts = model.Items
                .GroupBy(x => x.ProductID)
                .Where(x => x.Count() > 1)
                .ToList();

            if (duplicateProducts.Any())
            {
                ModelState.AddModelError("", "You cannot add the same product more than once.");
                await PopulateSelectListsAsync(model);
                return View(model);
            }

            decimal totalAmount = model.Items.Sum(item => item.Quantity * item.UnitCost);

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
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == item.ProductID);

                if (product == null)
                {
                    ModelState.AddModelError("", "Product not found.");
                    await PopulateSelectListsAsync(model);
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

        // GET: /Purchases/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.PurchaseID == id);

            if (purchase == null) return NotFound();

            var model = new PurchaseFormViewModel
            {
                PurchaseID = purchase.PurchaseID,
                SupplierID = purchase.SupplierID,
                PurchaseDate = purchase.PurchaseDate,
                TotalAmount = purchase.TotalAmount,
                Items = purchase.PurchaseItems.Select(item => new PurchaseItemFormViewModel
                {
                    PurchaseItemID = item.PurchaseItemID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost
                }).ToList()
            };

            await PopulateSelectListsAsync(model);

            return View(model);
        }

        // POST: /Purchases/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync(model);
                return View(model);
            }

            var duplicateProducts = model.Items
                .GroupBy(x => x.ProductID)
                .Where(x => x.Count() > 1)
                .ToList();

            if (duplicateProducts.Any())
            {
                ModelState.AddModelError("", "You cannot add the same product more than once.");
                await PopulateSelectListsAsync(model);
                return View(model);
            }

            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.PurchaseID == id);

            if (purchase == null) return NotFound();

            // Return old quantities to stock
            foreach (var oldItem in purchase.PurchaseItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == oldItem.ProductID);
                if (product != null)
                {
                    product.StockQuantity -= oldItem.Quantity;
                }
            }

            _context.PurchaseItems.RemoveRange(purchase.PurchaseItems);

            purchase.SupplierID = model.SupplierID;
            purchase.PurchaseDate = model.PurchaseDate;

            decimal totalAmount = 0;

            foreach (var item in model.Items)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == item.ProductID);

                if (product == null)
                {
                    ModelState.AddModelError("", "Product not found.");
                    await PopulateSelectListsAsync(model);
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

        // GET: /Purchases/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return NotFound();

            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(m => m.PurchaseID == id);

            if (purchase == null) return NotFound();

            var viewModel = new PurchaseDeleteViewModel
            {
                PurchaseID = purchase.PurchaseID,
                PurchaseDate = purchase.PurchaseDate,
                SupplierName = purchase.Supplier?.SupplierName ?? "N/A",
                TotalAmount = purchase.TotalAmount,
                ItemsCount = purchase.PurchaseItems.Count,
                Items = purchase.PurchaseItems.Select(pi => new PurchaseItemDetailsViewModel
                {
                    PurchaseItemID = pi.PurchaseItemID,
                    ProductID = pi.ProductID,
                    ProductName = pi.Product?.ProductName ?? "N/A",
                    SKU = pi.Product?.SKU ?? "N/A",
                    Quantity = pi.Quantity,
                    UnitCost = pi.UnitCost
                }).ToList()
            };

            foreach (var item in purchase.PurchaseItems)
            {
                if (item.Product != null && item.Product.StockQuantity < item.Quantity)
                {
                    viewModel.CanDelete = false;
                    viewModel.BlockingReason = $"Cannot delete purchase order. Product '{item.Product.ProductName}' current stock ({item.Product.StockQuantity}) is less than the quantity to revert ({item.Quantity}).";
                    break;
                }
            }

            return View(viewModel);
        }

        // POST: /Purchases/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0) return BadRequest();

            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(p => p.PurchaseID == id);

            if (purchase == null) return NotFound();

            foreach (var item in purchase.PurchaseItems)
            {
                var product = item.Product ?? await _context.Products.FindAsync(item.ProductID);
                if (product != null && product.StockQuantity < item.Quantity)
                {
                    TempData["ErrorMessage"] = $"Cannot delete purchase. Product '{product.ProductName}' stock ({product.StockQuantity}) is less than the quantity to revert ({item.Quantity}).";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
            }

            foreach (var item in purchase.PurchaseItems)
            {
                var product = item.Product ?? await _context.Products.FindAsync(item.ProductID);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                }
            }

            _context.PurchaseItems.RemoveRange(purchase.PurchaseItems);
            _context.Purchases.Remove(purchase);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Purchase deleted and inventory adjusted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // Helper Method To Populate Dropdown Lists inside ViewModel
        private async Task PopulateSelectListsAsync(PurchaseFormViewModel model)
        {
            var suppliers = await _context.Suppliers.ToListAsync();
            var products = await _context.Products.ToListAsync();

            model.Suppliers = suppliers.Select(s => new SelectListItem
            {
                Value = s.SupplierID.ToString(),
                Text = s.SupplierName
            }).ToList();

            model.Products = products.Select(p => new SelectListItem
            {
                Value = p.ProductID.ToString(),
                Text = $"{p.ProductName} (SKU: {p.SKU})"
            }).ToList();
        }
    }
}