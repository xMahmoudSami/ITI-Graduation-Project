namespace Inventory_Management_System.Services
{
    public interface ISupplierProductService
    {
        Task<PagedResult<SupplierProductListItemViewModel>> GetSupplierProductsAsync(SupplierProductFilterViewModel filter);
        Task<SupplierProductFormViewModel?> GetSupplierProductForEditAsync(int id);
        Task<SupplierProductDeleteViewModel?> GetSupplierProductForDeleteAsync(int id);
        Task<OperationResult> CreateSupplierProductAsync(SupplierProductFormViewModel model);
        Task<OperationResult> UpdateSupplierProductAsync(SupplierProductFormViewModel model);
        Task<OperationResult> DeleteSupplierProductAsync(int id);
        Task<bool> IsMappingDuplicateAsync(int supplierId, int productId, int? excludeMappingId = null);
    }
}
