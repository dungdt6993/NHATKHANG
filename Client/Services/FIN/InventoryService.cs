using D69soft.Shared.Models.ViewModels.DOC;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Net.Http.Json;
using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Data.Repositories.FIN
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

        public async Task<IEnumerable<ItemsGroupVM>> GetItemsGroupList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ItemsGroupVM>>($"api/Inventory/GetItemsGroupList");
        }
        public async Task<bool> UpdateItemsGroup(ItemsGroupVM _itemsGroupVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/UpdateItemsGroup", _itemsGroupVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }
        public async Task<bool> ContainsIGrpCode(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Inventory/ContainsIGrpCode/{id}");
        }

        public async Task<IEnumerable<ItemsUnitVM>> GetItemsUnitList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ItemsUnitVM>>($"api/Inventory/GetItemsUnitList");
        }
        public async Task<bool> UpdateItemsUnit(ItemsUnitVM _itemsUnitVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/UpdateItemsUnit", _itemsUnitVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }
        public async Task<bool> ContainsIUnitCode(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Inventory/ContainsIUnitCode/{id}");
        }

        public async Task<List<ItemsVM>> GetItemsList(FilterFinVM _filterFinVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Document/GetItemsList", _filterFinVM);

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
            var response = await _httpClient.PostAsJsonAsync($"api/Document/UpdateItems/{_quantitativeItemsVMs}", _itemsVM);

            return await response.Content.ReadFromJsonAsync<string>();
        }

        public async Task<bool> UpdateUrlImg(string _ICode, string _UrlImg)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Inventory/UpdateUrlImg/{_ICode}/{_UrlImg}");
        }
        public async Task<bool> ContainsICode(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Inventory/ContainsICode/{id}");
        }

        public async Task<IEnumerable<StockTypeVM>> GetStockTypeList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<StockTypeVM>>($"api/Inventory/GetStockTypeList");
        }

        public async Task<IEnumerable<StockVM>> GetStockList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<StockVM>>($"api/Inventory/GetStockList");
        }

        public async Task<List<InventoryVM>> GetInventorys(FilterFinVM _filterFinVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/GetInventorys", _filterFinVM);

            return await response.Content.ReadFromJsonAsync<List<InventoryVM>>();
        }

        public async Task<List<InventoryBookDetailVM>> GetInventoryBookDetails(FilterFinVM _filterFinVM, InventoryVM _inventoryVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Inventory/GetInventoryBookDetails/{_inventoryVM}", _filterFinVM );

            return await response.Content.ReadFromJsonAsync<List<InventoryBookDetailVM>>();
        }

        public async Task<float> GetInventoryCheck_Qty(DateTimeOffset _VDate, string _StockCode, string _ICode)
        {
            return await _httpClient.GetFromJsonAsync<float>($"api/Inventory/GetInventoryCheck_Qty/{_VDate}/{_StockCode}/{_ICode}");
        }

        //SyncSmile
        public async Task<bool> SyncDataSmile()
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Inventory/SyncDataSmile");
        }

    }
}
