using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Services.FIN;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.EA;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;

namespace D69soft.Client.Pages.EA
{
    partial class Cart
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] InventoryService inventoryService { get; set; }
        [Inject] RequestService requestService { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingScreen = true;

        LogVM logVM = new();

        //Filter
        FilterHrVM filterHrVM = new();
        FilterFinVM filterFinVM = new();

        //Items
        List<ItemsVM> itemsVMs;

        List<ItemsVM> search_itemsVMs;

        //Cart
        IEnumerable<CartVM> cartVMs;

        //Request
        RequestVM requestVM = new();

        IEnumerable<DepartmentVM> departments;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
            await js.InvokeAsync<object>("tooltip");
        }

        protected override async Task OnInitializedAsync()
        {
            UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "EA_Request"))
            {
                logVM.LogUser = UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "EA_Request";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            filterHrVM.DivisionID = "Bhaya";
            filterHrVM.UserID = String.Empty;
            departments = await organizationalChartService.GetDepartmentList(filterHrVM);

            cartVMs = await requestService.GetCarts(UserID);

            filterFinVM.IActive = true;

            search_itemsVMs = itemsVMs = await inventoryService.GetItemsList(filterFinVM);

            isLoadingScreen = false;
        }

        private string onchange_SearchValues
        {
            get { return filterFinVM.searchText; }
            set
            {
                filterFinVM.searchText = value;
                search_itemsVMs = itemsVMs.Where(x => x.IName.ToUpper().Contains(filterFinVM.searchText.ToUpper())).ToList();
            }
        }

        private async Task UpdateItemsCart(ItemsVM _itemsVM)
        {
            isLoading = true;

            await requestService.UpdateItemsCart(_itemsVM, UserID);

            cartVMs = await requestService.GetCarts(UserID);

            await js.Toast_Alert("Thêm giỏ hàng thành công!", SweetAlertMessageType.success);

            isLoading = false;
        }

        private async Task DelItemsCart(CartVM _cartVM)
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
            {
                isLoading = true;

                await requestService.DelItemsCart(_cartVM);

                cartVMs = await requestService.GetCarts(UserID);

                await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);

                isLoading = false;
            }
        }

        private async void onchange_QtyItemsCart(ChangeEventArgs e, CartVM _cartVM)
        {
            _cartVM.Qty = float.Parse(e.Value.ToString());

            _cartVM.UserID = UserID;

            await requestService.UpdateQtyItemsCart(_cartVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            StateHasChanged();
        }

        private async void onchange_NoteItemsCart(ChangeEventArgs e, CartVM _cartVM)
        {
            _cartVM.Note = e.Value.ToString();

            _cartVM.UserID = UserID;

            await requestService.UpdateNoteItemsCart(_cartVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            StateHasChanged();
        }

        //Send Request
        private async Task InitializeModal_Request()
        {
            isLoading = true;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModal_Request");

            isLoading = false;
        }

        private async Task SendRequest()
        {
            isLoading = true;

            await requestService.SendRequest(requestVM, UserID);

            logVM.LogDesc = "Gửi yêu cầu cấp hàng thành công!";
            await sysService.InsertLog(logVM);

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_Request");
            await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);

            navigationManager.NavigateTo("/EA/Request", true);

            isLoading = false;
        }

    }
}
