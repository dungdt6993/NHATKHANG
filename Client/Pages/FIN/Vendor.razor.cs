﻿using D69soft.Client.Extensions;
using D69soft.Client.Services.FIN;
using D69soft.Client.Services.HR;
using D69soft.Client.Services;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


namespace D69soft.Client.Pages.FIN
{
    partial class Vendor
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] PurchasingService purchasingService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        protected string UserID;

        LogVM logVM = new();

        //Vendor
        VendorVM vendorVM = new();
        List<VendorVM> vendorVMs;

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

            if (await sysService.CheckAccessFunc(UserID, "FIN_Vendor"))
            {
                logVM.LogUser = UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "FIN_Vendor";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            await GetVendors();

            isLoadingScreen = false;
        }

        private async Task GetVendors()
        {
            isLoading = true;

            vendorVM = new();

            vendorVMs = (await purchasingService.GetVendorList()).ToList();

            isLoading = false;
        }

        private void onclick_Selected(VendorVM _vendorVM)
        {
            vendorVM = _vendorVM == vendorVM ? new() : _vendorVM;
        }

        private string SetSelected(VendorVM _vendorVM)
        {
            if (vendorVM.VendorCode != _vendorVM.VendorCode)
            {
                return string.Empty;
            }
            return "selected";
        }

        private async Task InitializeModalUpdate_Vendor(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                vendorVM = new();
            }

            vendorVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Vendor");

            isLoading = false;
        }

        private async Task UpdateVendor(EditContext _formVendorVM, int _IsTypeUpdate)
        {
            vendorVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formVendorVM.Validate()) return;
            isLoading = true;

            if (vendorVM.IsTypeUpdate != 2)
            {
                vendorVM.VendorCode = await purchasingService.UpdateVendor(vendorVM);

                logVM.LogDesc = (vendorVM.IsTypeUpdate == 0? "Thêm mới": "Cập nhật") + " nhà cung cấp " + vendorVM.VendorCode + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                vendorVM.IsTypeUpdate = 1;
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    await purchasingService.UpdateVendor(vendorVM);

                    logVM.LogDesc = "Xóa nhà cung cấp " + vendorVM.VendorCode + "";
                    await sysService.InsertLog(logVM);

                    await GetVendors();

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Vendor");
                    await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                }
                else
                {
                    vendorVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

    }
}
