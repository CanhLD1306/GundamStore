using GundamStore.Data;

namespace GundamStore.Services
{
    public class UserService{
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}