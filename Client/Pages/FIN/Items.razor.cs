using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Services.FIN;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Utilities;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;
using D69soft.Shared.Models.ViewModels.HR;
using Newtonsoft.Json.Linq;
using Blazored.TextEditor;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Client.Pages.FIN
{
    partial class Items
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] VoucherService voucherService { get; set; }
        [Inject] PurchasingService purchasingService { get; set; }
        [Inject] InventoryService inventoryService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        //ItemsType
        IEnumerable<ItemsTypeVM> itemsTypeVMs;

        //ItemsClass
        ItemsClassVM itemsClassVM = new();
        IEnumerable<ItemsClassVM> itemsClassVMs;

        //ItemsGroup
        ItemsGroupVM itemsGroupVM = new();
        IEnumerable<ItemsGroupVM> itemsGroupVMs;

        //Items
        ItemsVM itemsVM = new();
        List<ItemsVM> itemsVMs;

        BlazoredTextEditor QuillHtml = new BlazoredTextEditor();

        //QuantitativeItems
        ItemsVM qi_itemsVM = new();
        QuantitativeItemsVM quantitativeItemsVM = new();
        List<QuantitativeItemsVM> quantitativeItemsVMs;

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

            if (await sysService.CheckAccessFunc(filterVM.UserID, "FIN_Items"))
            {
                logVM.LogUser = filterVM.UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "FIN_Items";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            filterVM.DivisionID = string.Empty;

            filterVM.IActive = true;

            await GetItems();

            isLoadingScreen = false;
        }

        private async Task GetItems()
        {
            isLoading = true;

            itemsVM = new();

            itemsVMs = await inventoryService.GetItemsList(filterVM);

            isLoading = false;
        }

        private void onclick_Selected(ItemsVM _itemsVM)
        {
            itemsVM = _itemsVM == itemsVM ? new() : _itemsVM;
        }

        private string SetSelected(ItemsVM _itemsVM)
        {
            if (itemsVM.ICode != _itemsVM.ICode)
            {
                return string.Empty;
            }
            return "selected";
        }

        private async void onchange_filter_IClsCode(string value)
        {
            isLoading = true;

            filterVM.IClsCode = value;

            itemsClassVM = String.IsNullOrEmpty(value) ? new() : itemsClassVMs.First(x => x.IClsCode == value);

            filterVM.IGrpCode = String.Empty;

            filterVM.searchText = String.Empty;

            await GetItems();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_IGrpCode(string value)
        {
            isLoading = true;

            filterVM.IGrpCode = value;

            itemsGroupVM = String.IsNullOrEmpty(value) ? new() : itemsGroupVMs.First(x => x.IGrpCode == value);

            filterVM.searchText = String.Empty;

            await GetItems();

            isLoading = false;

            StateHasChanged();
        }

        protected async void FilterItems(ChangeEventArgs args)
        {
            filterVM.searchText = args.Value.ToString();

            await GetItems();

            StateHasChanged();
        }

        private async Task onchange_filter_IActive(ChangeEventArgs args)
        {
            isLoading = true;

            filterVM.IActive = bool.Parse(args.Value.ToString());

            filterVM.searchText = String.Empty;

            await GetItems();

            isLoading = false;
        }

        private async Task InitializeModalUpdate_Items(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                itemsVM = new();
                quantitativeItemsVMs = new();

                itemsVM.IURLPicture1 = "/images/_default/no-image.png";
                itemsVM.IClsCode = filterVM.IClsCode;
                itemsVM.IGrpCode = filterVM.IGrpCode;
                itemsVM.IActive = true;
            }

            if (_IsTypeUpdate == 1)
            {
                if (!String.IsNullOrEmpty(itemsVM.IDetail))
                {
                    await QuillHtml.LoadHTMLContent(itemsVM.IDetail);
                }

                quantitativeItemsVMs = await inventoryService.GetQuantitativeItems(itemsVM.ICode);
            }

            itemsVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Items");

            isLoading = false;
        }

    }
}
