using GameStore.Client.Clients;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient<GamesClient>(client =>
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddHttpClient<GenresClient>(client =>
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddHttpClient<UserClient>(client =>
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

await builder.Build().RunAsync();