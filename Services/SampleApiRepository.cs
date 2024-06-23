using Microsoft.EntityFrameworkCore;
using sampleapi.DbContexts;
using sampleapi.Entities;

namespace sampleapi.Services
{
    public class SampleApiRepository : ISampleApiRepository
    {
        private readonly sampleapiContext _context;
        
        public SampleApiRepository(sampleapiContext context) 
        { 
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void DeleteUser(User user)
        {
            _context.Remove(user);
        }

        public async Task<User?> GetUserAsync(int userId)
        {
            return await _context.Users.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.OrderBy(u => u.UserName).ToListAsync();
        }


        public async Task<(IEnumerable<User>, PaginationMetadata)> GetUsersAsync(string? username, string? searchQuery,
            int pageNumber, int pageSize)
        {            
            var collection = _context.Users as IQueryable<User>;

            if (!string.IsNullOrWhiteSpace(username)) 
            {
                username = username.Trim();
                collection = collection.Where(u => u.UserName == username);
            }
            if (!string.IsNullOrWhiteSpace(searchQuery)) 
            { 
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.UserName.Contains(searchQuery)
                    || (a.FirstName.Contains(searchQuery))
                    || (a.LastName.Contains(searchQuery)));
            }

            var totalItemCount = await collection.CountAsync();

            var paginationMetadata = new PaginationMetadata(
                totalItemCount, pageSize, pageNumber);

            var collectioToReturn = await collection.OrderBy(u => u.UserName)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectioToReturn, paginationMetadata);
        }


        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public async Task<bool> UserExistAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }
    }
}

