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
    partial class DayOff
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] PayrollService payrollService { get; set; }
        [Inject] DutyRosterService dutyRosterService { get; set; }
        [Inject] DayOffService dayOffService { get; set; }

        protected string UserID;

        bool isLoading;
        bool isLoadingScreen = true;

        //PermisFunc
        bool IsOpenFunc;

        bool HR_DayOff_Config;
        bool HR_DayOff_Calc;
        bool HR_DayOff_Adjust;

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

        IEnumerable<ShiftVM> dayofftype_filter_list;

        //DayOff
        DayOffVM dayOffVM = new();
        List<DayOffVM> dayOffVMs;

        //Shift
        IEnumerable<ShiftVM> shiftVMs;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
            await js.InvokeAsync<object>("maskDate");
        }

        protected override async Task OnInitializedAsync()
        {
            UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "HR_DayOff"))
            {
                logVM.LogUser = UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "HR_DayOff";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            HR_DayOff_Config = await sysService.CheckAccessSubFunc(UserID, "HR_DayOff_Config");
            HR_DayOff_Calc = await sysService.CheckAccessSubFunc(UserID, "HR_DayOff_Calc");
            HR_DayOff_Adjust = await sysService.CheckAccessSubFunc(UserID, "HR_DayOff_Adjust");

            //Initialize Filter
            filterHrVM.UserID = dayOffVM.UserID = UserID;

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
            filterHrVM.DivisionID = division_filter_list.Count() > 0 ? division_filter_list.ElementAt(0).DivisionID : string.Empty;

            filterHrVM.SectionID = string.Empty;
            section_filter_list = await organizationalChartService.GetSectionList();

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartService.GetPositionList();

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);

            dayofftype_filter_list = await dayOffService.GetDayOffTypeList();
            filterHrVM.ShiftID = dayofftype_filter_list.ElementAt(0).ShiftTypeID;

            shiftVMs = await dutyRosterService.GetShiftList();

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
            await dayOffService.DayOff_calcDODefault(filterHrVM, 0);

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);

            dayOffVMs = null;

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
            await dayOffService.DayOff_calcDODefault(filterHrVM, 0);

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);

            dayOffVMs = null;

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

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);

            dayOffVMs = null;

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
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);

            dayOffVMs = null;

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

            dayOffVMs = null;

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

                dayOffVMs = null;

                isLoading = false;
            }
        }

        private async void reload_filter_eserial()
        {
            filterHrVM.Eserial = String.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);

            StateHasChanged();
        }

        private async void onchange_filter_eserial(string value)
        {
            isLoading = true;

            filterHrVM.Eserial = value;

            dayOffVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_dayofftype(string value)
        {
            isLoading = true;

            filterHrVM.ShiftID = value;

            dayOffVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async Task GetDayOffList(string _shiftID)
        {
            isLoading = true;
            if (filterHrVM.ShiftID == "AL" || filterHrVM.ShiftID == "DO" || filterHrVM.ShiftID == "PH")
            {
                dayOffVMs = await dayOffService.GetDayOffList(filterHrVM);
            }
            else
            {
                dayOffVMs = await dayOffService.GetSpecialDayOffList(filterHrVM);
            }

            isLoading = false;
        }

        private async Task CalcDayOff(string _shiftID)
        {
            isLoading = true;

            switch (_shiftID)
            {
                case "AL":
                    if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn tính dữ liệu ngày phép năm?", SweetAlertMessageType.question))
                    {
                        await dayOffService.DayOff_calcAL(filterHrVM);
                        await GetDayOffList(_shiftID);

                        await js.Toast_Alert("Tính dữ liệu thành công!", SweetAlertMessageType.success);
                    }
                    break;
                case "DO":
                    if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn tính dữ liệu ngày nghỉ/bù tuần?", SweetAlertMessageType.question))
                    {
                        await dayOffService.DayOff_calcDODefault(filterHrVM, 1);
                        await dayOffService.DayOff_calcDO(filterHrVM);
                        await GetDayOffList(_shiftID);

                        await js.Toast_Alert("Tính dữ liệu thành công!", SweetAlertMessageType.success);
                    }
                    break;
                case "PH":
                    if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn tính dữ liệu ngày nghỉ/bù lễ tết?", SweetAlertMessageType.question))
                    {
                        await dayOffService.DayOff_calcPHDefault(filterHrVM, 1);
                        await dayOffService.DayOff_calcPH(filterHrVM);
                        await GetDayOffList(_shiftID);

                        await js.Toast_Alert("Tính dữ liệu thành công!", SweetAlertMessageType.success);
                    }
                    break;
            }

            isLoading = false;
        }

        private void InitializeModalUpdate_AddBalance(DayOffVM _dayOffVM)
        {
            isLoading = true;

            dayOffVM = _dayOffVM;

            dayOffVM.dayOffType = filterHrVM.ShiftID;
            dayOffVM.Year = filterHrVM.Year;
            dayOffVM.Month = filterHrVM.Month;
            dayOffVM.UserID = UserID;

            isLoading = false;
        }

        private async Task UpdateAddBalance()
        {
            isLoading = true;

            await dayOffService.UpdateAddBalance(dayOffVM);

            switch (filterHrVM.ShiftID)
            {
                case "AL":
                    await dayOffService.DayOff_calcAL(filterHrVM);
                    break;
                case "DO":
                    await dayOffService.DayOff_calcDO(filterHrVM);
                    break;
                case "PH":
                    await dayOffService.DayOff_calcPH(filterHrVM);
                    break;
            }

            await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_AddBalance");
            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            dayOffVMs = await dayOffService.GetDayOffList(filterHrVM);

            isLoading = false;
        }

        private async Task CloseModalUpdate_AddBalance()
        {
            isLoading = true;

            dayOffVMs = await dayOffService.GetDayOffList(filterHrVM);

            isLoading = false;
        }

        //PH
        IEnumerable<PublicHolidayDefVM> publicHolidayDefVMs;
        PublicHolidayDefVM publicHolidayDefVM = new();
        private async Task InitializeModalList_PublicHoliday()
        {
            isLoading = true;

            publicHolidayDefVMs = await dayOffService.GetPublicHolidayList();

            isLoading = false;
        }

        private void InitializeModalUpdate_PublicHoliday(int _IsTypeUpdate, PublicHolidayDefVM _publicHolidayDefVM)
        {
            isLoading = true;

            publicHolidayDefVM = new();

            if (_IsTypeUpdate == 1)
            {
                publicHolidayDefVM = _publicHolidayDefVM;
            }

            publicHolidayDefVM.IsTypeUpdate = _IsTypeUpdate;

            isLoading = false;
        }

        private async Task UpdatePublicHoliday()
        {
            isLoading = true;

            if (publicHolidayDefVM.IsTypeUpdate != 2)
            {
                await dayOffService.UpdatePublicHoliday(publicHolidayDefVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_PublicHoliday");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    await dayOffService.UpdatePublicHoliday(publicHolidayDefVM);

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_PublicHoliday");
                    await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                }
                else
                {
                    publicHolidayDefVM.IsTypeUpdate = 1;
                }
            }

            publicHolidayDefVMs = await dayOffService.GetPublicHolidayList();

            isLoading = false;
        }

        private async Task CloseModalUpdate_PublicHoliday()
        {
            isLoading = true;

            publicHolidayDefVMs = await dayOffService.GetPublicHolidayList();

            isLoading = false;
        }

        //GlobalParameter
        IEnumerable<GlobalParameterVM> globalParameterVMs;
        GlobalParameterVM globalParameterVM = new();
        private async Task InitializeModalList_GlobalParameter()
        {
            isLoading = true;

            globalParameterVMs = await sysService.GetGlobalParameterList("DayOff");

            isLoading = false;
        }

        private void InitializeModalUpdate_GlobalParameter(int _IsTypeUpdate, GlobalParameterVM _globalParameterVM)
        {
            isLoading = true;

            globalParameterVM = new();

            if (_IsTypeUpdate == 1)
            {
                globalParameterVM = _globalParameterVM;
            }

            globalParameterVM.IsTypeUpdate = _IsTypeUpdate;

            isLoading = false;
        }

        private async Task UpdateGlobalParameter()
        {
            isLoading = true;

            if (globalParameterVM.IsTypeUpdate != 2)
            {
                await sysService.UpdateGlobalParameter(globalParameterVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_GlobalParameter");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }

            globalParameterVMs = await sysService.GetGlobalParameterList("DayOff");

            isLoading = false;
        }

        private async Task CloseModalUpdate_GlobalParameter()
        {
            isLoading = true;

            globalParameterVMs = await sysService.GetGlobalParameterList("DayOff");

            isLoading = false;
        }

    }
}
