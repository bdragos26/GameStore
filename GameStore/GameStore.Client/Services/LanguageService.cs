using Blazored.LocalStorage;
using System.Globalization;

namespace GameStore.Client.Services
{
    public class LanguageService
    {
        private readonly ILocalStorageService _localStorage;
        private const string LangKey = "selectedLanguage";

        public LanguageService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SetLanguageAsync(string culture)
        {
            await _localStorage.SetItemAsync(LangKey, culture);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(culture);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(culture);
        }

        public async Task<string> GetCurrentCultureAsync()
        {
            return await _localStorage.GetItemAsync<string>(LangKey) ?? "en";
        }
    }
}
