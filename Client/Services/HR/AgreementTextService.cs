using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Net.Http.Json;

namespace D69soft.Client.Services.HR
{
    public class AgreementTextService
    {
        private readonly HttpClient _httpClient;

        public AgreementTextService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<AgreementTextTypeVM>> GetAgreementTextTypeList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AgreementTextTypeVM>>($"api/AgreementText/GetAgreementTextTypeList");
        }

        public async Task<List<AgreementTextVM>> GetAgreementTextList(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/AgreementText/GetAgreementTextList", _filterVM);

            return await response.Content.ReadFromJsonAsync<List<AgreementTextVM>>();
        }

        public async Task<IEnumerable<RptVM>> PrintAgreementText(IEnumerable<AgreementTextVM> _agreementTexts, string _UserID)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/AgreementText/PrintAgreementText/{_UserID}", _agreementTexts);

            return await response.Content.ReadFromJsonAsync<IEnumerable<RptVM>>();
        }

        public async Task<IEnumerable<AdjustProfileVM>> GetAdjustProfileList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AdjustProfileVM>>($"api/AgreementText/GetAdjustProfileList");
        }

        public async Task<IEnumerable<AdjustProfileRptVM>> GetAdjustProfileRptList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AdjustProfileRptVM>>($"api/AgreementText/GetAdjustProfileRptList");
        }

        public async Task<bool> UpdateAdjustProfile(AdjustProfileVM _adjustProfileVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/AgreementText/UpdateAdjustProfile", _adjustProfileVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
}
