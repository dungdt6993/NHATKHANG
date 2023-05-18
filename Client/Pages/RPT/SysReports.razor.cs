using Data.Repositories.SYSTEM;
using Model.ViewModels.SYSTEM;
using WebApp.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Utilities;

namespace WebApp.Pages.RPT
{
    partial class SysReports
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        [Inject] SysRepository sysRepo { get; set; }
        [Inject] UserRepository userRepo { get; set; }

        [Inject] IWebHostEnvironment env { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingPage;

        [Parameter] public string ReportName { get; set; }

        bool IsEditRpt = false;

        FilterRptVM filterRptVM = new();
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
            isLoadingPage = true;

            UserID = (await authenticationStateTask).User.GetUserId();

            if (sysRepo.CkPermisSysRpt(UserID))
            {
                await sysRepo.insert_LogUserFunc(UserID, "SysRpt");
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            modules = await sysRepo.GetModule(UserID);
            filterRptVM.ModuleID = modules.First().ModuleID;

            rptgrps = await sysRepo.GetSysReportGroupByID(filterRptVM.ModuleID, UserID);

            rpts = await sysRepo.GetSysReportList(filterRptVM.ModuleID, filterRptVM.RptGrpID, UserID);

            isLoadingPage = false;
        }

        private async void onchange_filter_module(string value)
        {
            isLoading = true;

            filterRptVM.ModuleID = value;

            filterRptVM.RptGrpID = 0;

            filterRptVM.RptID = 0;

            rptgrps = await sysRepo.GetSysReportGroupByID(filterRptVM.ModuleID, UserID);

            rpts = await sysRepo.GetSysReportList(filterRptVM.ModuleID, filterRptVM.RptGrpID, UserID);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_rptgrp(int value)
        {
            isLoading = true;

            filterRptVM.RptGrpID = value;

            filterRptVM.RptID = 0;

            rpts = await sysRepo.GetSysReportList(filterRptVM.ModuleID, filterRptVM.RptGrpID, UserID);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_rpt(int value)
        {
            isLoading = true;

            rptVM = value == 0 ? new() : await sysRepo.GetSysReport(value);

            if(rptVM.PassUserID)
            {
                ReportName = rptVM.RptUrl + "?UserID=" + UserID + "";
            }
            else
            {
                ReportName = rptVM.RptUrl;
            }

            filterRptVM.RptID = value;

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

            string ReportDirectory = Path.Combine(env.ContentRootPath, $"{UrlDirectory.Reports}");
            String[] FileExtension = { "repx" };
            rptUrls = Utilities.LibraryFunc.GetFilesNameFrom(ReportDirectory, FileExtension, false);

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
                rptVM.UserID = UserID;
                await sysRepo.UpdateRpt(rptVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModal_Rpt");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

                ReportName = rptVM.RptUrl;

                rpts = await sysRepo.GetSysReportList(filterRptVM.ModuleID, filterRptVM.RptGrpID, UserID);


                onchange_filter_rpt(rptVM.RptID);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn muốn xóa?", SweetAlertMessageType.question))
                {
                    await sysRepo.DelRpt(rptVM.RptID);

                    string ReportFilePath = Path.Combine(env.ContentRootPath, $"{UrlDirectory.Reports}/" + rptVM.RptUrl + ".repx");

                    Utilities.LibraryFunc.DelFileFrom(ReportFilePath);

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModal_Rpt");
                    await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);

                    ReportName = null;

                    filterRptVM.RptID = 0;
                    rpts = await sysRepo.GetSysReportList(filterRptVM.ModuleID, filterRptVM.RptGrpID, UserID);
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
                rptGrpVM = await sysRepo.GetSysReportGroup(filterRptVM.RptGrpID);
                rptGrpVM.IsTypeUpdate = 1;
            }
            rptGrpVM.ModuleID = filterRptVM.ModuleID;

            isLoading = false;
        }

        private async Task UpdateGrtRpt()
        {
            isLoading = true;

            if (rptGrpVM.IsTypeUpdate != 2)
            {
                await sysRepo.UpdateGrtRpt(rptGrpVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModal_GrtRpt");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

                filterRptVM.RptID = 0;
                rptgrps = await sysRepo.GetSysReportGroupByID(filterRptVM.ModuleID, UserID);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn muốn xóa?", SweetAlertMessageType.question))
                {
                    await sysRepo.DelGrtRpt(rptGrpVM.RptGrpID);

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModal_GrtRpt");
                    await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);

                    filterRptVM.RptGrpID = 0;
                    rptgrps = await sysRepo.GetSysReportGroupByID(filterRptVM.ModuleID, UserID);
                    rpts = await sysRepo.GetSysReportList(filterRptVM.ModuleID, filterRptVM.RptGrpID, UserID);
                }
            }

            isLoading = false;
        }
    }
}
