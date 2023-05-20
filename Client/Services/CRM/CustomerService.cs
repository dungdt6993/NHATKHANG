using D69soft.Shared.Models.ViewModels.CRM;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Net.Http.Json;

namespace D69soft.Client.Services.CRM
{
    public class CustomerService
    {
        private readonly HttpClient _httpClient;

        public CustomerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> UpdateCustomer(CustomerVM _customerVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Customer/UpdateCustomer", _customerVM);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<CustomerVM> GetCustomerByID(string _CustomerID)
        {
            return await _httpClient.GetFromJsonAsync<CustomerVM>($"api/Customer/GetCustomerByID/{_CustomerID}");
        }

        public async Task<IEnumerable<CustomerVM>> GetCustomers()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CustomerVM>>($"api/Customer/GetCustomers");
        }

        public async Task<IEnumerable<CustomerVM>> SearchCustomers(string searchText)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CustomerVM>>($"api/Customer/SearchCustomers/{searchText}");
        }

        public async Task<bool> CheckContains_Customer(string _CustomerID)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Customer/CheckContains_Customer/{_CustomerID}");
        }

        public async Task<bool> CheckContains_Tel(string _Tel)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Customer/CheckContains_Tel/{_Tel}");
        }
    }
}
