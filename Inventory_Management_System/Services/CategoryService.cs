using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory_Management_System.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<CategoryListItemViewModel>> GetCategoriesAsync(CategoryFilterViewModel filter)
        {
            var query = _context.Categories.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var term = filter.Search.Trim().ToLower();
                query = query.Where(c => c.CategoryName.ToLower().Contains(term) || (c.Description != null && c.Description.ToLower().Contains(term)));
            }

            var totalItems = await query.CountAsync();
            var page = Math.Max(1, filter.Page);
            var pageSize = filter.PageSize is 10 or 25 or 50 or 100 ? filter.PageSize : 10;

            var items = await query
                .OrderBy(c => c.CategoryName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CategoryListItemViewModel
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    ProductsCount = c.Products.Count()
                })
                .ToListAsync();

            return new PagedResult<CategoryListItemViewModel>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }

        public async Task<CategoryFormViewModel?> GetCategoryForEditAsync(int id)
        {
            var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryID == id);
            if (category == null) return null;

            return new CategoryFormViewModel
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }

        public async Task<CategoryDetailsViewModel?> GetCategoryDetailsAsync(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null) return null;

            return new CategoryDetailsViewModel
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                Description = category.Description,
                Products = category.Products.Select(p => new CategoryProductItemViewModel
                {
                    ProductID = p.ProductID,
                    SKU = p.SKU,
                    ProductName = p.ProductName,
                    UnitPrice = p.UnitPrice,
                    StockQuantity = p.StockQuantity,
                    LowStockThreshold = p.LowStockThreshold
                }).OrderBy(p => p.ProductName).ToList()
            };
        }

        public async Task<CategoryDeleteViewModel?> GetCategoryForDeleteAsync(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Select(c => new CategoryDeleteViewModel
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    ProductsCount = c.Products.Count()
                })
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            return category;
        }

        public async Task<OperationResult> CreateCategoryAsync(CategoryFormViewModel model)
        {
            var nameUnique = await IsCategoryNameUniqueAsync(model.CategoryName);
            if (!nameUnique)
            {
                return OperationResult.Fail($"A category named '{model.CategoryName}' already exists.");
            }

            var entity = new Category
            {
                CategoryName = model.CategoryName.Trim(),
                Description = model.Description?.Trim()
            };

            try
            {
                _context.Categories.Add(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Ok("Category created successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("An error occurred while saving the category. " + ex.Message);
            }
        }

        public async Task<OperationResult> UpdateCategoryAsync(CategoryFormViewModel model)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == model.CategoryID);
            if (category == null)
            {
                return OperationResult.Fail("Category not found.");
            }

            var nameUnique = await IsCategoryNameUniqueAsync(model.CategoryName, model.CategoryID);
            if (!nameUnique)
            {
                return OperationResult.Fail($"A category named '{model.CategoryName}' already exists.");
            }

            category.CategoryName = model.CategoryName.Trim();
            category.Description = model.Description?.Trim();

            try
            {
                await _context.SaveChangesAsync();
                return OperationResult.Ok("Category updated successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("An error occurred while updating the category. " + ex.Message);
            }
        }

        public async Task<OperationResult> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return OperationResult.Fail("Category not found.");
            }

            // DELETE SAFETY: Prevent deletion if products exist
            var productCount = category.Products.Count;
            if (productCount > 0)
            {
                return OperationResult.Fail($"Cannot delete this category because it contains {productCount} product(s). Move or delete those products before deleting the category.");
            }

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return OperationResult.Ok("Category deleted successfully.");
            }
            catch (DbUpdateException)
            {
                return OperationResult.Fail("Cannot delete this category because related database records are linked to it.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("An error occurred while deleting the category: " + ex.Message);
            }
        }

        public async Task<bool> IsCategoryNameUniqueAsync(string name, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return true;
            var trimmed = name.Trim().ToLower();

            var query = _context.Categories.AsNoTracking().Where(c => c.CategoryName.ToLower() == trimmed);
            if (excludeId.HasValue && excludeId.Value > 0)
            {
                query = query.Where(c => c.CategoryID != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<List<SelectListItem>> GetCategorySelectListAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.CategoryName)
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryID.ToString(),
                    Text = c.CategoryName
                })
                .ToListAsync();
        }
    }
}
