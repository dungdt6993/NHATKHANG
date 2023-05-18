using Data.Repositories.HR;
using Data.Repositories.SYSTEM;
using Model.ViewModels.FIN;
using Model.ViewModels.HR;
using WebApp.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace WebApp.Pages.HR
{
    partial class Payslip
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        [Inject] SysRepository sysRepo { get; set; }

        [Inject] OrganizationalChartService organizationalChartRepo { get; set; }
        [Inject] DutyRosterService dutyRosterRepo { get; set; }
        [Inject] PayrollService payrollRepo { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingPage;

        //Filter
        FilterHrVM filterHrVM = new();
        IEnumerable<PeriodVM> year_filter_list;
        IEnumerable<PeriodVM> month_filter_list;
        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;
        IEnumerable<SectionVM> section_filter_list;
        IEnumerable<PositionVM> position_filter_list;
        IEnumerable<ProfileVM> eserial_filter_list;

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
            isLoadingPage = true;

            UserID = (await authenticationStateTask).User.GetUserId();

            if (sysRepo.checkPermisFunc(UserID, "HR_DutyRoster"))
            {
                await sysRepo.insert_LogUserFunc(UserID, "HR_DutyRoster");
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

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            division_filter_list = await organizationalChartRepo.GetDivisionList(filterHrVM);
            filterHrVM.DivisionID = division_filter_list.Count() > 0 ? division_filter_list.ElementAt(0).DivisionID : string.Empty;

            filterHrVM.SectionID = string.Empty;
            section_filter_list = await organizationalChartRepo.GetSectionList();

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartRepo.GetDepartmentList(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartRepo.GetPositionList();

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            isLoadingPage = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterHrVM.Month = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            payslipVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            filterHrVM.Year = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            filterHrVM.TrnCode = 0;
            filterHrVM.TrnSubCode = 0;

            payslipVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterHrVM.DivisionID = value;

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartRepo.GetDepartmentList(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            filterHrVM.TrnCode = 0;
            filterHrVM.TrnSubCode = 0;

            payslipVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_department(string value)
        {
            isLoading = true;

            filterHrVM.DepartmentID = value;

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            payslipVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_section(string value)
        {
            isLoading = true;

            filterHrVM.SectionID = value;

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            payslipVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private string[] onchange_filter_positiongroup
        {
            get
            {
                return filterHrVM.arrPositionID;
            }
            set
            {
                isLoading = true;

                filterHrVM.arrPositionID = (string[])value;

                filterHrVM.PositionGroupID = string.Join(",", (string[])value);

                reload_filter_eserial();

                payslipVMs = null;

                isLoading = false;
            }
        }

        private async void reload_filter_eserial()
        {
            filterHrVM.Eserial = String.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            StateHasChanged();
        }

        private async void onchange_filter_eserial(string value)
        {
            isLoading = true;

            filterHrVM.Eserial = value;

            payslipVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async Task onchange_filter_typepayslip(ChangeEventArgs args)
        {
            isLoading = true;

            filterHrVM.isTypeSearch = int.Parse(args.Value.ToString());

            filterHrVM.Eserial = string.Empty;

            payslipVMs = null;

            isLoading = false;
        }

        private async Task GetPayslipList()
        {
            isLoading = true;

            payslipVMs = await payrollRepo.GetPayslipList(filterHrVM);

            ReportName = "CustomNewReport";

            isLoading = false;
        }

        private async void onchange_salaryReply(string value, PayslipVM _payslipVM)
        {
            isLoading = true;

            _payslipVM.SalaryReply = value;

            _payslipVM.UserID = UserID;

            await payrollRepo.UpdateSalaryReply(_payslipVM);

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
