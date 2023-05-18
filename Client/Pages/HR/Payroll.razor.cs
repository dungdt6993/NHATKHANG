using Data.Repositories.HR;
using Data.Repositories.SYSTEM;
using Model.ViewModels.FIN;
using Model.ViewModels.HR;
using WebApp.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Data;

namespace WebApp.Pages.HR
{
    partial class Payroll
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        [Inject] SysRepository sysRepo { get; set; }

        [Inject] OrganizationalChartService organizationalChartRepo { get; set; }
        [Inject] DutyRosterService dutyRosterRepo { get; set; }
        [Inject] PayrollService payrollRepo { get; set; }

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

        IEnumerable<SalaryTransactionGroupVM> trngrp_filter_list;
        IEnumerable<SalaryTransactionCodeVM> trn_filter_list;

        //Payroll
        DataTable dtPayroll;

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
            isLoadingPage = true;

            UserID = (await authenticationStateTask).User.GetUserId();

            if (sysRepo.checkPermisFunc(UserID, "HR_DutyRoster"))
            {
                await sysRepo.insert_LogUserFunc(UserID, "HR_DutyRoster");
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            filterHrVM.UserID = UserID;

            year_filter_list = await sysRepo.GetYearFilter();
            filterHrVM.Year = DateTime.Now.Year;

            month_filter_list = await sysRepo.GetMonthFilter();
            filterHrVM.Month = DateTime.Now.Month;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

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

            trngrp_filter_list = await payrollRepo.GetTrnGroupCodeList();

            //DataExcel
            filterHrVM.strDataFromExcel = string.Empty;

            //LockSal
            lockSalaryVM = await payrollRepo.GetLockSalary(filterHrVM);

            isLoadingPage = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterHrVM.Month = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            //LockSal
            lockSalaryVM = await payrollRepo.GetLockSalary(filterHrVM);

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            filterHrVM.TrnCode = 0;
            filterHrVM.TrnSubCode = 0;

            dtPayroll = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            filterHrVM.Year = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            //LockSal
            lockSalaryVM = await payrollRepo.GetLockSalary(filterHrVM);

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            filterHrVM.TrnCode = 0;
            filterHrVM.TrnSubCode = 0;

            dtPayroll = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterHrVM.DivisionID = value;

            //LockSal
            lockSalaryVM = await payrollRepo.GetLockSalary(filterHrVM);

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartRepo.GetDepartmentList(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            filterHrVM.TrnCode = 0;
            filterHrVM.TrnSubCode = 0;

            dtPayroll = null;

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

            dtPayroll = null;

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

            dtPayroll = null;

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

                dtPayroll = null;

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

            dtPayroll = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_trncode(int value)
        {
            isLoading = true;

            filterHrVM.TrnCode = value;

            filterHrVM.TrnSubCode = 0;
            trn_filter_list = await payrollRepo.GetTrnCodeList(filterHrVM.TrnCode);

            dtPayroll = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_trnsubcode(int value)
        {
            isLoading = true;

            filterHrVM.TrnSubCode = value;

            dtPayroll = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void GetSalaryList()
        {
            isLoading = true;

            await Task.Yield();

            dtPayroll = payrollRepo.GetPayrollList(filterHrVM);

            isLoading = false;
            StateHasChanged();
        }

        private async Task CalcSalary()
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn tính lương Tháng " + filterHrVM.Month + " năm " + filterHrVM.Year + "?", SweetAlertMessageType.question))
            {
                dtPayroll = null;
                StateHasChanged();

                await payrollRepo.CalcSalary(filterHrVM);

                dtPayroll = payrollRepo.GetPayrollList(filterHrVM);

                lockSalaryVM = await payrollRepo.GetLockSalary(filterHrVM);

                isLoading = false;
                StateHasChanged();

                await js.Swal_Message("Thông báo.", "Tính lương thành công!", SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        private async Task CancelCalcSalary()
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn hủy tính lương?", SweetAlertMessageType.question))
            {
                await payrollRepo.CancelCalcSalary(filterHrVM);

                dtPayroll = null;

                lockSalaryVM = await payrollRepo.GetLockSalary(filterHrVM);

                await js.Toast_Alert("Hủy tính lương thành công!", SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        private async Task LockSalary()
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn khóa lương Tháng " + filterHrVM.Month + " năm " + filterHrVM.Year + "?", SweetAlertMessageType.question))
            {
                await payrollRepo.LockSalary(filterHrVM);

                lockSalaryVM = await payrollRepo.GetLockSalary(filterHrVM);

                await js.Swal_Message("Thông báo.", "Khóa lương thành công!", SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        private async Task CancelLockSalary()
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn hủy khóa lương?", SweetAlertMessageType.question))
            {
                await payrollRepo.CancelLockSalary(filterHrVM);

                lockSalaryVM = await payrollRepo.GetLockSalary(filterHrVM);

                await js.Toast_Alert("Hủy khóa lương thành công!", SweetAlertMessageType.success);
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

            salaryDefVMs = await payrollRepo.GetSalaryDefList();

            trngrp_list = await payrollRepo.GetTrnGroupCodeList();

            isLoading = false;
        }

        private async void onchange_trncode(int value)
        {
            isLoading = true;

            salaryDefVM.TrnCode = value;

            salaryDefVM.TrnSubCode = 0;
            trn_list = await payrollRepo.GetTrnCodeList(salaryDefVM.TrnCode);

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

            salaryDefVMs.ToList().ForEach(salaryDefVM => salaryDefVM.isNoShowBtnUpdate = true);

            _salaryDefVM.isUpdate = true;

            salaryDefVM = _salaryDefVM;
            salaryDefVM.isUpdate = true;

            trn_list = await payrollRepo.GetTrnCodeList(salaryDefVM.TrnCode);

            isLoading = false;
        }

        private async Task UpdateSalaryDef()
        {
            isLoading = true;

            if (!salaryDefVM.isUpdate)
            {
                salaryDefVMs = await payrollRepo.GetSalaryDefList();
            }

            if (salaryDefVM.isSave)
            {
                if(salaryDefVM.isCalcByShift)
                {
                    salaryDefVM.TrnCode = 0;
                    salaryDefVM.TrnSubCode = 0;
                }

                await payrollRepo.UpdateSalaryDef(salaryDefVM);
                salaryDefVMs = await payrollRepo.GetSalaryDefList();
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

            salaryTransactionCodeVMs = await payrollRepo.GetSalTrnCodeList();

            trngrp_list = await payrollRepo.GetTrnGroupCodeList();

            isLoading = false;
        }

        private async Task InitializeModalUpdate_SalTrnCode(int _isTypeUpdate, SalaryTransactionCodeVM _salaryTransactionCodeVM)
        {
            isLoading = true;

            salaryTransactionCodeVM = new();
            salaryTransactionCodeVMs = await payrollRepo.GetSalTrnCodeList();

            if (_isTypeUpdate == 0)
            {
                salaryTransactionCodeVM.Rate = 1;
            }

            if (_isTypeUpdate == 1)
            {
                salaryTransactionCodeVM = _salaryTransactionCodeVM;
            }

            salaryTransactionCodeVM.IsTypeUpdate = _isTypeUpdate;

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
                await payrollRepo.UpdateSalTrnCode(salaryTransactionCodeVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_SalTrnCode");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await payrollRepo.UpdateSalTrnCode(salaryTransactionCodeVM);

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

            salaryTransactionCodeVMs = await payrollRepo.GetSalTrnCodeList();

            isLoading = false;
        }

        private async Task CloseModalUpdate_SalTrnCode()
        {
            isLoading = true;

            salaryTransactionCodeVMs = await payrollRepo.GetSalTrnCodeList();

            isLoading = false;
        }

        //WDDefaut
        IEnumerable<WDDefaultVM> wdDefaultVMs;
        private async Task InitializeModalList_WDDefaut()
        {
            isLoading = true;

            wdDefaultVMs = await payrollRepo.GetWDDefautList(filterHrVM);

            isLoading = false;
        }

    }
}
