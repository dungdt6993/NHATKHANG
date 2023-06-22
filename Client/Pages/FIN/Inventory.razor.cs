using BlazorDateRangePicker;
using Blazored.Typeahead;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Services.HR;
using D69soft.Client.Services.FIN;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;

namespace D69soft.Client.Pages.FIN
{
    partial class Inventory
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] InventoryService inventoryService { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingScreen = true;

        LogVM logVM = new();

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
            

            UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "Stock_Inventory"))
            {
                logVM.LogType = "FUNC";
                logVM.LogName = "Stock_Inventory";
                logVM.LogUser = UserID;
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            filterHrVM.UserID = UserID;

            divisionVMs = await organizationalChartService.GetDivisionList(filterHrVM);
            filterFinVM.DivisionID = divisionVMs.Count() > 0 ? divisionVMs.ElementAt(0).DivisionID : string.Empty;

            filterFinVM.StartDate = DateTime.Now;
            filterFinVM.EndDate = DateTime.Now;

            stockTypeVMs = await inventoryService.GetStockTypeList();
            filterFinVM.StockTypeCode = stockTypeVMs.ElementAt(0).StockTypeCode;

            stockVMs = await inventoryService.GetStockList();

            await GetInventorys();

            isLoadingScreen = false;
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
            return await inventoryService.GetItemsList(filterFinVM);
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

            inventoryVMs = await inventoryService.GetInventorys(filterFinVM);

            isLoading = false;
        }

        bool IsViewInventoryBookDetail = false;
        private async Task viewInventoryBookDetail(InventoryVM _inventory)
        {
            isLoading = true;

            IsViewInventoryBookDetail = true;

            inventoryVM = _inventory;

            inventoryBookDetailVMs = await inventoryService.GetInventoryBookDetails(filterFinVM, inventoryVM);

            isLoading = false;
        }

    }
}
