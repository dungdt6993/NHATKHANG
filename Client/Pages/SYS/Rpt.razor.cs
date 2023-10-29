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

        IEnumerable<SysRptVM> rpts;
        SysRptVM rptVM = new();
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

            rpts = await sysService.GetRptList(filterVM.RptID, filterVM.UserID);

            isLoadingScreen = false;
        }

        private async void onchange_filter_module(string value)
        {
            isLoading = true;

            filterVM.ModuleID = value;

            filterVM.RptGrpID = 0;

            filterVM.RptID = 0;

            rpts = await sysService.GetRptList(filterVM.RptID, filterVM.UserID);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_rptgrp(int value)
        {
            isLoading = true;

            filterVM.RptGrpID = value;

            filterVM.RptID = 0;

            rpts = await sysService.GetRptList(filterVM.RptID, filterVM.UserID);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_rpt(int value)
        {
            isLoading = true;

            rptVM = value == 0 ? new() : (await sysService.GetRptList(value, filterVM.UserID)).FirstOrDefault();

            if (rptVM.PassUserID)
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
    }
}
