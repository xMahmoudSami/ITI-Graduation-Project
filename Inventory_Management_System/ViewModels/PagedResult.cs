namespace Inventory_Management_System.ViewModels
{
    public interface IPaginationInfo
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }
        int TotalItems { get; set; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        int StartItemIndex { get; }
        int EndItemIndex { get; }
    }

    public class PagedResult<T> : IPaginationInfo
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; } = 0;
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / Math.Max(1, PageSize));
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public int StartItemIndex => TotalItems == 0 ? 0 : (PageNumber - 1) * PageSize + 1;
        public int EndItemIndex => Math.Min(PageNumber * PageSize, TotalItems);
    }
}
