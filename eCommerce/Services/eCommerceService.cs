using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Collections;
using System.Net.Http.Json;

namespace D69soft.Client.Services.FIN
{
    public class eCommerceService
    {
        private readonly HttpClient _httpClient;

        public eCommerceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ItemsClassVM>> GetItemsClassList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ItemsClassVM>>($"api/Inventory/GetItemsClassList");
        }
        public async Task<IEnumerable<ItemsGroupVM>> GetItemsGroupList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ItemsGroupVM>>($"api/Inventory/GetItemsGroupList");
        }

        public async Task<List<ItemsVM>> GetItemsList(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/GetItemsList", _filterVM);

            return await response.Content.ReadFromJsonAsync<List<ItemsVM>>();
        }

    }
}
