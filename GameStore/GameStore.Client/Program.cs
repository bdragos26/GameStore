using Blazored.LocalStorage;
using GameStore.Client;
using GameStore.Client.Components;
using GameStore.Client.Services;
using GameStore.Client.Services.ApiClients;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

builder.Services.AddScoped<IGamesClient, GamesClient>();
builder.Services.AddScoped<IGenresClient, GenresClient>();
builder.Services.AddScoped<IUserClient, UserClient>();
builder.Services.AddScoped<IGameRatingClient, GameRatingClient>();
builder.Services.AddScoped<ICartClient, CartClient>();

builder.Services.AddMudServices();

builder.Services.AddAuthorizationCore();
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<LanguageService>();

var host = builder.Build();

// Set culture from local storage before running app
var localStorage = host.Services.GetRequiredService<ILocalStorageService>();
var lang = await localStorage.GetItemAsync<string>("selectedLanguage") ?? "en";
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(lang);
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(lang);

await host.RunAsync();