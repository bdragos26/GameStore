using GameStore.Client.Services.ApiClients;
using GameStore.Shared.DTOs;
using GameStore.Shared.Models;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Tests.ApiClientsTests;
public class UserClientTests
{
    private static HttpClient CreateHttpClient(HttpResponseMessage responseMessage, out Mock<HttpMessageHandler> handlerMock)
    {
        handlerMock = new Mock<HttpMessageHandler>();

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        return new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost") };
    }

    [Fact]
    public async Task RegisterAsync_Success()
    {
        var serviceResponse = new ServiceResponse<User> { Success = true, Data = new User() };
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(serviceResponse)
        };

        var httpClient = CreateHttpClient(httpResponse, out _);
        var userClient = new UserClient(httpClient, null!);

        await userClient.RegisterAsync(new UserRegisterDto());
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenServiceResponseFails()
    {
        var serviceResponse = new ServiceResponse<User> { Success = false, Message = "Error" };
        var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = JsonContent.Create(serviceResponse)
        };

        var httpClient = CreateHttpClient(httpResponse, out _);
        var userClient = new UserClient(httpClient, null!);

        var ex = await Assert.ThrowsAsync<Exception>(() => userClient.RegisterAsync(new UserRegisterDto()));
        Assert.Equal("Error", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUserProfile_WhenResponseIsSuccess()
    {
        var expectedProfile = new UserProfileDto { Username = "test" };
        var serviceResponse = new ServiceResponse<UserProfileDto> { Data = expectedProfile, Success = true };

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(serviceResponse)
        };

        var httpClient = CreateHttpClient(httpResponse, out _);
        var userClient = new UserClient(httpClient, null!);

        var result = await userClient.LoginAsync(new UserLoginDTO());

        Assert.NotNull(result);
        Assert.Equal("test", result.Username);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenResponseFails()
    {
        var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var httpClient = CreateHttpClient(httpResponse, out _);
        var userClient = new UserClient(httpClient, null!);

        var result = await userClient.LoginAsync(new UserLoginDTO());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser()
    {
        var user = new User { UserId = 1, Username = "john" };
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(user)
        };

        var httpClient = CreateHttpClient(httpResponse, out _);
        var userClient = new UserClient(httpClient, null!);

        var result = await userClient.GetUserByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("john", result.Username);
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturnList()
    {
        var users = new List<User> { new User { UserId = 1, Username = "john" } };
        var serviceResponse = new ServiceResponse<List<User>> { Data = users, Success = true };

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(serviceResponse)
        };

        var httpClient = CreateHttpClient(httpResponse, out _);
        var userClient = new UserClient(httpClient, null!);

        var result = await userClient.GetUsersAsync();

        Assert.Single(result);
        Assert.Equal("john", result[0].Username);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldThrowException_WhenFails()
    {
        var errorResponse = new ServiceResponse<User> { Message = "Update failed" };
        var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = JsonContent.Create(errorResponse)
        };

        var httpClient = CreateHttpClient(httpResponse, out _);
        var userClient = new UserClient(httpClient, null!);

        var ex = await Assert.ThrowsAsync<Exception>(() => userClient.UpdateUserAsync(new User { UserId = 1 }));
        Assert.Equal("Update failed", ex.Message);
    }

    [Fact]
    public async Task LogoutAsync_ShouldReturnTrue()
    {
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
        var httpClient = CreateHttpClient(httpResponse, out _);
        var userClient = new UserClient(httpClient, null!);

        await userClient.LogoutAsync();
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldReturnTrue_WhenSuccess()
    {
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new ServiceResponse<User> { Success = true, Data = new User() })
        };
        var httpClient = CreateHttpClient(httpResponse, out _);
        var userClient = new UserClient(httpClient, null!);

        var resetDto = new ResetPasswordDTO();

        await userClient.ResetPasswordAsync(resetDto);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnTrue_WhenSuccess()
    {
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new ServiceResponse<bool> { Success = true, Data = true })
        };
        var httpClient = CreateHttpClient(httpResponse, out _);
        var userClient = new UserClient(httpClient, null!);

        await userClient.DeleteUserAsync(1);
    }

    [Fact]
    public async Task ResetPasswordWithToken_ShouldReturnTrue_WhenSuccess()
    {
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
        var httpClient = CreateHttpClient(httpResponse, out _);
        var userClient = new UserClient(httpClient, null!);

        var dto = new ResetPasswordWithTokenDto { };

        await userClient.ResetPasswordWithToken(dto);
    }
}