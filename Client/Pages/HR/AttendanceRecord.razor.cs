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
    partial class AttendanceRecord
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        [Inject] SysRepository sysRepo { get; set; }
        [Inject] UserRepository userRepo { get; set; }

        [Inject] OrganizationalChartService organizationalChartRepo { get; set; }

        [Inject] PayrollService payrollRepo { get; set; }
        [Inject] DutyRosterService dutyRosterRepo { get; set; }
        [Inject] DayOffService dayOffRepo { get; set; }

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

        //AttendanceRecord
        DutyRosterVM arVM = new();
        IEnumerable<DutyRosterVM> arVMs;

        ProfileManagamentVM profileUserVM;

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
            isLoadingPage = true;

            UserID = (await authenticationStateTask).User.GetUserId();

            if (sysRepo.checkPermisFunc(UserID, "HR_AttendanceRecord"))
            {
                await sysRepo.insert_LogUserFunc(UserID, "HR_AttendanceRecord");
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            filterHrVM.UserID = arVM.UserID = UserID;

            year_filter_list = await sysRepo.GetYearFilter();
            filterHrVM.Year = DateTime.Now.Year;

            month_filter_list = await sysRepo.GetMonthFilter();
            filterHrVM.Month = DateTime.Now.Month;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            //Initialize AttendanceRecordDutyRoster
            await dutyRosterRepo.InitializeAttendanceRecordDutyRoster(filterHrVM);

            //Initialize DODefault
            await dayOffRepo.DayOff_calcDODefault(filterHrVM, 0);

            //Initialize PHDefault
            await dayOffRepo.DayOff_calcPHDefault(filterHrVM, 0);

            division_filter_list = await organizationalChartRepo.GetDivisionList(filterHrVM);
            filterHrVM.DivisionID = division_filter_list.Count()>0?division_filter_list.ElementAt(0).DivisionID:string.Empty;

            filterHrVM.SectionID = string.Empty;
            section_filter_list = await organizationalChartRepo.GetSectionList();

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartRepo.GetDepartmentList(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartRepo.GetPositionList();

            //filter Eserial = User
            profileUserVM = await dutyRosterRepo.GetProfileUser(filterHrVM);

            filterHrVM.DivisionID = profileUserVM.DivisionID;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);
            filterHrVM.Eserial = UserID;

            arVMs = await dutyRosterRepo.GetAttendanceRecordList(filterHrVM);

            isLoadingPage = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterHrVM.Month = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            //Initialize AttendanceRecordDutyRoster
            await dutyRosterRepo.InitializeAttendanceRecordDutyRoster(filterHrVM);

            //Initialize DODefault
            await dayOffRepo.DayOff_calcDODefault(filterHrVM,0);

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);
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

            //Initialize AttendanceRecordDutyRoster
            await dutyRosterRepo.InitializeAttendanceRecordDutyRoster(filterHrVM);

            //Initialize DODefault
            await dayOffRepo.DayOff_calcDODefault(filterHrVM,0);

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);
            filterHrVM.Eserial = eserial_filter_list.Count() > 0 ? eserial_filter_list.ElementAt(0).Eserial : string.Empty;

            arVMs = null;

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
            position_filter_list = await organizationalChartRepo.GetPositionList();

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);
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
            position_filter_list = await organizationalChartRepo.GetPositionList();

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);
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
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);
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
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);
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
                arVMs = await dutyRosterRepo.GetAttendanceRecordList(filterHrVM);
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

            var result = await dutyRosterRepo.UpdateShiftWork(_dutyRosterVM);

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
                    DutyRosterVM tmpDutyRosterVM = await dutyRosterRepo.GetDutyRosterByDay(filterHrVM, _dutyRosterVM);
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

            await dutyRosterRepo.UpdateExplain(_arVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_explainHOD(string value, DutyRosterVM _arVM)
        {
            isLoading = true;

            _arVM.ExplainHOD = value;

            await dutyRosterRepo.UpdateExplainHOD(_arVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_explainHR(string value, DutyRosterVM _arVM)
        {
            isLoading = true;

            _arVM.ExplainHR = value;

            await dutyRosterRepo.UpdateExplainHR(_arVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_confirmExplain(DutyRosterVM _arVM)
        {
            isLoading = true;

            await dutyRosterRepo.ConfirmExplain(_arVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_confirmLateSoon(DutyRosterVM _arVM)
        {
            isLoading = true;

            await dutyRosterRepo.ConfirmLateSoon(_arVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async Task CalcFingerData()
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn cập nhật dữ liệu vân tay?", SweetAlertMessageType.question))
            {
                await dutyRosterRepo.CalcFingerData(filterHrVM);

                arVMs = await dutyRosterRepo.GetAttendanceRecordList(filterHrVM);

                await js.Swal_Message("Thông báo.", "Cập nhật thành công!", SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        //DayOffDetail
        IEnumerable<DayOffVM> dayOffVMs;
        private async Task InitializeModal_DayOffDetail(string _eserial)
        {
            isLoading = true;

            dayOffVMs = await dayOffRepo.GetDayOffDetail(filterHrVM.Period, _eserial);

            await js.InvokeAsync<object>("ShowModal", "#InitializeModal_DayOffDetail");

            isLoading = false;
        }

    }
}
