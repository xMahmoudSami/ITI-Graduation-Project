using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory_Management_System.Services
{
    public interface ISupplierService
    {
        Task<PagedResult<SupplierListItemViewModel>> GetSuppliersAsync(SupplierFilterViewModel filter);
        Task<SupplierFormViewModel?> GetSupplierForEditAsync(int id);
        Task<SupplierDetailsViewModel?> GetSupplierDetailsAsync(int id);
        Task<SupplierDeleteViewModel?> GetSupplierForDeleteAsync(int id);
        Task<OperationResult> CreateSupplierAsync(SupplierFormViewModel model);
        Task<OperationResult> UpdateSupplierAsync(SupplierFormViewModel model);
        Task<OperationResult> DeleteSupplierAsync(int id);
        Task<List<SelectListItem>> GetSupplierSelectListAsync();
    }
}
