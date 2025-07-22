using GameStore.Services;
using GameStore.Shared.DTOs;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Identity;

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
                        DateOfBirth = UserRegisterDto.DateOfBirth,
                        Role = "User"
                    };

                    await userService.RegisterUserAsync(user, UserRegisterDto.Password);
                    return Results.Created();
                } 
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            });

            group.MapPost("/login", async (UserLoginDTO loginDto, IUserService userService) =>
            {
                var user = await userService.AuthenticateUserAsync(loginDto.Username, loginDto.Password);
                return user == null ? Results.Unauthorized() : Results.Ok(user);
            });

            group.MapPost("/logout", () => Results.Ok());

            group.MapGet("/", async (IUserService userService) =>
                Results.Ok(await userService.GetAllUsersAsync()));

            group.MapPut("/{id:int}", async (int id, User updatedUser, IUserService userService) =>
            {
                var existingUser = await userService.UpdateUserAsync(id, updatedUser);
                if (existingUser == null)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            });

            group.MapPost("/resetPass", async (ResetPasswordDTO resetPasswordDto, IUserService userService) =>
            {
                try
                {
                    var user = await userService.ResetUserPasswordAsync(resetPasswordDto.Email,
                        resetPasswordDto.CurrentPassword, resetPasswordDto.NewPassword);
                    if (user == null)
                    {
                        return Results.NotFound("User not found");
                    }

                    await userService.UpdateUserAsync(user.Id, user);
                    return Results.Ok();
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            });

            return group;
        }
    }
}
