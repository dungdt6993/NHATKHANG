using BlazorDateRangePicker;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using D69soft.Client.Services.FIN;
using D69soft.Client.Services.HR;
using D69soft.Client.Services;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;
using D69soft.Client.XLS;
using static System.Net.WebRequestMethods;

namespace D69soft.Client.Pages.FIN
{
    partial class Invoice
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] VoucherService voucherService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        //Division
        IEnumerable<DivisionVM> filter_divisionVMs;

        //Invoice
        List<InvoiceVM> invoiceVMs;

        //InvoiceBooks
        List<InventoryVM> invoiceBooks;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
            await js.InvokeAsync<object>("tooltip");
        }

        protected override async Task OnInitializedAsync()
        {
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(filterVM.UserID, "FIN_Invoice"))
            {
                logVM.LogUser = filterVM.UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "FIN_Invoice";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            filter_divisionVMs = await organizationalChartService.GetDivisionList(filterVM);
            filterVM.DivisionID = (await sysService.GetInfoUser(filterVM.UserID)).DivisionID;

            filterVM.VTypeID = "FIN_Purchasing";

            filterVM.StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            filterVM.EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddTicks(-1);

            await GetInvoices();

            isLoadingScreen = false;
        }

        private async void onchange_DivisionID(string value)
        {
            isLoading = true;

            filterVM.DivisionID = value;

            await GetInvoices();

            isLoading = false;

            StateHasChanged();
        }

        public async Task OnRangeSelect(DateRange _range)
        {
            filterVM.StartDate = _range.Start;
            filterVM.EndDate = _range.End;

            await GetInvoices();
        }

        private async Task GetInvoices()
        {
            isLoading = true;

            filterVM.TypeView = filterVM.TypeView == 2 ? 0 : filterVM.TypeView;

            invoiceVMs = await voucherService.GetInvoices(filterVM);

            isLoading = false;
        }

        protected async Task onchangeTypeView(int _TypeView)
        {
            isLoading = true;

            filterVM.TypeView = _TypeView;

            invoiceVMs = await voucherService.GetInvoices(filterVM);

            isLoading = false;
        }

        //RPT
        protected async Task ViewRPT(string _ReportName)
        {
            isLoading = true;

            filterVM.TypeView = 2;

            if (_ReportName == "FIN_So_chi_tiet_hoa_don")
            {
                invoiceBooks = await voucherService.GetInvoiceBooks(filterVM);
            }

            isLoading = false;
        }

        //Export Excel
        HttpClient Http;
        private async void ClickTemplateXLS()
        {
            Stream streamTemplate = await Http.GetStreamAsync("xls/template.xlsx");

            var xls = new Excel();
            await xls.TemplateWeatherForecastAsync(js, streamTemplate, invoiceVMs, "template.xlsx");
        }

    }
}
