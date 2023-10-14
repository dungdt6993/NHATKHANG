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

        bool isLoading;
        bool isLoadingScreen = true;

        //PermisFunc
        bool HR_DayOff_Config;
        bool HR_DayOff_Calc;
        bool HR_DayOff_Adjust;

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
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(filterVM.UserID, "HR_DayOff"))
            {
                logVM.LogUser = filterVM.UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "HR_DayOff";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            HR_DayOff_Config = await sysService.CheckAccessSubFunc(filterVM.UserID, "HR_DayOff_Config");
            HR_DayOff_Calc = await sysService.CheckAccessSubFunc(filterVM.UserID, "HR_DayOff_Calc");
            HR_DayOff_Adjust = await sysService.CheckAccessSubFunc(filterVM.UserID, "HR_DayOff_Adjust");

            //Initialize Filter
            dayOffVM.UserID = filterVM.UserID;

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

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            dayofftype_filter_list = await dayOffService.GetDayOffTypeList();
            filterVM.ShiftID = dayofftype_filter_list.ElementAt(0).ShiftTypeID;

            shiftVMs = await dutyRosterService.GetShiftList();

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
            await dayOffService.DayOff_calcDODefault(filterVM, 0);

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            dayOffVMs = null;

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
            await dayOffService.DayOff_calcDODefault(filterVM, 0);

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            dayOffVMs = null;

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

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            dayOffVMs = null;

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

            dayOffVMs = null;

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

            dayOffVMs = null;

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

                dayOffVMs = null;

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

            dayOffVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_dayofftype(string value)
        {
            isLoading = true;

            filterVM.ShiftID = value;

            dayOffVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async Task GetDayOffList(string _shiftID)
        {
            isLoading = true;
            if (filterVM.ShiftID == "AL" || filterVM.ShiftID == "DO" || filterVM.ShiftID == "PH")
            {
                dayOffVMs = await dayOffService.GetDayOffList(filterVM);
            }
            else
            {
                dayOffVMs = await dayOffService.GetSpecialDayOffList(filterVM);
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
                        await dayOffService.DayOff_calcAL(filterVM);
                        await GetDayOffList(_shiftID);

                        await js.Toast_Alert("Tính dữ liệu thành công!", SweetAlertMessageType.success);
                    }
                    break;
                case "DO":
                    if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn tính dữ liệu ngày nghỉ/bù tuần?", SweetAlertMessageType.question))
                    {
                        await dayOffService.DayOff_calcDODefault(filterVM, 1);
                        await dayOffService.DayOff_calcDO(filterVM);
                        await GetDayOffList(_shiftID);

                        await js.Toast_Alert("Tính dữ liệu thành công!", SweetAlertMessageType.success);
                    }
                    break;
                case "PH":
                    if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn tính dữ liệu ngày nghỉ/bù lễ tết?", SweetAlertMessageType.question))
                    {
                        await dayOffService.DayOff_calcPHDefault(filterVM, 1);
                        await dayOffService.DayOff_calcPH(filterVM);
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

            dayOffVM.dayOffType = filterVM.ShiftID;
            dayOffVM.Year = filterVM.Year;
            dayOffVM.Month = filterVM.Month;
            dayOffVM.UserID = filterVM.UserID;

            isLoading = false;
        }

        private async Task UpdateAddBalance()
        {
            isLoading = true;

            await dayOffService.UpdateAddBalance(dayOffVM);

            switch (filterVM.ShiftID)
            {
                case "AL":
                    await dayOffService.DayOff_calcAL(filterVM);
                    break;
                case "DO":
                    await dayOffService.DayOff_calcDO(filterVM);
                    break;
                case "PH":
                    await dayOffService.DayOff_calcPH(filterVM);
                    break;
            }

            await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_AddBalance");
            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            dayOffVMs = await dayOffService.GetDayOffList(filterVM);

            isLoading = false;
        }

        private async Task CloseModalUpdate_AddBalance()
        {
            isLoading = true;

            dayOffVMs = await dayOffService.GetDayOffList(filterVM);

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
