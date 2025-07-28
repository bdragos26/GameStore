using Blazored.LocalStorage;
using GameStore.Client;
using GameStore.Client.Services.ApiClients;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

builder.Services.AddScoped<IGamesClient, GamesClient>();
builder.Services.AddScoped<IGenresClient, GenresClient>();
builder.Services.AddScoped<IUserClient, UserClient>();
builder.Services.AddScoped<IGameRatingClient, GameRatingClient>();

builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();