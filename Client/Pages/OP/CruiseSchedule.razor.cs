using Data.Repositories.HR;
using Data.Repositories.SYSTEM;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Model.ViewModels.FIN;
using Model.ViewModels.HR;
using WebApp.Helpers;
using System.Data;
using Data.Repositories.OP;
using Model.ViewModels.OP;

namespace WebApp.Pages.OP
{
    partial class CruiseSchedule
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        [Inject] SysRepository sysRepo { get; set; }

        [Inject] OrganizationalChartService organizationalChartRepo { get; set; }
        [Inject] OPService opRepo { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingPage;

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
            isLoadingPage = true;

            UserID = (await authenticationStateTask).User.GetUserId();

            if (sysRepo.checkPermisFunc(UserID, "OP_CruiseSchedule"))
            {
                await sysRepo.insert_LogUserFunc(UserID, "OP_CruiseSchedule");
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            filterHrVM.UserID = UserID;

            year_filter_list = await sysRepo.GetYearFilter();
            filterHrVM.Year = DateTime.Now.Year;

            month_filter_list = await sysRepo.GetMonthFilter();
            filterHrVM.Month = DateTime.Now.Month;

            division_filter_list = await organizationalChartRepo.GetDivisionList(filterHrVM);
            filterHrVM.DivisionID = division_filter_list.Count() > 0 ? division_filter_list.ElementAt(0).DivisionID : string.Empty;

            cruiseScheduleVMs = await opRepo.GetCruiseSchedules(filterHrVM);
            cruiseStatusVMs = await opRepo.GetCruiseStatus();

            isLoadingPage = false;
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

            cruiseScheduleVMs = await opRepo.GetCruiseSchedules(filterHrVM);

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

            await opRepo.UpdateCruiseSchedule(cruiseScheduleVM);

            await GetCruiseSchedules();

            await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_CruiseSchedule");
            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;
        }

    }
}

