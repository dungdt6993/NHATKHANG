﻿using Data.Repositories.HR;
using Data.Repositories.SYSTEM;
using Model.ViewModels.FIN;
using Model.ViewModels.HR;
using WebApp.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace WebApp.Pages.HR
{
    partial class MonthlyIncome
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
            filterHrVM.UserID = monthlyIncomeTrnOtherVM.UserID = UserID;

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

            isLoadingPage = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterHrVM.Month = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            filterHrVM.TrnCode = 0;
            filterHrVM.TrnSubCode = 0;

            filterHrVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            filterHrVM.Year = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await dutyRosterRepo.GetEserialByID(filterHrVM, UserID);

            filterHrVM.TrnCode = 0;
            filterHrVM.TrnSubCode = 0;

            filterHrVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = null;

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

            filterHrVM.TrnCode = 0;
            filterHrVM.TrnSubCode = 0;

            filterHrVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = null;

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

            filterHrVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = null;

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

            filterHrVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = null;

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

                monthlyIncomeTrnOtherVMs = null;

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

            filterHrVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = await payrollRepo.GetMonthlyIncomeTrnOtherList(filterHrVM);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_trncode(int value)
        {
            isLoading = true;

            filterHrVM.TrnCode = value;

            filterHrVM.TrnSubCode = 0;
            trn_filter_list = await payrollRepo.GetTrnCodeList(filterHrVM.TrnCode);

            isLoading = false;

            StateHasChanged();
        }

        private void onchange_filter_trnsubcode(int value)
        {
            isLoading = true;

            filterHrVM.TrnSubCode = value;

            isLoading = false;

            StateHasChanged();
        }

        private async Task GetMonthlyIncomeTrnOtherList(int _isTypeSearch)
        {
            isLoading = true;

            filterHrVM.isTypeSearch = _isTypeSearch;

            if(_isTypeSearch == 1)
            {
                filterHrVM.DepartmentID = string.Empty;
                filterHrVM.PositionGroupID = string.Empty;
                filterHrVM.arrPositionID = new string[] { };
                filterHrVM.SectionID = string.Empty;
                filterHrVM.Eserial = string.Empty;
                filterHrVM.TrnCode = 0;
                filterHrVM.TrnSubCode= 0;
            }

            filterHrVM.IsChecked = false;
            monthlyIncomeTrnOtherVMs = await payrollRepo.GetMonthlyIncomeTrnOtherList(filterHrVM);

            isLoading = false;
        }

        private void CheckAll(object checkValue)
        {
            bool isChecked = (bool)checkValue;
            filterHrVM.IsChecked = isChecked;

            monthlyIncomeTrnOtherVMs.ToList().ForEach(e => e.IsChecked = isChecked);
        }

        private async Task InitializeModalUpdate_MonthlyIncomeTrnOther(int _isTypeUpdate, MonthlyIncomeTrnOtherVM _monthlyIncomeTrnOtherVM)
        {
            isLoading = true;

            monthlyIncomeTrnOtherVM = new();

            if (_isTypeUpdate == 0)
            {
                if (filterHrVM.Eserial == string.Empty)
                {
                    await js.Toast_Alert("Chưa chọn nhân viên!", SweetAlertMessageType.warning);
                }
                else
                {
                    if (filterHrVM.TrnCode == 0 || filterHrVM.TrnSubCode == 0)
                    {
                        await js.Toast_Alert("Chưa chọn giao dịch!", SweetAlertMessageType.warning);
                    }
                    else
                    {
                        monthlyIncomeTrnOtherVM.Period = filterHrVM.Period;
                        monthlyIncomeTrnOtherVM.Eserial = filterHrVM.Eserial;
                        monthlyIncomeTrnOtherVM.TrnCode = filterHrVM.TrnCode;
                        monthlyIncomeTrnOtherVM.TrnSubCode = filterHrVM.TrnSubCode;
                        monthlyIncomeTrnOtherVM.Qty = 1;

                        await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_MonthlyIncomeTrnOther");
                    }
                }
            }

            if (_isTypeUpdate == 1)
            {
                monthlyIncomeTrnOtherVM = _monthlyIncomeTrnOtherVM;

                await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_MonthlyIncomeTrnOther");
            }

            monthlyIncomeTrnOtherVM.UserID = UserID;
            monthlyIncomeTrnOtherVM.IsTypeUpdate = _isTypeUpdate;

            isLoading = false;
        }

        private async Task UpdateMITrnOther()
        {
            isLoading = true;

            await payrollRepo.UpdateMITrnOther(monthlyIncomeTrnOtherVM);
            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_MonthlyIncomeTrnOther");

            filterHrVM.IsChecked = false;
            filterHrVM.isTypeSearch = 0;
            monthlyIncomeTrnOtherVMs = await payrollRepo.GetMonthlyIncomeTrnOtherList(filterHrVM);

            isLoading = false;
        }

        private async Task UpdateMITrnOtherByIsCheck(int _isTypeUpdate)
        {
            isLoading = true;

            monthlyIncomeTrnOtherVM = new();

            monthlyIncomeTrnOtherVM.UserID = UserID;
            monthlyIncomeTrnOtherVM.IsTypeUpdate = _isTypeUpdate;

            monthlyIncomeTrnOtherVM.strSeqMITrnOther = string.Join(",",monthlyIncomeTrnOtherVMs.Select(x=> new { x.SeqMITrnOther, x.IsChecked }).Where(x => x.IsChecked).Select(x=>x.SeqMITrnOther));

            if (_isTypeUpdate == 2)
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa dữ liệu đã chọn?", SweetAlertMessageType.question))
                {
                    await payrollRepo.UpdateMITrnOther(monthlyIncomeTrnOtherVM);

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Shift");
                    await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);

                    filterHrVM.IsChecked = false;
                    monthlyIncomeTrnOtherVMs = await payrollRepo.GetMonthlyIncomeTrnOtherList(filterHrVM);
                }
            }

            if (_isTypeUpdate == 3)
            {
                if (filterHrVM.TrnCode == 0 || filterHrVM.TrnSubCode == 0)
                {
                    await js.Toast_Alert("Chưa chọn giao dịch!", SweetAlertMessageType.warning);
                }
                else
                {
                    monthlyIncomeTrnOtherVM.TrnCode = filterHrVM.TrnCode;
                    monthlyIncomeTrnOtherVM.TrnSubCode = filterHrVM.TrnSubCode;

                    await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_MonthlyIncomeTrnOther");
                }
            }

            isLoading = false;
        }

        private async Task GetDataPayrollFromExcel()
        {
            isLoading = true;

            if (await payrollRepo.GetDataMITrnOtherFromExcel(filterHrVM))
            {
                filterHrVM.IsChecked = false;
                await GetMonthlyIncomeTrnOtherList(1);
            }
            else
            {
                await js.Swal_Message("Thông báo!", "Dữ liệu không hợp lệ.", SweetAlertMessageType.error);
            }

            filterHrVM.strDataFromExcel = string.Empty;

            isLoading = false;
        }

    }
}
