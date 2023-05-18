using D69soft.Shared.Models.ViewModels.DOC;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Net.Http.Json;

namespace Data.Repositories.FIN
{
    public class VoucherService
    {
        private readonly HttpClient _httpClient;

        public VoucherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<VTypeVM>> GetVTypeVMs(string _FuncID)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<VTypeVM>>($"api/Voucher/GetVTypeVMs/{_FuncID}");
        }

        public async Task<IEnumerable<VSubTypeVM>> GetVSubTypeVMs(string _FuncID)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<VSubTypeVM>>($"api/Voucher/GetVSubTypeVMs/{_FuncID}");
        }

        public async Task<List<StockVoucherVM>> GetStockVouchers(FilterFinVM _filterFinVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Voucher/GetStockVouchers", _filterFinVM);

            return await response.Content.ReadFromJsonAsync<List<StockVoucherVM>>();
        }

        public async Task<List<StockVoucherDetailVM>> GetStockVoucherDetails(string _VNumber)
        {
            return await _httpClient.GetFromJsonAsync<List<StockVoucherDetailVM>>($"api/Voucher/GetStockVoucherDetails/{_VNumber}");
        }

        public async Task<IEnumerable<StockVoucherDetailVM>> GetSearchItems(FilterFinVM _filterFinVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Voucher/GetSearchItems", _filterFinVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<StockVoucherDetailVM>>();
        }

        public async Task<string> UpdateVoucher(StockVoucherVM _stockVoucherVM, IEnumerable<StockVoucherDetailVM> _stockVoucherDetailVMs)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Voucher/UpdateVoucher/{_stockVoucherDetailVMs}", _stockVoucherVM);

            return await response.Content.ReadFromJsonAsync<string>();
        }

    }
}
