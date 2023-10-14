using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Data;
using D69soft.Client.Services;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Client.Pages.HR
{
    partial class Payroll
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
        bool HR_Payroll_Calc;

        IEnumerable<PeriodVM> year_filter_list;
        IEnumerable<PeriodVM> month_filter_list;
        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;
        IEnumerable<SectionVM> section_filter_list;
        IEnumerable<PositionVM> position_filter_list;
        IEnumerable<EserialVM> eserial_filter_list;

        IEnumerable<SalaryTransactionGroupVM> trngrp_filter_list;
        IEnumerable<SalaryTransactionCodeVM> trn_filter_list;

        //Payroll
        DataTable dtPayroll;

        DataTable dtCountShiftTypeCalc;
        DataTable dtCountTrnGrp;
        DataTable dtSalaryDef;
        DataTable dtShiftTypeCalc;
        DataTable dtTrn;
        DataTable dtContent_ShiftTypeCalc;
        DataTable dtContent_Trn;

        //LockSal
        LockSalaryVM lockSalaryVM = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
        }

        protected override async Task OnInitializedAsync()
        {
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(filterVM.UserID, "HR_Payroll"))
            {
				logVM.LogUser = filterVM.UserID;
				logVM.LogType = "FUNC";
                logVM.LogName = "HR_Payroll";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            HR_Payroll_Calc = await sysService.CheckAccessSubFunc(filterVM.UserID, "HR_Payroll_Calc");

			//Initialize Filter
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

            //LockSal
            lockSalaryVM = await payrollService.GetLockSalary(filterVM);

            isLoadingScreen = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterVM.Month = value;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            //LockSal
            lockSalaryVM = await payrollService.GetLockSalary(filterVM);

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            filterVM.TrnCode = 0;
            filterVM.TrnSubCode = 0;

            dtPayroll = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            filterVM.Year = value;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            //LockSal
            lockSalaryVM = await payrollService.GetLockSalary(filterVM);

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            filterVM.TrnCode = 0;
            filterVM.TrnSubCode = 0;

            dtPayroll = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterVM.DivisionID = value;

            //LockSal
            lockSalaryVM = await payrollService.GetLockSalary(filterVM);

            filterVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterVM);

            filterVM.PositionGroupID = string.Empty;
            filterVM.arrPositionID = new string[] { };

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterService.GetEserialByID(filterVM);

            filterVM.TrnCode = 0;
            filterVM.TrnSubCode = 0;

            dtPayroll = null;

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

            dtPayroll = null;

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

            dtPayroll = null;

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

                dtPayroll = null;

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

            dtPayroll = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_trncode(int value)
        {
            isLoading = true;

            filterVM.TrnCode = value;

            filterVM.TrnSubCode = 0;
            trn_filter_list = await payrollService.GetTrnCodeList(filterVM.TrnCode);

            dtPayroll = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_trnsubcode(int value)
        {
            isLoading = true;

            filterVM.TrnSubCode = value;

            dtPayroll = null;

            isLoading = false;

            StateHasChanged();
        }

        private async Task GetSalaryList()
        {
            isLoading = true;

            dtPayroll = await payrollService.GetPayrollList(filterVM);

            var sqlCountShiftTypeCalc = "select Count(ShiftTypeID) + 3 as cShiftTypeID from HR.ShiftType where coalesce(PercentIncome,0) > 0";
            dtCountShiftTypeCalc = await sysService.ExecuteSQLQueryToDataTable(sqlCountShiftTypeCalc);

            var sqlCountTrnGrp = "select '['+cast(stg.TrnGroupCode as varchar)+'] '+ stg.TrnGroupName as TrnGrp, count(TrnCode) as cTrnCode from HR.SalaryTransactionGroup stg ";
            sqlCountTrnGrp += "join HR.SalaryTransactionCode stc on stc.TrnGroupCode = stg.TrnGroupCode group by stg.TrnGroupCode, stg.TrnGroupName order by TrnGrp ";
            dtCountTrnGrp = await sysService.ExecuteSQLQueryToDataTable(sqlCountTrnGrp);

            var sqlSalaryDef = "select * from HR.SalaryDef";
            dtSalaryDef = await sysService.ExecuteSQLQueryToDataTable(sqlSalaryDef);

            var sqlShiftTypeCalc = "select ShiftTypeID, PercentIncome from HR.ShiftType where coalesce(PercentIncome,0) > 0";
            dtShiftTypeCalc = await sysService.ExecuteSQLQueryToDataTable(sqlShiftTypeCalc);

            var sqlTrn = "select '['+cast(TrnCode as varchar)+'-'+cast(TrnSubCode as varchar)+'] '+ [TrnName] as Trn from HR.SalaryTransactionCode  ";
            sqlTrn += "order by TrnCode, TrnSubCode ";
            dtTrn = await sysService.ExecuteSQLQueryToDataTable(sqlTrn);

            var sqlContent_ShiftTypeCalc = "select ShiftTypeID from HR.ShiftType where coalesce(PercentIncome,0) > 0";
            dtContent_ShiftTypeCalc = await sysService.ExecuteSQLQueryToDataTable(sqlContent_ShiftTypeCalc);

            var sqlContent_Trn = "select 'TRN'+cast(TrnCode as varchar)+'x'+cast(TrnSubCode as varchar)+'' as Trn from HR.SalaryTransactionCode  ";
            sqlContent_Trn += "order by TrnCode, TrnSubCode ";
            dtContent_Trn = await sysService.ExecuteSQLQueryToDataTable(sqlContent_Trn);

            isLoading = false;
        }

        private async Task CalcSalary()
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn tính lương Tháng " + filterVM.Month + " năm " + filterVM.Year + "?", SweetAlertMessageType.question))
            {
                await payrollService.CalcSalary(filterVM);

                await GetSalaryList();

                lockSalaryVM = await payrollService.GetLockSalary(filterVM);

                logVM.LogDesc = "Tính lương " + "Tháng " + filterVM.Month + " năm " + filterVM.Year + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        private async Task CancelCalcSalary()
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn hủy tính lương?", SweetAlertMessageType.question))
            {
                await payrollService.CancelCalcSalary(filterVM);

                dtPayroll = null;

                lockSalaryVM = await payrollService.GetLockSalary(filterVM);

                logVM.LogDesc = "Hủy tính lương " + "Tháng " + filterVM.Month + " năm " + filterVM.Year + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        private async Task LockSalary()
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn khóa lương Tháng " + filterVM.Month + " năm " + filterVM.Year + "?", SweetAlertMessageType.question))
            {
                await payrollService.LockSalary(filterVM);

                lockSalaryVM = await payrollService.GetLockSalary(filterVM);

                logVM.LogDesc = "Khóa lương " + "Tháng " + filterVM.Month + " năm " + filterVM.Year + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        private async Task CancelLockSalary()
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn hủy khóa lương?", SweetAlertMessageType.question))
            {
                await payrollService.CancelLockSalary(filterVM);

                lockSalaryVM = await payrollService.GetLockSalary(filterVM);

                logVM.LogDesc = "Hủy khóa lương " + "Tháng " + filterVM.Month + " năm " + filterVM.Year + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        //SalaryDef
        IEnumerable<SalaryDefVM> salaryDefVMs;
        SalaryDefVM salaryDefVM = new();

        IEnumerable<SalaryTransactionGroupVM> trngrp_list;
        IEnumerable<SalaryTransactionCodeVM> trn_list;
        private async Task InitializeModalUpdate_SalaryDef()
        {
            isLoading = true;

            salaryDefVMs = await payrollService.GetSalaryDefList();

            trngrp_list = await payrollService.GetTrnGroupCodeList();

            isLoading = false;
        }

        private async void onchange_trncode(int value)
        {
            isLoading = true;

            salaryDefVM.TrnCode = value;

            salaryDefVM.TrnSubCode = 0;
            trn_list = await payrollService.GetTrnCodeList(salaryDefVM.TrnCode);

            isLoading = false;

            StateHasChanged();
        }

        private void onchange_trnsubcode(int value)
        {
            isLoading = true;

            salaryDefVM.TrnSubCode = value;

            isLoading = false;
        }

        private async Task ClickUpdateSalaryDef(SalaryDefVM _salaryDefVM)
        {
            isLoading = true;

            _salaryDefVM.isUpdate = true;

            salaryDefVM = _salaryDefVM;
            salaryDefVM.isUpdate = true;

            trn_list = await payrollService.GetTrnCodeList(salaryDefVM.TrnCode);

            isLoading = false;
        }

        private async Task UpdateSalaryDef()
        {
            isLoading = true;

            if (!salaryDefVM.isUpdate)
            {
                salaryDefVMs = await payrollService.GetSalaryDefList();
            }

            if (salaryDefVM.isSave)
            {
                await payrollService.UpdateSalaryDef(salaryDefVM);
                salaryDefVMs = await payrollService.GetSalaryDefList();
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        //SalTrnCode
        IEnumerable<SalaryTransactionCodeVM> salaryTransactionCodeVMs;
        SalaryTransactionCodeVM salaryTransactionCodeVM = new();

        private async Task InitializeModalList_SalTrnCode()
        {
            isLoading = true;

            salaryTransactionCodeVMs = await payrollService.GetSalTrnCodeList();

            trngrp_list = await payrollService.GetTrnGroupCodeList();

            isLoading = false;
        }

        private async Task InitializeModalUpdate_SalTrnCode(int _IsTypeUpdate, SalaryTransactionCodeVM _salaryTransactionCodeVM)
        {
            isLoading = true;

            salaryTransactionCodeVM = new();
            salaryTransactionCodeVMs = await payrollService.GetSalTrnCodeList();

            if (_IsTypeUpdate == 0)
            {
                salaryTransactionCodeVM.Rate = 1;
            }

            if (_IsTypeUpdate == 1)
            {
                salaryTransactionCodeVM = _salaryTransactionCodeVM;
            }

            salaryTransactionCodeVM.IsTypeUpdate = _IsTypeUpdate;

            isLoading = false;
        }

        private void onchange_trngrpcode(int value)
        {
            isLoading = true;

            salaryTransactionCodeVM.TrnGroupCode = value;

            isLoading = false;
        }

        private void onchange_TypePIT(ChangeEventArgs args)
        {
            isLoading = true;

            salaryTransactionCodeVM.TypePIT = int.Parse(args.Value.ToString());

            if(salaryTransactionCodeVM.TypePIT == 0)
            {
                salaryTransactionCodeVM.isPIT = false;
                salaryTransactionCodeVM.RatePIT = 0;
            }
            if (salaryTransactionCodeVM.TypePIT == 1)
            {
                salaryTransactionCodeVM.isPIT = true;
                salaryTransactionCodeVM.RatePIT = 1;
            }
            if (salaryTransactionCodeVM.TypePIT == 2)
            {
                salaryTransactionCodeVM.isPIT = true;
                salaryTransactionCodeVM.RatePIT = -1;
            }

            isLoading = false;
        }

        private void onchange_Rate(ChangeEventArgs args)
        {
            isLoading = true;

            salaryTransactionCodeVM.Rate = int.Parse(args.Value.ToString());

            isLoading = false;
        }

        private async Task UpdateSalTrnCode()
        {
            isLoading = true;

            if (salaryTransactionCodeVM.IsTypeUpdate != 2)
            {
                await payrollService.UpdateSalTrnCode(salaryTransactionCodeVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_SalTrnCode");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await payrollService.UpdateSalTrnCode(salaryTransactionCodeVM);

                    if (affectedRows > 0)
                    {
                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_SalTrnCode");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu lương liên quan.", SweetAlertMessageType.error);
                        salaryTransactionCodeVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    salaryTransactionCodeVM.IsTypeUpdate = 1;
                }
            }

            salaryTransactionCodeVMs = await payrollService.GetSalTrnCodeList();

            isLoading = false;
        }

        private async Task CloseModalUpdate_SalTrnCode()
        {
            isLoading = true;

            salaryTransactionCodeVMs = await payrollService.GetSalTrnCodeList();

            isLoading = false;
        }

        //WDDefaut
        IEnumerable<WDDefaultVM> wdDefaultVMs;
        private async Task InitializeModalList_WDDefaut()
        {
            isLoading = true;

            wdDefaultVMs = await payrollService.GetWDDefautList(filterVM);

            isLoading = false;
        }

    }
}
