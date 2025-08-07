using GameStore.Data;
using GameStore.Services;
using GameStore.Shared.DTOs;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;

namespace GameStore.Tests.ServiceTests
{
    public class UserServiceTests
    {
        private readonly GameStoreContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<GameStoreContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameStoreContext(options);
            _passwordHasher = new PasswordHasher<User>();
            _emailServiceMock = new Mock<IEmailService>();
            _userService = new UserService(_context, _passwordHasher, _emailServiceMock.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRegisterSuccessfully()
        {
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now)
            };

            var response = await _userService.RegisterUserAsync(registerDto, "Password123");

            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(registerDto.Username, response.Data.Username);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldFail_WhenUsernameExists()
        {
            await RegisterUserAsync();

            var duplicate = new UserRegisterDto
            {
                Username = "testuser",
                Email = "duplicate@example.com",
                FirstName = "Another",
                LastName = "User",
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now)
            };

            var result = await _userService.RegisterUserAsync(duplicate, "Pass");

            Assert.False(result.Success);
            Assert.Equal("Username already exists", result.Message);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ShouldSucceed_WithCorrectPassword()
        {
            await RegisterUserAsync();

            var result = await _userService.AuthenticateUserAsync("testuser", "Password123");

            Assert.True(result.Success);
            Assert.Equal("Authentication successful", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ShouldFail_WithWrongPassword()
        {
            await RegisterUserAsync();

            var result = await _userService.AuthenticateUserAsync("testuser", "WrongPassword");

            Assert.False(result.Success);
            Assert.Equal("Invalid password", result.Message);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUser()
        {
            await RegisterUserAsync();
            var result = await _userService.GetUserByEmailAsync("test@example.com");

            Assert.NotNull(result.Data);
            Assert.Equal("test@example.com", result.Data.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnCorrectUser()
        {
            var user = await RegisterUserAsync();
            var result = await _userService.GetUserByIdAsync(user.UserId);

            Assert.NotNull(result.Data);
            Assert.Equal(user.UserId, result.Data.UserId);
        }

        [Fact]
        public async Task UserExistsAsync_ShouldReturnTrue_IfExists()
        {
            await RegisterUserAsync();
            var exists = await _userService.UserExistsAsync("testuser");

            Assert.True(exists);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAll()
        {
            await RegisterUserAsync();
            await RegisterUserAsync("anotheruser", "another@example.com");

            var result = await _userService.GetAllUsersAsync();

            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldRemoveUser()
        {
            var user = await RegisterUserAsync();
            var result = await _userService.DeleteUserAsync(user.UserId);

            Assert.True(result.Success);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task ResetUserPasswordAsync_ShouldUpdatePassword_WhenCorrectCurrentPassword()
        {
            await RegisterUserAsync();

            var dto = new ResetPasswordDTO
            {
                Email = "test@example.com",
                CurrentPassword = "Password123",
                NewPassword = "NewPass123"
            };

            var result = await _userService.ResetUserPasswordAsync(dto);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task ResetUserPasswordAsync_ShouldFail_WhenWrongPassword()
        {
            await RegisterUserAsync();

            var dto = new ResetPasswordDTO
            {
                Email = "test@example.com",
                CurrentPassword = "WrongPass",
                NewPassword = "NewPass123"
            };

            var result = await _userService.ResetUserPasswordAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Password is incorrect", result.Message);
        }

        private async Task<User> RegisterUserAsync(string username = "testuser", string email = "test@example.com")
        {
            var dto = new UserRegisterDto
            {
                Username = username,
                Email = email,
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now)
            };
            var result = await _userService.RegisterUserAsync(dto, "Password123");
            return result.Data!;
        }
    }
}
