using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory_Management_System.Services
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryListItemViewModel>> GetCategoriesAsync(CategoryFilterViewModel filter);
        Task<CategoryFormViewModel?> GetCategoryForEditAsync(int id);
        Task<CategoryDetailsViewModel?> GetCategoryDetailsAsync(int id);
        Task<CategoryDeleteViewModel?> GetCategoryForDeleteAsync(int id);
        Task<OperationResult> CreateCategoryAsync(CategoryFormViewModel model);
        Task<OperationResult> UpdateCategoryAsync(CategoryFormViewModel model);
        Task<OperationResult> DeleteCategoryAsync(int id);
        Task<bool> IsCategoryNameUniqueAsync(string name, int? excludeId = null);
        Task<List<SelectListItem>> GetCategorySelectListAsync();
    }
}
