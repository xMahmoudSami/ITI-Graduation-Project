namespace Inventory_Management_System.Services
{
    public class SupplierProductService : ISupplierProductService
    {
        private readonly ApplicationDbContext _context;

        public SupplierProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<SupplierProductListItemViewModel>> GetSupplierProductsAsync(SupplierProductFilterViewModel filter)
        {
            var query = _context.SupplierProducts
                .AsNoTracking()
                .Include(sp => sp.Supplier)
                .Include(sp => sp.Product)
                    .ThenInclude(p => p.Category)
                .AsQueryable();

            if (filter.SupplierID.HasValue && filter.SupplierID.Value > 0)
            {
                query = query.Where(sp => sp.SupplierID == filter.SupplierID.Value);
            }

            if (filter.ProductID.HasValue && filter.ProductID.Value > 0)
            {
                query = query.Where(sp => sp.ProductID == filter.ProductID.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var term = filter.Search.Trim().ToLower();
                query = query.Where(sp => sp.SupplierSKU.ToLower().Contains(term)
                                       || (sp.Supplier != null && sp.Supplier.SupplierName.ToLower().Contains(term))
                                       || (sp.Product != null && sp.Product.ProductName.ToLower().Contains(term))
                                       || (sp.Product != null && sp.Product.SKU.ToLower().Contains(term)));
            }

            var totalItems = await query.CountAsync();
            var page = Math.Max(1, filter.Page);
            var pageSize = filter.PageSize is 10 or 25 or 50 or 100 ? filter.PageSize : 10;

            var items = await query
                .OrderBy(sp => sp.Supplier != null ? sp.Supplier.SupplierName : "")
                .ThenBy(sp => sp.Product != null ? sp.Product.ProductName : "")
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(sp => new SupplierProductListItemViewModel
                {
                    SupplierProductID = sp.SupplierProductID,
                    SupplierID = sp.SupplierID,
                    SupplierName = sp.Supplier != null ? sp.Supplier.SupplierName : "Unknown Supplier",
                    ProductID = sp.ProductID,
                    ProductName = sp.Product != null ? sp.Product.ProductName : "Unknown Product",
                    ProductSKU = sp.Product != null ? sp.Product.SKU : "",
                    CategoryName = sp.Product != null && sp.Product.Category != null ? sp.Product.Category.CategoryName : "Uncategorized",
                    SupplierSKU = sp.SupplierSKU,
                    ContractPrice = sp.ContractPrice,
                    LeadTimeDays = sp.LeadTimeDays
                })
                .ToListAsync();

            return new PagedResult<SupplierProductListItemViewModel>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }

        public async Task<SupplierProductFormViewModel?> GetSupplierProductForEditAsync(int id)
        {
            var mapping = await _context.SupplierProducts
                .AsNoTracking()
                .Include(sp => sp.Supplier)
                .Include(sp => sp.Product)
                .FirstOrDefaultAsync(sp => sp.SupplierProductID == id);

            if (mapping == null) return null;

            return new SupplierProductFormViewModel
            {
                SupplierProductID = mapping.SupplierProductID,
                SupplierID = mapping.SupplierID,
                ProductID = mapping.ProductID,
                SupplierName = mapping.Supplier?.SupplierName,
                ProductName = mapping.Product?.ProductName,
                SupplierSKU = mapping.SupplierSKU,
                ContractPrice = mapping.ContractPrice,
                LeadTimeDays = mapping.LeadTimeDays
            };
        }

        public async Task<SupplierProductDeleteViewModel?> GetSupplierProductForDeleteAsync(int id)
        {
            var mapping = await _context.SupplierProducts
                .AsNoTracking()
                .Include(sp => sp.Supplier)
                .Include(sp => sp.Product)
                .FirstOrDefaultAsync(sp => sp.SupplierProductID == id);

            if (mapping == null) return null;

            return new SupplierProductDeleteViewModel
            {
                SupplierProductID = mapping.SupplierProductID,
                SupplierID = mapping.SupplierID,
                SupplierName = mapping.Supplier?.SupplierName ?? "Unknown Supplier",
                ProductID = mapping.ProductID,
                ProductName = mapping.Product?.ProductName ?? "Unknown Product",
                SupplierSKU = mapping.SupplierSKU,
                ContractPrice = mapping.ContractPrice,
                LeadTimeDays = mapping.LeadTimeDays
            };
        }

        public async Task<OperationResult> CreateSupplierProductAsync(SupplierProductFormViewModel model)
        {
            // 1. Verify Supplier exists
            var supplier = await _context.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.SupplierID == model.SupplierID);
            if (supplier == null)
            {
                return OperationResult.Fail("The selected supplier does not exist.");
            }

            // 2. Verify Product exists
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductID == model.ProductID);
            if (product == null)
            {
                return OperationResult.Fail("The selected product does not exist.");
            }

            // 3. Verify duplicate mapping before SaveChanges
            var isDuplicate = await IsMappingDuplicateAsync(model.SupplierID, model.ProductID);
            if (isDuplicate)
            {
                return OperationResult.Fail($"A mapping between Supplier '{supplier.SupplierName}' and Product '{product.ProductName}' already exists.");
            }

            var entity = new SupplierProduct
            {
                SupplierID = model.SupplierID,
                ProductID = model.ProductID,
                SupplierSKU = model.SupplierSKU.Trim(),
                ContractPrice = model.ContractPrice,
                LeadTimeDays = model.LeadTimeDays
            };

            try
            {
                _context.SupplierProducts.Add(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Ok("Supplier-Product mapping created successfully.");
            }
            catch (DbUpdateException)
            {
                return OperationResult.Fail("A database conflict occurred. This supplier-product combination already exists.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("An error occurred while creating the mapping: " + ex.Message);
            }
        }

        public async Task<OperationResult> UpdateSupplierProductAsync(SupplierProductFormViewModel model)
        {
            var mapping = await _context.SupplierProducts.FirstOrDefaultAsync(sp => sp.SupplierProductID == model.SupplierProductID);
            if (mapping == null)
            {
                return OperationResult.Fail("Supplier-Product mapping not found.");
            }

            // Verify duplicate mapping excluding current record
            var isDuplicate = await IsMappingDuplicateAsync(model.SupplierID, model.ProductID, model.SupplierProductID);
            if (isDuplicate)
            {
                return OperationResult.Fail("Another mapping between this supplier and product already exists.");
            }

            mapping.SupplierSKU = model.SupplierSKU.Trim();
            mapping.ContractPrice = model.ContractPrice;
            mapping.LeadTimeDays = model.LeadTimeDays;

            try
            {
                await _context.SaveChangesAsync();
                return OperationResult.Ok("Supplier-Product mapping updated successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("An error occurred while updating the mapping: " + ex.Message);
            }
        }

        public async Task<OperationResult> DeleteSupplierProductAsync(int id)
        {
            var mapping = await _context.SupplierProducts.FirstOrDefaultAsync(sp => sp.SupplierProductID == id);
            if (mapping == null)
            {
                return OperationResult.Fail("Supplier-Product mapping not found.");
            }

            try
            {
                _context.SupplierProducts.Remove(mapping);
                await _context.SaveChangesAsync();
                return OperationResult.Ok("Supplier-Product mapping removed successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("An error occurred while deleting the mapping: " + ex.Message);
            }
        }

        public async Task<bool> IsMappingDuplicateAsync(int supplierId, int productId, int? excludeMappingId = null)
        {
            var query = _context.SupplierProducts.AsNoTracking().Where(sp => sp.SupplierID == supplierId && sp.ProductID == productId);
            if (excludeMappingId.HasValue && excludeMappingId.Value > 0)
            {
                query = query.Where(sp => sp.SupplierProductID != excludeMappingId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
