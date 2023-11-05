using D69soft.Shared.Models.Entities.SYSTEM;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http;
using System.Net.Http.Json;

namespace D69soft.Client.Services
{
    public class SysService
    {
        private readonly HttpClient _httpClient;

        public SysService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //ExecuteSQLQueryToDataTable
        public async Task<DataTable> ExecuteSQLQueryToDataTable(string _sql)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Sys/ExecuteSQLQueryToDataTable", _sql);

            return JsonConvert.DeserializeObject<DataTable>(await response.Content.ReadAsStringAsync());
        }

        //Log
        public async Task InsertLog(LogVM _logVM)
        {
            await _httpClient.PostAsJsonAsync($"api/Sys/InsertLog", _logVM);
        }

        public async Task<List<LogVM>> GetLog()
        {
            return await _httpClient.GetFromJsonAsync<List<LogVM>>($"api/Sys/GetLog");
        }

        public async Task<bool> CheckAccessFunc(string _UserID, string _FuncID)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Sys/CheckAccessFunc/{_UserID}/{_FuncID}");
        }

        public async Task<bool> CheckAccessSubFunc(string _UserID, string _SubFuncID)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Sys/CheckAccessSubFunc/{_UserID}/{_SubFuncID}");
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

        //GlobalParameter
        public async Task<IEnumerable<GlobalParameterVM>> GetGlobalParameterList(string _FunID)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GlobalParameterVM>>($"api/Sys/GetGlobalParameterList/{_FunID}");
        }

        public async Task<bool> UpdateGlobalParameter(GlobalParameterVM _globalParameterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Sys/UpdateGlobalParameter", _globalParameterVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        //Common
        public async Task<IEnumerable<PeriodVM>> GetPeriodFilter()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PeriodVM>>($"api/Sys/GetPeriodFilter");
        }

        public async Task<IEnumerable<PeriodVM>> GetMonthFilter()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PeriodVM>>($"api/Sys/GetMonthFilter");
        }

        public async Task<IEnumerable<PeriodVM>> GetYearFilter()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PeriodVM>>($"api/Sys/GetYearFilter");
        }

        public async Task<IEnumerable<PeriodVM>> GetDayFilter(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Sys/GetDayFilter", _filterVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<PeriodVM>>();
        }

        //RPT
        public async Task<IEnumerable<RptVM>> GetRptList(int _RptID, string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<RptVM>>($"api/Sys/GetRptList/{_RptID}/{_UserID}");
        }

        public async Task<bool> CheckPermisRpt(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Sys/CheckPermisRpt/{_UserID}");
        }
    }
}
