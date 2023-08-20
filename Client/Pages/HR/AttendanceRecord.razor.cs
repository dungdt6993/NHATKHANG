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

        protected string UserID;

        bool isLoading;
        bool isLoadingScreen = true;

        //PermisFunc
        bool IsOpenFunc;
        int Role;

        bool HR_AttendanceRecord_CalcFingerData;
        bool HR_DutyRoster_Update;

        LogVM logVM = new();

        //Filter
        FilterHrVM filterHrVM = new();
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
            UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "HR_AttendanceRecord"))
            {
                logVM.LogUser = UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "HR_AttendanceRecord";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            Role = await authService.GetRole(UserID);

            HR_AttendanceRecord_CalcFingerData = await sysService.CheckAccessSubFunc(UserID, "HR_AttendanceRecord_CalcFingerData");
            HR_DutyRoster_Update = await sysService.CheckAccessSubFunc(UserID, "HR_DutyRoster_Update");

            //Initialize Filter
            filterHrVM.UserID = arVM.UserID = UserID;

            year_filter_list = await sysService.GetYearFilter();
            filterHrVM.Year = DateTime.Now.Year;

            month_filter_list = await sysService.GetMonthFilter();
            filterHrVM.Month = DateTime.Now.Month;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            //Initialize AttendanceRecordDutyRoster
            await dutyRosterService.InitializeAttendanceRecordDutyRoster(filterHrVM);

            //Initialize DODefault
            await dayOffService.DayOff_calcDODefault(filterHrVM, 0);

            //Initialize PHDefault
            await dayOffService.DayOff_calcPHDefault(filterHrVM, 0);

            division_filter_list = await organizationalChartService.GetDivisionList(filterHrVM);
            filterHrVM.DivisionID = (await sysService.GetInfoUser(UserID)).DivisionID;

            filterHrVM.SectionID = string.Empty;
            section_filter_list = await organizationalChartService.GetSectionList();

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartService.GetPositionList();

            //filter Eserial = User
            profileUserVM = await dutyRosterService.GetProfileUser(filterHrVM);

            filterHrVM.DivisionID = profileUserVM.DivisionID;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);
            filterHrVM.Eserial = UserID;

            arVMs = await dutyRosterService.GetAttendanceRecordList(filterHrVM);

            IsOpenFunc = await payrollService.IsOpenFunc(filterHrVM);

            isLoadingScreen = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterHrVM.Month = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            IsOpenFunc = await payrollService.IsOpenFunc(filterHrVM);

            //Initialize AttendanceRecordDutyRoster
            await dutyRosterService.InitializeAttendanceRecordDutyRoster(filterHrVM);

            //Initialize DODefault
            await dayOffService.DayOff_calcDODefault(filterHrVM,0);

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);
            filterHrVM.Eserial = eserial_filter_list.Count() > 0 ? eserial_filter_list.ElementAt(0).Eserial : string.Empty;

            arVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            filterHrVM.Year = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            IsOpenFunc = await payrollService.IsOpenFunc(filterHrVM);

            //Initialize AttendanceRecordDutyRoster
            await dutyRosterService.InitializeAttendanceRecordDutyRoster(filterHrVM);

            //Initialize DODefault
            await dayOffService.DayOff_calcDODefault(filterHrVM,0);

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);
            filterHrVM.Eserial = eserial_filter_list.Count() > 0 ? eserial_filter_list.ElementAt(0).Eserial : string.Empty;

            arVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterHrVM.DivisionID = value;

            IsOpenFunc = await payrollService.IsOpenFunc(filterHrVM);

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };
            position_filter_list = await organizationalChartService.GetPositionList();

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);
            filterHrVM.Eserial = eserial_filter_list.Count() > 0 ? eserial_filter_list.ElementAt(0).Eserial : string.Empty;

            arVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_department(string value)
        {
            isLoading = true;

            filterHrVM.DepartmentID = value;

            filterHrVM.SectionID = string.Empty;

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };
            position_filter_list = await organizationalChartService.GetPositionList();

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);
            filterHrVM.Eserial = eserial_filter_list.Count() > 0 ? eserial_filter_list.ElementAt(0).Eserial : string.Empty;

            arVMs = null;

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
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);
            filterHrVM.Eserial = eserial_filter_list.Count()>0?eserial_filter_list.ElementAt(0).Eserial:string.Empty;

            arVMs = null;

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

                arVMs = null;

                isLoading = false;
            }
        }

        private async void reload_filter_eserial()
        {
            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);
            filterHrVM.Eserial = eserial_filter_list.ElementAt(0).Eserial;

            StateHasChanged();
        }

        private async void onchange_filter_eserial(string value)
        {
            isLoading = true;

            filterHrVM.Eserial = value;

            arVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async Task GetAttendanceRecordList()
        {
            isLoading = true;

            if (eserial_filter_list.Count() > 0)
            {
                arVMs = await dutyRosterService.GetAttendanceRecordList(filterHrVM);
            }

            isLoading = false;
        }

        private async void onchange_updateshift(string value, DutyRosterVM _dutyRosterVM)
        {
            _dutyRosterVM.inputLoading_updateShift = true;

            _dutyRosterVM.UserID = UserID;

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
                    DutyRosterVM tmpDutyRosterVM = await dutyRosterService.GetDutyRosterByDay(filterHrVM, _dutyRosterVM);
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
                await dutyRosterService.CalcFingerData(filterHrVM);

                arVMs = await dutyRosterService.GetAttendanceRecordList(filterHrVM);

                await js.Swal_Message("Thông báo.", "Cập nhật thành công!", SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        //DayOffDetail
        IEnumerable<DayOffVM> dayOffVMs;
        private async Task InitializeModal_DayOffDetail(string _eserial)
        {
            isLoading = true;

            dayOffVMs = await dayOffService.GetDayOffDetail(filterHrVM.Period, _eserial);

            await js.InvokeAsync<object>("ShowModal", "#InitializeModal_DayOffDetail");

            isLoading = false;
        }

    }
}
