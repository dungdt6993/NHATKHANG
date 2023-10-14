using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;
using System.Data;

namespace D69soft.Client.Pages.HR
{
    partial class MonthlyIncome
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] DutyRosterService dutyRosterService { get; set; }
        [Inject] PayrollService payrollService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        //PermisFunc
        bool IsOpenFunc;

        IEnumerable<PeriodVM> year_filter_list;
        IEnumerable<PeriodVM> month_filter_list;
        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;
        IEnumerable<SectionVM> section_filter_list;
        IEnumerable<PositionVM> position_filter_list;
        IEnumerable<EserialVM> eserial_filter_list;

        IEnumerable<SalaryTransactionGroupVM> trngrp_filter_list;
        IEnumerable<SalaryTransactionCodeVM> trn_filter_list;

        //Monthly Income
        MonthlyIncomeTrnOtherVM monthlyIncomeTrnOtherVM = new();
        List<MonthlyIncomeTrnOtherVM> monthlyIncomeTrnOtherVMs;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
            await js.InvokeAsync<object>("maskCurrency");
        }

        protected override async Task OnInitializedAsync()
        {      
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(filterVM.UserID, "HR_DutyRoster"))
            {
                logVM.LogUser = filterVM.UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "HR_DutyRoster";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            monthlyIncomeTrnOtherVM.UserID = filterVM.UserID;

            year_filter_list = await sysService.GetYearFilter();
            filterVM.Year = DateTime.Now.Year;

            month_filter_list = await sysService.GetMonthFilter();
            filterVM.Month = DateTime.Now.Month;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

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

            trngrp_filter_list = await payrollService.GetTrnGroupCodeList();

            //DataExcel
            filterVM.strDataFromExcel = string.Empty;

            IsOpenFunc = await payrollService.IsOpenFunc(filterVM);

            isLoadingScreen = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterVM.Month = value;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            IsOpenFunc = await payrollService.IsOpenFunc(filterVM);

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            filterVM.TrnCode = 0;
            filterVM.TrnSubCode = 0;

            filterVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            filterVM.Year = value;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            IsOpenFunc = await payrollService.IsOpenFunc(filterVM);

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            filterVM.TrnCode = 0;
            filterVM.TrnSubCode = 0;

            filterVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterVM.DivisionID = value;

            IsOpenFunc = await payrollService.IsOpenFunc(filterVM);

            filterVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterVM);

            filterVM.PositionGroupID = string.Empty;
            filterVM.arrPositionID = new string[] { };

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            filterVM.TrnCode = 0;
            filterVM.TrnSubCode = 0;

            filterVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = null;

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

            filterVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = null;

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

            filterVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = null;

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

                monthlyIncomeTrnOtherVMs = null;

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

            filterVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = await payrollService.GetMonthlyIncomeTrnOtherList(filterVM);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_trncode(int value)
        {
            isLoading = true;

            filterVM.TrnCode = value;

            filterVM.TrnSubCode = 0;
            trn_filter_list = await payrollService.GetTrnCodeList(filterVM.TrnCode);

            isLoading = false;

            StateHasChanged();
        }

        private void onchange_filter_trnsubcode(int value)
        {
            isLoading = true;

            filterVM.TrnSubCode = value;

            isLoading = false;

            StateHasChanged();
        }

        private async Task GetMonthlyIncomeTrnOtherList(int _IsTypeSearch)
        {
            isLoading = true;

            filterVM.IsTypeSearch = _IsTypeSearch;

            if(_IsTypeSearch == 1)
            {
                filterVM.DepartmentID = string.Empty;
                filterVM.PositionGroupID = string.Empty;
                filterVM.arrPositionID = new string[] { };
                filterVM.SectionID = string.Empty;
                filterVM.Eserial = string.Empty;
                filterVM.TrnCode = 0;
                filterVM.TrnSubCode= 0;
            }

            filterVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = await payrollService.GetMonthlyIncomeTrnOtherList(filterVM);

            isLoading = false;
        }

        private void CheckAll(object checkValue)
        {
            bool isChecked = (bool)checkValue;
            filterVM.IsChecked = isChecked;

            monthlyIncomeTrnOtherVMs.ToList().ForEach(e => e.IsChecked = isChecked);
        }

        private async Task InitializeModalUpdate_MonthlyIncomeTrnOther(int _IsTypeUpdate, MonthlyIncomeTrnOtherVM _monthlyIncomeTrnOtherVM)
        {
            isLoading = true;

            monthlyIncomeTrnOtherVM = new();

            if (_IsTypeUpdate == 0)
            {
                if (filterVM.Eserial == string.Empty)
                {
                    await js.Toast_Alert("Chưa chọn nhân viên!", SweetAlertMessageType.warning);
                }
                else
                {
                    if (filterVM.TrnCode == 0 || filterVM.TrnSubCode == 0)
                    {
                        await js.Toast_Alert("Chưa chọn giao dịch!", SweetAlertMessageType.warning);
                    }
                    else
                    {
                        monthlyIncomeTrnOtherVM.Period = filterVM.Period;
                        monthlyIncomeTrnOtherVM.Eserial = filterVM.Eserial;
                        monthlyIncomeTrnOtherVM.TrnCode = filterVM.TrnCode;
                        monthlyIncomeTrnOtherVM.TrnSubCode = filterVM.TrnSubCode;
                        monthlyIncomeTrnOtherVM.Qty = 1;

                        await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_MonthlyIncomeTrnOther");
                    }
                }
            }

            if (_IsTypeUpdate == 1)
            {
                monthlyIncomeTrnOtherVM = _monthlyIncomeTrnOtherVM;

                await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_MonthlyIncomeTrnOther");
            }

            monthlyIncomeTrnOtherVM.UserID = filterVM.UserID;
            monthlyIncomeTrnOtherVM.IsTypeUpdate = _IsTypeUpdate;

            isLoading = false;
        }

        private async Task UpdateMITrnOther()
        {
            isLoading = true;

            await payrollService.UpdateMITrnOther(monthlyIncomeTrnOtherVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_MonthlyIncomeTrnOther");

            filterVM.IsChecked = false;
            filterVM.IsTypeSearch = 0;
            monthlyIncomeTrnOtherVMs = await payrollService.GetMonthlyIncomeTrnOtherList(filterVM);

            isLoading = false;
        }

        private async Task UpdateMITrnOtherByIsCheck(int _IsTypeUpdate)
        {
            isLoading = true;

            monthlyIncomeTrnOtherVM = new();

            monthlyIncomeTrnOtherVM.UserID = filterVM.UserID;
            monthlyIncomeTrnOtherVM.IsTypeUpdate = _IsTypeUpdate;

            monthlyIncomeTrnOtherVM.strSeqMITrnOther = string.Join(",",monthlyIncomeTrnOtherVMs.Select(x=> new { x.SeqMITrnOther, x.IsChecked }).Where(x => x.IsChecked).Select(x=>x.SeqMITrnOther));

            if (_IsTypeUpdate == 2)
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa dữ liệu đã chọn?", SweetAlertMessageType.question))
                {
                    await payrollService.UpdateMITrnOther(monthlyIncomeTrnOtherVM);

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Shift");
                    await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);

                    filterVM.IsChecked = false;
                    monthlyIncomeTrnOtherVMs = await payrollService.GetMonthlyIncomeTrnOtherList(filterVM);
                }
            }

            if (_IsTypeUpdate == 3)
            {
                if (filterVM.TrnCode == 0 || filterVM.TrnSubCode == 0)
                {
                    await js.Toast_Alert("Chưa chọn giao dịch!", SweetAlertMessageType.warning);
                }
                else
                {
                    monthlyIncomeTrnOtherVM.TrnCode = filterVM.TrnCode;
                    monthlyIncomeTrnOtherVM.TrnSubCode = filterVM.TrnSubCode;

                    await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_MonthlyIncomeTrnOther");
                }
            }

            isLoading = false;
        }

        private async Task GetDataPayrollFromExcel()
        {
            isLoading = true;

            if (await payrollService.GetDataMITrnOtherFromExcel(filterVM))
            {
                filterVM.IsChecked = false;
                await GetMonthlyIncomeTrnOtherList(1);
            }
            else
            {
                await js.Swal_Message("Thông báo!", "Dữ liệu không hợp lệ.", SweetAlertMessageType.error);
            }

            filterVM.strDataFromExcel = string.Empty;

            isLoading = false;
        }

    }
}
