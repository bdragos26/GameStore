using GameStore.Data;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<User>> RegisterUserAsync(User user, string password);
        Task<ServiceResponse<User?>> AuthenticateUserAsync(string username, string password);
        Task<ServiceResponse<User?>> GetUserByEmailAsync(string email);
        Task<ServiceResponse<List<User>>> GetAllUsersAsync();
        Task<bool> UserExistsAsync(string username);
        Task<ServiceResponse<User>> UpdateUserAsync(int id, User updatedUser);
        Task<ServiceResponse<User>> ResetUserPasswordAsync(string email, string currentPassword, string newPassword);
        Task<ServiceResponse<bool>> DeleteUserAsync(int UserId);
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

        public async Task<ServiceResponse<User>> RegisterUserAsync(User user, string password)
        {
            var response = new ServiceResponse<User>();

            try
            {
                if (await UserExistsAsync(user.Username))
                {
                    response.Success = false;
                    response.Message = "Username already exists";
                    return response;
                }

                user.PasswordHash = _passwordHasher.HashPassword(user, password);
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                response.Data = user;
                response.Success = true;
                response.Message = "Registration successful";
                return response;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return response;
            }
        }

        public async Task<ServiceResponse<User?>> AuthenticateUserAsync(string username, string password)
        {
            var response = new ServiceResponse<User?>();

            try
            {

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = $"User with username {username} not found!";
                    return response;
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                if (result == PasswordVerificationResult.Success)
                {
                    response.Data = user;
                    response.Success = true;
                    response.Message = "Authentication successful";
                    return response;
                }

                response.Success = false;
                response.Message = "Invalid password";
                return response;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return response;
            }
        }
        public async Task<ServiceResponse<User?>> GetUserByEmailAsync(string email)
        {
            var response = new ServiceResponse<User?>()
            {
                Data = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email)
            };

            return response;

        }
        public async Task<ServiceResponse<List<User>>> GetAllUsersAsync()
        {
            var response = new ServiceResponse<List<User>>
            {
                Data = await _dbContext.Users.AsNoTracking().ToListAsync()
            };

            return response;

        }
        public async Task<bool> UserExistsAsync(string username)
        {
            return await _dbContext.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<ServiceResponse<User>> UpdateUserAsync(int id, User updatedUser)
        {
            var response = new ServiceResponse<User>
            {

                Data = await _dbContext.Users.FindAsync(id)
            };

            _dbContext.Entry(response.Data)
                .CurrentValues
                .SetValues(updatedUser);

            await _dbContext.SaveChangesAsync();
            return response;
        }
        public async Task<ServiceResponse<User>> ResetUserPasswordAsync(string email, string currentPassword, string newPassword)
        {
            var response = new ServiceResponse<User>();

            try
            {
                var userResponse = await GetUserByEmailAsync(email);
                if (userResponse.Data == null)
                {
                    response.Success = false;
                    response.Message = $"User with email {email} not found!";
                    return response;
                }

                var result = _passwordHasher.VerifyHashedPassword(userResponse.Data, userResponse.Data.PasswordHash, currentPassword);
                if (result != PasswordVerificationResult.Success)
                {
                    response.Success = false;
                    response.Message = "Password is incorrect";
                    return response;
                }

                userResponse.Data.PasswordHash = _passwordHasher.HashPassword(userResponse.Data, newPassword);

                userResponse.Success = true;
                return userResponse;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return response;
            }
        }

        public async Task<ServiceResponse<bool>> DeleteUserAsync(int userId)
        {
            var response = new ServiceResponse<bool>();
            var user = await _dbContext.Users.FindAsync(userId);

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found!";
                return response;
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            response.Success = true;
            response.Data = true;
            return response;
        }
    }
}
