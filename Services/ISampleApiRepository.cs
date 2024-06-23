using sampleapi.Entities;

namespace sampleapi.Services
{
    public interface ISampleApiRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();

        Task<(IEnumerable<User>, PaginationMetadata)> GetUsersAsync(string? username, string? searchQuery, 
            int pageNumber, int pageSize);

        Task<User?> GetUserAsync(int userId);

        Task<bool> UserExistAsync(int userId);

        Task AddUserAsync(User user);

        Task<bool> SaveChangesAsync();

        void DeleteUser(User user);
    }
}
