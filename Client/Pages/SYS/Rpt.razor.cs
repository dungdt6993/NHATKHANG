using D69soft.Client.Extensions;
using D69soft.Client.Services;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Shared.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace D69soft.Client.Pages.SYS
{
    partial class Rpt
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] AuthService authService { get; set; }

        bool isLoading;
        bool isLoadingScreen;

        //log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        [Parameter] public string ReportName { get; set; }

        bool IsEditRpt = false;

        List<SysRptVM> modules = new();
        List<SysRptVM> rptgrps = new();

        IEnumerable<SysRptVM> rpts;

        RptGrpVM rptGrpVM = new();
        RptVM rptVM = new();
        String[] rptUrls;

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

            if (await sysService.CheckPermisRpt(filterVM.UserID))
            {
                logVM.LogType = "FUNC";
                logVM.LogName = "RPT";
                logVM.LogUser = filterVM.UserID;
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            filterVM.isLeader = (await sysService.GetInfoUser(filterVM.UserID)).isLeader;

            modules = await sysService.GetModuleRpt(filterVM.UserID);
            filterVM.ModuleID = modules.First().ModuleID;

            rptgrps = await sysService.GetRptGrpByID(filterVM.ModuleID, filterVM.UserID);

            rpts = await sysService.GetRptList(filterVM.ModuleID, filterVM.RptGrpID, filterVM.UserID);

            isLoadingScreen = false;
        }

        private async void onchange_filter_module(string value)
        {
            isLoading = true;

            filterVM.ModuleID = value;

            filterVM.RptGrpID = 0;

            filterVM.RptID = 0;

            rptgrps = await sysService.GetRptGrpByID(filterVM.ModuleID, filterVM.UserID);

            rpts = await sysService.GetRptList(filterVM.ModuleID, filterVM.RptGrpID, filterVM.UserID);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_rptgrp(int value)
        {
            isLoading = true;

            filterVM.RptGrpID = value;

            filterVM.RptID = 0;

            rpts = await sysService.GetRptList(filterVM.ModuleID, filterVM.RptGrpID, filterVM.UserID);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_rpt(int value)
        {
            isLoading = true;

            rptVM = value == 0 ? new() : await sysService.GetRpt(value);

            if(rptVM.PassUserID)
            {
                ReportName = rptVM.RptUrl + "?UserID=" + filterVM.UserID + "";
            }
            else
            {
                ReportName = rptVM.RptUrl;
            }

            filterVM.RptID = value;

            isLoading = false;

            StateHasChanged();
        }

        private void EditReport()
        {
            IsEditRpt = IsEditRpt ? false : true;

            ReportName = ReportName == null ? "CustomNewReport" : ReportName;
        }

        private void InitializeModal_Rpt(int typeUpdate)
        {
            isLoading = true;

            string ReportDirectory = Path.Combine(Directory.GetCurrentDirectory(), $"{UrlDirectory.Reports}");
            String[] FileExtension = { "repx" };
            rptUrls = LibraryFunc.GetFilesNameFrom(ReportDirectory, FileExtension, false);

            if (typeUpdate == 0)
            {
                rptVM = new();
                rptVM.RptGrpID = rptgrps.First().RptGrpID;
                rptVM.RptUrl = rptUrls.First();
                rptVM.IsTypeUpdate = 0;
            }
            else
            {
                rptVM.IsTypeUpdate = 1;
            }

            isLoading = false;
        }

        private async Task UpdateRpt()
        {
            isLoading = true;

            if (rptVM.IsTypeUpdate != 2)
            {
                rptVM.UserID = filterVM.UserID;
                await sysService.UpdateRpt(rptVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModal_Rpt");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

                ReportName = rptVM.RptUrl;

                rpts = await sysService.GetRptList(filterVM.ModuleID, filterVM.RptGrpID, filterVM.UserID);


                onchange_filter_rpt(rptVM.RptID);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn muốn xóa?", SweetAlertMessageType.question))
                {
                    await sysService.DelRpt(rptVM.RptID);

                    string ReportFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{UrlDirectory.Reports}/" + rptVM.RptUrl + ".repx");

                    LibraryFunc.DelFileFrom(ReportFilePath);

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModal_Rpt");
                    await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);

                    ReportName = null;

                    filterVM.RptID = 0;
                    rpts = await sysService.GetRptList(filterVM.ModuleID, filterVM.RptGrpID, filterVM.UserID);
                }
            }

            isLoading = false;
        }

        private async Task InitializeModal_GrtRpt(int typeUpdate)
        {
            isLoading = true;

            rptGrpVM = new();

            if (typeUpdate == 0)
            {
                rptGrpVM.IsTypeUpdate = 0;
            }
            else
            {
                rptGrpVM = await sysService.GetRptGrp(filterVM.RptGrpID);
                rptGrpVM.IsTypeUpdate = 1;
            }
            rptGrpVM.ModuleID = filterVM.ModuleID;

            isLoading = false;
        }

        private async Task UpdateGrtRpt()
        {
            isLoading = true;

            if (rptGrpVM.IsTypeUpdate != 2)
            {
                await sysService.UpdateRptGrp(rptGrpVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModal_GrtRpt");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

                filterVM.RptID = 0;
                rptgrps = await sysService.GetRptGrpByID(filterVM.ModuleID, filterVM.UserID);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn muốn xóa?", SweetAlertMessageType.question))
                {
                    await sysService.DelRptGrp(rptGrpVM.RptGrpID);

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModal_GrtRpt");
                    await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);

                    filterVM.RptGrpID = 0;
                    rptgrps = await sysService.GetRptGrpByID(filterVM.ModuleID, filterVM.UserID);
                    rpts = await sysService.GetRptList(filterVM.ModuleID, filterVM.RptGrpID, filterVM.UserID);
                }
            }

            isLoading = false;
        }
    }
}
