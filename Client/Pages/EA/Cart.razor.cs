using Data.Repositories.FIN;
using Data.Repositories.HR;
using Data.Repositories.SYSTEM;
using Model.Entities.HR;
using Model.ViewModels.EA;
using Model.ViewModels.FIN;
using Model.ViewModels.HR;
using Model.ViewModels.POS;
using WebApp.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace WebApp.Pages.EA
{
    partial class Cart
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [Inject] SysRepository sysRepo { get; set; }

        [Inject] InventoryService inventoryRepo { get; set; }
        [Inject] RequestService requestRepo { get; set; }

        [Inject] OrganizationalChartService organizationalChartRepo { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingPage;

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
            isLoadingPage = true;

            UserID = (await authenticationStateTask).User.GetUserId();

            if (sysRepo.checkPermisFunc(UserID, "EA_Request"))
            {
                await sysRepo.insert_LogUserFunc(UserID, "EA_Request");
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            filterHrVM.DivisionID = "Bhaya";
            filterHrVM.UserID = String.Empty;
            departments = await organizationalChartRepo.GetDepartmentList(filterHrVM);

            cartVMs = await requestRepo.GetCarts(UserID);

            filterFinVM.IActive = true;

            search_itemsVMs = itemsVMs = await inventoryRepo.GetItemsList(filterFinVM);

            isLoadingPage = false;
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

            await requestRepo.UpdateItemsCart(_itemsVM, UserID);

            cartVMs = await requestRepo.GetCarts(UserID);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;
        }

        private async Task DelItemsCart(CartVM _cartVM)
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
            {
                isLoading = true;

                await requestRepo.DelItemsCart(_cartVM);

                cartVMs = await requestRepo.GetCarts(UserID);

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

                isLoading = false;
            }
        }

        private async void onchange_QtyItemsCart(ChangeEventArgs e, CartVM _cartVM)
        {
            _cartVM.Qty = float.Parse(e.Value.ToString());

            _cartVM.UserID = UserID;

            await requestRepo.UpdateQtyItemsCart(_cartVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            StateHasChanged();
        }

        private async void onchange_NoteItemsCart(ChangeEventArgs e, CartVM _cartVM)
        {
            _cartVM.Note = e.Value.ToString();

            _cartVM.UserID = UserID;

            await requestRepo.UpdateNoteItemsCart(_cartVM);

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

            await requestRepo.SendRequest(requestVM, UserID);

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_Request");

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            navigationManager.NavigateTo("/EA/Request", true);

            isLoading = false;
        }

    }
}
