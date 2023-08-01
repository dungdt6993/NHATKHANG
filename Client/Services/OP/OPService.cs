using Model.ViewModels.OP;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.OP;
using System.Net.Http.Json;

namespace D69soft.Client.Services.OP
{
    public class OPService
    {
        private readonly HttpClient _httpClient;

        public OPService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<TenderVM>> GetTenders(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OP/GetTenders", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<TenderVM>>();
        }

        //CruiseSchedule
        public async Task<List<CruiseScheduleVM>> GetCruiseSchedules(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OP/GetCruiseSchedules", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<List<CruiseScheduleVM>>();
        }

        public async Task<IEnumerable<CruiseStatusVM>> GetCruiseStatus()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CruiseStatusVM>>($"api/OP/GetCruiseStatus");
        }

        public async Task<bool> UpdateCruiseSchedule(CruiseScheduleVM _cruiseScheduleVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OP/UpdateCruiseSchedule", _cruiseScheduleVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        //TenderSchedule
        public async Task<IEnumerable<TenderScheduleVM>> GetTenderSchedules(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OP/GetTenderSchedules", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<TenderScheduleVM>>();
        }

        public async Task<bool> UpdateTenderShift(TenderScheduleVM _tenderScheduleVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OP/UpdateTenderShift", _tenderScheduleVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateTenderStatus(TenderScheduleVM _tenderScheduleVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OP/UpdateTenderStatus", _tenderScheduleVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

    }
}
