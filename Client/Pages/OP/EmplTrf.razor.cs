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

        protected string UserID;

        bool isLoading;

        bool isLoadingScreen = true;

        LogVM logVM = new();

        //Filter
        FilterHrVM filterHrVM = new();

        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<PositionVM> position_filter_list;

        //DutyRoster
        DutyRosterVM dutyRosterVM = new();
        IEnumerable<DutyRosterVM> dutyRosterVMs;

        IEnumerable<DutyRosterVM> dutyRosterNotes;

        //Tender
        TenderScheduleVM tenderScheduleVM = new();
        IEnumerable<TenderScheduleVM> tenderScheduleVMs;

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
            UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "OP_EmplTrf"))
            {
                logVM.LogType = "FUNC";
                logVM.LogName = "OP_EmplTrf";
                logVM.LogUser = UserID;
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            filterHrVM.UserID = dutyRosterVM.UserID = UserID;

            filterHrVM.dDate = DateTime.Now;

            division_filter_list = await organizationalChartService.GetDivisionList(filterHrVM);
            filterHrVM.DivisionID = division_filter_list.Count() > 0 ? division_filter_list.ElementAt(0).DivisionID : string.Empty;

            filterHrVM.ShiftID = string.Empty;
            shiftVMs = await dutyRosterService.GetShiftList();

            filterHrVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartService.GetPositionList();

            isLoadingScreen = false;
        }

        public async Task OnRangeSelect_dDate(DateRange _range)
        {
            filterHrVM.dDate = _range.Start;

            dutyRosterVMs = null;

            dutyRosterNotes = null;
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterHrVM.DivisionID = value;

            filterHrVM.ShiftID = string.Empty;
            filterHrVM.arrShiftID = new string[] { };

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };

            position_filter_list = await organizationalChartService.GetPositionList();

            dutyRosterVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private string[] onchange_filter_shift
        {
            get
            {
                return filterHrVM.arrShiftID;
            }
            set
            {
                isLoading = true;

                filterHrVM.arrShiftID = (string[])value;

                filterHrVM.ShiftID = string.Join(",", (string[])value);

                dutyRosterVMs = null;

                isLoading = false;
            }
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

                dutyRosterVMs = null;

                isLoading = false;
            }
        }

        private async Task GetEmplTrfList()
        {
            isLoading = true;

            dutyRosterVMs = await dutyRosterService.GetEmplTrfList(filterHrVM);

            dutyRosterNotes = await dutyRosterService.GetDutyRosterNotes(filterHrVM);

            tenderScheduleVMs = await opService.GetTenderSchedules(filterHrVM);

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

                    filterHrVM.Year = int.Parse(filterHrVM.dDate.Value.ToString("yyyy"));
                    filterHrVM.Month = int.Parse(filterHrVM.dDate.Value.ToString("MM"));
					filterHrVM.Day = _dutyRosterVM.dDate.Day;

					filterHrVM.SectionID = String.Empty;
                    filterHrVM.DepartmentID = String.Empty;
                    filterHrVM.Eserial = _dutyRosterVM.Eserial;

                    DutyRosterVM tmpDutyRosterVM = (await dutyRosterService.GetDutyRosterList(filterHrVM)).First();

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

            dutyRosterVM.UserID = UserID;
            dutyRosterVM.dDate = filterHrVM.dDate.Value.DateTime;

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

        private async Task InitializeModalUpdate_TenderTrf(TenderScheduleVM _tenderScheduleVM)
        {
            isLoading = true;

            tenderScheduleVM = _tenderScheduleVM;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_TenderTrf");

            isLoading = false;
        }

        private async void onchange_updateTenderShift(string value)
        {
            isLoading = true;

            tenderScheduleVM.ShiftID = value.Replace("/", ",").Trim().ToUpper();

            await opService.UpdateTenderShift(tenderScheduleVM);

            await GetEmplTrfList();

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

        private async void onclick_TenderStatus(bool _tenderStatus)
        {
            isLoading = true;

            tenderScheduleVM.TenderStatus = _tenderStatus;

            await opService.UpdateTenderStatus(tenderScheduleVM);

            await GetEmplTrfList();

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;

            StateHasChanged();
        }

    }
}
