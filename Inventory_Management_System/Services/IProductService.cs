using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory_Management_System.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductListItemViewModel>> GetProductsAsync(ProductFilterViewModel filter);
        Task<ProductFormViewModel?> GetProductForEditAsync(int id);
        Task<ProductDetailsViewModel?> GetProductDetailsAsync(int id);
        Task<ProductDeleteViewModel?> GetProductForDeleteAsync(int id);
        Task<OperationResult> CreateProductAsync(ProductFormViewModel model);
        Task<OperationResult> UpdateProductAsync(ProductFormViewModel model);
        Task<OperationResult> DeleteProductAsync(int id);
        Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null);
        Task<List<SelectListItem>> GetProductSelectListAsync();
    }
}
