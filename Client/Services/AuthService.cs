using Blazored.LocalStorage;
using D69soft.Client.Extensions;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components.Authorization;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace D69soft.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
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

        public async Task<int> GetRole(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<int>($"api/Auth/GetRole/{_UserID}");
        }

        public async Task<IEnumerable<PermissionUserVM>> GetPermissionUser(string _Eserial, string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PermissionUserVM>>($"api/Auth/GetPermissionUser/{_Eserial}/{_UserID}");
        }

        //JWT
        public async Task<LoginResponseVM> Login(UserVM _userVM)
        {
            var result = await _httpClient.PostAsJsonAsync("/api/Auth/Login", _userVM);
            var content = await result.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<LoginResponseVM>(content,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            if (!result.IsSuccessStatusCode)
            {
                return loginResponse;
            }
            await _localStorage.SetItemAsync("authToken", loginResponse.Token);
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(_userVM.Eserial);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResponse.Token);
            return loginResponse;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

    }
}