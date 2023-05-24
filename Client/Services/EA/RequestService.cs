using D69soft.Shared.Models.ViewModels.DOC;
using D69soft.Shared.Models.ViewModels.EA;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Net.Http.Json;

namespace D69soft.Client.Services.FIN
{
    public class RequestService
    {
        private readonly HttpClient _httpClient;

        public RequestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CartVM>> GetCarts(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CartVM>>($"api/Request/GetCarts/{_UserID}");
        }

        public async Task<bool> UpdateItemsCart(ItemsVM _itemsVM, string _UserID)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Request/UpdateItemsCart/{_UserID}", _itemsVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DelItemsCart(CartVM _cartVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Request/DelItemsCart", _cartVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateQtyItemsCart(CartVM _cartVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Request/UpdateQtyItemsCart", _cartVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateNoteItemsCart(CartVM _cartVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Request/UpdateNoteItemsCart", _cartVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> SendRequest(RequestVM _requestVM, string _UserID)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Request/SendRequest/{_UserID}", _requestVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<RequestVM>> GetRequest(FilterFinVM _filterFinVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Request/GetRequest", _filterFinVM);

            return await response.Content.ReadFromJsonAsync<List<RequestVM>>();
        }

        public async Task<bool> SendApprove(RequestVM _requestVM, string _type)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Request/SendApprove/{_type}", _requestVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateQtyApproved(RequestVM _requestVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Request/UpdateQtyApproved", _requestVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateRDNote(RequestVM _requestVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Request/UpdateRDNote", _requestVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        //Get RequestDetail to Stock
        public async Task<List<StockVoucherDetailVM>> GetRequestDetailToStockVoucherDetail(string _RequestCode)
        {
            return await _httpClient.GetFromJsonAsync<List<StockVoucherDetailVM>>($"api/Request/GetRequestDetailToStockVoucherDetail/{_RequestCode}");
        }
    }
}
