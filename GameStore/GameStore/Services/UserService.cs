using GameStore.Data;
using GameStore.Shared.DTOs;
using GameStore.Shared.Mappers;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<User>> RegisterUserAsync(UserRegisterDto user, string password);
        Task<ServiceResponse<UserProfileDto?>> AuthenticateUserAsync(string username, string password);
        Task<ServiceResponse<User?>> GetUserByEmailAsync(string email);
        Task<ServiceResponse<List<User>>> GetAllUsersAsync();
        Task<bool> UserExistsAsync(string username);
        Task<ServiceResponse<User>> UpdateUserAsync(int id, User updatedUser);
        Task<ServiceResponse<User?>> GetUserByIdAsync(int userId);
        Task<ServiceResponse<User>> ResetUserPasswordAsync(ResetPasswordDTO resetPasswordDto);
        Task<ServiceResponse<bool>> DeleteUserAsync(int UserId);
        Task InitiatePasswordReset(string email);
        Task<bool> ResetPasswordWithToken(ResetPasswordWithTokenDto dto);
    }
    public class UserService : IUserService
    {
        private readonly GameStoreContext _dbContext;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;

        public UserService(GameStoreContext dbContext, PasswordHasher<User> passwordHasher, IEmailService emailService)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        public async Task<ServiceResponse<User>> RegisterUserAsync(UserRegisterDto registerDto, string password)
        {
            var response = new ServiceResponse<User>();

            try
            {
                if (await UserExistsAsync(registerDto.Username))
                {
                    response.Success = false;
                    response.Message = "Username already exists";
                    return response;
                }

                var user = UserMapper.ToEntity(registerDto);
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

        public async Task<ServiceResponse<UserProfileDto?>> AuthenticateUserAsync(string username, string password)
        {
            var response = new ServiceResponse<UserProfileDto?>();

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
                    var userDto = UserMapper.ToProfileDto(user);
                    response.Data = userDto;
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
            var response = new ServiceResponse<User>();

            try
            {
                var existingUser = await _dbContext.Users.FindAsync(id);
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }

                existingUser.Username = updatedUser.Username;
                existingUser.FirstName = updatedUser.FirstName;
                existingUser.LastName = updatedUser.LastName;
                existingUser.DateOfBirth = updatedUser.DateOfBirth;
                existingUser.Role = updatedUser.Role;

                if (existingUser.Email != updatedUser.Email)
                {
                    if (await _dbContext.Users.AnyAsync(u => u.Email == updatedUser.Email && u.UserId != id))
                    {
                        response.Success = false;
                        response.Message = "Email is already in use";
                        return response;
                    }
                    existingUser.Email = updatedUser.Email;
                }

                await _dbContext.SaveChangesAsync();
                response.Data = existingUser;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<User?>> GetUserByIdAsync(int userId)
        {
            return new ServiceResponse<User?>
            {
                Data = await _dbContext.Users.FindAsync(userId)
            };
        }

        public async Task<ServiceResponse<User>> ResetUserPasswordAsync(ResetPasswordDTO resetPasswordDto)
        {
            var response = new ServiceResponse<User>();

            try
            {
                var userResponse = await GetUserByEmailAsync(resetPasswordDto.Email);
                if (userResponse.Data == null)
                {
                    response.Success = false;
                    response.Message = $"User with email {resetPasswordDto.Email} not found!";
                    return response;
                }

                var result = _passwordHasher.VerifyHashedPassword(userResponse.Data, userResponse.Data.PasswordHash, resetPasswordDto.CurrentPassword);
                if (result != PasswordVerificationResult.Success)
                {
                    response.Success = false;
                    response.Message = "Password is incorrect";
                    return response;
                }

                userResponse.Data.PasswordHash = _passwordHasher.HashPassword(userResponse.Data, resetPasswordDto.NewPassword);

                userResponse.Success = true;
                return userResponse!;
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

        public async Task InitiatePasswordReset(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return;

            var token = new PasswordResetToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = user.UserId,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IsUsed = false
            };

            _dbContext.PasswordResetTokens.Add(token);
            await _dbContext.SaveChangesAsync();

            await _emailService.SendPasswordResetEmailAsync(user.Email, token.Token);
        }

        public async Task<bool> ResetPasswordWithToken(ResetPasswordWithTokenDto dto)
        {
            var tokenRecord = await _dbContext.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.Token == dto.Token &&
                    !t.IsUsed &&
                    t.ExpiresAt > DateTime.UtcNow);

            if (tokenRecord == null)
                return false;

            tokenRecord.User.PasswordHash = _passwordHasher.HashPassword(tokenRecord.User, dto.NewPassword);
            tokenRecord.IsUsed = true;

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
