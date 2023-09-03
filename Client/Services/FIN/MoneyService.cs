using D69soft.Shared.Models.ViewModels.FIN;
using System.Net.Http.Json;

namespace D69soft.Client.Services.FIN
{
    public class MoneyService
    {
        private readonly HttpClient _httpClient;

        public MoneyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<BankVM>> GetBankList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<BankVM>>($"api/Money/GetBankList");
        }

        public async Task<string> UpdateBank(BankVM _bankVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Money/UpdateBank", _bankVM);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
