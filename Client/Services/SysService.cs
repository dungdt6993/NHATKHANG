using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Net.Http;
using System.Net.Http.Json;

namespace D69soft.Server.Services
{
    public class SysService
    {
        private readonly HttpClient _httpClient;

        public SysService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //Log Err
        public async Task ErrorLog(ErrorLogVM _errorLogVM)
        {

        }

        //Info User
        public async Task<UserVM> GetInfoUser(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<UserVM>($"api/Sys/GetInfoUser/{_UserID}");
        }

        //Menu Func
        public async Task<IEnumerable<FuncVM>> GetModuleMenu(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<FuncVM>>($"api/Sys/GetModuleMenu/{_UserID}");
        }

        public async Task<IEnumerable<FuncVM>> GetFuncMenuGroup(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<FuncVM>>($"api/Sys/GetFuncMenuGroup/{_UserID}");
        }

        public async Task<IEnumerable<FuncVM>> GetFuncMenu(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<FuncVM>>($"api/Sys/GetFuncMenu/{_UserID}");
        }

        public async Task<bool> CheckViewFuncMenuRpt(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Sys/CheckViewFuncMenuRpt/{_UserID}");
        }

    }
}
