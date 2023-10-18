using BlazorDateRangePicker;
using Blazored.TextEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Model.ViewModels.OP;
using D69soft.Client.Services;
using D69soft.Client.Services.HR;
using D69soft.Client.Services.OP;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;

namespace D69soft.Client.Pages.OP
{
    partial class EmplTrf
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] DutyRosterService dutyRosterService { get; set; }
        [Inject] OPService opService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        //log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<PositionVM> position_filter_list;

        //DutyRoster
        DutyRosterVM dutyRosterVM = new();
        IEnumerable<DutyRosterVM> dutyRosterVMs;

        IEnumerable<DutyRosterVM> dutyRosterNotes;

        //Vehicle
        VehicleScheduleVM vehicleScheduleVM = new();
        IEnumerable<VehicleScheduleVM> vehicleScheduleVMs;

        //Shift
        IEnumerable<ShiftVM> shiftVMs;

        BlazoredTextEditor QuillHtml = new BlazoredTextEditor();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");

            if (!String.IsNullOrEmpty(dutyRosterVM.DutyRosterNote))
            {
                await QuillHtml.LoadHTMLContent(dutyRosterVM.DutyRosterNote);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(filterVM.UserID, "OP_EmplTrf"))
            {
                logVM.LogType = "FUNC";
                logVM.LogName = "OP_EmplTrf";
                logVM.LogUser = filterVM.UserID;
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            dutyRosterVM.UserID = filterVM.UserID;

            filterVM.dDate = DateTime.Now;

            division_filter_list = await organizationalChartService.GetDivisionList(filterVM);
            filterVM.DivisionID = (await sysService.GetInfoUser(filterVM.UserID)).DivisionID;

            filterVM.ShiftID = string.Empty;
            shiftVMs = await dutyRosterService.GetShiftList();

            filterVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartService.GetPositionList();

            isLoadingScreen = false;
        }

        public async Task OnRangeSelect_dDate(DateRange _range)
        {
            filterVM.dDate = _range.Start;

            dutyRosterVMs = null;

            dutyRosterNotes = null;
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterVM.DivisionID = value;

            filterVM.ShiftID = string.Empty;
            filterVM.arrShiftID = new string[] { };

            filterVM.PositionGroupID = string.Empty;
            filterVM.arrPositionID = new string[] { };

            position_filter_list = await organizationalChartService.GetPositionList();

            dutyRosterVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private string[] onchange_filter_shift
        {
            get
            {
                return filterVM.arrShiftID;
            }
            set
            {
                isLoading = true;

                filterVM.arrShiftID = (string[])value;

                filterVM.ShiftID = string.Join(",", (string[])value);

                dutyRosterVMs = null;

                isLoading = false;
            }
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

                dutyRosterVMs = null;

                isLoading = false;
            }
        }

        private async Task GetEmplTrfList()
        {
            isLoading = true;

            dutyRosterVMs = await dutyRosterService.GetEmplTrfList(filterVM);

            dutyRosterNotes = await dutyRosterService.GetDutyRosterNotes(filterVM);

            vehicleScheduleVMs = await opService.GetVehicleSchedules(filterVM);

            isLoading = false;
        }

        private async Task InitializeModalUpdate_EmplTrf(DutyRosterVM _dutyRoster)
        {
            isLoading = true;

            dutyRosterVM = _dutyRoster;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_EmplTrf");

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

                    filterVM.Year = int.Parse(filterVM.dDate.Value.ToString("yyyy"));
                    filterVM.Month = int.Parse(filterVM.dDate.Value.ToString("MM"));
					filterVM.Day = _dutyRosterVM.dDate.Day;

					filterVM.SectionID = String.Empty;
                    filterVM.DepartmentID = String.Empty;
                    filterVM.Eserial = _dutyRosterVM.Eserial;

                    DutyRosterVM tmpDutyRosterVM = (await dutyRosterService.GetDutyRosterList(filterVM)).First();

                    _dutyRosterVM.WorkShift = tmpDutyRosterVM.WorkShift;
                    _dutyRosterVM.ColorHEX = tmpDutyRosterVM.ColorHEX;
                    _dutyRosterVM.inputLoading_updateShift = false;
                    StateHasChanged();

                    await GetEmplTrfList();

                    await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
                    break;
            }
            _dutyRosterVM.inputLoading_updateShift = false;

            StateHasChanged();
        }

        private async void onchange_UpdatePositionWork(string value)
        {
            isLoading = true;

            dutyRosterVM.PositionID = value;

            await dutyRosterService.UpdatePositionWork(dutyRosterVM);

            await GetEmplTrfList();

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async Task InitializeModalUpdate_DutyRosterNote(string _shift, string _positiongrp, string _dutyRosterNote)
        {
            isLoading = true;

            dutyRosterVM = new();

            dutyRosterVM.UserID = filterVM.UserID;
            dutyRosterVM.dDate = filterVM.dDate.Value.DateTime;

            dutyRosterVM.ShiftID = _shift;
            dutyRosterVM.PositionGroupID = _positiongrp;

            dutyRosterVM.DutyRosterNote = _dutyRosterNote;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_DutyRosterNote");

            isLoading = false;
        }

        private async void UpdateDutyRosterNote()
        {
            isLoading = true;

            dutyRosterVM.DutyRosterNote = await QuillHtml.GetHTML() != "<p><br></p>" ? await QuillHtml.GetHTML() : String.Empty;

            await dutyRosterService.UpdateDutyRosterNote(dutyRosterVM);

            await GetEmplTrfList();

            await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_DutyRosterNote");
            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async Task InitializeModalUpdate_VehicleTrf(VehicleScheduleVM _vehicleScheduleVM)
        {
            isLoading = true;

            vehicleScheduleVM = _vehicleScheduleVM;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_VehicleTrf");

            isLoading = false;
        }

        private async void onchange_updateVehicleShift(string value)
        {
            isLoading = true;

            vehicleScheduleVM.ShiftID = value.Replace("/", ",").Trim().ToUpper();

            await opService.UpdateVehicleShift(vehicleScheduleVM);

            await GetEmplTrfList();

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async void onclick_VehicleStatus(bool _vehicleStatus)
        {
            isLoading = true;

            vehicleScheduleVM.VehicleStatus = _vehicleStatus;

            await opService.UpdateVehicleStatus(vehicleScheduleVM);

            await GetEmplTrfList();

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

    }
}
