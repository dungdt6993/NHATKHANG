using D69soft.Shared.Models.ViewModels.FIN;
using System.Collections;
using System.Net.Http.Json;

namespace D69soft.Client.Services.FIN
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

        public async Task<IEnumerable<VSubTypeVM>> GetVSubTypeVMs(string _VTypeID)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<VSubTypeVM>>($"api/Voucher/GetVSubTypeVMs/{_VTypeID}");
        }

        public async Task<List<VoucherVM>> GetVouchers(FilterFinVM _filterFinVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Voucher/GetVouchers", _filterFinVM);

            return await response.Content.ReadFromJsonAsync<List<VoucherVM>>();
        }

        public async Task<List<VoucherDetailVM>> GetVoucherDetails(string _VNumber)
        {
            return await _httpClient.GetFromJsonAsync<List<VoucherDetailVM>>($"api/Voucher/GetVoucherDetails/{_VNumber}");
        }

        public async Task<IEnumerable<VoucherDetailVM>> GetSearchItems(VoucherVM _voucherVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Voucher/GetSearchItems", _voucherVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<VoucherDetailVM>>();
        }

        public async Task<string> UpdateVoucher(VoucherVM _stockVoucherVM, IEnumerable<VoucherDetailVM> _stockVoucherDetailVMs)
        {
            ArrayList arrayList = new ArrayList();
            arrayList.Add(_stockVoucherVM);
            arrayList.Add(_stockVoucherDetailVMs);

            var response = await _httpClient.PostAsJsonAsync($"api/Voucher/UpdateVoucher", arrayList);

            return await response.Content.ReadAsStringAsync();
        }

        //VAT
        public async Task<IEnumerable<VATDefVM>> GetVATDefs()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<VATDefVM>>($"api/Voucher/GetVATDefs");
        }
        public async Task<List<InvoiceVM>> GetInvoices(FilterFinVM _filterFinVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Voucher/GetInvoices", _filterFinVM);

            return await response.Content.ReadFromJsonAsync<List<InvoiceVM>>();
        }

        //Account
        public async Task<IEnumerable<AccountVM>> GetAccounts()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AccountVM>>($"api/Voucher/GetAccounts");
        }

        //RPT
        public async Task<List<VoucherDetailVM>> GetMoneyBooks(FilterFinVM _filterFinVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Voucher/GetMoneyBooks", _filterFinVM);

            return await response.Content.ReadFromJsonAsync<List<VoucherDetailVM>>();
        }

        public async Task<List<InventoryVM>> GetInvoiceBooks(FilterFinVM _filterFinVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Voucher/GetInvoiceBooks", _filterFinVM);

            return await response.Content.ReadFromJsonAsync<List<InventoryVM>>();
        }

    }
}
