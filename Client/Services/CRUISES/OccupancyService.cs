using System.Net.Http.Json;

namespace D69soft.Client.Services.CRUISES
{
    public class OccupancyService
    {
        private readonly HttpClient _httpClient;

        public OccupancyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //SyncCRS
        public async Task<bool> SyncDataCRS()
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Occupancy/SyncDataCRS");
        }
    }
}
