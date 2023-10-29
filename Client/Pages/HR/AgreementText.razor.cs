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
    partial class AgreementText
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] DutyRosterService dutyRosterService { get; set; }
        [Inject] AgreementTextService agreementTextService { get; set; }

        bool isLoading;

        bool isLoadingScreen = true;

        //Log
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
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(filterVM.UserID, "HR_AgreementText"))
            {
                logVM.LogType = "FUNC";
                logVM.LogName = "HR_AgreementText";
                logVM.LogUser = filterVM.UserID;
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

            agreementtexttype_filter_list = await agreementTextService.GetAgreementTextTypeList();

            isLoadingScreen = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            await dutyRosterService.InitializeAttendanceRecordDutyRoster(filterVM);

            filterVM.Month = value;

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);
            agreementTextVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            await dutyRosterService.InitializeAttendanceRecordDutyRoster(filterVM);

            filterVM.Year = value;

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);
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

            filterVM.DivisionID = value;

            filterVM.is2625 = divisionFilterVM.is2625;

            filterVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterVM);

            filterVM.PositionGroupID = string.Empty;
            filterVM.arrPositionID = new string[] { };

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            agreementTextVMs = null;

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

            agreementTextVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_section(string value)
        {
            isLoading = true;

            filterVM.SectionID = value;

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            agreementTextVMs = null;

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

                agreementTextVMs = null;

                isLoading = false;
            }
        }
        private void onchange_filter_eserial(string value)
        {
            isLoading = true;

            filterVM.Eserial = value;

            agreementTextVMs = null;

            isLoading = false;

            StateHasChanged();
        }
        private async void reload_filter_eserial()
        {
            filterVM.Eserial = String.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            StateHasChanged();
        }

        private void onchange_filter_agreementtext(int value)
        {
            isLoading = true;

            filterVM.RptID = value;

            agreementTextVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async Task GetAgreementTextList()
        {
            isLoading = true;

            agreementTextVMs = await agreementTextService.GetAgreementTextList(filterVM);

            isLoading = false;
        }

        private void CheckAll(object checkValue)
        {
            bool isChecked = (bool)checkValue;
            filterVM.IsChecked = isChecked;
            agreementTextVMs.ToList().ForEach(e => e.IsChecked = isChecked);
        }

        private async Task PrintAgreementText()
        {
            IEnumerable<SysRptVM> sysRptVMs = await agreementTextService.PrintAgreementText(agreementTextVMs.Where(x => x.IsChecked == true), filterVM.UserID);

            foreach (var sysReport in sysRptVMs)
            {
                await js.InvokeAsync<object>("openInNewTab", "/SYS/RptViewer/" + sysReport.RptUrl + "");
            }

            filterVM.IsChecked = false;

            await GetAgreementTextList();
        }

        private async Task InitializeModal_AgreementText()
        {
            isLoading = true;

            adjustProfileVMs = await agreementTextService.GetAdjustProfileList();
            adjustProfileRptVMs = await agreementTextService.GetAdjustProfileRptList();

            isLoading = false;
        }

        private async Task InitializeModal_AdjustProfileRpt(AdjustProfileVM _adjustProfileVM)
        {
            isLoading = true;

            adjustProfileVM = new();

            adjustProfileVM = _adjustProfileVM;

            sysRptVMs = await sysService.GetRptList(0, filterVM.UserID);

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

            await agreementTextService.UpdateAdjustProfile(adjustProfileVM);

            await InitializeModal_AgreementText();

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_AdjustProfileRpt");
            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;
        }
    }
}
