using GameStore.Shared.DTOs;
using GameStore.Shared.Models;

namespace GameStore.Shared.Mappers
{
    public static class UserMapper
    {
        public static User ToEntity(UserRegisterDto dto)
        {
            return new User()
            {
                Username = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                Role = Roles.User
            };
        }

        public static UserRegisterDto ToRegisterDto(User user)
        {
            return new UserRegisterDto
            {
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth
            };
        }

        public static UserLoginDTO ToLoginDto(User user)
        {
            return new UserLoginDTO()
            {
                Username = user.Username,
                Password = string.Empty
            };
        }

        public static UserProfileDto ToProfileDto(User user)
        {
            return new UserProfileDto()
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                CreatedAt = user.CreatedAt,
                Role = user.Role
            };
        }

        public static void UpdateEntityFromDto(UserRegisterDto dto, User entity)
        {
            entity.Username = dto.Username;
            entity.Email = dto.Email;
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.DateOfBirth = dto.DateOfBirth;
        }
    }
}
