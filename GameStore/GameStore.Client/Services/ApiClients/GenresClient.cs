﻿using GameStore.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace GameStore.Client.Services.ApiClients
{
    public interface IGenresClient
    {
        Task<List<Genre>> GetGenresAsync();
    }
    public class GenresClient : IGenresClient
    {
        private readonly HttpClient _httpClient;

        public GenresClient(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Genre>> GetGenresAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Genre>>>("/genres");
            return response?.Data ?? new List<Genre>();
        }
    }
}
