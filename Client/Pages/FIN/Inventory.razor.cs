using BlazorDateRangePicker;
using Blazored.Typeahead;
using Data.Repositories.FIN;
using Data.Repositories.HR;
using Data.Repositories.SYSTEM;
using Model.ViewModels.FIN;
using Model.ViewModels.HR;
using Utilities;
using WebApp.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace WebApp.Pages.FIN
{
    partial class Inventory
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [Inject] SysRepository sysRepo { get; set; }

        [Inject] OrganizationalChartService organizationalChartRepo { get; set; }
        [Inject] InventoryService inventoryRepo { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingPage;

        //Filter
        FilterFinVM filterFinVM = new();
        FilterHrVM filterHrVM = new();

        //Division
        IEnumerable<DivisionVM> divisionVMs;

        //Stock
        IEnumerable<StockVM> stockVMs;
        IEnumerable<StockTypeVM> stockTypeVMs;

        //Items
        ItemsVM itemsVM = null;

        //Inventory
        InventoryVM inventoryVM = new();
        List<InventoryVM> inventoryVMs;

        //InventoryBookDetailVM
        List<InventoryBookDetailVM> inventoryBookDetailVMs;

        private BlazoredTypeahead<StockVoucherDetailVM, StockVoucherDetailVM> txtSearchItems;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
        }

        protected override async Task OnInitializedAsync()
        {
            isLoadingPage = true;

            UserID = (await authenticationStateTask).User.GetUserId();

            if (sysRepo.checkPermisFunc(UserID, "Stock_Inventory"))
            {
                await sysRepo.insert_LogUserFunc(UserID, "Stock_Inventory");
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            filterHrVM.UserID = UserID;

            divisionVMs = await organizationalChartRepo.GetDivisionList(filterHrVM);
            filterFinVM.DivisionID = divisionVMs.Count() > 0 ? divisionVMs.ElementAt(0).DivisionID : string.Empty;

            filterFinVM.StartDate = DateTime.Now;
            filterFinVM.EndDate = DateTime.Now;

            stockTypeVMs = await inventoryRepo.GetStockTypeList();
            filterFinVM.StockTypeCode = stockTypeVMs.ElementAt(0).StockTypeCode;

            stockVMs = await inventoryRepo.GetStockList();

            await GetInventorys();

            isLoadingPage = false;
        }

        public void OnRangeSelect(DateRange _range)
        {
            filterFinVM.StartDate = _range.Start;
            filterFinVM.EndDate = _range.End;
        }

        public string onchange_StockTypeCode
        {
            get
            {
                return filterFinVM.StockTypeCode;
            }
            set
            {
                filterFinVM.StockTypeCode = value;
                filterFinVM.StockCode = String.Empty;
            }
        }

        private async Task<IEnumerable<ItemsVM>> SearchItems(string searchText)
        {
            filterFinVM.searchText = searchText;
            filterFinVM.IActive = true;
            return await inventoryRepo.GetItemsList(filterFinVM);
        }

        private async Task SelectedItem(ItemsVM result)
        {
            if (result != null)
            {
                itemsVM = result;
                filterFinVM.ICode = result.ICode;
            }
            else
            {
                itemsVM = null;
                filterFinVM.ICode = String.Empty;
            }
        }

        private async Task GetInventorys()
        {
            isLoading = true;

            IsViewInventoryBookDetail = false;

            inventoryVMs = await inventoryRepo.GetInventorys(filterFinVM);

            isLoading = false;
        }

        bool IsViewInventoryBookDetail = false;
        private async Task viewInventoryBookDetail(InventoryVM _inventory)
        {
            isLoading = true;

            IsViewInventoryBookDetail = true;

            inventoryVM = _inventory;

            inventoryBookDetailVMs = await inventoryRepo.GetInventoryBookDetails(filterFinVM, inventoryVM);

            isLoading = false;
        }

    }
}
