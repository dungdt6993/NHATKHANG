using D69soft.Client.Extensions;
using D69soft.Client.Services.FIN;
using D69soft.Client.Services;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace D69soft.Client.Pages.FIN
{
    partial class Cashier
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] CashierService cashierService { get; set; }
        [Inject] CustomerService customerService { get; set; }

        [Inject] InventoryService inventoryService { get; set; }

        [Inject] IConfiguration configuration { get; set; }
        private HubConnection hubConnection;

        //private HubConnection hubConnection;

        bool isLoading;
        bool isLoadingScreen = true;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        //Stock - POS
        List<StockVM> stockVMs;

        //RoomTable
        IEnumerable<RoomTableAreaVM> roomTableAreaList;
        IEnumerable<RoomTableVM> roomTableList;

        //Items
        List<ItemsVM> itemsVMs;
        List<ItemsVM> search_itemsVMs;

        //Voucher
        VoucherVM voucherVM = new();
        List<VoucherVM> voucherVMs;

        //VoucherDetail
        VoucherDetailVM voucherDetailVM = new();
        List<VoucherDetailVM> voucherDetailVMs;

        //Customer
        CustomerVM customer = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("openWinNewTab");
            }
            await js.InvokeAsync<object>("maskCurrency");
            await js.InvokeAsync<object>("maskPercent"); ;
        }

        protected override async Task OnInitializedAsync()
        {
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(filterVM.UserID, "FIN_Cashier"))
            {
                logVM.LogUser = filterVM.UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "FIN_Cashier";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Stock
            stockVMs = (await inventoryService.GetStockList()).ToList();

            filterVM.StockCode = stockVMs.Count() == 1 ? stockVMs.ElementAt(0).StockCode : String.Empty;

            if (!String.IsNullOrEmpty(filterVM.StockCode))
            {
                roomTableAreaList = await cashierService.GetRoomTableArea(filterVM.StockCode);
                roomTableList = await cashierService.GetRoomTable(filterVM);

                search_itemsVMs = itemsVMs = await cashierService.GetItems(filterVM);
            }

            hubConnection = new HubConnectionBuilder()
                    .WithUrl(configuration["BaseAddress"] + "cashierHub")
                    .Build();

            hubConnection.On<string, string, string>("Receive_LoadRoomTable", (StockCode, RoomTableCode, UserID) =>
            {
                if (StockCode == filterVM.StockCode)
                {
                    if (!String.IsNullOrEmpty(voucherVM.VNumber))
                    {
                        if (filterVM.UserID != UserID && voucherVM.RoomTableCode == RoomTableCode && RoomTableCode != "TakeOut")
                        {
                            js.Swal_Message("" + voucherVM.RoomTableAreaName + "/" + voucherVM.RoomTableName + "", "Nhân viên mã <strong>#" + UserID + "</strong> đang cập nhật!.", SweetAlertMessageType.warning);
                            voucherVM = new();
                        }
                    }

                    FilterRoomTable(filterVM.RoomTableAreaCode);
                }

                StateHasChanged();
            });

            await hubConnection.StartAsync();


            isLoadingScreen = false;
        }

        private async Task ChoosePointOfSale(string _posCode)
        {
            filterVM.StockCode = _posCode;

            roomTableAreaList = await cashierService.GetRoomTableArea(filterVM.StockCode);
            roomTableList = await cashierService.GetRoomTable(filterVM);

            search_itemsVMs = itemsVMs = await cashierService.GetItems(filterVM);
        }

        private async void FilterRoomTable(string _RoomTableAreaCode)
        {
            isLoading = true;

            filterVM.RoomTableAreaCode = _RoomTableAreaCode;

            roomTableList = await cashierService.GetRoomTable(filterVM);

            isLoading = false;

            StateHasChanged();
        }

        private void FilterItems(string _iGrpCode)
        {
            isLoading = true;

            filterVM.searchValues = String.Empty;

            filterVM.IGrpCode = _iGrpCode;

            search_itemsVMs = itemsVMs.Where(x => x.IGrpCode.ToUpper().Contains(filterVM.IGrpCode.ToUpper())).ToList();

            isLoading = false;

            StateHasChanged();
        }

        private string onchange_SearchValues
        {
            get { return filterVM.searchValues; }
            set
            {
                filterVM.searchValues = value;
                search_itemsVMs = itemsVMs.Where(x => x.IGrpCode.ToUpper().Contains(filterVM.IGrpCode.ToUpper())).Where(x => x.IName.ToUpper().Contains(filterVM.searchValues.ToUpper())).ToList();
            }
        }

        private async Task OpenRoomTable(string _RoomTableCode, bool _IsOpen)
        {
            isLoading = true;

            filterVM.RoomTableCode = _RoomTableCode;
            filterVM.IsOpen = _IsOpen;

            if (_IsOpen && voucherVM.IsClickChangeRoomTable)
            {
                await js.Toast_Alert("Phòng/bàn đã được mở, vui lòng kiểm tra lại!", SweetAlertMessageType.warning);
            }
            else
            {
                await cashierService.OpenRoomTable(filterVM, voucherVM);

                if (voucherVM.IsClickChangeRoomTable == 1)
                {
                    await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
                }

                voucherVM = await cashierService.GetInfoInvoice(filterVM);

                customer = voucherVM.CustomerID != "" ? await customerService.GetCustomerByID(voucherVM.CustomerID) : new();

                invoiceItemsList = await cashierService.GetInvoiceItems(voucherVM.VNumber);

                invoiceTotal = await cashierService.GetInvoiceTotal(voucherVM.VNumber);

                filterVM.ICode = String.Empty;

                await hubConnection.SendAsync("Send_LoadRoomTable", filterVM.StockCode, filterVM.RoomTableCode, filterVM.UserID);
            }

            isLoading = false;
        }

        private async Task OpenTakeOut()
        {
            isLoading = true;

            filterVM.RoomTableCode = "TakeOut";

            await cashierService.OpenTakeOut(filterVM);

            if (voucherVM.IsClickChangeRoomTable == 1 && voucherVM.RoomTableCode != "TakeOut")
            {
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

                await hubConnection.SendAsync("Send_LoadRoomTable", filterVM.StockCode, voucherVM.RoomTableCode, filterVM.UserID);
            }

            voucherVM = await cashierService.GetInfoInvoice(filterVM);

            customer = voucherVM.CustomerID != "" ? await customerService.GetCustomerByID(voucherVM.CustomerID) : new();

            invoiceItemsList = await cashierService.GetInvoiceItems(voucherVM.VNumber);

            invoiceTotal = await cashierService.GetInvoiceTotal(voucherVM.VNumber);

            filterVM.ICode = String.Empty;

            isLoading = false;
        }

        private async Task ChooseItems(string _iCode)
        {
            if (await js.Swal_Confirm("" + itemsVMs.Where(x => x.ICode == _iCode).Select(x => x.IName).First() + " - " + String.Format("{0:#,##0.##}", itemsVMs.Where(x => x.ICode == _iCode).Select(x => x.IPrice).First()) + "", $"Bạn có muốn cập nhật mặt hàng này?", SweetAlertMessageType.question))
            {
                filterVM.VNumber = voucherVM.VNumber;
                filterVM.ICode = _iCode;

                await cashierService.ChooseItems(filterVM);

                invoiceItemsList = await cashierService.GetInvoiceItems(voucherVM.VNumber);

                invoiceTotal = await cashierService.GetInvoiceTotal(voucherVM.VNumber);

                filterVM.ReportName = "CustomNewReport";

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
        }

        private async Task DelInvoiceItems(int seq)
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
            {
                await cashierService.DelInvoiceItems(voucherVM.VNumber, seq);

                invoiceItemsList = await cashierService.GetInvoiceItems(voucherVM.VNumber);

                invoiceTotal = await cashierService.GetInvoiceTotal(voucherVM.VNumber);

                filterVM.ReportName = "CustomNewReport";
            }
        }

        private async Task DelInvoice()
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn muốn hủy?", SweetAlertMessageType.question))
            {
                await cashierService.DelInvoice(voucherVM.VNumber);

                await hubConnection.SendAsync("Send_LoadRoomTable", filterVM.StockCode, voucherVM.RoomTableCode, filterVM.UserID);

                voucherVM = new();
            }

        }

        private void InitializeModal_UpdateInvoiceDetail(InvoiceVM _invoiceDetail)
        {
            isLoading = true;

            invoiceDetail = new InvoiceVM();

            invoiceDetail = _invoiceDetail;

            isLoading = false;
        }

        private int onchange_Qty
        {
            get { return invoiceDetail.Qty; }
            set
            {
                isLoading = true;

                invoiceDetail.Qty = value == 0 ? 1 : value;

                isLoading = false;
            }
        }

        private decimal onchange_ItemsDiscountPrice
        {
            get { return invoiceDetail.Items_DiscountPrice; }
            set
            {
                isLoading = true;

                invoiceDetail.Items_DiscountPrice = value > invoiceDetail.Price ? invoiceDetail.Price : value;

                invoiceDetail.Items_DiscountPercent = invoiceDetail.Price != 0 ? invoiceDetail.Items_DiscountPrice / invoiceDetail.Price * 100 : 0;

                isLoading = false;
            }
        }

        private decimal onchange_ItemsDiscountPercent
        {
            get { return invoiceDetail.Items_DiscountPercent; }
            set
            {
                isLoading = true;

                invoiceDetail.Items_DiscountPercent = value;

                invoiceDetail.Items_DiscountPrice = invoiceDetail.Price * invoiceDetail.Items_DiscountPercent / 100;

                isLoading = false;
            }
        }

        private void click_UpdateQty(string value)
        {
            isLoading = true;

            if (value == "+")
            {
                invoiceDetail.Qty++;
            }
            if (value == "-")
            {
                invoiceDetail.Qty = invoiceDetail.Qty > 1 ? invoiceDetail.Qty - 1 : 1;
            }

            isLoading = false;
        }

        private void click_ItemsDiscountPercentSuggest(int value)
        {
            isLoading = true;

            invoiceDetail.Items_DiscountPercent = value;

            invoiceDetail.Items_DiscountPrice = invoiceDetail.Price * invoiceDetail.Items_DiscountPercent / 100;

            isLoading = false;
        }

        private async Task UpdateInvoiceDetail()
        {
            isLoading = true;

            await cashierService.UpdateInvoiceDetail(invoiceDetail);

            invoiceItemsList = await cashierService.GetInvoiceItems(voucherVM.VNumber);
            invoiceTotal = await cashierService.GetInvoiceTotal(voucherVM.VNumber);

            filterVM.ICode = invoiceDetail.ICode;

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_UpdateInvoiceDetail");

            filterVM.ReportName = "CustomNewReport";

            isLoading = false;
        }

        private async Task CloseModal_UpdateInvoiceDetail()
        {
            isLoading = true;

            invoiceItemsList = await cashierService.GetInvoiceItems(voucherVM.VNumber);

            isLoading = false;
        }

        private void InitializeModal_InvoiceDiscount(InvoiceVM _invoiceDetail)
        {
            isLoading = true;

            invoiceDetail = new InvoiceVM();

            invoiceDetail = _invoiceDetail;

            isLoading = false;
        }

        private decimal onchange_InvoiceDiscountPrice
        {
            get { return invoiceDetail.Invoice_DiscountPrice; }
            set
            {
                isLoading = true;

                invoiceDetail.Invoice_DiscountPrice = value > invoiceDetail.sumAmount ? invoiceDetail.sumAmount : value;

                invoiceDetail.Invoice_DiscountPercent = invoiceDetail.sumAmount != 0 ? invoiceDetail.Invoice_DiscountPrice / invoiceDetail.sumAmount * 100 : 0;

                invoiceDetail.sumAmountPay = invoiceDetail.sumAmount - invoiceDetail.Invoice_DiscountPrice - invoiceDetail.Invoice_TaxPercent * invoiceDetail.sumAmount / 100;

                isLoading = false;
            }
        }

        private decimal onchange_InvoiceDiscountPercent
        {
            get { return invoiceDetail.Invoice_DiscountPercent; }
            set
            {
                isLoading = true;

                invoiceDetail.Invoice_DiscountPercent = value;

                invoiceDetail.Invoice_DiscountPrice = invoiceDetail.sumAmount * invoiceDetail.Invoice_DiscountPercent / 100;

                invoiceDetail.sumAmountPay = invoiceDetail.sumAmount - invoiceDetail.Invoice_DiscountPrice - invoiceDetail.Invoice_TaxPercent * invoiceDetail.sumAmount / 100;

                isLoading = false;
            }
        }

        private void click_InvoiceDiscountPercentSuggest(int value)
        {
            isLoading = true;

            invoiceDetail.Invoice_DiscountPercent = value;

            invoiceDetail.Invoice_DiscountPrice = invoiceDetail.sumAmount * invoiceDetail.Invoice_DiscountPercent / 100;

            invoiceDetail.sumAmountPay = invoiceDetail.sumAmount - invoiceDetail.Invoice_DiscountPrice - invoiceDetail.Invoice_TaxPercent * invoiceDetail.sumAmount / 100;

            isLoading = false;
        }

        private async Task UpdateInvoiceDiscount()
        {
            isLoading = true;

            await cashierService.UpdateInvoiceDiscount(invoiceDetail);

            invoiceTotal = await cashierService.GetInvoiceTotal(voucherVM.VNumber);

            filterVM.ReportName = "CustomNewReport";

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_InvoiceDiscount");
            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;
        }

        private async Task CloseModal_InvoiceDiscount()
        {
            isLoading = true;

            invoiceTotal = await cashierService.GetInvoiceTotal(voucherVM.VNumber);

            isLoading = false;
        }

        private void InitializeModal_Customer(int typeUpdate)
        {
            isLoading = true;

            if (typeUpdate == 0)
            {
                customer = new CustomerVM();
                customer.IsTypeUpdate = 0;
            }
            else
            {
                customer.IsTypeUpdate = 1;
            }

            isLoading = false;
        }

        private async Task UpdateCustomer()
        {
            isLoading = true;

            customer.CustomerID = voucherVM.CustomerID = await customerService.UpdateCustomer(customer);
            await cashierService.UpdateInvoiceCustomer(voucherVM);

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_Customer");
            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;
        }

        private async Task<IEnumerable<CustomerVM>> SearchCustomer(string searchText)
        {
            return await customerService.SearchCustomers(searchText);
        }

        private async Task SelectedCustomer(CustomerVM result)
        {
            if (result != null)
            {
                customer = result;
                voucherVM.CustomerID = result.CustomerID;
            }
            else
            {
                customer = new();
                voucherVM.CustomerID = "";
            }
            await cashierService.UpdateInvoiceCustomer(voucherVM);
        }

        private async Task ChangeRoomTableAsync()
        {
            isLoading = true;

            voucherVM.IsClickChangeRoomTable = 1;

            await js.Toast_Alert("Chọn phòng/bàn trống để chuyển!", SweetAlertMessageType.warning);

            isLoading = false;
        }

        private async Task InitializeModal_Payment()
        {
            isLoading = true;

            payment = new PaymentVM();

            payment.VNumber = voucherVM.VNumber;

            payment.PModeCode = "CASH";

            payment.sumAmountPay = payment.CustomerAmount = invoiceTotal.sumAmountPay;

            customerAmountSuggestList = await cashierService.GetCustomerAmountSuggest(invoiceTotal.sumAmountPay);

            isLoading = false;
        }

        private decimal onchange_CustomerAmount
        {
            get { return payment.CustomerAmount; }
            set
            {
                isLoading = true;

                payment.CustomerAmount = value;

                payment.ReturnAmount = payment.sumAmountPay - payment.CustomerAmount;

                isLoading = false;
            }
        }

        private void click_CalculatorUpdateInvoiceDtl(string value)
        {
            isLoading = true;

            switch (value)
            {
                case "1":
                    payment.CustomerAmount = payment.CustomerAmount.ToString().Length < 9 ? payment.CustomerAmount * 10 + 1 : payment.CustomerAmount;
                    break;
                case "2":
                    payment.CustomerAmount = payment.CustomerAmount.ToString().Length < 9 ? payment.CustomerAmount * 10 + 2 : payment.CustomerAmount; ;
                    break;
                case "3":
                    payment.CustomerAmount = payment.CustomerAmount.ToString().Length < 9 ? payment.CustomerAmount * 10 + 3 : payment.CustomerAmount; ;
                    break;
                case "4":
                    payment.CustomerAmount = payment.CustomerAmount.ToString().Length < 9 ? payment.CustomerAmount * 10 + 4 : payment.CustomerAmount; ;
                    break;
                case "5":
                    payment.CustomerAmount = payment.CustomerAmount.ToString().Length < 9 ? payment.CustomerAmount * 10 + 5 : payment.CustomerAmount; ;
                    break;
                case "6":
                    payment.CustomerAmount = payment.CustomerAmount.ToString().Length < 9 ? payment.CustomerAmount * 10 + 6 : payment.CustomerAmount; ;
                    break;
                case "7":
                    payment.CustomerAmount = payment.CustomerAmount.ToString().Length < 9 ? payment.CustomerAmount * 10 + 7 : payment.CustomerAmount; ;
                    break;
                case "8":
                    payment.CustomerAmount = payment.CustomerAmount.ToString().Length < 9 ? payment.CustomerAmount * 10 + 8 : payment.CustomerAmount; ;
                    break;
                case "9":
                    payment.CustomerAmount = payment.CustomerAmount.ToString().Length < 9 ? payment.CustomerAmount * 10 + 9 : payment.CustomerAmount; ;
                    break;
                case "0":
                    payment.CustomerAmount = payment.CustomerAmount.ToString().Length < 9 ? payment.CustomerAmount * 10 : payment.CustomerAmount; ;
                    break;
                case "00":
                    payment.CustomerAmount = payment.CustomerAmount.ToString().Length < 8 ? payment.CustomerAmount * 100 : payment.CustomerAmount;
                    break;
                case "000":
                    payment.CustomerAmount = payment.CustomerAmount.ToString().Length < 6 ? payment.CustomerAmount * 1000 : payment.CustomerAmount;
                    break;
                case "x":
                    payment.CustomerAmount = (payment.CustomerAmount - int.Parse(payment.CustomerAmount.ToString().Substring(payment.CustomerAmount.ToString().Length - 1, 1))) / 10;
                    break;
                case "c":
                    payment.CustomerAmount = 0;
                    break;
            }

            payment.ReturnAmount = payment.sumAmountPay - payment.CustomerAmount;

            isLoading = false;

        }

        private void click_PaymentMode(string value)
        {
            isLoading = true;

            payment.PModeCode = value;

            isLoading = false;
        }

        private void click_CustomerAmountSuggest(decimal value)
        {
            isLoading = true;

            payment.CustomerAmount = value;

            payment.ReturnAmount = payment.sumAmountPay - payment.CustomerAmount;

            isLoading = false;
        }

        private async Task SavePayment()
        {
            isLoading = true;

            await cashierService.SavePayment(payment, filterVM.StockCode, filterVM.UserID);

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_Payment");
            await js.Toast_Alert("Thanh toán thành công!", SweetAlertMessageType.success);

            await hubConnection.SendAsync("Send_LoadRoomTable", filterVM.StockCode, voucherVM.RoomTableCode, filterVM.UserID);

            payment.isPayment = 1;

            voucherVM = new();

            roomTableList = await cashierService.GetRoomTable(filterVM);

            isLoading = false;
        }

        private void ClickTabRoomTable()
        {
            voucherVM = new();
        }

        private void ClickTabMenu()
        {
            voucherVM.IsClickChangeRoomTable = 0;
        }

        protected async Task PrintInvoice()
        {
            filterVM.ReportName = "POS_PrintInvoice?VNumber=" + voucherVM.VNumber + "";
            await js.InvokeAsync<object>("ShowModal", "#InitializeModalView_Rpt");
        }
    }
}
