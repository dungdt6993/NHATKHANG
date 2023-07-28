﻿using D69soft.Client.Extensions;
using D69soft.Client.Services;
using D69soft.Client.Services.FIN;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.Entities.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Model.ViewModels.OP;

namespace D69soft.Client.Pages.FIN
{
    partial class Stock
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

        //Stock
        StockVM stockVM = new();
        List<StockVM> stockVMs;

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

            if (await sysService.CheckAccessFunc(UserID, "FIN_Stock"))
            {
                logVM.LogUser = UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "FIN_Stock";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            await GetStocks();

            isLoadingScreen = false;
        }

        private async Task GetStocks()
        {
            isLoading = true;

            stockVM = new();

            filterFinVM.searchText = String.Empty;
            stockVMs = (await inventoryService.GetStockList()).ToList();

            isLoading = false;
        }

        private void onclick_Selected(StockVM _stockVM)
        {
            stockVM = _stockVM == stockVM ? new() : _stockVM;
        }

        private string SetSelected(StockVM _stockVM)
        {
            if (stockVM.StockCode != _stockVM.StockCode)
            {
                return string.Empty;
            }
            return "selected";
        }

        private async Task InitializeModalUpdate_Stock(int _IsTypeUpdate)
        {
            isLoading = true;

            stockVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Stock");

            isLoading = false;
        }

        private async Task UpdateStock(EditContext _formStockVM, int _IsTypeUpdate)
        {
            stockVM.IsTypeUpdate = _IsTypeUpdate;
            if (!_formStockVM.Validate()) return;

            isLoading = true;

            if (stockVM.IsTypeUpdate != 2)
            {
                await inventoryService.UpdateStock(stockVM);

                logVM.LogDesc = (stockVM.IsTypeUpdate == 0 ? "Thêm mới" : "Cập nhật") + " kho " + stockVM.StockCode + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                stockVM.IsTypeUpdate = 1;
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await inventoryService.UpdateStock(stockVM);

                    if (affectedRows > 0)
                    {
                        logVM.LogDesc = "Xóa kho " + stockVM.StockCode + "";
                        await sysService.InsertLog(logVM);

                        await GetStocks();

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Stock");
                        await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        stockVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    stockVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

    }
}