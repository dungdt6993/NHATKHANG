using Data.Repositories.HR;
using Data.Repositories.SYSTEM;
using Model.ViewModels.FIN;
using Model.ViewModels.HR;
using Model.ViewModels.SYSTEM;
using Utilities;
using WebApp.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace WebApp.Pages.HR
{
    partial class DayOff
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        [Inject] SysRepository sysRepo { get; set; }

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
            isLoadingPage = true;

            UserID = (await authenticationStateTask).User.GetUserId();

            if (sysRepo.checkPermisFunc(UserID, "HR_DayOff"))
            {
                await sysRepo.insert_LogUserFunc(UserID, "HR_DayOff");
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            filterHrVM.UserID = dayOffVM.UserID = UserID;

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
            filterHrVM.DivisionID = division_filter_list.Count() > 0 ? division_filter_list.ElementAt(0).DivisionID : string.Empty;

            filterHrVM.SectionID = string.Empty;
            section_filter_list = await organizationalChartRepo.GetSectionList();

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartRepo.GetDepartmentList(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartRepo.GetPositionList();

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            dayofftype_filter_list = await dayOffRepo.GetDayOffTypeList();
            filterHrVM.ShiftID = dayofftype_filter_list.ElementAt(0).ShiftTypeID;

            shiftVMs = await dutyRosterRepo.GetShiftList();

            isLoadingPage = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterHrVM.Month = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            if (filterHrVM.Month != 0)
            {
                //Initialize AttendanceRecordDutyRoster
                await dutyRosterRepo.InitializeAttendanceRecordDutyRoster(filterHrVM);

                //Initialize DODefault
                await dayOffRepo.DayOff_calcDODefault(filterHrVM, 0);
            }

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            dayOffVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            filterHrVM.Year = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            if(filterHrVM.Month!=0)
            {
                //Initialize AttendanceRecordDutyRoster
                await dutyRosterRepo.InitializeAttendanceRecordDutyRoster(filterHrVM);

                //Initialize DODefault
                await dayOffRepo.DayOff_calcDODefault(filterHrVM, 0);
            }

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            dayOffVMs = null;

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
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

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
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

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
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

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
                dayOffVMs = await dayOffRepo.GetDayOffList(filterHrVM);
            }
            else
            {
                dayOffVMs = await dayOffRepo.GetSpecialDayOffList(filterHrVM);
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
                        await dayOffRepo.DayOff_calcAL(filterHrVM);
                        await GetDayOffList(_shiftID);

                        await js.Toast_Alert("Tính dữ liệu thành công!", SweetAlertMessageType.success);
                    }
                    break;
                case "DO":
                    if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn tính dữ liệu ngày nghỉ/bù tuần?", SweetAlertMessageType.question))
                    {
                        await dayOffRepo.DayOff_calcDODefault(filterHrVM, 1);
                        await dayOffRepo.DayOff_calcDO(filterHrVM);
                        await GetDayOffList(_shiftID);

                        await js.Toast_Alert("Tính dữ liệu thành công!", SweetAlertMessageType.success);
                    }
                    break;
                case "PH":
                    if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn tính dữ liệu ngày nghỉ/bù lễ tết?", SweetAlertMessageType.question))
                    {
                        await dayOffRepo.DayOff_calcPHDefault(filterHrVM, 1);
                        await dayOffRepo.DayOff_calcPH(filterHrVM);
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

            await dayOffRepo.UpdateAddBalance(dayOffVM);

            await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_AddBalance");
            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            dayOffVMs = await dayOffRepo.GetDayOffList(filterHrVM);

            isLoading = false;
        }

        private async Task CloseModalUpdate_AddBalance()
        {
            isLoading = true;

            dayOffVMs = await dayOffRepo.GetDayOffList(filterHrVM);

            isLoading = false;
        }

        //PH
        IEnumerable<PublicHolidayDefVM> publicHolidayDefVMs;
        PublicHolidayDefVM publicHolidayDefVM = new();
        private async Task InitializeModalList_PublicHoliday()
        {
            isLoading = true;

            publicHolidayDefVMs = await dayOffRepo.GetPublicHolidayList();

            isLoading = false;
        }

        private void InitializeModalUpdate_PublicHoliday(int _isTypeUpdate, PublicHolidayDefVM _publicHolidayDefVM)
        {
            isLoading = true;

            publicHolidayDefVM = new();

            if (_isTypeUpdate == 1)
            {
                publicHolidayDefVM = _publicHolidayDefVM;
            }

            publicHolidayDefVM.IsTypeUpdate = _isTypeUpdate;

            isLoading = false;
        }

        private async Task UpdatePublicHoliday()
        {
            isLoading = true;

            if (publicHolidayDefVM.IsTypeUpdate != 2)
            {
                await dayOffRepo.UpdatePublicHoliday(publicHolidayDefVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_PublicHoliday");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    await dayOffRepo.UpdatePublicHoliday(publicHolidayDefVM);

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_PublicHoliday");
                    await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                }
                else
                {
                    publicHolidayDefVM.IsTypeUpdate = 1;
                }
            }

            publicHolidayDefVMs = await dayOffRepo.GetPublicHolidayList();

            isLoading = false;
        }

        private async Task CloseModalUpdate_PublicHoliday()
        {
            isLoading = true;

            publicHolidayDefVMs = await dayOffRepo.GetPublicHolidayList();

            isLoading = false;
        }

        //GlobalParameter
        IEnumerable<GlobalParameterVM> globalParameterVMs;
        GlobalParameterVM globalParameterVM = new();
        private async Task InitializeModalList_GlobalParameter()
        {
            isLoading = true;

            globalParameterVMs = await sysRepo.GetGlobalParameterList("DayOff");

            isLoading = false;
        }

        private void InitializeModalUpdate_GlobalParameter(int _isTypeUpdate, GlobalParameterVM _globalParameterVM)
        {
            isLoading = true;

            globalParameterVM = new();

            if (_isTypeUpdate == 1)
            {
                globalParameterVM = _globalParameterVM;
            }

            globalParameterVM.IsTypeUpdate = _isTypeUpdate;

            isLoading = false;
        }

        private async Task UpdateGlobalParameter()
        {
            isLoading = true;

            if (globalParameterVM.IsTypeUpdate != 2)
            {
                await sysRepo.UpdateGlobalParameter(globalParameterVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_GlobalParameter");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }

            globalParameterVMs = await sysRepo.GetGlobalParameterList("DayOff");

            isLoading = false;
        }

        private async Task CloseModalUpdate_GlobalParameter()
        {
            isLoading = true;

            globalParameterVMs = await sysRepo.GetGlobalParameterList("DayOff");

            isLoading = false;
        }

    }
}
