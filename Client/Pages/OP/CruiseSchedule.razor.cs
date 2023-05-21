using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Services.HR;
using D69soft.Client.Services.OP;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.OP;
using D69soft.Client.Helpers;
using D69soft.Shared.Models.ViewModels.SYSTEM;

namespace D69soft.Client.Pages.OP
{
    partial class CruiseSchedule
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] OPService opService { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingScreen = true;

        LogVM logVM = new();

        //Filter
        FilterHrVM filterHrVM = new();
        IEnumerable<PeriodVM> year_filter_list;
        IEnumerable<PeriodVM> month_filter_list;
        IEnumerable<DivisionVM> division_filter_list;

        //CruiseSchedule
        CruiseScheduleVM cruiseScheduleVM = new();
        List<CruiseScheduleVM> cruiseScheduleVMs;

        //CruiseStatus
        IEnumerable<CruiseStatusVM> cruiseStatusVMs;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");

            await js.InvokeAsync<object>("maskCurrency");
        }

        protected override async Task OnInitializedAsync()
        {
            

            UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "OP_CruiseSchedule"))
            {
                logVM.LogType = "FUNC";
                logVM.LogName = "OP_CruiseSchedule";
                logVM.LogUser = UserID;
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            filterHrVM.UserID = UserID;

            year_filter_list = await sysService.GetYearFilter();
            filterHrVM.Year = DateTime.Now.Year;

            month_filter_list = await sysService.GetMonthFilter();
            filterHrVM.Month = DateTime.Now.Month;

            division_filter_list = await organizationalChartService.GetDivisionList(filterHrVM);
            filterHrVM.DivisionID = division_filter_list.Count() > 0 ? division_filter_list.ElementAt(0).DivisionID : string.Empty;

            cruiseScheduleVMs = await opService.GetCruiseSchedules(filterHrVM);
            cruiseStatusVMs = await opService.GetCruiseStatus();

            isLoadingScreen = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterHrVM.Month = value;

            await GetCruiseSchedules();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            filterHrVM.Year = value;

            await GetCruiseSchedules();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterHrVM.DivisionID = value;

            await GetCruiseSchedules();

            isLoading = false;

            StateHasChanged();
        }

        private async Task GetCruiseSchedules()
        {
            isLoading = true;

            cruiseScheduleVMs = await opService.GetCruiseSchedules(filterHrVM);

            isLoading = false;
        }

        private async Task InitializeModalUpdate_CruiseSchedule(CruiseScheduleVM _cruiseScheduleVM)
        {
            isLoading = true;

            cruiseScheduleVM = _cruiseScheduleVM;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_CruiseSchedule");

            isLoading = false;
        }

        private async Task UpdateCruiseSchedule()
        {
            isLoading = true;

            await opService.UpdateCruiseSchedule(cruiseScheduleVM);

            await GetCruiseSchedules();

            await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_CruiseSchedule");
            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;
        }

    }
}

