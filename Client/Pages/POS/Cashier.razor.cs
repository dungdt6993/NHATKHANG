using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Services.POS;
using D69soft.Client.Services.CRM;
using D69soft.Shared.Models.ViewModels.POS;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.CRM;
using D69soft.Client.Helpers;

namespace D69soft.Client.Pages.POS
{
    public partial class Cashier
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] CashierService cashierService { get; set; }
        [Inject] CustomerService customerService { get; set; }


        bool isLoading;

        bool isLoadingScreen = true;

        //private HubConnection hubConnection;

        //Filter
        FilterPosVM filterPosVM = new();

        //PointOfSale
        IEnumerable<PointOfSaleVM> pointOfSaleVMs;

        //RoomTable
        IEnumerable<RoomTableAreaVM> roomTableAreaList;
        IEnumerable<RoomTableVM> roomTableList;

        //Items
        List<ItemsVM> itemsVMs;
        List<ItemsVM> search_itemsVMs;

        //Invoice
        InvoiceVM infoInvoice = new();
        InvoiceVM invoiceTotal = new();
        IEnumerable<InvoiceVM> invoiceItemsList;

        //Payment
        PaymentVM payment = new();
        InvoiceVM invoiceDetail = new();
        IEnumerable<PaymentVM> customerAmountSuggestList;

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
            

            filterPosVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(filterPosVM.UserID, "POS_Cashier"))
            {
                await sysService.InsertLogUserFunc(filterPosVM.UserID, "POS_Cashier");
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            pointOfSaleVMs = await cashierService.GetPointOfSale();

            filterPosVM.POSCode = pointOfSaleVMs.Count()==1? pointOfSaleVMs.ElementAt(0).POSCode : String.Empty;

            if (!String.IsNullOrEmpty(filterPosVM.POSCode))
            {
                roomTableAreaList = await cashierService.GetRoomTableArea(filterPosVM.POSCode);
                roomTableList = await cashierService.GetRoomTable(filterPosVM);

                search_itemsVMs = itemsVMs = await cashierService.GetItems(filterPosVM);
            }

            //hubConnection = new HubConnectionBuilder()
            //        .WithUrl(navigationManager.ToAbsoluteUri("/cashierHub"))
            //        .Build();

            //hubConnection.On<string, string, string>("Receive_LoadRoomTable", (POSCode, roomTableID, userID) =>
            //{
            //    if (POSCode == filterPosVM.POSCode)
            //    {
            //        if (!String.IsNullOrEmpty(infoInvoice.CheckNo))
            //        {
            //            if (filterPosVM.UserID != userID && infoInvoice.RoomTableID == roomTableID && roomTableID != "TakeOut")
            //            {
            //                js.Swal_Message("" + infoInvoice.RoomTableAreaName + "/" + infoInvoice.RoomTableName + "", "Nhân viên mã <strong>#" + userID + "</strong> đang cập nhật!.", SweetAlertMessageType.warning);
            //                infoInvoice = new();
            //            }
            //        }

            //        FilterRoomTable(filterPosVM.RoomTableAreaID);
            //    }

            //    StateHasChanged();
            //});

            //await hubConnection.StartAsync();


            isLoadingScreen = false;
        }

        private async Task ChoosePointOfSale(string _posCode)
        {
            filterPosVM.POSCode = _posCode;

            roomTableAreaList = await cashierService.GetRoomTableArea(filterPosVM.POSCode);
            roomTableList = await cashierService.GetRoomTable(filterPosVM);

            search_itemsVMs = itemsVMs = await cashierService.GetItems(filterPosVM);
        }

        private async void FilterRoomTable(string _roomTableAreaID)
        {
            isLoading = true;

            filterPosVM.RoomTableAreaID = _roomTableAreaID;

            roomTableList = await cashierService.GetRoomTable(filterPosVM);

            isLoading = false;

            StateHasChanged();
        }

        private void FilterItems(string _iGrpCode)
        {
            isLoading = true;

            filterPosVM.searchValues = String.Empty;

            filterPosVM.IGrpCode = _iGrpCode;

            search_itemsVMs = itemsVMs.Where(x => x.IGrpCode.ToUpper().Contains(filterPosVM.IGrpCode.ToUpper())).ToList();

            isLoading = false;

            StateHasChanged();
        }

        private string onchange_SearchValues
        {
            get { return filterPosVM.searchValues; }
            set
            {
                filterPosVM.searchValues = value;
                search_itemsVMs = itemsVMs.Where(x => x.IGrpCode.ToUpper().Contains(filterPosVM.IGrpCode.ToUpper())).Where(x => x.IName.ToUpper().Contains(filterPosVM.searchValues.ToUpper())).ToList();
            }
        }

        private async Task OpenRoomTable(string _roomTableID, int _isOpen)
        {
            isLoading = true;

            filterPosVM.RoomTableID = _roomTableID;
            filterPosVM.isOpen = _isOpen;

            if (_isOpen == 1 && infoInvoice.IsClickChangeRoomTable == 1)
            {
                await js.Toast_Alert("Phòng/bàn đã được mở, vui lòng kiểm tra lại!", SweetAlertMessageType.warning);
            }
            else
            {
                await cashierService.OpenRoomTable(filterPosVM, infoInvoice);

                if (infoInvoice.IsClickChangeRoomTable == 1)
                {
                    await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
                }

                infoInvoice = await cashierService.GetInfoInvoice(filterPosVM);

                customer = infoInvoice.CustomerID != "" ? await customerService.GetCustomerByID(infoInvoice.CustomerID) : new();

                invoiceItemsList = await cashierService.GetInvoiceItems(infoInvoice.CheckNo);

                invoiceTotal = await cashierService.GetInvoiceTotal(infoInvoice.CheckNo);

                filterPosVM.ICode = String.Empty;

                //await hubConnection.SendAsync("Send_LoadRoomTable", filterPosVM.POSCode, filterPosVM.RoomTableID, filterPosVM.UserID);
            }

            isLoading = false;
        }

        private async Task OpenTakeOut()
        {
            isLoading = true;

            filterPosVM.RoomTableID = "TakeOut";

            await cashierService.OpenTakeOut(filterPosVM);

            if (infoInvoice.IsClickChangeRoomTable == 1 && infoInvoice.RoomTableID != "TakeOut")
            {
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

                //await hubConnection.SendAsync("Send_LoadRoomTable", filterPosVM.POSCode, infoInvoice.RoomTableID, filterPosVM.UserID);
            }

            infoInvoice = await cashierService.GetInfoInvoice(filterPosVM);

            customer = infoInvoice.CustomerID != "" ? await customerService.GetCustomerByID(infoInvoice.CustomerID) : new();

            invoiceItemsList = await cashierService.GetInvoiceItems(infoInvoice.CheckNo);

            invoiceTotal = await cashierService.GetInvoiceTotal(infoInvoice.CheckNo);

            filterPosVM.ICode = String.Empty;

            isLoading = false;
        }

        private async Task ChooseItems(string _iCode)
        {
            if (await js.Swal_Confirm(""+ itemsVMs.Where(x => x.ICode == _iCode).Select(x => x.IName).First() + " - " + String.Format("{0:#,##0.##}", itemsVMs.Where(x => x.ICode == _iCode).Select(x => x.IPrice).First()) + "", $"Bạn có muốn cập nhật mặt hàng này?", SweetAlertMessageType.question))
            {
                filterPosVM.CheckNo = infoInvoice.CheckNo;
                filterPosVM.ICode = _iCode;

                await cashierService.ChooseItems(filterPosVM);

                invoiceItemsList = await cashierService.GetInvoiceItems(infoInvoice.CheckNo);

                invoiceTotal = await cashierService.GetInvoiceTotal(infoInvoice.CheckNo);

                filterPosVM.ReportName = "CustomNewReport";

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
        }

        private async Task DelInvoiceItems(int seq)
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
            {
                await cashierService.DelInvoiceItems(infoInvoice.CheckNo, seq);

                invoiceItemsList = await cashierService.GetInvoiceItems(infoInvoice.CheckNo);

                invoiceTotal = await cashierService.GetInvoiceTotal(infoInvoice.CheckNo);

                filterPosVM.ReportName = "CustomNewReport";
            }
        }

        private async Task DelInvoice()
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn muốn hủy?", SweetAlertMessageType.question))
            {
                await cashierService.DelInvoice(infoInvoice.CheckNo);

                //await hubConnection.SendAsync("Send_LoadRoomTable", filterPosVM.POSCode, infoInvoice.RoomTableID, filterPosVM.UserID);

                infoInvoice = new();
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

            invoiceItemsList = await cashierService.GetInvoiceItems(infoInvoice.CheckNo);
            invoiceTotal = await cashierService.GetInvoiceTotal(infoInvoice.CheckNo);

            filterPosVM.ICode = invoiceDetail.ICode;

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_UpdateInvoiceDetail");

            filterPosVM.ReportName = "CustomNewReport";

            isLoading = false;
        }

        private async Task CloseModal_UpdateInvoiceDetail()
        {
            isLoading = true;

            invoiceItemsList = await cashierService.GetInvoiceItems(infoInvoice.CheckNo);

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

            invoiceTotal = await cashierService.GetInvoiceTotal(infoInvoice.CheckNo);

            filterPosVM.ReportName = "CustomNewReport";

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_InvoiceDiscount");
            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;
        }

        private async Task CloseModal_InvoiceDiscount()
        {
            isLoading = true;

            invoiceTotal = await cashierService.GetInvoiceTotal(infoInvoice.CheckNo);

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

            customer.CustomerID = infoInvoice.CustomerID = await customerService.UpdateCustomer(customer);
            await cashierService.UpdateInvoiceCustomer(infoInvoice);

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
                infoInvoice.CustomerID = result.CustomerID;
            }
            else
            {
                customer = new();
                infoInvoice.CustomerID = "";
            }
            await cashierService.UpdateInvoiceCustomer(infoInvoice);
        }

        private async Task ChangeRoomTableAsync()
        {
            isLoading = true;

            infoInvoice.IsClickChangeRoomTable = 1;

            await js.Toast_Alert("Chọn phòng/bàn trống để chuyển!", SweetAlertMessageType.warning);

            isLoading = false;
        }

        private async Task InitializeModal_Payment()
        {
            isLoading = true;

            payment = new PaymentVM();

            payment.CheckNo = infoInvoice.CheckNo;

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

            await cashierService.SavePayment(payment, filterPosVM.POSCode, filterPosVM.UserID);

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_Payment");
            await js.Toast_Alert("Thanh toán thành công!", SweetAlertMessageType.success);

            //await hubConnection.SendAsync("Send_LoadRoomTable", filterPosVM.POSCode, infoInvoice.RoomTableID, filterPosVM.UserID);

            payment.isPayment = 1;

            infoInvoice = new();

            roomTableList = await cashierService.GetRoomTable(filterPosVM);

            isLoading = false;
        }

        private void ClickTabRoomTable()
        {
            infoInvoice = new();
        }

        private void ClickTabMenu()
        {
            infoInvoice.IsClickChangeRoomTable = 0;
        }


        protected async Task PrintInvoice()
        {
            filterPosVM.ReportName = "POS_PrintInvoice?CheckNo="+ infoInvoice.CheckNo+ "";
            await js.InvokeAsync<object>("ShowModal", "#InitializeModalView_Rpt");
        }

        private async ValueTask DisposeAsync()
        {
            //if (hubConnection is not null)
            //{
            //    await hubConnection.DisposeAsync();
            //}
        }
    }
}
