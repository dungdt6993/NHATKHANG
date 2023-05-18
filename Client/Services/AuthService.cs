using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Data;
using System.Net.Http;
using System.Net.Http.Json;

namespace D69soft.Client.Services
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

        public async Task<int> GetRole(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<int>($"api/Auth/GetRole/{_UserID}");
        }

        public async Task<IEnumerable<PermissionUserVM>> GetPermissionUser(string _Eserial, string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PermissionUserVM>>($"api/Auth/GetPermissionUser/{_Eserial}/{_UserID}");
        }

    }
}