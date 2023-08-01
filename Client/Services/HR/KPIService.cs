using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using System.Data;
using System.Net.Http.Json;

namespace D69soft.Client.Services.HR
{
    public class KPIService
    {
        private readonly HttpClient _httpClient;

        public KPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //Filter
        public async Task<IEnumerable<PeriodVM>> GetMonthFilter(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/GetMonthFilter", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<PeriodVM>>();
        }
        public async Task<IEnumerable<PeriodVM>> GetYearFilter(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/GetYearFilter", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<PeriodVM>>();
        }

        public async Task<IEnumerable<DivisionVM>> GetDivisions(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/GetDivisions", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<DivisionVM>>();
        }

        public async Task<IEnumerable<DepartmentVM>> GetDepartments(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/GetDepartments", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<DepartmentVM>>();
        }

        public async Task<IEnumerable<EserialVM>> GetEserials(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/GetEserials", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<EserialVM>>();
        }

        //End Filter

        public async Task<IEnumerable<KPIVM>> GetKPIs(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/GetKPIs", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<KPIVM>>();
        }

        public async Task<RankVM> GetRank(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/GetRank", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<RankVM>();
        }

        public async Task<List<RankVM>> GetRanks(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/GetRanks", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<List<RankVM>>();
        }

        public async Task<bool> UpdateStaffScore(KPIVM _kpiVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/UpdateStaffScore", _kpiVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateFinalScore(KPIVM _kpiVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/UpdateFinalScore", _kpiVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateActualDescription(KPIVM _kpiVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/UpdateActualDescription", _kpiVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateManagerComment(KPIVM _kpiVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/UpdateManagerComment", _kpiVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateStaffNote(RankVM _rankVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/UpdateStaffNote", _rankVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateManagerNote(RankVM _rankVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/UpdateManagerNote", _rankVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateTarget(KPIVM _kpiVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/UpdateTarget", _kpiVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> InitializeKPI(FilterHrVM _filterHrVM, string _Eserial)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/InitializeKPI/{_Eserial}", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> SendKPI(RankVM _rankVM, string _type)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/KPI/SendKPI/{_type}", _rankVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

    }
}
