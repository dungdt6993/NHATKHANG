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

        //Bank
        public async Task<IEnumerable<BankVM>> GetBankList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<BankVM>>($"api/Money/GetBankList");
        }

        public async Task<bool> CheckContainsSwiftCode(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Money/CheckContainsSwiftCode/{id}");
        }

        public async Task<string> UpdateBank(BankVM _bankVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Money/UpdateBank", _bankVM);

            return await response.Content.ReadAsStringAsync();
        }

        //BankAccount
        public async Task<IEnumerable<BankAccountVM>> GetBankAccountList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<BankAccountVM>>($"api/Money/GetBankAccountList");
        }

        public async Task<bool> CheckContainsBankAccount(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Money/CheckContainsBankAccount/{id}");
        }

        public async Task<int> UpdateBankAccount(BankAccountVM _bankAccountVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Money/UpdateBankAccount", _bankAccountVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }
    }
}
