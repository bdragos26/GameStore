using GameStore.Client.Clients;
using GameStore.Components;
using GameStore.Data;
using GameStore.Endpoints;
using GameStore.Services;
using GameStore.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpClient();

builder.Services.AddScoped<IGamesClient, GamesClient>();
builder.Services.AddScoped<IGenresClient, GenresClient>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGenreService, GenreService>();

builder.Services.AddScoped<PasswordHasher<User>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserClient, UserClient>();

var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddDbContext<GameStoreContext>(options => options.UseSqlite(connString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GameStore.Client._Imports).Assembly);

app.MapGamesEndpoints();
app.MapGenreEndpoints();
app.MapUsersEndpoints();

await app.MigrateDbAsync();

app.Run();
