using D69soft.Shared.Models.ViewModels.FIN;
using System.Net.Http.Json;

namespace D69soft.Client.Services.FIN
{
    public class PurchasingService
    {
        private readonly HttpClient _httpClient;

        public PurchasingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<VendorVM>> GetVendorList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<VendorVM>>($"api/Purchasing/GetVendorList");
        }

        public async Task<string> UpdateVendor(VendorVM _vendorVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Purchasing/UpdateVendor", _vendorVM);

            return await response.Content.ReadAsStringAsync();
        }

    }
}
