namespace Divar.Models
{
    public interface IAdminRepository
    {
        Task<List<User>> GetUsersAsync();
        Task<List<Advertisement>> GetAdvertisementsAsync();
        Task<List<Comment>> GetCommentsAsync();
        Task<User> GetUserByIdAsync(int id);
        Task DeleteUserAsync(int id, HttpContext httpContext);
    }
}
