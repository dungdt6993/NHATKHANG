using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Net.Http.Json;

namespace Data.Repositories.FIN
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

    }
}
