using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;

namespace D69soft.Client.Pages.HR
{
    partial class Payslip
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] DutyRosterService dutyRosterService { get; set; }
        [Inject] PayrollService payrollService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        //log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        IEnumerable<PeriodVM> year_filter_list;
        IEnumerable<PeriodVM> month_filter_list;
        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;
        IEnumerable<SectionVM> section_filter_list;
        IEnumerable<PositionVM> position_filter_list;
        IEnumerable<EserialVM> eserial_filter_list;

        //Payslip
        List<PayslipVM> payslipVMs;
        PayslipVM payslipVM = new();

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

            if (await sysService.CheckAccessFunc(filterVM.UserID, "HR_Payslip"))
            {
                logVM.LogUser = filterVM.UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "HR_Payslip";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            year_filter_list = await sysService.GetYearFilter();
            filterVM.Year = DateTime.Now.Year;

            month_filter_list = await sysService.GetMonthFilter();
            filterVM.Month = DateTime.Now.Month;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            division_filter_list = await organizationalChartService.GetDivisionList(filterVM);
            filterVM.DivisionID = (await sysService.GetInfoUser(filterVM.UserID)).DivisionID;

            filterVM.SectionID = string.Empty;
            section_filter_list = await organizationalChartService.GetSectionList();

            filterVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterVM);

            filterVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartService.GetPositionList();

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            isLoadingScreen = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterVM.Month = value;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            payslipVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            filterVM.Year = value;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            filterVM.TrnCode = 0;
            filterVM.TrnSubCode = 0;

            payslipVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterVM.DivisionID = value;

            filterVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterVM);

            filterVM.PositionGroupID = string.Empty;
            filterVM.arrPositionID = new string[] { };

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            filterVM.TrnCode = 0;
            filterVM.TrnSubCode = 0;

            payslipVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_department(string value)
        {
            isLoading = true;

            filterVM.DepartmentID = value;

            filterVM.PositionGroupID = string.Empty;
            filterVM.arrPositionID = new string[] { };

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            payslipVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_section(string value)
        {
            isLoading = true;

            filterVM.SectionID = value;

            filterVM.PositionGroupID = string.Empty;
            filterVM.arrPositionID = new string[] { };

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            payslipVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private string[] onchange_filter_positiongroup
        {
            get
            {
                return filterVM.arrPositionID;
            }
            set
            {
                isLoading = true;

                filterVM.arrPositionID = (string[])value;

                filterVM.PositionGroupID = string.Join(",", (string[])value);

                reload_filter_eserial();

                payslipVMs = null;

                isLoading = false;
            }
        }

        private async void reload_filter_eserial()
        {
            filterVM.Eserial = String.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            StateHasChanged();
        }

        private async void onchange_filter_eserial(string value)
        {
            isLoading = true;

            filterVM.Eserial = value;

            payslipVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async Task onchange_filter_typepayslip(ChangeEventArgs args)
        {
            isLoading = true;

            filterVM.IsTypeSearch = int.Parse(args.Value.ToString());

            filterVM.Eserial = string.Empty;

            payslipVMs = null;

            isLoading = false;
        }

        private async Task GetPayslipList()
        {
            isLoading = true;

            payslipVMs = await payrollService.GetPayslipList(filterVM);

            ReportName = "CustomNewReport";

            isLoading = false;
        }

        private async void onchange_salaryReply(string value, PayslipVM _payslipVM)
        {
            isLoading = true;

            _payslipVM.SalaryReply = value;

            _payslipVM.UserID = filterVM.UserID;

            await payrollService.UpdateSalaryReply(_payslipVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        string ReportName = String.Empty;
        private async Task viewPayslip(string _id)
        {
            ReportName = "Payslip?ID=" + _id + "";
            await js.InvokeAsync<object>("ShowModal", "#InitializeModalView_Rpt");
        }

    }
}
