using GameStore.Services;
using GameStore.Shared.DTOs;
using GameStore.Shared.Models;

namespace GameStore.Endpoints
{
    public static class UsersEndpoints
    {
        public static RouteGroupBuilder MapUsersEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/users");

            group.MapPost("/register", async (UserRegisterDto UserRegisterDto, IUserService userService) =>
            {
                try
                {
                    var user = new User()
                    {
                        Username = UserRegisterDto.Username,
                        Email = UserRegisterDto.Email,
                        FirstName = UserRegisterDto.FirstName,
                        LastName = UserRegisterDto.LastName,
                        DateOfBirth = UserRegisterDto.DateOfBirth
                    };

                    await userService.RegisterUserAsync(user, UserRegisterDto.Password);
                    return Results.Created();
                } 
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            });

            group.MapPost("/login", async (UserLoginDTO userLoginDto, IUserService userService) =>
            {
                var user = await userService.AuthenticateUserAsync(userLoginDto.Username, userLoginDto.Password);
                return user == null ? Results.Unauthorized() : Results.Ok(user);
            });

            group.MapGet("/", async (IUserService userService) =>
                Results.Ok(await userService.GetAllUsers()));

            return group;
        }
    }
}
