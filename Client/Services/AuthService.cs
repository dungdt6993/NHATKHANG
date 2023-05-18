using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Net.Http;
using System.Net.Http.Json;

namespace D69soft.Server.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> LoginRequest(UserVM _userVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Auth/LoginRequest", _userVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<bool> ChangePass(ChangePassVM _changePassVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Auth/ChangePass", _changePassVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> CheckChangePassDefault(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Auth/CheckChangePassDefault/{_UserID}");
        }

    }
}