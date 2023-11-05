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

        IEnumerable<RptVM> rpts;
        RptVM rptVM = new();

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

        protected async Task ViewRPT(int value)
        {
            isLoading = true;

            filterVM.RptID = value;

            if(filterVM.RptID != 0)
            {
                rptVM = (await sysService.GetRptList(value, filterVM.UserID)).First();

                if (rptVM.PassUserID)
                {
                    ReportName = rptVM.RptUrl + "?UserID=" + filterVM.UserID + "";
                }
                else
                {
                    ReportName = rptVM.RptUrl;
                }

                await js.InvokeAsync<object>("ShowModal", "#InitializeModalView_Rpt");
            }

            filterVM.RptID = 0;

            isLoading = false;
        }
    }
}
