using GameStore.Client.Endpoints;
using GameStore.Services;
using GameStore.Shared.DTOs;
using GameStore.Shared.Models;

namespace GameStore.Endpoints
{
    public static class UsersEndpoints
    {
        public static RouteGroupBuilder MapUsersEndpoints(this WebApplication app)
        {
            var group = app.MapGroup(EndpointsRoutes.UserRoutes.baseRoute);

            group.MapPost(EndpointsRoutes.UserRoutes.registerRoute, async (UserRegisterDto UserRegisterDto, IUserService userService) =>
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

                var response = await userService.RegisterUserAsync(user, UserRegisterDto.Password);
                return response.Success ? Results.Ok(response) : Results.BadRequest(response);
            });

            group.MapPost(EndpointsRoutes.UserRoutes.loginRoute, async (UserLoginDTO loginDto, IUserService userService) =>
            {
                var response = await userService.AuthenticateUserAsync(loginDto.Username, loginDto.Password);
                return response.Success ? Results.Ok(response) : Results.Unauthorized();
            });

            group.MapPost(EndpointsRoutes.UserRoutes.logoutRoute, () => Results.Ok());

            group.MapGet("/", async (IUserService userService) =>
            {
                var response = await userService.GetAllUsersAsync();
                return !response.Success ? Results.BadRequest(response) : Results.Ok(response);
            });

            group.MapPut(EndpointsRoutes.UserRoutes.baseWithIdRoute, async (int userId, User updatedUser, IUserService userService) =>
            {
                var existingUserResponse = await userService.UpdateUserAsync(userId, updatedUser);
                return !existingUserResponse.Success ? Results.NotFound() : Results.NoContent();
            });

            group.MapPost(EndpointsRoutes.UserRoutes.resetPassRoute, async (ResetPasswordDTO resetPasswordDto, IUserService userService) =>
            {
                var response = await userService.ResetUserPasswordAsync(resetPasswordDto.Email,
                    resetPasswordDto.CurrentPassword, resetPasswordDto.NewPassword);
                if (!response.Success)
                {
                    return Results.BadRequest(response);
                }

                await userService.UpdateUserAsync(response.Data!.UserId, response.Data);
                return Results.Ok(response);
            });

            group.MapDelete(EndpointsRoutes.UserRoutes.baseWithIdRoute, async (IUserService service, int userId) =>
            {
                var response = await service.DeleteUserAsync(userId);
                return !response.Success ? Results.BadRequest(response) : Results.Ok(response);
            });

            return group;
        }
    }
}
