using D69soft.Shared.Models.ViewModels.HR;
using System.Net.Http.Json;

namespace D69soft.Client.Services.HR
{
    public class DayOffService
    {
        private readonly HttpClient _httpClient;

        public DayOffService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<DayOffVM>> GetDayOffList(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DayOff/GetDayOffList", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<List<DayOffVM>>();
        }

        public async Task<IEnumerable<ShiftVM>> GetDayOffTypeList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ShiftVM>>($"api/DayOff/GetDayOffTypeList");
        }

        public async Task<bool> DayOff_calcAL(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DayOff/DayOff_calcAL", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DayOff_calcDO(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DayOff/DayOff_calcDO", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DayOff_calcPH(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DayOff/DayOff_calcPH", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DayOff_calcControlDAYOFF(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DayOff/DayOff_calcControlDAYOFF", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DayOff_calcDODefault(FilterHrVM _filterHrVM, int _typeCalc)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DayOff/DayOff_calcDODefault/{_typeCalc}", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DayOff_calcPHDefault(FilterHrVM _filterHrVM, int _typeCalc)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DayOff/DayOff_calcPHDefault/{_typeCalc}", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateAddBalance(DayOffVM _shiftVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DayOff/UpdateAddBalance", _shiftVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<IEnumerable<DayOffVM>> GetDayOffDetail(int _Period, string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DayOffVM>>($"api/DayOff/GetDayOffDetail/{_Period}/{_Eserial}");
        }

        //SpecialDayOff
        public async Task<List<DayOffVM>> GetSpecialDayOffList(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DayOff/GetSpecialDayOffList", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<List<DayOffVM>>();
        }

        //PH
        public async Task<IEnumerable<PublicHolidayDefVM>> GetPublicHolidayList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PublicHolidayDefVM>>($"api/DayOff/GetPublicHolidayList");
        }

        public async Task<bool> ContainsPublicHoliday(int _PHDay, int _PHMonth, bool _isLunar)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/DayOff/ContainsPublicHoliday/{_PHDay}/{_PHMonth}/{_isLunar}");
        }

        public async Task<bool> UpdatePublicHoliday(PublicHolidayDefVM _publicHolidayDefVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DayOff/UpdatePublicHoliday", _publicHolidayDefVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

    }
}
