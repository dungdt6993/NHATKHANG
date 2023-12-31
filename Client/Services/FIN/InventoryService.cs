﻿using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Collections;
using System.Net.Http.Json;

namespace D69soft.Client.Services.FIN
{
    public class InventoryService
    {
        private readonly HttpClient _httpClient;

        public InventoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ItemsTypeVM>> GetItemsTypes()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ItemsTypeVM>>($"api/Inventory/GetItemsTypes");
        }

        public async Task<IEnumerable<ItemsClassVM>> GetItemsClassList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ItemsClassVM>>($"api/Inventory/GetItemsClassList");
        }

        public async Task<int> UpdateItemsClass(ItemsClassVM _itemsClassVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/UpdateItemsClass", _itemsClassVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }
        public async Task<bool> ContainsIClsCode(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Inventory/ContainsIClsCode/{id}");
        }

        public async Task<IEnumerable<ItemsGroupVM>> GetItemsGroupList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ItemsGroupVM>>($"api/Inventory/GetItemsGroupList");
        }
        public async Task<int> UpdateItemsGroup(ItemsGroupVM _itemsGroupVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/UpdateItemsGroup", _itemsGroupVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }
        public async Task<bool> ContainsIGrpCode(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Inventory/ContainsIGrpCode/{id}");
        }

        public async Task<IEnumerable<ItemsUnitVM>> GetItemsUnitList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ItemsUnitVM>>($"api/Inventory/GetItemsUnitList");
        }
        public async Task<int> UpdateItemsUnit(ItemsUnitVM _itemsUnitVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/UpdateItemsUnit", _itemsUnitVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }
        public async Task<bool> ContainsIUnitCode(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Inventory/ContainsIUnitCode/{id}");
        }

        public async Task<List<ItemsVM>> GetItemsList(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/GetItemsList", _filterVM);

            return await response.Content.ReadFromJsonAsync<List<ItemsVM>>();
        }

        public async Task<IEnumerable<ItemsVM>> GetItemsForQuantitative(string _ItemSearch, string _ICode)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ItemsVM>>($"api/Inventory/GetItemsForQuantitative/{_ItemSearch}/{_ICode}");
        }

        public async Task<List<QuantitativeItemsVM>> GetQuantitativeItems(string _ICode)
        {
            return await _httpClient.GetFromJsonAsync<List<QuantitativeItemsVM>>($"api/Inventory/GetQuantitativeItems/{_ICode}");
        }

        public async Task<string> UpdateItems(ItemsVM _itemsVM, IEnumerable<QuantitativeItemsVM> _quantitativeItemsVMs)
        {
            ArrayList _arrayList = new ArrayList();
            _arrayList.Add(_itemsVM);
            _arrayList.Add(_quantitativeItemsVMs);

            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/UpdateItems", _arrayList);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> ContainsICode(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Inventory/ContainsICode/{id}");
        }

        //Stock
        public async Task<IEnumerable<StockVM>> GetStockList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<StockVM>>($"api/Inventory/GetStockList");
        }

        public async Task<bool> ContainsStockCode(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Inventory/ContainsStockCode/{id}");
        }

        public async Task<int> UpdateStock(StockVM _stockVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/UpdateStock", _stockVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<List<InventoryVM>> GetInventorys(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/GetInventorys", _filterVM);

            return await response.Content.ReadFromJsonAsync<List<InventoryVM>>();
        }

        public async Task<List<InventoryBookDetailVM>> GetInventoryBookDetails(FilterVM _filterVM, InventoryVM _inventoryVM)
        {
            ArrayList _arrayList = new ArrayList();
            _arrayList.Add(_filterVM);
            _arrayList.Add(_inventoryVM);

            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/GetInventoryBookDetails", _arrayList);

            return await response.Content.ReadFromJsonAsync<List<InventoryBookDetailVM>>();
        }

        public async Task<decimal> GetInventoryCheck_Qty(DateTimeOffset _VDate, VoucherDetailVM _voucherDetailVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/GetInventoryCheck_Qty/{_VDate.ToString("yyyy-MM-dd")}", _voucherDetailVM);

            return await response.Content.ReadFromJsonAsync<decimal>();
        }

    }
}
