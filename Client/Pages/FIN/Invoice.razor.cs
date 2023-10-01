using BlazorDateRangePicker;
using Blazored.Typeahead;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;
using D69soft.Client.Services.FIN;
using D69soft.Client.Services.HR;
using D69soft.Client.Services;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Utilities;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using D69soft.Server.Services.HR;
using System.Text;

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

        protected string UserID;

        LogVM logVM = new();

        //Filter
        FilterFinVM filterFinVM = new();
        FilterHrVM filterHrVM = new();

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
            filterHrVM.UserID = UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "FIN_Invoice"))
            {
                logVM.LogUser = UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "FIN_Invoice";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            filter_divisionVMs = await organizationalChartService.GetDivisionList(filterHrVM);
            filterFinVM.DivisionID = (await sysService.GetInfoUser(UserID)).DivisionID;

            filterFinVM.VTypeID = "FIN_Purchasing";

            filterFinVM.StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            filterFinVM.EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddTicks(-1);

            await GetInvoices();

            isLoadingScreen = false;
        }

        private async void onchange_DivisionID(string value)
        {
            isLoading = true;

            filterFinVM.DivisionID = value;

            await GetInvoices();

            isLoading = false;

            StateHasChanged();
        }

        public async Task OnRangeSelect(DateRange _range)
        {
            filterFinVM.StartDate = _range.Start;
            filterFinVM.EndDate = _range.End;

            await GetInvoices();
        }

        private async Task GetInvoices()
        {
            isLoading = true;

            invoiceVMs = await voucherService.GetInvoices(filterFinVM);

            isLoading = false;
        }

        //RPT
        protected async Task ViewRPT(string _ReportName)
        {
            isLoading = true;

            filterFinVM.TypeView = 3;

            if (_ReportName == "FIN_So_chi_tiet_hoa_don")
            {
                invoiceBooks = await voucherService.GetInvoiceBooks(filterFinVM);
            }

            isLoading = false;
        }

        //Export
        public void ExportToExcel(string htmlTable)
        {
            return File(Encoding.ASCII.GetBytes(htmlTable),"application/vnd.ms-excel","htmlTable.xlsx");
        }
    }
}
