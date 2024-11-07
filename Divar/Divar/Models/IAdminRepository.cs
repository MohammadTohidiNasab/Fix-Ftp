namespace Divar.Models
{
    public interface IAdminRepository
    {
        Task<List<CustomUser>> GetUsersAsync();
        Task<List<Advertisement>> GetAdvertisementsAsync();
        Task<List<Comment>> GetCommentsAsync();
        Task<CustomUser> GetUserByIdAsync(string id); // تغییر به string
        Task DeleteUserAsync(string id, HttpContext httpContext); // تغییر به string
    }
}