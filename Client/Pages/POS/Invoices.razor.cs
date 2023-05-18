using BlazorDateRangePicker;
using Data.Repositories.FIN;
using Data.Repositories.POS;
using Data.Repositories.SYSTEM;
using Model.ViewModels.FIN;
using Model.ViewModels.POS;
using WebApp.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace WebApp.Pages.POS
{
    partial class Invoices
    {
        [Inject] IJSRuntime js { get; set; }

        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        [Inject] SysRepository sysRepo { get; set; }

        [Inject] VoucherService voucherRepo { get; set; }
        [Inject] CashierService cashierRepo { get; set; }

        bool isLoading;

        bool isLoadingPage;

        //Filter
        FilterPosVM filterPosVM = new();

        //PointOfSale
        IEnumerable<PointOfSaleVM> pointOfSaleVMs;

        //Invoices
        InvoiceVM invoiceVM = new();
        List<InvoiceVM> invoiceVMs;

        //Voucher
        StockVoucherVM stockVoucherVM = new();
        List<StockVoucherDetailVM> stockVoucherDetailVMs;

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

            filterPosVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (sysRepo.checkPermisFunc(filterPosVM.UserID, "POS_Invoices"))
            {
                await sysRepo.insert_LogUserFunc(filterPosVM.UserID, "POS_Invoices");
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            pointOfSaleVMs = await cashierRepo.GetPointOfSale();

            filterPosVM.StartDate = DateTime.Now;
            filterPosVM.EndDate = DateTime.Now;

            filterPosVM.searchActive = 2;

            GetInvoices();

            isLoadingPage = false;
        }

        private async void GetInvoices()
        {
            isLoading = true;

            invoiceVMs = await cashierRepo.GetInvoices(filterPosVM);

            invoiceVM = new();

            filterPosVM.ReportName = "CustomNewReport";

            isLoading = false;

            StateHasChanged();
        }

        private void onclick_Selected(InvoiceVM _invoiceVM)
        {
            invoiceVM = _invoiceVM == invoiceVM ? new() : _invoiceVM;
        }

        private string SetSelected(InvoiceVM _invoiceVM)
        {
            if (invoiceVM.CheckNo != _invoiceVM.CheckNo)
            {
                return string.Empty;
            }
            return "selected";
        }

        private async void onchange_filter_POSCode(string value)
        {
            isLoading = true;

            filterPosVM.POSCode = value;

            GetInvoices();

            isLoading = false;
        }

        public async Task OnRangeSelect(DateRange _range)
        {
            filterPosVM.StartDate = _range.Start;
            filterPosVM.EndDate = _range.End;

            GetInvoices();
        }

        private int onchange_searchActive
        {
            get { return filterPosVM.searchActive; }
            set
            {
                filterPosVM.searchActive = value;

                GetInvoices();
            }
        }

        private async Task ActiveInvoice(bool _INVActive)
        {
            var question = _INVActive ? "Ghi sổ" : "Bỏ ghi sổ";

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn " + question + "?", SweetAlertMessageType.question))
            {
                invoiceVM.INVActive = _INVActive;

                await cashierRepo.ActiveInvoice(invoiceVM);

                if(invoiceVM.INVActive)
                {
                    //Tu dong tao phieu xuat ban hang hoa tu dinh luong thanh pham
                    stockVoucherVM = new();
                    stockVoucherDetailVMs = new();

                    stockVoucherVM.UserID = filterPosVM.UserID;
                    stockVoucherVM.DivisionID = pointOfSaleVMs.Where(x => x.POSCode == invoiceVM.POSCode).Select(x => x.DivisionID).First();

                    stockVoucherVM.IsTypeUpdate = 0;
                    stockVoucherVM.VTypeID = "STOCK_Output";
                    stockVoucherVM.VSubTypeID = "STOCK_Output_SalePOS";
                    stockVoucherVM.Reference_VNumber = invoiceVM.CheckNo;
                    stockVoucherVM.Reference_StockCode = pointOfSaleVMs.Where(x => x.POSCode == invoiceVM.POSCode).Select(x => x.StockCode).First();
                    stockVoucherVM.Reference_VSubTypeID = "POS_Cashier";
                    stockVoucherVM.VDesc = $"Xuất kho theo hóa đơn bán hàng - {invoiceVM.CheckNo} ";
                    stockVoucherVM.VDate = invoiceVM.IDate;

                    stockVoucherDetailVMs = await cashierRepo.QI_StockVoucherDetails(invoiceVM.CheckNo);

                    await voucherRepo.UpdateVoucher(stockVoucherVM, stockVoucherDetailVMs);
                }

                GetInvoices();

                await js.Toast_Alert("" + question + " thành công!", SweetAlertMessageType.success);
            }
        }

        string ReportName = String.Empty;

        protected async Task PrintInvoice()
        {
            filterPosVM.ReportName = "POS_PrintInvoice?CheckNo=" + invoiceVM.CheckNo + "";
            await js.InvokeAsync<object>("ShowModal", "#InitializeModalView_Rpt");
        }

        private async Task SyncDataSmile()
        {
            isLoading = true;

            await cashierRepo.SyncDataSmile();

            GetInvoices();

            isLoading = false;

            await js.Toast_Alert("Đồng bộ dữ liệu thành công!", SweetAlertMessageType.success);
        }
    }
}
