namespace Inventory_Management_System.Services
{
    public interface IAIService
    {
        Task<string> AskAsync(string userQuery);
    }
}
