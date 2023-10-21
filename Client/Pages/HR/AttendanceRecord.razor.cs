using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Client.Services;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;

namespace D69soft.Client.Pages.HR
{
    partial class AttendanceRecord
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] AuthService authService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] PayrollService payrollService { get; set; }
        [Inject] DutyRosterService dutyRosterService { get; set; }
        [Inject] DayOffService dayOffService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        bool HR_AttendanceRecord_CalcFingerData;
        bool HR_DutyRoster_Update;

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

        //AttendanceRecord
        DutyRosterVM arVM = new();
        IEnumerable<DutyRosterVM> arVMs;

        ProfileVM profileUserVM;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
                await js.InvokeAsync<object>("focusInputNextID");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
        }

        protected override async Task OnInitializedAsync()
        {
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            logVM.LogUser = filterVM.UserID;
            logVM.LogType = "FUNC";
            logVM.LogName = "HR_AttendanceRecord";
            await sysService.InsertLog(logVM);

            filterVM.isLeader = (await sysService.GetInfoUser(filterVM.UserID)).isLeader;

            HR_AttendanceRecord_CalcFingerData = await sysService.CheckAccessSubFunc(filterVM.UserID, "HR_AttendanceRecord_CalcFingerData");
            HR_DutyRoster_Update = await sysService.CheckAccessSubFunc(filterVM.UserID, "HR_DutyRoster_Update");

            //Initialize Filter
            arVM.UserID = filterVM.UserID;

            year_filter_list = await sysService.GetYearFilter();
            filterVM.Year = DateTime.Now.Year;

            month_filter_list = await sysService.GetMonthFilter();
            filterVM.Month = DateTime.Now.Month;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            //Initialize AttendanceRecordDutyRoster
            await dutyRosterService.InitializeAttendanceRecordDutyRoster(filterVM);

            //Initialize DODefault
            await dayOffService.DayOff_calcDODefault(filterVM, 0);

            //Initialize PHDefault
            await dayOffService.DayOff_calcPHDefault(filterVM, 0);

            division_filter_list = await organizationalChartService.GetDivisionList(filterVM);
            filterVM.DivisionID = (await sysService.GetInfoUser(filterVM.UserID)).DivisionID;

            filterVM.SectionID = string.Empty;
            section_filter_list = await organizationalChartService.GetSectionList();

            filterVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterVM);

            filterVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartService.GetPositionList();

            //filter Eserial = User
            profileUserVM = await dutyRosterService.GetProfileUser(filterVM);

            filterVM.DivisionID = profileUserVM.DivisionID;

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);
            filterVM.Eserial = filterVM.UserID;

            arVMs = await dutyRosterService.GetAttendanceRecordList(filterVM);

            filterVM.IsOpenFunc = await payrollService.IsOpenFunc(filterVM);

            isLoadingScreen = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterVM.Month = value;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            filterVM.IsOpenFunc = await payrollService.IsOpenFunc(filterVM);

            //Initialize AttendanceRecordDutyRoster
            await dutyRosterService.InitializeAttendanceRecordDutyRoster(filterVM);

            //Initialize DODefault
            await dayOffService.DayOff_calcDODefault(filterVM,0);

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);
            filterVM.Eserial = eserial_filter_list.Count() > 0 ? eserial_filter_list.ElementAt(0).Eserial : string.Empty;

            arVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            filterVM.Year = value;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            filterVM.IsOpenFunc = await payrollService.IsOpenFunc(filterVM);

            //Initialize AttendanceRecordDutyRoster
            await dutyRosterService.InitializeAttendanceRecordDutyRoster(filterVM);

            //Initialize DODefault
            await dayOffService.DayOff_calcDODefault(filterVM,0);

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);
            filterVM.Eserial = eserial_filter_list.Count() > 0 ? eserial_filter_list.ElementAt(0).Eserial : string.Empty;

            arVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterVM.DivisionID = value;

            filterVM.IsOpenFunc = await payrollService.IsOpenFunc(filterVM);

            filterVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterVM);

            filterVM.PositionGroupID = string.Empty;
            filterVM.arrPositionID = new string[] { };
            position_filter_list = await organizationalChartService.GetPositionList();

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);
            filterVM.Eserial = eserial_filter_list.Count() > 0 ? eserial_filter_list.ElementAt(0).Eserial : string.Empty;

            arVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_department(string value)
        {
            isLoading = true;

            filterVM.DepartmentID = value;

            filterVM.SectionID = string.Empty;

            filterVM.PositionGroupID = string.Empty;
            filterVM.arrPositionID = new string[] { };
            position_filter_list = await organizationalChartService.GetPositionList();

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);
            filterVM.Eserial = eserial_filter_list.Count() > 0 ? eserial_filter_list.ElementAt(0).Eserial : string.Empty;

            arVMs = null;

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
            filterVM.Eserial = eserial_filter_list.Count()>0?eserial_filter_list.ElementAt(0).Eserial:string.Empty;

            arVMs = null;

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

                arVMs = null;

                isLoading = false;
            }
        }

        private async void reload_filter_eserial()
        {
            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);
            filterVM.Eserial = eserial_filter_list.ElementAt(0).Eserial;

            StateHasChanged();
        }

        private async void onchange_filter_eserial(string value)
        {
            isLoading = true;

            filterVM.Eserial = value;

            arVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async Task GetAttendanceRecordList()
        {
            isLoading = true;

            if (eserial_filter_list.Count() > 0)
            {
                arVMs = await dutyRosterService.GetAttendanceRecordList(filterVM);
            }

            isLoading = false;
        }

        private async void onchange_updateshift(string value, DutyRosterVM _dutyRosterVM)
        {
            _dutyRosterVM.inputLoading_updateShift = true;

            _dutyRosterVM.UserID = filterVM.UserID;

            _dutyRosterVM.OldWorkShift = _dutyRosterVM.WorkShift;

            _dutyRosterVM.WorkShift = value.Replace("/", ",").Trim().ToUpper();

            string[] arrShift = _dutyRosterVM.WorkShift.Split(',');

            if (arrShift.Length == 1)
            {
                _dutyRosterVM.FirstShiftID = arrShift[0].ToString();
                _dutyRosterVM.SecondShiftID = string.Empty;
            }
            else if (arrShift.Length == 2)
            {
                _dutyRosterVM.FirstShiftID = arrShift[0].ToString();
                _dutyRosterVM.SecondShiftID = arrShift[1].ToString();
            }

            var result = await dutyRosterService.UpdateShiftWork(_dutyRosterVM);

            switch (result)
            {
                case "Err_Shift":
                    _dutyRosterVM.WorkShift = _dutyRosterVM.OldWorkShift;
                    _dutyRosterVM.inputLoading_updateShift = false;
                    await js.Swal_Message("Cảnh báo!", "Mã ca <strong>" + value.ToUpper() + "</strong> không hợp lệ.", SweetAlertMessageType.warning);
                    break;
                case "Err_OFF":
                    _dutyRosterVM.WorkShift = _dutyRosterVM.OldWorkShift;
                    _dutyRosterVM.inputLoading_updateShift = false;
                    await js.Swal_Message("Cảnh báo!", "Sử dụng ký hiệu <strong>OFF</strong> để nhập ngày nghỉ.", SweetAlertMessageType.warning);
                    break;
                default:
                    DutyRosterVM tmpDutyRosterVM = await dutyRosterService.GetDutyRosterByDay(filterVM, _dutyRosterVM);
                    _dutyRosterVM.WorkShift = tmpDutyRosterVM.WorkShift;
                    _dutyRosterVM.ColorHEX = tmpDutyRosterVM.ColorHEX;
                    _dutyRosterVM.inputLoading_updateShift = false;
                    StateHasChanged();

                    await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
                    break;
            }

            _dutyRosterVM.inputLoading_updateShift = false;

            StateHasChanged();
        }

        private async void onchange_explain(string value, DutyRosterVM _arVM)
        {
            isLoading = true;

            _arVM.Explain = value;

            await dutyRosterService.UpdateExplain(_arVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_explainHOD(string value, DutyRosterVM _arVM)
        {
            isLoading = true;

            _arVM.ExplainHOD = value;

            await dutyRosterService.UpdateExplainHOD(_arVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_explainHR(string value, DutyRosterVM _arVM)
        {
            isLoading = true;

            _arVM.ExplainHR = value;

            await dutyRosterService.UpdateExplainHR(_arVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_confirmExplain(DutyRosterVM _arVM)
        {
            isLoading = true;

            await dutyRosterService.ConfirmExplain(_arVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_confirmLateSoon(DutyRosterVM _arVM)
        {
            isLoading = true;

            await dutyRosterService.ConfirmLateSoon(_arVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async Task CalcFingerData()
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn cập nhật dữ liệu vân tay?", SweetAlertMessageType.question))
            {
                await dutyRosterService.CalcFingerData(filterVM);

                arVMs = await dutyRosterService.GetAttendanceRecordList(filterVM);

                await js.Swal_Message("Thông báo.", "Cập nhật thành công!", SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        //DayOffDetail
        IEnumerable<DayOffVM> dayOffVMs;
        private async Task InitializeModal_DayOffDetail(string _eserial)
        {
            isLoading = true;

            dayOffVMs = await dayOffService.GetDayOffDetail(filterVM.Period, _eserial);

            await js.InvokeAsync<object>("ShowModal", "#InitializeModal_DayOffDetail");

            isLoading = false;
        }

    }
}
