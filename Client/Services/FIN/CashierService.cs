﻿using D69soft.Shared.Models.ViewModels.FIN;
using System.Net.Http.Json;
using System.Collections;
using D69soft.Shared.Models.ViewModels.SYSTEM;

namespace D69soft.Client.Services.FIN
{
    public class CashierService
    {
        private readonly HttpClient _httpClient;

        public CashierService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<PointOfSaleVM>> GetPointOfSale()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PointOfSaleVM>>($"api/Cashier/GetPointOfSale");
        }

        public async Task<IEnumerable<RoomTableAreaVM>> GetRoomTableArea(string _POSCode)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<RoomTableAreaVM>>($"api/Cashier/GetRoomTableArea/{_POSCode}");
        }

        public async Task<IEnumerable<RoomTableVM>> GetRoomTable(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Cashier/GetRoomTable", _filterVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<RoomTableVM>>();
        }

        public async Task<List<ItemsVM>> GetItems(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Cashier/GetItems", _filterVM);

            return await response.Content.ReadFromJsonAsync<List<ItemsVM>>();
        }

        public async Task<bool> OpenRoomTable(FilterVM _filterVM, InvoiceVM _invoiceVM)
        {
            ArrayList _arrayList = new ArrayList();
            _arrayList.Add(_filterVM);
            _arrayList.Add(_invoiceVM);

            var response = await _httpClient.PostAsJsonAsync($"api/Cashier/OpenRoomTable", _arrayList);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> OpenTakeOut(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Cashier/OpenTakeOut", _filterVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> ChooseItems(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Cashier/ChooseItems", _filterVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateInvoiceCustomer(InvoiceVM _invoiceVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Cashier/UpdateInvoiceCustomer", _invoiceVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateInvoiceDetail(InvoiceVM _invoiceDetail)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Cashier/UpdateInvoiceDetail", _invoiceDetail);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateInvoiceDiscount(InvoiceVM _invoiceDetail)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Cashier/UpdateInvoiceDiscount", _invoiceDetail);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<InvoiceVM> GetInfoInvoice(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Cashier/GetInfoInvoice", _filterVM);

            return await response.Content.ReadFromJsonAsync<InvoiceVM>();
        }

        public async Task<IEnumerable<InvoiceVM>> GetInvoiceItems(string _CheckNo)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<InvoiceVM>>($"api/Cashier/GetInvoiceItems/{_CheckNo}");
        }

        public async Task<InvoiceVM> GetInvoiceTotal(string _CheckNo)
        {
            return await _httpClient.GetFromJsonAsync<InvoiceVM>($"api/Cashier/GetInvoiceTotal/{_CheckNo}");
        }

        public async Task<bool> DelInvoiceItems(string _CheckNo, int _Seq)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Cashier/DelInvoiceItems/{_CheckNo}/{_Seq}");
        }

        public async Task<bool> DelInvoice(string _CheckNo)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Cashier/DelInvoice/{_CheckNo}");
        }

        public async Task<IEnumerable<PaymentModeVM>> GetPaymentModeList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PaymentModeVM>>($"api/Cashier/GetPaymentModeList");
        }

        public async Task<bool> SavePayment(PaymentVM _paymentVM, string _POSCode, string _UserID)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Cashier/SavePayment/{_POSCode}/{_UserID}", _paymentVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<IEnumerable<PaymentVM>> GetCustomerAmountSuggest(decimal _AmountPay)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PaymentVM>>($"api/Cashier/GetCustomerAmountSuggest/{_AmountPay}");
        }

        //Invoice
        public async Task<List<InvoiceVM>> GetInvoices(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Cashier/GetInvoices", _filterVM);

            return await response.Content.ReadFromJsonAsync<List<InvoiceVM>>();
        }

        public async Task<bool> ActiveInvoice(InvoiceVM _invoiceVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Cashier/ActiveInvoice", _invoiceVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<VoucherDetailVM>> QI_StockVoucherDetails(string _CheckNo)
        {
            return await _httpClient.GetFromJsonAsync<List<VoucherDetailVM>>($"api/Cashier/QI_StockVoucherDetails/{_CheckNo}");
        }

    }
}
