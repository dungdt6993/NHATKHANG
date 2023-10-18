using Model.ViewModels.OP;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.OP;
using System.Net.Http.Json;
using D69soft.Shared.Models.ViewModels.SYSTEM;

namespace D69soft.Client.Services.OP
{
    public class OPService
    {
        private readonly HttpClient _httpClient;

        public OPService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //CruiseSchedule
        public async Task<List<CruiseScheduleVM>> GetCruiseSchedules(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OP/GetCruiseSchedules", _filterVM);

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

        //VehicleSchedule
        public async Task<IEnumerable<VehicleVM>> GetVehicles(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OP/GetVehicles", _filterVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<VehicleVM>>();
        }

        public async Task<IEnumerable<VehicleScheduleVM>> GetVehicleSchedules(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OP/GetVehicleSchedules", _filterVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<VehicleScheduleVM>>();
        }

        public async Task<bool> UpdateVehicleShift(VehicleScheduleVM _tenderScheduleVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OP/UpdateVehicleShift", _tenderScheduleVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateVehicleStatus(VehicleScheduleVM _tenderScheduleVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OP/UpdateVehicleStatus", _tenderScheduleVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

    }
}
