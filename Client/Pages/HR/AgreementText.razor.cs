using Data.Repositories.HR;
using Data.Repositories.SYSTEM;
using Model.ViewModels.FIN;
using Model.ViewModels.HR;
using Model.ViewModels.SYSTEM;
using WebApp.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Model.Entities.HR;

namespace WebApp.Pages.HR
{
    partial class AgreementText
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] NavigationManager navigationManager { get; set; }

        [Inject] SysRepository sysRepo { get; set; }

        [Inject] OrganizationalChartService organizationalChartRepo { get; set; }

        [Inject] DutyRosterService dutyRosterRepo { get; set; }
        [Inject] AgreementTextService agreementTextRepo { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingPage;

        FilterHrVM filterHrVM = new();

        IEnumerable<PeriodVM> year_filter_list;
        IEnumerable<PeriodVM> month_filter_list;
        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;
        IEnumerable<SectionVM> section_filter_list;
        IEnumerable<PositionVM> position_filter_list;
        IEnumerable<ProfileVM> eserial_filter_list;
        IEnumerable<AgreementTextTypeVM> agreementtexttype_filter_list;

        List<AgreementTextVM> agreementTextVMs;

        IEnumerable<AdjustProfileVM> adjustProfileVMs;

        IEnumerable<AdjustProfileRptVM> adjustProfileRptVMs;
        AdjustProfileVM adjustProfileVM = new();

        IEnumerable<SysRptVM> sysRptVMs;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
                await js.InvokeAsync<object>("openInNewTab");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
        }

        protected override async Task OnInitializedAsync()
        {
            isLoadingPage = true;

            UserID = (await authenticationStateTask).User.GetUserId();

            if (sysRepo.checkPermisFunc(UserID, "HR_AgreementText"))
            {
                await sysRepo.insert_LogUserFunc(UserID, "HR_AgreementText");
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

            filterHrVM.SectionID = string.Empty;
            section_filter_list = await organizationalChartRepo.GetSectionList();

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartRepo.GetDepartmentList(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartRepo.GetPositionList();

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            agreementtexttype_filter_list = await agreementTextRepo.GetAgreementTextTypeList();

            isLoadingPage = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            await dutyRosterRepo.InitializeAttendanceRecordDutyRoster(filterHrVM);

            filterHrVM.Month = value;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);
            agreementTextVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            await dutyRosterRepo.InitializeAttendanceRecordDutyRoster(filterHrVM);

            filterHrVM.Year = value;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);
            agreementTextVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            DivisionVM divisionFilterVM = new();

            if (value != String.Empty)
            {
                divisionFilterVM = division_filter_list.First(x => x.DivisionID == value);
            }

            filterHrVM.DivisionID = value;

            filterHrVM.is2625 = divisionFilterVM.is2625;

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartRepo.GetDepartmentList(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            agreementTextVMs = null;

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

            agreementTextVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_section(string value)
        {
            isLoading = true;

            filterHrVM.SectionID = value;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            agreementTextVMs = null;

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

                agreementTextVMs = null;

                isLoading = false;
            }
        }
        private void onchange_filter_eserial(string value)
        {
            isLoading = true;

            filterHrVM.Eserial = value;

            agreementTextVMs = null;

            isLoading = false;

            StateHasChanged();
        }
        private async void reload_filter_eserial()
        {
            filterHrVM.Eserial = String.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            StateHasChanged();
        }

        private void onchange_filter_agreementtext(int value)
        {
            isLoading = true;

            filterHrVM.RptID = value;

            agreementTextVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async Task GetAgreementTextList()
        {
            isLoading = true;

            agreementTextVMs = await agreementTextRepo.GetAgreementTextList(filterHrVM);

            isLoading = false;
        }

        private void CheckAll(object checkValue)
        {
            bool isChecked = (bool)checkValue;
            filterHrVM.IsChecked = isChecked;
            agreementTextVMs.ToList().ForEach(e => e.IsChecked = isChecked);
        }

        private async Task PrintAgreementText()
        {
            IEnumerable<SysRptVM> sysReports = await agreementTextRepo.PrintAgreementText(agreementTextVMs.Where(x => x.IsChecked == true), UserID);

            foreach (var sysReport in sysReports)
            {
                await js.InvokeAsync<object>("openInNewTab", "/RPT/SysReportViewer/" + sysReport.RptUrl + "");
            }

            filterHrVM.IsChecked = false;
            agreementTextVMs = await agreementTextRepo.GetAgreementTextList(filterHrVM);
        }

        private async Task InitializeModal_AgreementText()
        {
            isLoading = true;

            adjustProfileVMs = await agreementTextRepo.GetAdjustProfileList();
            adjustProfileRptVMs = await agreementTextRepo.GetAdjustProfileRptList();

            isLoading = false;
        }

        private async Task InitializeModal_AdjustProfileRpt(AdjustProfileVM _adjustProfileVM)
        {
            isLoading = true;

            adjustProfileVM = new();

            adjustProfileVM = _adjustProfileVM;

            sysRptVMs = await sysRepo.GetSysReportList("HR",0, UserID);

            adjustProfileVM.arrRptID = adjustProfileRptVMs.Where(x => x.AdjustProfileID == _adjustProfileVM.AdjustProfileID).Select(x => x.RptID).ToArray();
            adjustProfileVM.strRpt = string.Join(",", (int[])adjustProfileVM.arrRptID);

            isLoading = false;
        }

        private int[] onchange_rpt
        {
            get
            {
                return adjustProfileVM.arrRptID;
            }
            set
            {
                isLoading = true;

                adjustProfileVM.arrRptID = (int[])value;

                adjustProfileVM.strRpt = string.Join(",", (int[])value);

                isLoading = false;
            }
        }

        private async Task UpdateAdjustProfileRpt()
        {
            isLoading = true;

            await agreementTextRepo.UpdateAdjustProfile(adjustProfileVM);

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_AdjustProfileRpt");
            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            adjustProfileVMs = await agreementTextRepo.GetAdjustProfileList();
            adjustProfileRptVMs = await agreementTextRepo.GetAdjustProfileRptList();

            isLoading = false;
        }

        private async Task CloseAdjustProfileRpt()
        {
            adjustProfileVMs = await agreementTextRepo.GetAdjustProfileList();
        }
    }
}
