using GameStore.Services;
using GameStore.Shared.DTOs;
using GameStore.Shared.Endpoints;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace GameStore.Endpoints
{
    public static class UsersEndpoints
    {
        public static RouteGroupBuilder MapUsersEndpoints(this WebApplication app)
        {
            var group = app.MapGroup(EndpointsRoutes.User._base);

            group.MapPost(EndpointsRoutes.User.register, async (UserRegisterDto registerDto, IUserService userService) =>
            {
                var response = await userService.RegisterUserAsync(registerDto, registerDto.Password);
                return response.Success ? Results.Ok(response) : Results.BadRequest(response);
            });

            group.MapPost(EndpointsRoutes.User.login, async (UserLoginDTO loginDto, IUserService userService) =>
            {
                var response = await userService.AuthenticateUserAsync(loginDto.Username, loginDto.Password);

                if (response.Success)
                    return Results.Ok(response);
                else
                    return Results.Json(response, statusCode: StatusCodes.Status401Unauthorized);
            });

            group.MapPost(EndpointsRoutes.User.logout, [Authorize] () => Results.Ok());

            group.MapGet("/", [Authorize(Roles = "Admin")] async (IUserService userService) =>
            {
                var response = await userService.GetAllUsersAsync();
                return !response.Success ? Results.BadRequest(response) : Results.Ok(response);
            });

            group.MapPut(EndpointsRoutes.User.update, [Authorize] async (int userId, User updatedUser, IUserService userService) =>
            {
                var existingUserResponse = await userService.GetUserByIdAsync(userId);
                if (existingUserResponse == null || !existingUserResponse.Success)
                {
                    return Results.NotFound("User not found");
                }

                if (!string.IsNullOrEmpty(updatedUser.Email))
                {
                    var emailUserResponse = await userService.GetUserByEmailAsync(updatedUser.Email);
                    if (emailUserResponse.Success && emailUserResponse.Data != null && emailUserResponse.Data.UserId != userId)
                    {
                        return Results.BadRequest("Email is already in use by another account");
                    }
                }

                updatedUser.PasswordHash = existingUserResponse.Data.PasswordHash;

                var updateResponse = await userService.UpdateUserAsync(userId, updatedUser);
                return updateResponse.Success ? Results.Ok(updateResponse) : Results.BadRequest(updateResponse);
            });

            group.MapPost(EndpointsRoutes.User.resetPass, [Authorize] async (ResetPasswordDTO resetPasswordDto, IUserService userService) =>
            {
                var response = await userService.ResetUserPasswordAsync(resetPasswordDto);
                if (!response.Success)
                {
                    return Results.BadRequest(response);
                }

                await userService.UpdateUserAsync(response.Data!.UserId, response.Data);
                return Results.Ok(response);
            });

            group.MapDelete(EndpointsRoutes.User.delete, [Authorize(Roles = "Admin")] async (IUserService service, int userId) =>
            {
                var response = await service.DeleteUserAsync(userId);
                return !response.Success ? Results.BadRequest(response) : Results.Ok(response);
            });

            group.MapPost(EndpointsRoutes.User.forgotPass, async (ForgotPasswordDto dto, IUserService userService) =>
            {
                await userService.InitiatePasswordReset(dto.Email);
                return Results.Ok();
            });

            group.MapPost(EndpointsRoutes.User.reset, async (ResetPasswordWithTokenDto dto, IUserService userService) =>
            {
                var success = await userService.ResetPasswordWithToken(dto);
                return success ? Results.Ok() : Results.BadRequest("Invalid or expired token");
            });

            group.MapGet(EndpointsRoutes.User.getById, [Authorize] async (int userId, IUserService userService) =>
            {
                var response = await userService.GetUserByIdAsync(userId);
                return response.Success ? Results.Ok(response) : Results.BadRequest(response);
            });

            return group;
        }
    }
}
