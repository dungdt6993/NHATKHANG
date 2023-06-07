using BlazorDateRangePicker;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Services.FIN;
using D69soft.Client.Services.POS;
using D69soft.Shared.Models.ViewModels.POS;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extension;

namespace D69soft.Client.Pages.POS
{
    partial class Invoices
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }

        [Inject] VoucherService voucherService { get; set; }
        [Inject] CashierService cashierService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        //PermisFunc
        bool POS_Invoices_Active;
        bool POS_Invoices_CancelActive;

        LogVM logVM = new();

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
            filterPosVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(filterPosVM.UserID, "POS_Invoices"))
            {
                logVM.LogUser = filterPosVM.UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "POS_Invoices";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

			POS_Invoices_Active = await sysService.CheckAccessSubFunc(filterPosVM.UserID, "POS_Invoices_Active");
			POS_Invoices_CancelActive = await sysService.CheckAccessSubFunc(filterPosVM.UserID, "POS_Invoices_CancelActive");

            pointOfSaleVMs = await cashierService.GetPointOfSale();

            filterPosVM.StartDate = DateTime.Now;
            filterPosVM.EndDate = DateTime.Now;

            filterPosVM.searchActive = 2;

            GetInvoices();

            isLoadingScreen = false;
        }

        private async void GetInvoices()
        {
            isLoading = true;

            invoiceVMs = await cashierService.GetInvoices(filterPosVM);

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

                await cashierService.ActiveInvoice(invoiceVM);

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

                    stockVoucherDetailVMs = await cashierService.QI_StockVoucherDetails(invoiceVM.CheckNo);

                    await voucherService.UpdateVoucher(stockVoucherVM, stockVoucherDetailVMs);
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

            await cashierService.SyncDataSmile();

            GetInvoices();

            isLoading = false;

            await js.Toast_Alert("Đồng bộ dữ liệu thành công!", SweetAlertMessageType.success);
        }
    }
}
