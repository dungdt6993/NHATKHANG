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
        [Inject] VoucherService voucherService { get; set; }
        [Inject] CustomerService customerService { get; set; }
        [Inject] InventoryService inventoryService { get; set; }

        [Inject] IConfiguration configuration { get; set; }
        private HubConnection hubConnection;

        bool isLoading;
        bool isLoadingScreen = true;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        //Stock - POS
        List<StockVM> stockVMs;

        //RoomTable
        IEnumerable<RoomTableVM> roomTableVMs;

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
        CustomerVM customerVM = new();

        //Payment
        IEnumerable<VoucherVM> amountSuggests;

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

            filterVM.DivisionID = (await sysService.GetInfoUser(filterVM.UserID)).DivisionID;

            //Stock
            stockVMs = (await inventoryService.GetStockList()).ToList();

            filterVM.StockCode = stockVMs.Count() == 1 ? stockVMs.ElementAt(0).StockCode : String.Empty;

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
                            js.Swal_Message("" + voucherVM.RoomTableName + "", "Nhân viên mã <strong>#" + UserID + "</strong> đang cập nhật!.", SweetAlertMessageType.warning);
                            voucherVM = new();
                        }
                    }

                    FilterRoomTable();
                }

                StateHasChanged();
            });

            await hubConnection.StartAsync();


            isLoadingScreen = false;
        }

        private async Task ChoosePointOfSale(string _StockCode)
        {
            isLoading = true;

            filterVM.StockCode = _StockCode;

            roomTableVMs = await voucherService.GetRoomTable(filterVM);

            filterVM.IActive = true;
            filterVM.IsSale = true;
            search_itemsVMs = itemsVMs = await inventoryService.GetItemsList(filterVM);

            isLoading = false;
        }

        private async void FilterRoomTable()
        {
            isLoading = true;

            roomTableVMs = await voucherService.GetRoomTable(filterVM);

            isLoading = false;

            StateHasChanged();
        }

        private void FilterItems(string _IGrpCode)
        {
            isLoading = true;

            filterVM.searchValues = String.Empty;

            filterVM.IGrpCode = _IGrpCode;

            if(String.IsNullOrEmpty(filterVM.IGrpCode))
            {
                search_itemsVMs = itemsVMs;
            }
            else
            {
                search_itemsVMs = itemsVMs.Where(x => x.IGrpCode.ToUpper().Contains(filterVM.IGrpCode.ToUpper())).ToList();
            }

            isLoading = false;

            StateHasChanged();
        }

        private string onchange_SearchValues
        {
            get { return filterVM.searchValues; }
            set
            {
                filterVM.searchValues = value;
                search_itemsVMs = itemsVMs.Where(x => x.IName.ToUpper().Contains(filterVM.searchValues.ToUpper())).ToList();
            }
        }

        private async Task OpenRoomTable(RoomTableVM _roomTableVM)
        {
            isLoading = true;

            if (_roomTableVM.IsOpen)
            {
                if(voucherVM.IsClickChangeRoomTable)
                {
                    await js.Toast_Alert("Phòng/bàn đã được mở, vui lòng kiểm tra lại!", SweetAlertMessageType.warning);
                }
                else
                {
                    filterVM.FuncID = "FIN_Sale";
                    filterVM.VNumber = _roomTableVM.VNumber;

                    filterVM.StartDate = DateTimeOffset.MinValue;
                    filterVM.EndDate = DateTimeOffset.MaxValue;

                    voucherVM = (await voucherService.GetVouchers(filterVM)).First();
                    voucherDetailVMs = await voucherService.GetVoucherDetails(voucherVM.VNumber);
                }
            }
            else
            {
                if (!voucherVM.IsClickChangeRoomTable)
                {
                    voucherVM = new();
                    voucherDetailVMs = new();

                    voucherVM.VTypeID = "FIN_Sale";
                    voucherVM.IsTypeUpdate = 0;

                    voucherVM.DivisionID = filterVM.DivisionID;
                    voucherVM.VCode = "BH";
                    voucherVM.VDate = DateTime.Now;
                    voucherVM.VDesc = "Bán hàng POS";
                    voucherVM.CustomerCode = "KL";
                    voucherVM.PaymentTypeCode = "CASH";
                    voucherVM.StockCode = filterVM.StockCode;
                    voucherVM.RoomTableCode = _roomTableVM.RoomTableCode;
                    voucherVM.EserialPerform = filterVM.UserID;

                    voucherVM.VNumber = await voucherService.UpdateVoucher(voucherVM, voucherDetailVMs);

                    logVM.LogDesc = "Mở phòng/bàn #" + _roomTableVM.RoomTableCode + "";
                    await sysService.InsertLog(logVM);

                    await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);

                    filterVM.FuncID = "FIN_Sale";
                    filterVM.VNumber = voucherVM.VNumber;

                    filterVM.StartDate = DateTimeOffset.MinValue;
                    filterVM.EndDate = DateTimeOffset.MaxValue;

                    voucherVM = (await voucherService.GetVouchers(filterVM)).First();
                }
                else
                {
                    logVM.LogDesc = "Chuyển phòng/bàn #" + voucherVM.RoomTableCode + " sang #" + _roomTableVM.RoomTableCode + "";

                    voucherVM.RoomTableCode = _roomTableVM.RoomTableCode;
                    voucherVM.VNumber = await voucherService.UpdateVoucher(voucherVM, voucherDetailVMs);

                    voucherVM = (await voucherService.GetVouchers(filterVM)).First();

                    await sysService.InsertLog(logVM);

                    await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                }

                voucherVM.IsTypeUpdate = 1;

                await hubConnection.SendAsync("Send_LoadRoomTable", filterVM.StockCode, filterVM.RoomTableCode, filterVM.UserID);
            }

            isLoading = false;
        }

        private async Task OpenTakeOut()
        {
            isLoading = true;

            filterVM.RoomTableCode = "TakeOut";

            //await cashierService.OpenTakeOut(filterVM);

            //if (voucherVM.IsClickChangeRoomTable && voucherVM.RoomTableCode != "TakeOut")
            //{
            //    await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            //    await hubConnection.SendAsync("Send_LoadRoomTable", filterVM.StockCode, voucherVM.RoomTableCode, filterVM.UserID);
            //}

            //voucherVM = await cashierService.GetInfoInvoice(filterVM);

            //customer = voucherVM.CustomerCode != String.Empty ? await customerService.GetCustomerByID(voucherVM.CustomerCode) : new();

            //invoiceItemsList = await cashierService.GetInvoiceItems(voucherVM.VNumber);

            //invoiceTotal = await cashierService.GetInvoiceTotal(voucherVM.VNumber);

            //filterVM.ICode = String.Empty;

            isLoading = false;
        }

        private async Task ChooseItems(string _ICode)
        {
            isLoading = true;

            if (await js.Swal_Confirm("" + itemsVMs.Where(x => x.ICode == _ICode).Select(x => x.IName).First() + " - " + String.Format("{0:#,##0.##}", itemsVMs.Where(x => x.ICode == _ICode).Select(x => x.IPrice).First()) + "", $"Bạn có muốn chọn mặt hàng này?", SweetAlertMessageType.question))
            {
                filterVM.VNumber = voucherVM.VNumber;

                filterVM.searchItems = _ICode;
                voucherDetailVM = (await voucherService.GetSearchItems(filterVM)).First();

                voucherDetailVM.SeqVD = voucherDetailVMs.Count == 0 ? 1 : voucherDetailVMs.Select(x => x.SeqVD).Max() + 1;

                voucherDetailVM.VDQty = 1;

                voucherDetailVM.VATCode = String.Empty;
                voucherDetailVM.VATRate = 0;
                voucherDetailVM.FromStockCode = String.Empty;
                voucherDetailVM.ToStockCode = String.Empty;
                voucherDetailVM.InventoryCheck_StockCode = filterVM.StockCode;

                voucherDetailVM.VDAmount = Math.Round(voucherDetailVM.VDPrice * voucherDetailVM.VDQty, MidpointRounding.AwayFromZero);

                voucherDetailVM.VDDiscountAmount = Math.Round(voucherDetailVM.VDDiscountPercent * voucherDetailVM.VDAmount / 100, MidpointRounding.AwayFromZero);

                voucherDetailVMs.Add(voucherDetailVM);

                voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();

                voucherVM.IsTypeUpdate = 1;
                voucherVM.VNumber = await voucherService.UpdateVoucher(voucherVM, voucherDetailVMs);

                voucherDetailVMs = await voucherService.GetVoucherDetails(voucherVM.VNumber);

                filterVM.ReportName = "CustomNewReport";

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        private async Task onclick_removeItems(VoucherDetailVM _voucherDetailVM)
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn xóa?", SweetAlertMessageType.question))
            {
                voucherDetailVMs.Remove(_voucherDetailVM);

                voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();

                voucherVM.VNumber = await voucherService.UpdateVoucher(voucherVM, voucherDetailVMs);

                filterVM.ReportName = "CustomNewReport";

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
        }

        private async Task DelVoucher()
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn muốn hủy?", SweetAlertMessageType.question))
            {
                voucherVM.IsTypeUpdate = 2;

                await voucherService.UpdateVoucher(voucherVM, voucherDetailVMs);

                logVM.LogDesc = "Xóa hóa đơn bán hàng số " + voucherVM.VNumber + "";
                await sysService.InsertLog(logVM);

                await hubConnection.SendAsync("Send_LoadRoomTable", filterVM.StockCode, voucherVM.RoomTableCode, filterVM.UserID);

                voucherVM = new();
            }

        }

        private async Task InitializeModal_UpdateVoucherDetail(VoucherDetailVM _voucherDetailVM)
        {
            isLoading = true;

            voucherDetailVM = _voucherDetailVM;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModal_UpdateVoucherDetail");

            isLoading = false;
        }

        private decimal onchange_Qty
        {
            get { return voucherDetailVM.VDQty; }
            set
            {
                isLoading = true;

                voucherDetailVM.VDQty = value == 0 ? 1 : value;

                voucherDetailVM.VDAmount = Math.Round(voucherDetailVM.VDPrice * voucherDetailVM.VDQty, MidpointRounding.AwayFromZero);

                voucherDetailVM.VDDiscountAmount = Math.Round(voucherDetailVM.VDDiscountPercent * voucherDetailVM.VDAmount / 100, MidpointRounding.AwayFromZero);

                voucherDetailVM.VATAmount = Math.Round((voucherDetailVM.VDAmount - voucherDetailVM.VDDiscountAmount) * voucherDetailVM.VATRate, MidpointRounding.AwayFromZero);

                voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();

                isLoading = false;
            }
        }

        private void click_UpdateQty(string value)
        {
            isLoading = true;

            if (value == "+")
            {
                voucherDetailVM.VDQty++;
            }
            if (value == "-")
            {
                voucherDetailVM.VDQty = voucherDetailVM.VDQty > 1 ? voucherDetailVM.VDQty - 1 : 1;
            }

            voucherDetailVM.VDAmount = Math.Round(voucherDetailVM.VDPrice * voucherDetailVM.VDQty, MidpointRounding.AwayFromZero);

            voucherDetailVM.VDDiscountAmount = Math.Round(voucherDetailVM.VDDiscountPercent * voucherDetailVM.VDAmount / 100, MidpointRounding.AwayFromZero);

            voucherDetailVM.VATAmount = Math.Round((voucherDetailVM.VDAmount - voucherDetailVM.VDDiscountAmount) * voucherDetailVM.VATRate, MidpointRounding.AwayFromZero);

            voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();

            isLoading = false;
        }

        private decimal onchange_VDPrice
        {
            get { return voucherDetailVM.VDPrice; }
            set
            {
                isLoading = true;

                voucherDetailVM.VDPrice = value;

                voucherDetailVM.VDAmount = Math.Round(voucherDetailVM.VDPrice * voucherDetailVM.VDQty, MidpointRounding.AwayFromZero);

                voucherDetailVM.VDDiscountAmount = Math.Round(voucherDetailVM.VDDiscountPercent * voucherDetailVM.VDAmount / 100, MidpointRounding.AwayFromZero);

                voucherDetailVM.VATAmount = Math.Round((voucherDetailVM.VDAmount - voucherDetailVM.VDDiscountAmount) * voucherDetailVM.VATRate, MidpointRounding.AwayFromZero);

                voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();

                isLoading = false;
            }
        }

        private decimal onchange_VDDiscountAmount
        {
            get { return voucherDetailVM.VDDiscountAmount; }
            set
            {
                isLoading = true;

                voucherDetailVM.VDDiscountAmount = value > voucherDetailVM.VDAmount ? voucherDetailVM.VDDiscountAmount : value;

                voucherDetailVM.VDDiscountPercent = voucherDetailVM.VDDiscountAmount != 0 ? Math.Round(voucherDetailVM.VDDiscountAmount * 100 / voucherDetailVM.VDAmount, 2, MidpointRounding.AwayFromZero) : 0;

                voucherDetailVM.VATAmount = Math.Round((voucherDetailVM.VDAmount - voucherDetailVM.VDDiscountAmount) * voucherDetailVM.VATRate, MidpointRounding.AwayFromZero);

                voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();

                isLoading = false;
            }
        }

        private decimal onchange_VDDiscountPercent
        {
            get { return voucherDetailVM.VDDiscountPercent; }
            set
            {
                isLoading = true;

                voucherDetailVM.VDDiscountPercent = value > 100 ? voucherDetailVM.VDDiscountPercent : value;

                voucherDetailVM.VDDiscountPercent = Math.Round(value, 2, MidpointRounding.AwayFromZero);

                voucherDetailVM.VDDiscountAmount = Math.Round(voucherDetailVM.VDDiscountPercent * voucherDetailVM.VDAmount / 100, MidpointRounding.AwayFromZero);

                voucherDetailVM.VATAmount = Math.Round((voucherDetailVM.VDAmount - voucherDetailVM.VDDiscountAmount) * voucherDetailVM.VATRate, MidpointRounding.AwayFromZero);

                voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();

                isLoading = false;
            }
        }

        private void click_VDDiscountPercentSuggest(int value)
        {
            isLoading = true;

            voucherDetailVM.VDDiscountPercent = value;

            voucherDetailVM.VDDiscountAmount = Math.Round(voucherDetailVM.VDDiscountPercent * voucherDetailVM.VDAmount / 100, MidpointRounding.AwayFromZero);

            voucherDetailVM.VATAmount = Math.Round((voucherDetailVM.VDAmount - voucherDetailVM.VDDiscountAmount) * voucherDetailVM.VATRate, MidpointRounding.AwayFromZero);

            voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();

            isLoading = false;
        }

        private async Task UpdateVoucherDetail()
        {
            isLoading = true;

            voucherVM.VNumber = await voucherService.UpdateVoucher(voucherVM, voucherDetailVMs);

            voucherDetailVMs = await voucherService.GetVoucherDetails(voucherVM.VNumber);

            filterVM.SeqVD = voucherDetailVM.SeqVD;

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_UpdateVoucherDetail");

            filterVM.ReportName = "CustomNewReport";

            isLoading = false;
        }

        private async Task CloseModal_UpdateVoucherDetail()
        {
            isLoading = true;

            voucherDetailVMs = await voucherService.GetVoucherDetails(voucherVM.VNumber);

            isLoading = false;
        }

        private void InitializeModal_InvoiceDiscount(InvoiceVM _invoiceDetail)
        {
            isLoading = true;

            //invoiceDetail = new InvoiceVM();

            //invoiceDetail = _invoiceDetail;

            isLoading = false;
        }

        private decimal onchange_InvoiceDiscountPrice
        {
            get { return voucherDetailVM.VDDiscountAmount; }
            set
            {
                isLoading = true;

                //voucherDetailVM.VDDiscountAmount = value > voucherDetailVM.VDAmount ? voucherDetailVM.VDAmount : value;

                //voucherDetailVM.VDDiscountPercent = voucherDetailVM.VDAmount != 0 ? voucherDetailVM.VDDiscountAmount / voucherDetailVM.VDAmount * 100 : 0;

                //voucherDetailVM.sumAmountPay = voucherDetailVM.VDAmount - voucherDetailVM.VDDiscountAmount - voucherDetailVM.Invoice_TaxPercent * voucherDetailVM.sumAmount / 100;

                isLoading = false;
            }
        }

        private decimal onchange_InvoiceDiscountPercent
        {
            //get { return voucherDetailVM.Invoice_DiscountPercent; }
            set
            {
                isLoading = true;

                //voucherDetailVM.Invoice_DiscountPercent = value;

                //voucherDetailVM.Invoice_DiscountPrice = voucherDetailVM.sumAmount * voucherDetailVM.Invoice_DiscountPercent / 100;

                //voucherDetailVM.sumAmountPay = voucherDetailVM.sumAmount - voucherDetailVM.Invoice_DiscountPrice - voucherDetailVM.Invoice_TaxPercent * voucherDetailVM.sumAmount / 100;

                isLoading = false;
            }
        }

        private void click_InvoiceDiscountPercentSuggest(int value)
        {
            isLoading = true;

            //voucherDetailVM.Invoice_DiscountPercent = value;

            //voucherDetailVM.Invoice_DiscountPrice = voucherDetailVM.sumAmount * voucherDetailVM.Invoice_DiscountPercent / 100;

            //voucherDetailVM.sumAmountPay = voucherDetailVM.sumAmount - voucherDetailVM.Invoice_DiscountPrice - voucherDetailVM.Invoice_TaxPercent * voucherDetailVM.sumAmount / 100;

            isLoading = false;
        }

        private async Task UpdateInvoiceDiscount()
        {
            isLoading = true;

            //await cashierService.UpdateInvoiceDiscount(invoiceDetail);

            //invoiceTotal = await cashierService.GetInvoiceTotal(voucherVM.VNumber);

            //filterVM.ReportName = "CustomNewReport";

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_InvoiceDiscount");
            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            isLoading = false;
        }

        private async Task CloseModal_InvoiceDiscount()
        {
            isLoading = true;

            //invoiceTotal = await cashierService.GetInvoiceTotal(voucherVM.VNumber);

            isLoading = false;
        }

        private void InitializeModal_Customer(int typeUpdate)
        {
            isLoading = true;

            if (typeUpdate == 0)
            {
                customerVM = new CustomerVM();
                customerVM.IsTypeUpdate = 0;
            }
            else
            {
                customerVM.IsTypeUpdate = 1;
            }

            isLoading = false;
        }

        private async Task UpdateCustomer()
        {
            isLoading = true;

            //customer.CustomerCode = voucherVM.CustomerCode = await customerService.UpdateCustomer(customer);
            //await cashierService.UpdateInvoiceCustomer(voucherVM);

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
            //if (result != null)
            //{
            //    customer = result;
            //    voucherVM.CustomerCode = result.CustomerCode;
            //}
            //else
            //{
            //    customer = new();
            //    voucherVM.CustomerCode = "";
            //}
            //await cashierService.UpdateInvoiceCustomer(voucherVM);
        }

        private async Task ChangeRoomTableAsync()
        {
            isLoading = true;

            voucherVM.IsClickChangeRoomTable = true;

            await js.Toast_Alert("Chọn phòng/bàn trống để chuyển!", SweetAlertMessageType.warning);

            isLoading = false;
        }

        private async Task InitializeModal_Payment()
        {
            isLoading = true;

            voucherVM.PaymentTypeCode = "CASH";

            voucherVM.PaymentAmount = voucherVM.TotalAmount;

            amountSuggests = await voucherService.GetAmountSuggest(voucherVM.TotalAmount);

            await js.InvokeAsync<object>("ShowModal", "#InitializeModal_Payment");

            isLoading = false;
        }

        private void click_CalculatorPayment(string value)
        {
            isLoading = true;

            switch (value)
            {
                case "1":
                    voucherVM.PaymentAmount = voucherVM.PaymentAmount.ToString().Length < 9 ? voucherVM.PaymentAmount * 10 + 1 : voucherVM.PaymentAmount;
                    break;
                case "2":
                    voucherVM.PaymentAmount = voucherVM.PaymentAmount.ToString().Length < 9 ? voucherVM.PaymentAmount * 10 + 2 : voucherVM.PaymentAmount; ;
                    break;
                case "3":
                    voucherVM.PaymentAmount = voucherVM.PaymentAmount.ToString().Length < 9 ? voucherVM.PaymentAmount * 10 + 3 : voucherVM.PaymentAmount; ;
                    break;
                case "4":
                    voucherVM.PaymentAmount = voucherVM.PaymentAmount.ToString().Length < 9 ? voucherVM.PaymentAmount * 10 + 4 : voucherVM.PaymentAmount; ;
                    break;
                case "5":
                    voucherVM.PaymentAmount = voucherVM.PaymentAmount.ToString().Length < 9 ? voucherVM.PaymentAmount * 10 + 5 : voucherVM.PaymentAmount; ;
                    break;
                case "6":
                    voucherVM.PaymentAmount = voucherVM.PaymentAmount.ToString().Length < 9 ? voucherVM.PaymentAmount * 10 + 6 : voucherVM.PaymentAmount; ;
                    break;
                case "7":
                    voucherVM.PaymentAmount = voucherVM.PaymentAmount.ToString().Length < 9 ? voucherVM.PaymentAmount * 10 + 7 : voucherVM.PaymentAmount; ;
                    break;
                case "8":
                    voucherVM.PaymentAmount = voucherVM.PaymentAmount.ToString().Length < 9 ? voucherVM.PaymentAmount * 10 + 8 : voucherVM.PaymentAmount; ;
                    break;
                case "9":
                    voucherVM.PaymentAmount = voucherVM.PaymentAmount.ToString().Length < 9 ? voucherVM.PaymentAmount * 10 + 9 : voucherVM.PaymentAmount; ;
                    break;
                case "0":
                    voucherVM.PaymentAmount = voucherVM.PaymentAmount.ToString().Length < 9 ? voucherVM.PaymentAmount * 10 : voucherVM.PaymentAmount; ;
                    break;
                case "00":
                    voucherVM.PaymentAmount = voucherVM.PaymentAmount.ToString().Length < 8 ? voucherVM.PaymentAmount * 100 : voucherVM.PaymentAmount;
                    break;
                case "000":
                    voucherVM.PaymentAmount = voucherVM.PaymentAmount.ToString().Length < 6 ? voucherVM.PaymentAmount * 1000 : voucherVM.PaymentAmount;
                    break;
                case "x":
                    voucherVM.PaymentAmount = (voucherVM.PaymentAmount - int.Parse(voucherVM.PaymentAmount.ToString().Substring(voucherVM.PaymentAmount.ToString().Length - 1, 1))) / 10;
                    break;
                case "c":
                    voucherVM.PaymentAmount = 0;
                    break;
            }

            isLoading = false;

        }

        private void click_PaymentMode(string value)
        {
            isLoading = true;

            voucherVM.PaymentTypeCode = value;

            isLoading = false;
        }

        private void click_AmountSuggest(decimal value)
        {
            isLoading = true;

            voucherVM.PaymentAmount = value;

            isLoading = false;
        }

        private async Task Payment()
        {
            isLoading = true;

            voucherVM.PaymentAmount = voucherVM.PaymentAmount > voucherVM.TotalAmount ? voucherVM.TotalAmount : voucherVM.PaymentAmount;

            voucherVM.VActive = true;
            voucherVM.IsTypeUpdate = 4;

            await voucherService.UpdateVoucher(voucherVM, voucherDetailVMs);

            VoucherVM _voucherVM = new();
            VoucherDetailVM _voucherDetailVM = new();
            List<VoucherDetailVM> _voucherDetailVMs = new();

            if (voucherVM.VTypeID == "FIN_Sale")
            {
                _voucherVM.VDesc = "Thu tiền " + voucherVM.VDesc;
                if (voucherVM.PaymentTypeCode == "CASH")
                {
                    _voucherVM.VCode = "PT";
                    _voucherVM.VTypeID = "FIN_Cash_Receipt";
                    _voucherVM.VSubTypeID = "FIN_Cash_Receipt_Customer";
                }

                if (voucherVM.PaymentTypeCode == "BANK")
                {
                    _voucherVM.VCode = "GBC";
                    _voucherVM.VTypeID = "FIN_Deposit_Credit";
                    _voucherVM.VSubTypeID = "FIN_Deposit_Credit_Customer";
                }
            }

            _voucherVM.IsTypeUpdate = 0;

            _voucherVM.UserID = filterVM.UserID;

            _voucherVM.DivisionID = filterVM.DivisionID;

            _voucherVM.BankAccountID = voucherVM.BankAccountID;

            _voucherVM.EserialPerform = voucherVM.EserialPerform;

            _voucherVM.VDate = voucherVM.VDate;

            _voucherVM.TotalAmount = voucherVM.PaymentAmount;

            _voucherVM.VReference = voucherVM.VNumber;

            _voucherVM.VActive = true;

            _voucherDetailVM.VDDesc = _voucherVM.VDesc;
            _voucherDetailVM.VDQty = 1;
            _voucherDetailVM.VDPrice = voucherVM.PaymentAmount;
            _voucherDetailVM.VDAmount = voucherVM.PaymentAmount;

            _voucherDetailVMs.Add(_voucherDetailVM);

            await voucherService.UpdateVoucher(_voucherVM, _voucherDetailVMs);

            await hubConnection.SendAsync("Send_LoadRoomTable", filterVM.StockCode, voucherVM.RoomTableCode, filterVM.UserID);

            await PrintVoucher("Phieu_xuat_kho_ban_hang");

            logVM.LogDesc = "Thanh toán hóa đơn số " + voucherVM.VNumber + "";
            await sysService.InsertLog(logVM);

            await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);

            voucherVM = new();
            voucherDetailVMs = new();

            await js.InvokeAsync<object>("CloseModal", "#InitializeModal_Payment");

            isLoading = false;
        }

        private void ClickTabRoomTable()
        {
            voucherVM = new();
        }

        private void ClickTabMenu()
        {
            voucherVM.IsClickChangeRoomTable = false;
        }

        protected async Task PrintVoucher(string _ReportName)
        {
            filterVM.ReportName = $"{_ReportName}?VNumber={voucherVM.VNumber}";
            await js.InvokeAsync<object>("ShowModal", "#InitializeModalView_Rpt");
        }
    }
}
