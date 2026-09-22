using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory_Management_System.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext _context;

        public SupplierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<SupplierListItemViewModel>> GetSuppliersAsync(SupplierFilterViewModel filter)
        {
            var query = _context.Suppliers
                .AsNoTracking()
                .Include(s => s.SupplierProducts)
                .Include(s => s.Purchases)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var term = filter.Search.Trim().ToLower();
                query = query.Where(s => s.SupplierName.ToLower().Contains(term)
                                      || s.ContactName.ToLower().Contains(term)
                                      || s.Email.ToLower().Contains(term)
                                      || s.Phone.ToLower().Contains(term));
            }

            var totalItems = await query.CountAsync();
            var page = Math.Max(1, filter.Page);
            var pageSize = filter.PageSize is 10 or 25 or 50 or 100 ? filter.PageSize : 10;

            var items = await query
                .OrderBy(s => s.SupplierName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SupplierListItemViewModel
                {
                    SupplierID = s.SupplierID,
                    SupplierName = s.SupplierName,
                    ContactName = s.ContactName,
                    Phone = s.Phone,
                    Email = s.Email,
                    Address = s.Address,
                    MappedProductsCount = s.SupplierProducts.Count(),
                    PurchasesCount = s.Purchases.Count()
                })
                .ToListAsync();

            return new PagedResult<SupplierListItemViewModel>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }

        public async Task<SupplierFormViewModel?> GetSupplierForEditAsync(int id)
        {
            var supplier = await _context.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.SupplierID == id);
            if (supplier == null) return null;

            return new SupplierFormViewModel
            {
                SupplierID = supplier.SupplierID,
                SupplierName = supplier.SupplierName,
                ContactName = supplier.ContactName,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address
            };
        }

        public async Task<SupplierDetailsViewModel?> GetSupplierDetailsAsync(int id)
        {
            var supplier = await _context.Suppliers
                .AsNoTracking()
                .Include(s => s.Purchases)
                .Include(s => s.SupplierProducts)
                    .ThenInclude(sp => sp.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(s => s.SupplierID == id);

            if (supplier == null) return null;

            return new SupplierDetailsViewModel
            {
                SupplierID = supplier.SupplierID,
                SupplierName = supplier.SupplierName,
                ContactName = supplier.ContactName,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address,
                PurchasesCount = supplier.Purchases.Count,
                MappedProducts = supplier.SupplierProducts.Select(sp => new SupplierProductItemViewModel
                {
                    SupplierProductID = sp.SupplierProductID,
                    ProductID = sp.ProductID,
                    ProductName = sp.Product?.ProductName ?? "Unknown Product",
                    SKU = sp.Product?.SKU ?? "",
                    CategoryName = sp.Product?.Category?.CategoryName ?? "Uncategorized",
                    SupplierSKU = sp.SupplierSKU,
                    ContractPrice = sp.ContractPrice,
                    LeadTimeDays = sp.LeadTimeDays,
                    UnitPrice = sp.Product?.UnitPrice ?? 0m,
                    StockQuantity = sp.Product?.StockQuantity ?? 0
                }).OrderBy(p => p.ProductName).ToList()
            };
        }

        public async Task<SupplierDeleteViewModel?> GetSupplierForDeleteAsync(int id)
        {
            var supplier = await _context.Suppliers
                .AsNoTracking()
                .Include(s => s.Purchases)
                .Include(s => s.SupplierProducts)
                .FirstOrDefaultAsync(s => s.SupplierID == id);

            if (supplier == null) return null;

            return new SupplierDeleteViewModel
            {
                SupplierID = supplier.SupplierID,
                SupplierName = supplier.SupplierName,
                ContactName = supplier.ContactName,
                Phone = supplier.Phone,
                Email = supplier.Email,
                PurchasesCount = supplier.Purchases.Count,
                MappedProductsCount = supplier.SupplierProducts.Count
            };
        }

        public async Task<OperationResult> CreateSupplierAsync(SupplierFormViewModel model)
        {
            var entity = new Supplier
            {
                SupplierName = model.SupplierName.Trim(),
                ContactName = model.ContactName.Trim(),
                Phone = model.Phone.Trim(),
                Email = model.Email.Trim(),
                Address = model.Address.Trim()
            };

            try
            {
                _context.Suppliers.Add(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Ok("Supplier created successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("An error occurred while saving the supplier: " + ex.Message);
            }
        }

        public async Task<OperationResult> UpdateSupplierAsync(SupplierFormViewModel model)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierID == model.SupplierID);
            if (supplier == null)
            {
                return OperationResult.Fail("Supplier not found.");
            }

            supplier.SupplierName = model.SupplierName.Trim();
            supplier.ContactName = model.ContactName.Trim();
            supplier.Phone = model.Phone.Trim();
            supplier.Email = model.Email.Trim();
            supplier.Address = model.Address.Trim();

            try
            {
                await _context.SaveChangesAsync();
                return OperationResult.Ok("Supplier updated successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("An error occurred while updating the supplier: " + ex.Message);
            }
        }

        public async Task<OperationResult> DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.Purchases)
                .FirstOrDefaultAsync(s => s.SupplierID == id);

            if (supplier == null)
            {
                return OperationResult.Fail("Supplier not found.");
            }

            // DELETE SAFETY: Check purchase history
            if (supplier.Purchases.Any())
            {
                return OperationResult.Fail($"This supplier cannot be deleted because {supplier.Purchases.Count} purchase history record(s) exist for this supplier.");
            }

            try
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
                return OperationResult.Ok("Supplier deleted successfully.");
            }
            catch (DbUpdateException)
            {
                return OperationResult.Fail("Cannot delete this supplier because related records depend on it.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("An error occurred while deleting the supplier: " + ex.Message);
            }
        }

        public async Task<List<SelectListItem>> GetSupplierSelectListAsync()
        {
            return await _context.Suppliers
                .AsNoTracking()
                .OrderBy(s => s.SupplierName)
                .Select(s => new SelectListItem
                {
                    Value = s.SupplierID.ToString(),
                    Text = s.SupplierName
                })
                .ToListAsync();
        }
    }
}
