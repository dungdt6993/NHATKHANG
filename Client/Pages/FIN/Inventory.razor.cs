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

        bool isLoading;
        bool isLoadingScreen = true;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        //Division
        IEnumerable<DivisionVM> divisionVMs;

        //Stock
        IEnumerable<StockVM> stockVMs;

        //Items
        ItemsVM itemsVM = null;

        //Inventory
        InventoryVM inventoryVM = new();
        List<InventoryVM> inventoryVMs;

        //InventoryBookDetailVM
        List<InventoryBookDetailVM> inventoryBookDetailVMs;

        private BlazoredTypeahead<VoucherDetailVM, VoucherDetailVM> txtSearchItems;

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
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(filterVM.UserID, "FIN_Inventory"))
            {
                logVM.LogType = "FUNC";
                logVM.LogName = "FIN_Inventory";
                logVM.LogUser = filterVM.UserID;
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            divisionVMs = await organizationalChartService.GetDivisionList(filterVM);
            filterVM.DivisionID = (await sysService.GetInfoUser(filterVM.UserID)).DivisionID;

            filterVM.StartDate = DateTime.Now;
            filterVM.EndDate = DateTime.Now;

            stockVMs = await inventoryService.GetStockList();

            await GetInventorys();

            isLoadingScreen = false;
        }

        public void OnRangeSelect(DateRange _range)
        {
            filterVM.StartDate = _range.Start;
            filterVM.EndDate = _range.End;
        }

        private async Task<IEnumerable<ItemsVM>> SearchItems(string searchText)
        {
            filterVM.searchText = searchText;
            filterVM.IActive = true;
            return await inventoryService.GetItemsList(filterVM);
        }

        private async Task SelectedItem(ItemsVM result)
        {
            if (result != null)
            {
                itemsVM = result;
                filterVM.ICode = result.ICode;
            }
            else
            {
                itemsVM = null;
                filterVM.ICode = String.Empty;
            }
        }

        private async Task GetInventorys()
        {
            isLoading = true;

            IsViewInventoryBookDetail = false;

            inventoryVMs = await inventoryService.GetInventorys(filterVM);

            isLoading = false;
        }

        bool IsViewInventoryBookDetail = false;
        private async Task viewInventoryBookDetail(InventoryVM _inventory)
        {
            isLoading = true;

            IsViewInventoryBookDetail = true;

            inventoryVM = _inventory;

            inventoryBookDetailVMs = await inventoryService.GetInventoryBookDetails(filterVM, inventoryVM);

            isLoading = false;
        }

    }
}
