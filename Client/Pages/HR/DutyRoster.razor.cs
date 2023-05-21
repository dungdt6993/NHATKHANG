using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Client.Helpers;
using D69soft.Shared.Utilities;
using D69soft.Shared.Models.ViewModels.SYSTEM;

namespace D69soft.Client.Pages.HR
{
    partial class DutyRoster
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

        LogVM logVM = new();

        //Filter
        FilterHrVM filterHrVM = new();
        IEnumerable<PeriodVM> year_filter_list;
        IEnumerable<PeriodVM> month_filter_list;
        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;
        IEnumerable<SectionVM> section_filter_list;
        IEnumerable<PositionVM> position_filter_list;
        IEnumerable<ProfileVM> eserial_filter_list;

        //DutyRoster
        DutyRosterVM dutyRosterVM = new();
        List<DutyRosterVM> dutyRosterVMs;
        List<DutyRosterVM> search_dutyRosterVMs;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
                await js.InvokeAsync<object>("focusInputNextID");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
            await js.InvokeAsync<object>("maskDate");
            await js.InvokeAsync<object>("maskTime");
        }

        protected override async Task OnInitializedAsync()
        {       
            UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "HR_DutyRoster"))
            {
                logVM.LogType = "FUNC";
                logVM.LogName = "HR_DutyRoster";
                logVM.LogUser = UserID;
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            filterHrVM.UserID = dutyRosterVM.UserID = UserID;

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

            isLoadingScreen = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterHrVM.Month = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            //Initialize AttendanceRecordDutyRoster
            await dutyRosterService.InitializeAttendanceRecordDutyRoster(filterHrVM);

            //Initialize DODefault
            await dayOffService.DayOff_calcDODefault(filterHrVM,0);

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);

            dutyRosterVMs = search_dutyRosterVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            filterHrVM.Year = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            //Initialize AttendanceRecordDutyRoster
            await dutyRosterService.InitializeAttendanceRecordDutyRoster(filterHrVM);

            //Initialize DODefault
            await dayOffService.DayOff_calcDODefault(filterHrVM,0);

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);

            dutyRosterVMs = search_dutyRosterVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterHrVM.DivisionID = value;

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };
            position_filter_list = await organizationalChartService.GetPositionList();

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);

            dutyRosterVMs = search_dutyRosterVMs = null;

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

            dutyRosterVMs = search_dutyRosterVMs = null;

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

            dutyRosterVMs = search_dutyRosterVMs = null;

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

                dutyRosterVMs = search_dutyRosterVMs = null;

                isLoading = false;
            }
        }

        private async void reload_filter_eserial()
        {
            filterHrVM.Eserial = String.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterHrVM, UserID);

            StateHasChanged();
        }

        private void onchange_filter_eserial(string value)
        {
            isLoading = true;

            filterHrVM.Eserial = value;

            dutyRosterVMs = search_dutyRosterVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        #region Pagination

        int totalPages;
        int totalRecords;
        int curPage;
        int pagerSize;
        int pageSize;
        int startPage;
        int endPage;

        public void refreshRecords(int currentPage)
        {
            curPage = currentPage;

            search_dutyRosterVMs = dutyRosterVMs.Where(x=>x.dDate.Day==1).Skip((curPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public void SetPagerSize(string direction)
        {
            if (direction == "forward" && endPage < totalPages)
            {
                startPage = endPage + 1;
                if (endPage + pagerSize < totalPages)
                {
                    endPage = startPage + pagerSize - 1;
                }
                else
                {
                    endPage = totalPages;
                }
            }
            else if (direction == "back" && startPage > 1)
            {
                endPage = startPage - 1;
                startPage = startPage - pagerSize;
            }

        }

        public void NavigateToPage(string direction)
        {
            if (direction == "next")
            {
                if (curPage < totalPages)
                {
                    if (curPage == endPage)
                    {
                        SetPagerSize("forward");
                    }
                    curPage += 1;
                }
            }
            else if (direction == "previous")
            {
                if (curPage > 1)
                {
                    if (curPage == startPage)
                    {
                        SetPagerSize("back");
                    }
                    curPage -= 1;
                }
            }
            refreshRecords(curPage);
        }

        #endregion

        private async Task GetDutyRosterList()
        {
            isLoading = true;

            dutyRosterVMs = await dutyRosterService.GetDutyRosterList(filterHrVM);

            curPage = 1;
            pagerSize = 3;
            pageSize = 15;

            startPage = 0;
            endPage = 0;

            totalRecords = dutyRosterVMs.Where(x => x.dDate.Day == 1).Count();
            totalPages = (int)Math.Ceiling(totalRecords / (decimal)pageSize);
            SetPagerSize("forward");

            refreshRecords(curPage);

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

        //Lock
        LockDutyRosterVM lockDutyRosterVM = new();
        IEnumerable<LockDutyRosterVM> lockDutyRosterVMs;

        private async Task InitializeModal_LockDutyRoster()
        {
            isLoading = true;

            lockDutyRosterVM = new();

            lockDutyRosterVMs = await dutyRosterService.GetLockDutyRoster(filterHrVM);


            DivisionVM divisionFilterHrVM = new();

            divisionFilterHrVM = division_filter_list.First(x => x.DivisionID == filterHrVM.DivisionID);

            filterHrVM.is2625 = divisionFilterHrVM.is2625;

            DateTime fDate;
            DateTime tDate;

            if (filterHrVM.is2625 == 1)
            {
                fDate = new DateTime(filterHrVM.Year, filterHrVM.Month, 26).AddMonths(-1);
                tDate = new DateTime(filterHrVM.Year, filterHrVM.Month, 25);

            }
            else
            {
                fDate = new DateTime(filterHrVM.Year, filterHrVM.Month, 1);
                tDate = new DateTime(filterHrVM.Year, filterHrVM.Month, DateTime.DaysInMonth(filterHrVM.Year, filterHrVM.Month));
            }

            lockDutyRosterVM.LockFrom = fDate;
            lockDutyRosterVM.LockTo = tDate;

            isLoading = false;
        }
        public string onchange_LockTo
        {
            get
            {
                return lockDutyRosterVM.LockTo.HasValue ? lockDutyRosterVM.LockTo.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                lockDutyRosterVM.LockTo = LibraryFunc.FormatDateDDMMYYYY(value, lockDutyRosterVM.LockTo);

                DateTime fDate;
                DateTime tDate;
                if (filterHrVM.is2625 == 1)
                {
                    fDate = new DateTime(filterHrVM.Year, filterHrVM.Month, 26).AddMonths(-1);
                    tDate = new DateTime(filterHrVM.Year, filterHrVM.Month, 25);
                }
                else
                {
                    fDate = new DateTime(filterHrVM.Year, filterHrVM.Month, 1);
                    tDate = new DateTime(filterHrVM.Year, filterHrVM.Month, DateTime.DaysInMonth(filterHrVM.Year, filterHrVM.Month));
                }
                lockDutyRosterVM.LockTo = lockDutyRosterVM.LockTo < fDate ? fDate : lockDutyRosterVM.LockTo;
                lockDutyRosterVM.LockTo = lockDutyRosterVM.LockTo > tDate ? tDate : lockDutyRosterVM.LockTo;
            }
        }

        private async Task LockDutyRoster()
        {
            isLoading = true;

            lockDutyRosterVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;
            lockDutyRosterVM.DivisionID = filterHrVM.DivisionID;
            lockDutyRosterVM.DepartmentID = filterHrVM.DepartmentID;

            if (await dutyRosterService.LockDutyRoster(lockDutyRosterVM, UserID))
            {
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

                lockDutyRosterVMs = await dutyRosterService.GetLockDutyRoster(filterHrVM);

                dutyRosterVMs = search_dutyRosterVMs = null;
            }

            isLoading = false;
        }

        //Shift
        ShiftVM shiftVM = new();
        IEnumerable<ShiftVM> shiftVMs;
        IEnumerable<ShiftTypeVM> shiftTypeVMs;

        private async Task InitializeModal_Shift()
        {
            isLoading = true;

            shiftVMs = await dutyRosterService.GetShiftList();

            isLoading = false;
        }
        private async Task InitializeModalUpdate_Shift(int _isTypeUpdate, ShiftVM _shiftVM)
        {
            isLoading = true;

            shiftVM = new();
            shiftTypeVMs = await dutyRosterService.GetShiftTypeList();

            if (_isTypeUpdate == 0)
            {
                shiftVM.ColorHEX = "#ffffff";
                shiftVM.isActive = true;
            }

            if (_isTypeUpdate == 1)
            {
                shiftVM = _shiftVM;
            }

            shiftVM.IsTypeUpdate = _isTypeUpdate;

            isLoading = false;
        }

        public string onchange_BeginTime
        {
            get
            {
                return shiftVM.BeginTime.HasValue ? shiftVM.BeginTime.Value.ToString("HH:mm") : "";
            }
            set
            {
                shiftVM.BeginTime = LibraryFunc.FormatTimeHHMM(value, shiftVM.BeginTime);
            }
        }

        public string onchange_EndTime
        {
            get
            {
                return shiftVM.EndTime.HasValue ? shiftVM.EndTime.Value.ToString("HH:mm") : "";
            }
            set
            {
                shiftVM.EndTime = LibraryFunc.FormatTimeHHMM(value, shiftVM.EndTime);
            }
        }

        private async Task UpdateShift()
        {
            isLoading = true;

            if (shiftVM.IsTypeUpdate != 2)
            {
                await dutyRosterService.UpdateShift(shiftVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Shift");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await dutyRosterService.UpdateShift(shiftVM);

                    if (affectedRows > 0)
                    {
                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Shift");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu chấm công liên quan.", SweetAlertMessageType.error);
                        shiftVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    shiftVM.IsTypeUpdate = 1;
                }
            }

            shiftVMs = await dutyRosterService.GetShiftList();

            isLoading = false;
        }

        private async Task CloseModalUpdate_Shift()
        {
            isLoading = true;

            shiftVMs = await dutyRosterService.GetShiftList();

            isLoading = false;
        }

        private async Task CalcControlDAYOFF()
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn kiểm soát ngày nghỉ Tháng "+filterHrVM.Month+" năm "+filterHrVM.Year+"?", SweetAlertMessageType.question))
            {
                await dayOffService.DayOff_calcControlDAYOFF(filterHrVM);

                await GetDutyRosterList();

                await js.Swal_Message("Thông báo.", "Kiểm soát ngày nghỉ thành công!", SweetAlertMessageType.success);
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

