using D69soft.Shared.Models.Entities.SYSTEM;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
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

        //Log Err
        public async Task ErrorLog(ErrorLogVM _errorLogVM)
        {

        }

        //Log
        public async Task<bool> InsertLogUserFunc(string _UserID, string _FuncID)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Sys/InsertLogUserFunc/{_UserID}/{_FuncID}");
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

        public async Task<IEnumerable<PeriodVM>> GetDayFilter(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Sys/GetDayFilter", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<PeriodVM>>();
        }

        //RPT
        public async Task<List<SysRptVM>> GetModuleRpt(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<List<SysRptVM>>($"api/Sys/GetModuleRpt/{_UserID}");
        }
        public async Task<List<SysRptVM>> GetSysReportGroupByID(string _ModuleID, string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<List<SysRptVM>>($"api/Sys/GetSysReportGroupByID/{_ModuleID}/{_UserID}");
        }

        public async Task<IEnumerable<SysRptVM>> GetSysReportList(string _ModuleID, int _RptGrptID, string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<SysRptVM>>($"api/Sys/GetSysReportGroupByID/{_ModuleID}/{_RptGrptID}/{_UserID}");
        }

        public async Task<RptVM> GetSysReport(int _RptID)
        {
            return await _httpClient.GetFromJsonAsync<RptVM>($"api/Sys/GetSysReport/{_RptID}");
        }

        public async Task<bool> UpdateRpt(RptVM _rptVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Sys/UpdateRpt", _rptVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DelRpt(int _RptID)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Sys/DelRpt/{_RptID}");
        }

        public async Task<RptGrpVM> GetSysReportGroup(int _RptGrpID)
        {
            return await _httpClient.GetFromJsonAsync<RptGrpVM>($"api/Sys/GetSysReportGroup/{_RptGrpID}");
        }

        public async Task<string> UpdateGrtRpt(RptGrpVM _rptGrpVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Sys/UpdateGrtRpt", _rptGrpVM);

            return await response.Content.ReadFromJsonAsync<string>();
        }

        public async Task<bool> DelGrtRpt(int _RptGrpID)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Sys/DelGrtRpt/{_RptGrpID}");
        }

        public async Task<bool> CheckPermisSysRpt(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Sys/CheckPermisSysRpt/{_UserID}");
        }
    }
}
