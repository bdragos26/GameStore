using System.Reflection.Metadata.Ecma335;
using GameStore.Data;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(User user, string password);
        Task<User?> AuthenticateUserAsync(string username, string password);
        Task<User?> GetUserById(int id);
        Task<List<User>> GetAllUsers();
        Task<bool> UserExistsAsync(string username);
        Task<User> UpdateUserAsync(int id, User updatedUser);
    }
    public class UserService : IUserService
    {
        public readonly GameStoreContext _dbContext;
        public readonly PasswordHasher<User> _passwordHasher;

        public UserService(GameStoreContext dbContext, PasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            if (await UserExistsAsync(user.Username))
            {
                throw new Exception("Username already exists");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User?> AuthenticateUserAsync(string username, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }
        public async Task<User?> GetUserById(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }
        public async Task<List<User>> GetAllUsers()
        {
            return await _dbContext.Users.AsNoTracking().ToListAsync();
        }
        public async Task<bool> UserExistsAsync(string username)
        {
            return await _dbContext.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<User> UpdateUserAsync(int id, User updatedUser)
        {
            var existingUser = await _dbContext.Users.FindAsync(id);
            _dbContext.Entry(existingUser)
                .CurrentValues
                .SetValues(updatedUser);
            
            await _dbContext.SaveChangesAsync();
            return existingUser;
        }
    }
}
