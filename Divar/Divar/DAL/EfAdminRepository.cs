
public class EfAdminRepository : IAdminRepository
{
    private readonly DivarDbContext _context;

    public EfAdminRepository(DivarDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomUser>> GetUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<List<Advertisement>> GetAdvertisementsAsync()
    {
        return await _context.Advertisements.ToListAsync();
    }

    public async Task<List<Comment>> GetCommentsAsync()
    {
        return await _context.Comments.ToListAsync();
    }

    public async Task<CustomUser> GetUserByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task DeleteUserAsync(string id, HttpContext httpContext)
    {
        var user = await GetUserByIdAsync(id);
        if (user != null)
        {
            var advertisements = _context.Advertisements.Where(a => a.CustomUserId == id);
            _context.Advertisements.RemoveRange(advertisements);
            _context.Users.Remove(user);

            // پاک کردن سشن فقط برای کاربر حذف شده
            if (httpContext.Session.GetString("UserId") == id)
            {
                httpContext.Session.Clear();
            }

            await _context.SaveChangesAsync();
        }
    }
}
