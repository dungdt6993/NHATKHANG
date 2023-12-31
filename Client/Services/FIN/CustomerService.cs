﻿using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Net.Http.Json;

namespace D69soft.Client.Services.FIN
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

        public async Task<CustomerVM> GetCustomerByID(string _CustomerCode)
        {
            return await _httpClient.GetFromJsonAsync<CustomerVM>($"api/Customer/GetCustomerByID/{_CustomerCode}");
        }

        public async Task<IEnumerable<CustomerVM>> GetCustomers()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CustomerVM>>($"api/Customer/GetCustomers");
        }

        public async Task<IEnumerable<CustomerVM>> SearchCustomers(string searchText)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CustomerVM>>($"api/Customer/SearchCustomers/{searchText}");
        }

        public async Task<bool> ContainsCustomerTel(string _CustomerTel)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Customer/ContainsCustomerTel/{_CustomerTel}");
        }
    }
}
