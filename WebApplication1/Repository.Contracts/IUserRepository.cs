using CcStore.Models;
using System.Linq.Expressions;

namespace CcStore.Repository.Contracts
{
    public interface IUserRepository
    {
        Task<string> CreateUserAsync(User user);
        Task<IEnumerable<User>> FindAllAsync();
        Task<IEnumerable<User>> FindByConditionAsync(Expression<Func<User, bool>> expression);
        Task<User> FindByIdAsync(object id);
        Task<int> GetNextIdAsync();
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByRefreshTokenAsync(string refreshToken);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
        Task<string> Login(string username, string password);
        Task<string> Register(User user);
        Task UpdateUserAsync(User user);
    }
}