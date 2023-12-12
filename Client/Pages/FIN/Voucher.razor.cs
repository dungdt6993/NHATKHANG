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
using Blazored.TextEditor;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace D69soft.Client.Pages.FIN
{
    partial class Voucher
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] VoucherService voucherService { get; set; }
        [Inject] PurchasingService purchasingService { get; set; }
        [Inject] InventoryService inventoryService { get; set; }
        [Inject] CustomerService customerService { get; set; }
        [Inject] MoneyService moneyService { get; set; }

        [Inject] ProfileService profileService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        //Para
        [Parameter]
        public string _FuncID { get; set; }

        //PermisFunc
        bool FIN_Voucher_Update;
        bool FIN_Voucher_CancelActive;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        //Division
        IEnumerable<DivisionVM> filter_divisionVMs;

        //Profile
        private List<ProfileVM> profileVMs;

        //VType
        IEnumerable<VTypeVM> filter_vTypeVMs;
        IEnumerable<VSubTypeVM> vSubTypeVMs;

        //Voucher
        VoucherVM voucherVM = new();
        List<VoucherVM> voucherVMs;

        //VoucherDetail
        VoucherDetailVM voucherDetailVM = new();
        List<VoucherDetailVM> voucherDetailVMs;

        //Vendor
        VendorVM vendorVM = new();
        List<VendorVM> vendorVMs;

        //Customer
        CustomerVM customerVM = new();
        List<CustomerVM> customerVMs;

        //Stock
        StockVM stockVM = new();
        List<StockVM> stockVMs;

        //Bank
        List<BankVM> bankVMs;

        //BankAccount
        BankAccountVM bankAccountVM = new();
        List<BankAccountVM> bankAccountVMs;

        //ItemsType
        IEnumerable<ItemsTypeVM> itemsTypeVMs;

        //ItemsClass
        ItemsClassVM itemsClassVM = new();
        IEnumerable<ItemsClassVM> itemsClassVMs;

        //ItemsGroup
        ItemsGroupVM itemsGroupVM = new();
        IEnumerable<ItemsGroupVM> itemsGroupVMs;

        //Items
        ItemsVM itemsVM = new();
        List<ItemsVM> itemsVMs;
        BlazoredTextEditor QuillHtml = new BlazoredTextEditor();

        //QuantitativeItems
        ItemsVM qi_itemsVM = new();
        QuantitativeItemsVM quantitativeItemsVM = new();
        List<QuantitativeItemsVM> quantitativeItemsVMs;

        //Unit
        ItemsUnitVM itemsUnitVM = new();
        IEnumerable<ItemsUnitVM> itemsUnitVMs;

        //VAT
        IEnumerable<VATDefVM> vatDefVMs;

        //Account
        IEnumerable<AccountVM> accountVMs;

        //RPT
        string ReportName = String.Empty;

        //Inventory
        List<InventoryVM> inventoryVMs;

        //Cash
        List<VoucherDetailVM> moneyBooks;

        private BlazoredTypeahead<VoucherDetailVM, VoucherDetailVM> txtSearchItems;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
            await js.InvokeAsync<object>("tooltip");

            await js.InvokeAsync<object>("maskDate");
            await js.InvokeAsync<object>("maskDateTime");
            await js.InvokeAsync<object>("maskCurrency");
            await js.InvokeAsync<object>("maskPercent");

            await js.InvokeAsync<object>("keyPressNextInput");
        }

        protected override async Task OnInitializedAsync()
        {
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            filterVM.FuncID = _FuncID;

            if (await sysService.CheckAccessFunc(filterVM.UserID, filterVM.FuncID))
            {
                logVM.LogUser = filterVM.UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = filterVM.FuncID;
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            FIN_Voucher_Update = await sysService.CheckAccessSubFunc(filterVM.UserID, $"{filterVM.FuncID}_Update");
            FIN_Voucher_CancelActive = await sysService.CheckAccessSubFunc(filterVM.UserID, $"{filterVM.FuncID}_CancelActive");

            filter_divisionVMs = await organizationalChartService.GetDivisionList(filterVM);
            filterVM.DivisionID = (await sysService.GetInfoUser(filterVM.UserID)).DivisionID;

            filterVM.searchActive = 2;

            filterVM.StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            filterVM.EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddTicks(-1);

            //Bank
            bankVMs = (await moneyService.GetBankList()).ToList();

            //BankAccount
            bankAccountVMs = (await moneyService.GetBankAccountList()).ToList();

            //Vendor
            vendorVMs = (await purchasingService.GetVendorList()).ToList();

            //Customer
            customerVMs = (await customerService.GetCustomers()).ToList();

            //Stock
            stockVMs = (await inventoryService.GetStockList()).ToList();

            //Items
            itemsUnitVMs = await inventoryService.GetItemsUnitList();
            itemsTypeVMs = await inventoryService.GetItemsTypes();
            itemsClassVMs = await inventoryService.GetItemsClassList();
            itemsGroupVMs = await inventoryService.GetItemsGroupList();

            //Account
            accountVMs = await voucherService.GetAccounts();

            //VAT
            vatDefVMs = await voucherService.GetVATDefs();

            //Profile
            filterVM.DivisionID = filterVM.DivisionID;
            filterVM.TypeProfile = 0;
            profileVMs = await profileService.GetProfileList(filterVM);

            await GetVouchers();

            isLoadingScreen = false;
        }

        protected override async Task OnParametersSetAsync()
        {
            await OnInitializedAsync();
        }

        private async void onchange_DivisionID(string value)
        {
            isLoading = true;

            filterVM.DivisionID = value;

            await GetVouchers();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_VTypeID(string value)
        {
            isLoading = true;

            filterVM.VTypeID = value;

            await GetVouchers();

            isLoading = false;

            StateHasChanged();
        }

        public async Task OnRangeSelect(DateRange _range)
        {
            filterVM.StartDate = _range.Start;
            filterVM.EndDate = _range.End;

            await GetVouchers();
        }

        private async Task GetVouchers()
        {
            isLoading = true;

            filterVM.FuncID = _FuncID;
            filter_vTypeVMs = await voucherService.GetVTypeVMs(filterVM.FuncID);

            voucherVM = new();
            voucherDetailVMs = new();

            filterVM.TypeView = 0;

            voucherVMs = await voucherService.GetVouchers(filterVM);

            isLoading = false;
        }

        private void onclick_Selected(VoucherVM _voucherVM)
        {
            voucherVM = _voucherVM == voucherVM ? new() : _voucherVM;
        }

        private string SetSelected(VoucherVM _voucherVM)
        {
            if (voucherVM.VNumber != _voucherVM.VNumber)
            {
                return string.Empty;
            }
            return "selected";
        }

        private async Task InitializeModalUpdate_Voucher(string _vTypeID, int _IsTypeUpdate)
        {
            isLoading = true;

            filterVM.CategoryName = "Voucher";

            vSubTypeVMs = await voucherService.GetVSubTypeVMs(_vTypeID);

            if (filterVM.FuncID == "FIN_Cash" || filterVM.FuncID == "FIN_Deposit")
            {
                vSubTypeVMs = vSubTypeVMs.Where(x => x.VSubTypeID != "FIN_Cash_Payment_Vendor" && x.VSubTypeID != "FIN_Cash_Receipt_Customer" && x.VSubTypeID != "FIN_Deposit_Debit_Vendor" && x.VSubTypeID != "FIN_Deposit_Credit_Customer");
            }

            if (_IsTypeUpdate == 0)
            {
                voucherVM = new();
                voucherDetailVMs = new();

                voucherVM.VCode = filter_vTypeVMs.Where(x => x.VTypeID == _vTypeID).Select(x => x.VCode).First();

                if (filterVM.FuncID != "FIN_Cash" && filterVM.FuncID != "FIN_Deposit")
                {
                    filterVM.ITypeCode = voucherVM.ITypeCode = "HH";
                    voucherVM.PaymentTypeCode = "CASH";
                }

                voucherVM.EserialPerform = filterVM.UserID;
            }

            if (_IsTypeUpdate != 0)
            {
                voucherDetailVMs = await voucherService.GetVoucherDetails(voucherVM.VNumber);
            }

            if (_IsTypeUpdate == 0 || _IsTypeUpdate == 5)
            {
                voucherVM.VDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                voucherVM.VActive = false;
            }

            if (_IsTypeUpdate == 5)
            {
                foreach (var _voucherDetailVM in voucherDetailVMs)
                {
                    _voucherDetailVM.InventoryCheck_Qty = await inventoryService.GetInventoryCheck_Qty(voucherVM.VDate.Value, _voucherDetailVM);
                }

                voucherDetailVMs.ForEach(e => e.VDPrice = e.IPrice);

                voucherDetailVMs.ForEach(e => { e.FromStockCode = e.StockDefault; e.FromStockName = stockVMs.Where(x => x.StockCode == e.StockDefault).Select(x => x.StockName).FirstOrDefault(); });

                voucherDetailVMs.ForEach(e => { e.ToStockCode = e.StockDefault; e.ToStockName = stockVMs.Where(x => x.StockCode == e.StockDefault).Select(x => x.StockName).FirstOrDefault(); });
            }

            if (_IsTypeUpdate == 6)
            {
                voucherVM.IsInvoice = true;
            }

            voucherVM.UserID = filterVM.UserID;

            voucherVM.VTypeID = _vTypeID;

            voucherVM.DivisionID = filterVM.DivisionID;
            voucherVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Voucher");

            await js.InvokeAsync<object>("bootrap_select_refresh");

            isLoading = false;
        }

        public async Task OnRangeSelect_VDate(DateRange _range)
        {
            if (voucherVM.VTypeID == "FIN_Trf" || voucherVM.VTypeID == "FIN_Output" || voucherVM.VTypeID == "FIN_InventoryCheck" || voucherVM.VTypeID == "FIN_Sale")
            {
                await UpdateAllInventoryCheck_Qty();
            }
        }

        private string onchange_VSubTypeID
        {
            get { return voucherVM.VSubTypeID; }
            set
            {
                voucherVM.VSubTypeID = value;

                voucherDetailVMs = new();
                voucherVM.TotalAmount = 0;

                voucherVM.VDesc = vSubTypeVMs.Where(x => x.VSubTypeID == voucherVM.VSubTypeID).Select(x => x.VSubTypeDesc).FirstOrDefault() + " ngày " + DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        private string onchange_ITypeCode
        {
            get { return voucherVM.ITypeCode; }
            set
            {
                voucherVM.ITypeCode = value;

                voucherDetailVMs = new();
                voucherVM.TotalAmount = 0;
            }
        }

        private bool onchange_IsInvoice
        {
            get { return voucherVM.IsInvoice; }
            set
            {
                voucherVM.IsInvoice = value;

                //Hóa đơn
                if (!voucherVM.IsInvoice)
                {
                    voucherVM.InvoiceNumber = String.Empty;
                    voucherVM.InvoiceDate = null;

                    voucherDetailVMs.ForEach(e => { e.VATCode = String.Empty; e.VATRate = 0; });

                    foreach (var _voucherDetailVM in voucherDetailVMs)
                    {
                        onchange_VAT(String.Empty, _voucherDetailVM);
                    }
                }
            }
        }

        private string onchange_VendorCode
        {
            get { return voucherVM.VendorCode; }
            set
            {
                voucherVM.VendorCode = value;

                var _ITypeCode = voucherVM.ITypeCode == "HH" ? "hàng" : "dịch vụ";

                voucherVM.VDesc = $"Mua {_ITypeCode} của " + vendorVMs.Where(x => x.VendorCode == voucherVM.VendorCode).Select(x => x.VendorName).FirstOrDefault();
            }
        }

        private string onchange_CustomerCode
        {
            get { return voucherVM.CustomerCode; }
            set
            {
                voucherVM.CustomerCode = value;

                var _ITypeCode = voucherVM.ITypeCode == "HH" ? "hàng" : "dịch vụ";

                voucherVM.VDesc = $"Bán {_ITypeCode} cho " + customerVMs.Where(x => x.CustomerCode == voucherVM.CustomerCode).Select(x => x.CustomerName).FirstOrDefault() + "";
            }
        }

        private async Task<IEnumerable<VoucherDetailVM>> SearchItems(string _searchItems)
        {
            filterVM.searchItems = _searchItems;
            return await voucherService.GetSearchItems(filterVM);
        }

        private async Task SelectedItem(VoucherDetailVM _voucherDetailVM)
        {
            if (_voucherDetailVM != null)
            {
                _voucherDetailVM.SeqVD = voucherDetailVMs.Count == 0 ? 1 : voucherDetailVMs.Select(x => x.SeqVD).Max() + 1;

                _voucherDetailVM.VDQty = 1;

                _voucherDetailVM.VDAmount = _voucherDetailVM.VDPrice;

                //Hóa đơn
                if (!voucherVM.IsInvoice)
                {
                    _voucherDetailVM.VATCode = String.Empty;
                    _voucherDetailVM.VATRate = 0;
                }

                StateHasChanged();

                if (voucherVM.VTypeID == "FIN_Output" || voucherVM.VTypeID == "FIN_Trf" || voucherVM.VTypeID == "FIN_InventoryCheck" || voucherVM.VTypeID == "FIN_Sale")
                {
                    await UpdateInventoryCheck_Qty(_voucherDetailVM);
                }

                voucherDetailVMs.Add(_voucherDetailVM);

                onchange_VAT(_voucherDetailVM.VATCode, _voucherDetailVM);

                await txtSearchItems.Focus();
            }

            await js.InvokeAsync<object>("updateScrollToBottom","Voucher");
        }

        private async Task CreateVoucherDetail()
        {
            VoucherDetailVM _voucherDetailVM = new();

            _voucherDetailVM.SeqVD = voucherDetailVMs.Count == 0 ? 1 : voucherDetailVMs.Select(x => x.SeqVD).Max() + 1;
            _voucherDetailVM.VDQty = 1;

            voucherDetailVMs.Add(_voucherDetailVM);

            await js.InvokeAsync<object>("updateScrollToBottom", "Voucher");
        }

        private void onchange_VDQty(ChangeEventArgs e, VoucherDetailVM _voucherDetailVM)
        {
            _voucherDetailVM.VDQty = Math.Round(decimal.Parse(e.Value.ToString()), 2, MidpointRounding.AwayFromZero);

            _voucherDetailVM.VDAmount = Math.Round(_voucherDetailVM.VDPrice * _voucherDetailVM.VDQty, MidpointRounding.AwayFromZero);

            _voucherDetailVM.VDDiscountAmount = Math.Round(_voucherDetailVM.VDDiscountPercent * _voucherDetailVM.VDAmount / 100, MidpointRounding.AwayFromZero);

            _voucherDetailVM.VATAmount = Math.Round((_voucherDetailVM.VDAmount - _voucherDetailVM.VDDiscountAmount) * _voucherDetailVM.VATRate, MidpointRounding.AwayFromZero);

            voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();

            StateHasChanged();
        }

        private void onchange_VDPrice(ChangeEventArgs e, VoucherDetailVM _voucherDetailVM)
        {
            _voucherDetailVM.VDPrice = Math.Round(decimal.Parse(e.Value.ToString()), 2, MidpointRounding.AwayFromZero);

            if (voucherVM.VTypeID == "FIN_Cash_Payment" || voucherVM.VTypeID == "FIN_Cash_Receipt" || voucherVM.VTypeID == "FIN_Deposit_Credit" || voucherVM.VTypeID == "FIN_Deposit_Debit")
            {
                if (!String.IsNullOrEmpty(voucherVM.VReference))
                {
                    _voucherDetailVM.VDPrice = _voucherDetailVM.VDPrice > voucherVM.PaymentAmount ? voucherVM.PaymentAmount : _voucherDetailVM.VDPrice;
                }
            }

            _voucherDetailVM.VDAmount = Math.Round(_voucherDetailVM.VDPrice * _voucherDetailVM.VDQty, MidpointRounding.AwayFromZero);

            _voucherDetailVM.VDDiscountAmount = Math.Round(_voucherDetailVM.VDDiscountPercent * _voucherDetailVM.VDAmount / 100, MidpointRounding.AwayFromZero);

            _voucherDetailVM.VATAmount = Math.Round((_voucherDetailVM.VDAmount - _voucherDetailVM.VDDiscountAmount) * _voucherDetailVM.VATRate, MidpointRounding.AwayFromZero);

            voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();

            StateHasChanged();
        }

        private void onchange_VDDiscountPercent(ChangeEventArgs e, VoucherDetailVM _voucherDetailVM)
        {
            _voucherDetailVM.VDDiscountPercent = Math.Round(decimal.Parse(e.Value.ToString()), 2, MidpointRounding.AwayFromZero);

            _voucherDetailVM.VDDiscountAmount = Math.Round(_voucherDetailVM.VDDiscountPercent * _voucherDetailVM.VDAmount / 100, MidpointRounding.AwayFromZero);

            _voucherDetailVM.VATAmount = Math.Round((_voucherDetailVM.VDAmount - _voucherDetailVM.VDDiscountAmount) * _voucherDetailVM.VATRate, MidpointRounding.AwayFromZero);

            voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();

            StateHasChanged();
        }

        private void onchange_VDDiscountAmount(ChangeEventArgs e, VoucherDetailVM _voucherDetailVM)
        {
            _voucherDetailVM.VDDiscountAmount = Math.Round(decimal.Parse(e.Value.ToString()), MidpointRounding.AwayFromZero);

            _voucherDetailVM.VDDiscountPercent = _voucherDetailVM.VDDiscountAmount != 0 ? Math.Round(_voucherDetailVM.VDDiscountAmount * 100 / _voucherDetailVM.VDAmount, 2, MidpointRounding.AwayFromZero) : 0;

            _voucherDetailVM.VATAmount = Math.Round((_voucherDetailVM.VDAmount - _voucherDetailVM.VDDiscountAmount) * _voucherDetailVM.VATRate, MidpointRounding.AwayFromZero);

            voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();

            StateHasChanged();
        }

        private void onchange_VAT(string value, VoucherDetailVM _voucherDetailVM)
        {
            isLoading = true;

            var _VDTotalAmount = _voucherDetailVM.VDAmount - _voucherDetailVM.VDDiscountAmount + _voucherDetailVM.VATAmount;

            _voucherDetailVM.VATCode = value;

            _voucherDetailVM.VATRate = vatDefVMs.Where(x => x.VATCode == _voucherDetailVM.VATCode).Select(x => x.VATRate).FirstOrDefault();

            _voucherDetailVM.VDAmount = Math.Round(_VDTotalAmount / ((1 + _voucherDetailVM.VATRate) * (1 - _voucherDetailVM.VDDiscountPercent / 100)), MidpointRounding.AwayFromZero);

            if (_voucherDetailVM.VDQty != 0)
            {
                _voucherDetailVM.VDPrice = Math.Round(_voucherDetailVM.VDAmount / _voucherDetailVM.VDQty, 2, MidpointRounding.AwayFromZero);
            }

            _voucherDetailVM.VDDiscountAmount = Math.Round(_voucherDetailVM.VDAmount * _voucherDetailVM.VDDiscountPercent / 100, MidpointRounding.AwayFromZero);

            _voucherDetailVM.VATAmount = _VDTotalAmount - _voucherDetailVM.VDAmount + _voucherDetailVM.VDDiscountAmount;

            voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_InventoryCheck_StockCode(string value, VoucherDetailVM _voucherDetailVM)
        {
            isLoading = true;

            _voucherDetailVM.InventoryCheck_StockCode = value;

            await UpdateInventoryCheck_Qty(_voucherDetailVM);

            isLoading = false;

            StateHasChanged();
        }

        private async Task onclick_copyInventoryCheck_StockCode(VoucherDetailVM _voucherDetailVM)
        {
            isLoading = true;

            voucherDetailVMs.Where(x => x.SeqVD > _voucherDetailVM.SeqVD).ToList().ForEach(e => { e.InventoryCheck_StockCode = _voucherDetailVM.InventoryCheck_StockCode; e.InventoryCheck_StockName = stockVMs.Where(x => x.StockCode == _voucherDetailVM.InventoryCheck_StockCode).Select(x => x.StockName).FirstOrDefault(); });

            foreach (var _voucherDetailVM_0 in voucherDetailVMs.Where(x => x.SeqVD > _voucherDetailVM.SeqVD))
            {
                _voucherDetailVM_0.InventoryCheck_Qty = await inventoryService.GetInventoryCheck_Qty(voucherVM.VDate.Value, _voucherDetailVM_0);
            }

            isLoading = false;
        }

        private async void onchange_FromStockCode(string value, VoucherDetailVM _voucherDetailVM)
        {
            isLoading = true;

            _voucherDetailVM.FromStockCode = _voucherDetailVM.InventoryCheck_StockCode = value;

            await UpdateInventoryCheck_Qty(_voucherDetailVM);

            isLoading = false;

            StateHasChanged();
        }

        private async Task onclick_copyFromStockCode(VoucherDetailVM _voucherDetailVM)
        {
            isLoading = true;

            voucherDetailVMs.Where(x => x.SeqVD > _voucherDetailVM.SeqVD).ToList().ForEach(e => { e.InventoryCheck_StockCode = e.FromStockCode = _voucherDetailVM.FromStockCode; e.FromStockName = stockVMs.Where(x => x.StockCode == _voucherDetailVM.FromStockCode).Select(x => x.StockName).FirstOrDefault(); });

            foreach (var _voucherDetailVM_0 in voucherDetailVMs.Where(x => x.SeqVD > _voucherDetailVM.SeqVD))
            {
                _voucherDetailVM_0.InventoryCheck_Qty = await inventoryService.GetInventoryCheck_Qty(voucherVM.VDate.Value, _voucherDetailVM_0);
            }

            isLoading = false;
        }

        private async Task onclick_removeItems(VoucherDetailVM _voucherDetailVM)
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn xóa?", SweetAlertMessageType.question))
            {
                voucherDetailVMs.Remove(_voucherDetailVM);

                voucherVM.TotalAmount = voucherDetailVMs.Select(x => x.VDAmount - x.VDDiscountAmount + x.VATAmount).Sum();
            }
        }

        private void UpdateItem(VoucherDetailVM result, VoucherDetailVM _voucherDetailVM)
        {
            if (result != null)
            {
                _voucherDetailVM.IsUpdateItem = 0;
                _voucherDetailVM.ICode = result.ICode;
                _voucherDetailVM.IName = result.IName;
                _voucherDetailVM.IUnitName = result.IUnitName;
                _voucherDetailVM.VDPrice = result.VDPrice;
                _voucherDetailVM.FromStockCode = String.Empty;
                _voucherDetailVM.FromStockName = String.Empty;
                _voucherDetailVM.ToStockCode = String.Empty;
                _voucherDetailVM.ToStockName = String.Empty;
                _voucherDetailVM.VATCode = String.Empty;
                _voucherDetailVM.VATRate = 0;
            }
            else
            {
                _voucherDetailVM.IsUpdateItem = 0;
            }

            StateHasChanged();
        }

        private async Task InitializeModalUpdate_PostExcel()
        {
            isLoading = true;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_PostExcel");

            isLoading = false;
        }

        private async Task GetDataFromExcel()
        {
            isLoading = true;

            try
            {
                var str = LibraryFunc.RepalceWhiteSpace(voucherVM.DataFromExcel.Trim()).Replace(" \t", "\t").Replace("\t ", "\t").Replace(".", ",").Replace(" ", "\n");
                var lines = Regex.Split(str, "\n");

                foreach (var itemfinger in lines)
                {
                    if (string.IsNullOrWhiteSpace(itemfinger))
                        continue;

                    voucherDetailVM = new();

                    var itemSplit = Regex.Split(itemfinger, "\t").Select(x => x.Trim()).ToArray();

                    filterVM.ICode = itemSplit[0].Trim();

                    IEnumerable<VoucherDetailVM> _voucherDetailVMs;

                    _voucherDetailVMs = await SearchItems(filterVM.ICode);

                    voucherDetailVM = _voucherDetailVMs.First();

                    if (voucherVM.VTypeID == "FIN_InventoryCheck")
                    {
                        voucherDetailVM.InventoryCheck_ActualQty = decimal.Parse(itemSplit[1].Trim());
                    }
                    else
                    {
                        voucherDetailVM.VDQty = decimal.Parse(itemSplit[1].Trim());
                    }

                    voucherDetailVM.SeqVD = voucherDetailVMs.Count == 0 ? 1 : voucherDetailVMs.Select(x => x.SeqVD).Max() + 1;

                    if (voucherVM.VTypeID == "FIN_Trf" || voucherVM.VTypeID == "FIN_Output" || voucherVM.VTypeID == "FIN_InventoryCheck" || voucherVM.VTypeID == "FIN_Sale")
                    {
                        if (voucherVM.VTypeID == "FIN_Trf" || voucherVM.VTypeID == "FIN_Output" || voucherVM.VTypeID == "FIN_Sale")
                        {
                            voucherDetailVM.InventoryCheck_StockCode = voucherDetailVM.FromStockCode;
                        }
                        await UpdateInventoryCheck_Qty(voucherDetailVM);
                    }

                    voucherDetailVMs.Add(voucherDetailVM);

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_PostExcel");
                }

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            catch (Exception)
            {
                await js.Swal_Message("Thông báo!", "Dữ liệu không hợp lệ.", SweetAlertMessageType.error);
            }

            voucherVM.DataFromExcel = string.Empty;

            isLoading = false;
        }

        private async Task UpdateVoucher(EditContext formVoucherVM, int _IsTypeUpdate)
        {
            voucherVM.IsTypeUpdate = _IsTypeUpdate;

            if (!formVoucherVM.Validate()) return;

            isLoading = true;

            voucherVM.IsTypeUpdate = voucherVM.IsTypeUpdate == 5 ? 0 : voucherVM.IsTypeUpdate;

            voucherVM.IsTypeUpdate = voucherVM.IsTypeUpdate == 6 ? 1 : voucherVM.IsTypeUpdate;

            if (voucherVM.IsTypeUpdate != 2)
            {
                if (voucherDetailVMs.Count == 0)
                {
                    await js.Swal_Message("Cảnh báo!", "Chi tiết hàng hóa, dịch vụ không được trống.", SweetAlertMessageType.warning);
                    isLoading = false;
                    return;
                }

                if (voucherVM.VTypeID == "FIN_Purchasing")
                {
                    voucherDetailVMs.ToList().ForEach(x => x.FromStockCode = String.Empty);

                    if (!voucherVM.IsInventory)
                    {
                        voucherDetailVMs.ToList().ForEach(x => { x.FromStockCode = String.Empty; x.ToStockCode = String.Empty; });
                    }
                    else
                    {
                        if (voucherDetailVMs.Where(x => String.IsNullOrEmpty(x.ToStockCode)).Count() > 0)
                        {
                            await js.Swal_Message("Cảnh báo!", "Kho nhập không được trống.", SweetAlertMessageType.warning);
                            isLoading = false;
                            return;
                        }
                    }
                }

                if (voucherVM.VTypeID == "FIN_Sale")
                {
                    voucherDetailVMs.ToList().ForEach(x => x.ToStockCode = String.Empty);

                    if (!voucherVM.IsInventory)
                    {
                        voucherDetailVMs.ToList().ForEach(x => { x.FromStockCode = String.Empty; x.ToStockCode = String.Empty; });
                    }
                    else
                    {
                        if (voucherDetailVMs.Where(x => String.IsNullOrEmpty(x.FromStockCode)).Count() > 0)
                        {
                            await js.Swal_Message("Cảnh báo!", "Kho xuất không được trống.", SweetAlertMessageType.warning);
                            isLoading = false;
                            return;
                        }
                    }
                }

                if (voucherVM.VTypeID == "FIN_Input")
                {
                    voucherDetailVMs.ToList().ForEach(x => x.FromStockCode = String.Empty);

                    if (voucherDetailVMs.Where(x => String.IsNullOrEmpty(x.ToStockCode)).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Kho nhập không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }
                }

                if (voucherVM.VTypeID == "FIN_Output")
                {
                    voucherDetailVMs.ToList().ForEach(x => x.ToStockCode = String.Empty);

                    if (voucherDetailVMs.Where(x => String.IsNullOrEmpty(x.FromStockCode)).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Kho xuất không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }
                }

                if (voucherVM.VTypeID == "FIN_Trf")
                {
                    if (voucherDetailVMs.Where(x => String.IsNullOrEmpty(x.ToStockCode)).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Kho nhập không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }

                    if (voucherDetailVMs.Where(x => String.IsNullOrEmpty(x.FromStockCode)).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Kho xuất không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }
                }

                if (voucherVM.VTypeID == "FIN_InventoryCheck")
                {
                    if (voucherDetailVMs.Where(x => String.IsNullOrEmpty(x.InventoryCheck_StockCode)).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Kho kiểm kê không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }

                    voucherDetailVMs.ToList().ForEach(x => { x.FromStockCode = String.Empty; x.ToStockCode = String.Empty; });

                    voucherDetailVMs.ToList().Where(x => x.InventoryCheck_ActualQty > x.InventoryCheck_Qty).ToList().ForEach(x => { x.ToStockCode = x.InventoryCheck_StockCode; x.VDQty = Math.Abs(x.InventoryCheck_ActualQty - x.InventoryCheck_Qty); });
                    voucherDetailVMs.ToList().Where(x => x.InventoryCheck_ActualQty < x.InventoryCheck_Qty).ToList().ForEach(x => { x.FromStockCode = x.InventoryCheck_StockCode; x.VDQty = Math.Abs(x.InventoryCheck_ActualQty - x.InventoryCheck_Qty); });
                }

                if (voucherVM.VTypeID == "FIN_Cash_Payment" || voucherVM.VTypeID == "FIN_Cash_Receipt" || voucherVM.VTypeID == "FIN_Deposit_Credit" || voucherVM.VTypeID == "FIN_Deposit_Debit")
                {
                    if (voucherDetailVMs.Where(x => String.IsNullOrEmpty(x.VDDesc)).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Diễn giải không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }

                    if (voucherDetailVMs.Where(x => x.VDPrice == 0).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Số tiền không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }
                }

                if (voucherVM.VTypeID == "FIN_Cash_Payment" || voucherVM.VTypeID == "FIN_Cash_Receipt" || voucherVM.VTypeID == "FIN_Deposit_Credit" || voucherVM.VTypeID == "FIN_Deposit_Debit")
                {
                    if (!String.IsNullOrEmpty(voucherVM.VReference))
                    {
                        voucherVM.VActive = true;
                    }
                }

                voucherVM.VNumber = await voucherService.UpdateVoucher(voucherVM, voucherDetailVMs);

                voucherVM.IsTypeUpdate = 3;

                logVM.LogDesc = "Cập nhật chứng từ " + voucherVM.VNumber + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    await voucherService.UpdateVoucher(voucherVM, voucherDetailVMs);

                    logVM.LogDesc = "Xóa chứng từ " + voucherVM.VNumber + "";
                    await sysService.InsertLog(logVM);

                    await GetVouchers();

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Voucher");
                    await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                }
                else
                {
                    voucherVM.IsTypeUpdate = 1;
                }
            }

            ReportName = "CustomNewReport";

            isLoading = false;
        }

        private async Task ActiveVoucher(int _IsTypeUpdate, bool _VActive)
        {
            isLoading = true;

            var question = _VActive ? "Ghi sổ" : "Bỏ ghi sổ";

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn " + question + "?", SweetAlertMessageType.question))
            {
                if (_VActive)
                {
                    if (voucherVM.IsInventory)
                    {
                        if (voucherVM.VTypeID == "FIN_Trf" || voucherVM.VTypeID == "FIN_Output" || voucherVM.VTypeID == "FIN_Sale")
                        {
                            foreach (var _voucherDetailVM in voucherDetailVMs)
                            {
                                _voucherDetailVM.InventoryCheck_Qty = await inventoryService.GetInventoryCheck_Qty(voucherVM.VDate.Value, _voucherDetailVM);
                            }

                            if (voucherDetailVMs.GroupBy(x => new { x.ICode, x.FromStockCode, x.InventoryCheck_Qty }).Select(x => new { InventoryCheck_Qty = x.Max(x => x.InventoryCheck_Qty), sumQty = x.Sum(x => x.VDQty) }).Where(x => x.sumQty > x.InventoryCheck_Qty).Count() > 0)
                            {
                                await js.Swal_Message("Cảnh báo!", "Số lượng xuất kho vượt quá số lượng tồn trong kho.", SweetAlertMessageType.warning);
                                isLoading = false;
                                return;
                            }
                        }
                    }

                    if (voucherVM.IsPayment)
                    {
                        VoucherVM _voucherVM = new();
                        VoucherDetailVM _voucherDetailVM = new();
                        List<VoucherDetailVM> _voucherDetailVMs = new();

                        if (voucherVM.VTypeID == "FIN_Purchasing")
                        {
                            _voucherVM.VDesc = "Trả tiền " + voucherVM.VDesc;
                            if (voucherVM.PaymentTypeCode == "CASH")
                            {
                                _voucherVM.VCode = "PC";
                                _voucherVM.VTypeID = "FIN_Cash_Payment";
                                _voucherVM.VSubTypeID = "FIN_Cash_Payment_Vendor";
                            }

                            if (voucherVM.PaymentTypeCode == "BANK")
                            {
                                _voucherVM.VCode = "GBN";
                                _voucherVM.VTypeID = "FIN_Deposit_Debit";
                                _voucherVM.VSubTypeID = "FIN_Deposit_Debit_Vendor";
                            }
                        }

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

                        _voucherVM.TotalAmount = voucherVM.TotalAmount;

                        _voucherVM.VReference = voucherVM.VNumber;

                        _voucherVM.VActive = true;

                        _voucherDetailVM.VDDesc = _voucherVM.VDesc;
                        _voucherDetailVM.VDQty = 1;
                        _voucherDetailVM.VDPrice = voucherVM.TotalAmount;
                        _voucherDetailVM.VDAmount = voucherVM.TotalAmount;

                        _voucherDetailVMs.Add(_voucherDetailVM);

                        await voucherService.UpdateVoucher(_voucherVM, _voucherDetailVMs);
                    }
                }

                voucherVM.IsTypeUpdate = _IsTypeUpdate;
                voucherVM.VActive = _VActive;

                await voucherService.UpdateVoucher(voucherVM, voucherDetailVMs);

                voucherVM.IsTypeUpdate = 3;

                logVM.LogDesc = question + " chứng từ " + voucherVM.VNumber + "";
                await sysService.InsertLog(logVM);

                await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        protected async Task PrintVoucher(string _ReportName)
        {
            ReportName = $"{_ReportName}?VNumber={voucherVM.VNumber}";
            await js.InvokeAsync<object>("ShowModal", "#InitializeModalView_Rpt");
        }

        private async Task UpdateAllInventoryCheck_Qty()
        {
            isLoading = true;

            foreach (var _voucherDetailVM in voucherDetailVMs)
            {
                await UpdateInventoryCheck_Qty(_voucherDetailVM);
            }

            isLoading = false;
        }

        private async Task UpdateInventoryCheck_Qty(VoucherDetailVM _voucherDetailVM)
        {
            _voucherDetailVM.InventoryCheck_Qty = await inventoryService.GetInventoryCheck_Qty(voucherVM.VDate.Value, _voucherDetailVM);
        }

        //Lập phiếu thu tiền/trả tiền
        private async Task InitializeModalUpdate_VoucherReference(string _vTypeID, string _vSubTypeID)
        {
            isLoading = true;

            if (_vTypeID == "FIN_Cash_Receipt" || _vTypeID == "FIN_Cash_Payment")
            {
                filterVM.FuncID = "FIN_Cash";

                filter_vTypeVMs = await voucherService.GetVTypeVMs("FIN_Cash");
            }

            if (_vTypeID == "FIN_Deposit_Credit" || _vTypeID == "FIN_Deposit_Debit")
            {
                filterVM.FuncID = "FIN_Deposit";

                filter_vTypeVMs = await voucherService.GetVTypeVMs("FIN_Deposit");
            }

            vSubTypeVMs = await voucherService.GetVSubTypeVMs(_vTypeID);

            vSubTypeVMs = vSubTypeVMs.Where(x => x.VSubTypeID == _vSubTypeID);

            bankAccountVMs = (await moneyService.GetBankAccountList()).ToList();

            var _VNumber = voucherVM.VNumber;
            var _VDDesc = voucherVM.VDesc;
            var _sumPrice = voucherVM.TotalAmount - voucherVM.PaymentAmount;

            voucherVM = new();

            voucherDetailVM = new();
            voucherDetailVMs = new();

            voucherVM.VTypeID = _vTypeID;
            voucherVM.VSubTypeID = _vSubTypeID;

            voucherVM.IsTypeUpdate = 0;

            voucherVM.UserID = filterVM.UserID;

            voucherVM.DivisionID = filterVM.DivisionID;

            if (_vTypeID == "FIN_Cash_Receipt")
            {
                voucherVM.VCode = "PT";
                voucherVM.VDesc = "Thu tiền " + _VDDesc;
            }

            if (_vTypeID == "FIN_Deposit_Credit")
            {
                voucherVM.VCode = "GBC";
                voucherVM.VDesc = "Thu tiền " + _VDDesc;
            }

            if (_vTypeID == "FIN_Cash_Payment")
            {
                voucherVM.VCode = "PC";
                voucherVM.VDesc = "Trả tiền " + _VDDesc;
            }

            if (_vTypeID == "FIN_Deposit_Debit")
            {
                voucherVM.VCode = "GBN";
                voucherVM.VDesc = "Trả tiền " + _VDDesc;
            }

            voucherVM.VDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            voucherVM.VReference = _VNumber;

            voucherVM.VActive = false;

            voucherDetailVM.SeqVD = 1;
            voucherDetailVM.VDDesc = voucherVM.VDesc;

            voucherDetailVM.VDPrice = _sumPrice;

            voucherVM.TotalAmount = voucherVM.PaymentAmount = _sumPrice;

            voucherDetailVMs.Add(voucherDetailVM);

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Voucher");

            await js.InvokeAsync<object>("bootrap_select_refresh");

            isLoading = false;
        }

        //RPT
        protected async Task ViewRPT(string _ReportName)
        {
            isLoading = true;

            filterVM.TypeView = 1;

            ReportName = _ReportName;

            if (_ReportName == "FIN_So_quy_tien")
            {
                moneyBooks = await voucherService.GetMoneyBooks(filterVM);
            }

            if (_ReportName == "FIN_Tong_hop_ton_kho")
            {
                inventoryVMs = await inventoryService.GetInventorys(filterVM);
            }

            isLoading = false;
        }

        //BankAccount
        private async Task GetBankAccounts()
        {
            isLoading = true;

            filterVM.CategoryName = "BankAccount";

            bankAccountVM = new();

            bankAccountVMs = (await moneyService.GetBankAccountList()).ToList();

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalList_BankAccount");

            isLoading = false;
        }

        private void onclick_Selected(BankAccountVM _bankAccount)
        {
            bankAccountVM = _bankAccount == bankAccountVM ? new() : _bankAccount;
        }

        private string SetSelected(BankAccountVM _bankAccount)
        {
            if (bankAccountVM.BankAccountID != _bankAccount.BankAccountID)
            {
                return string.Empty;
            }
            return "selected";
        }

        private async Task InitializeModalUpdate_BankAccount(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                bankAccountVM = new();
            }

            bankAccountVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_BankAccount");

            isLoading = false;
        }

        private async Task UpdateBankAccount(EditContext _formBankAccountVM, int _IsTypeUpdate)
        {
            bankAccountVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formBankAccountVM.Validate()) return;
            isLoading = true;

            if (bankAccountVM.IsTypeUpdate != 2)
            {
                await moneyService.UpdateBankAccount(bankAccountVM);

                logVM.LogDesc = (bankAccountVM.IsTypeUpdate == 0 ? "Thêm mới" : "Cập nhật") + " tài khoản ngân hàng " + bankAccountVM.BankAccount + " - " + bankVMs.Where(x => x.SwiftCode == bankAccountVM.SwiftCode).Select(x => x.BankShortName).First() + "";
                await sysService.InsertLog(logVM);

                if (filterVM.CategoryName == "BankAccount")
                {
                    bankAccountVMs = (await moneyService.GetBankAccountList()).ToList();
                    bankAccountVM = new();
                }

                await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_BankAccount");

                bankAccountVM.IsTypeUpdate = 1;
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await moneyService.UpdateBankAccount(bankAccountVM);

                    if (affectedRows > 0)
                    {
                        logVM.LogDesc = "Xóa tài khoản ngân hàng " + bankAccountVM.BankAccount + " - " + bankVMs.Where(x => x.SwiftCode == bankAccountVM.SwiftCode).Select(x => x.BankShortName).First() + "";
                        await sysService.InsertLog(logVM);

                        bankAccountVMs = (await moneyService.GetBankAccountList()).ToList();
                        bankAccountVM = new();

                        await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_BankAccount");
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        stockVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    bankAccountVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

        //Vendor
        private async Task GetVendors()
        {
            isLoading = true;

            filterVM.CategoryName = "Vendor";

            vendorVM = new();

            vendorVMs = (await purchasingService.GetVendorList()).ToList();

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalList_Vendor");

            isLoading = false;
        }

        private void onclick_Selected(VendorVM _vendorVM)
        {
            vendorVM = _vendorVM == vendorVM ? new() : _vendorVM;
        }

        private string SetSelected(VendorVM _vendorVM)
        {
            if (vendorVM.VendorCode != _vendorVM.VendorCode)
            {
                return string.Empty;
            }
            return "selected";
        }

        private async Task InitializeModalUpdate_Vendor(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                vendorVM = new();
            }

            vendorVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Vendor");

            isLoading = false;
        }

        private async Task UpdateVendor(EditContext _formVendorVM, int _IsTypeUpdate)
        {
            vendorVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formVendorVM.Validate()) return;
            isLoading = true;

            if (vendorVM.IsTypeUpdate != 2)
            {
                vendorVM.VendorCode = await purchasingService.UpdateVendor(vendorVM);

                logVM.LogDesc = (vendorVM.IsTypeUpdate == 0 ? "Thêm mới" : "Cập nhật") + " nhà cung cấp " + vendorVM.VendorCode + "";
                await sysService.InsertLog(logVM);

                if (filterVM.CategoryName == "Vendor")
                {
                    vendorVMs = (await purchasingService.GetVendorList()).ToList();
                    vendorVM = new();
                }

                if (filterVM.CategoryName == "Voucher")
                {
                    vendorVMs.Insert(0, vendorVM);
                    voucherVM.VendorCode = vendorVM.VendorCode;
                }

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Vendor");
                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                vendorVM.IsTypeUpdate = 1;
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    var result = await purchasingService.UpdateVendor(vendorVM);

                    if (result != "Err_NotDel")
                    {
                        logVM.LogDesc = "Xóa nhà cung cấp " + vendorVM.VendorCode + "";
                        await sysService.InsertLog(logVM);

                        vendorVMs = (await purchasingService.GetVendorList()).ToList();
                        vendorVM = new();

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Vendor");
                        await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        vendorVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    vendorVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

        //Customer
        private async Task GetCustomers()
        {
            isLoading = true;

            filterVM.CategoryName = "Customer";

            customerVM = new();

            customerVMs = (await customerService.GetCustomers()).ToList();

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalList_Customer");

            isLoading = false;
        }

        private void onclick_Selected(CustomerVM _customerVM)
        {
            customerVM = _customerVM == customerVM ? new() : _customerVM;
        }

        private string SetSelected(CustomerVM _customerVM)
        {
            if (customerVM.CustomerCode != _customerVM.CustomerCode)
            {
                return string.Empty;
            }
            return "selected";
        }

        private async Task InitializeModalUpdate_Customer(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                customerVM = new();
            }

            customerVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Customer");

            isLoading = false;
        }

        private async Task UpdateCustomer(EditContext _formVendorVM, int _IsTypeUpdate)
        {
            customerVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formVendorVM.Validate()) return;
            isLoading = true;

            if (customerVM.IsTypeUpdate != 2)
            {
                customerVM.CustomerCode = await customerService.UpdateCustomer(customerVM);

                logVM.LogDesc = (customerVM.IsTypeUpdate == 0 ? "Thêm mới" : "Cập nhật") + " khách hàng " + customerVM.CustomerCode + "";
                await sysService.InsertLog(logVM);

                if (filterVM.CategoryName == "Customer")
                {
                    customerVMs = (await customerService.GetCustomers()).ToList();
                    customerVM = new();
                }

                if (filterVM.CategoryName == "Voucher")
                {
                    customerVMs.Insert(0, customerVM);
                    voucherVM.CustomerCode = customerVM.CustomerCode;
                }

                await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Customer");

                customerVM.IsTypeUpdate = 1;
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    var result = await customerService.UpdateCustomer(customerVM);

                    if (result != "Err_NotDel")
                    {
                        logVM.LogDesc = "Xóa khách hàng " + customerVM.CustomerCode + "";
                        await sysService.InsertLog(logVM);

                        customerVMs = (await customerService.GetCustomers()).ToList();
                        customerVM = new();

                        await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Customer");
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        vendorVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    customerVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

        //Stock
        private async Task GetStocks()
        {
            isLoading = true;

            filterVM.CategoryName = "Stock";

            stockVM = new();

            stockVMs = (await inventoryService.GetStockList()).ToList();

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalList_Stock");

            isLoading = false;
        }

        private void onclick_Selected(StockVM _stockVM)
        {
            stockVM = _stockVM == stockVM ? new() : _stockVM;
        }

        private string SetSelected(StockVM _stockVM)
        {
            if (stockVM.StockCode != _stockVM.StockCode)
            {
                return string.Empty;
            }
            return "selected";
        }

        private async Task InitializeModalUpdate_Stock(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                stockVM = new();

                stockVM.StockActive = true;
            }

            stockVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Stock");

            isLoading = false;
        }

        private async Task UpdateStock(EditContext _formStockVM, int _IsTypeUpdate)
        {
            stockVM.IsTypeUpdate = _IsTypeUpdate;
            if (!_formStockVM.Validate()) return;

            isLoading = true;

            if (stockVM.IsTypeUpdate != 2)
            {
                await inventoryService.UpdateStock(stockVM);

                logVM.LogDesc = (stockVM.IsTypeUpdate == 0 ? "Thêm mới" : "Cập nhật") + " kho " + stockVM.StockCode + "";
                await sysService.InsertLog(logVM);

                if (filterVM.CategoryName == "Stock")
                {
                    stockVMs = (await inventoryService.GetStockList()).ToList();
                    stockVM = new();
                }

                await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Stock");

                stockVM.IsTypeUpdate = 1;
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await inventoryService.UpdateStock(stockVM);

                    if (affectedRows > 0)
                    {
                        logVM.LogDesc = "Xóa kho " + stockVM.StockCode + "";
                        await sysService.InsertLog(logVM);

                        stockVMs = (await inventoryService.GetStockList()).ToList();
                        stockVM = new();

                        await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Stock");
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        stockVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    stockVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

        //ItemsClass
        private async Task InitializeModalUpdate_ItemsClass(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                itemsClassVM = new();
            }

            itemsClassVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_ItemsClass");

            isLoading = false;
        }
        private async Task UpdateItemsClass(EditContext _formItemsClassVM, int _IsTypeUpdate)
        {
            itemsClassVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formItemsClassVM.Validate()) return;

            isLoading = true;

            if (itemsClassVM.IsTypeUpdate != 2)
            {
                await inventoryService.UpdateItemsClass(itemsClassVM);

                logVM.LogDesc = (itemsClassVM.IsTypeUpdate == 0 ? "Thêm mới" : "Cập nhật") + " lớp hàng " + itemsClassVM.IClsCode + "";
                await sysService.InsertLog(logVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ItemsClass");
                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                itemsClassVMs = await inventoryService.GetItemsClassList();
                itemsVM.IClsCode = itemsClassVM.IClsCode;
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await inventoryService.UpdateItemsClass(itemsClassVM);

                    if (affectedRows > 0)
                    {
                        logVM.LogDesc = "Xóa lớp hàng " + itemsClassVM.IClsCode + "";
                        await sysService.InsertLog(logVM);

                        itemsClassVMs = await inventoryService.GetItemsClassList();
                        itemsVM.IClsCode = String.Empty;

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ItemsClass");
                        await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        itemsClassVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    itemsClassVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

        //ItemsGroup
        private async Task InitializeModalUpdate_ItemsGroup(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                itemsGroupVM = new();
                itemsGroupVM.IClsCode = itemsVM.IClsCode;
            }

            itemsGroupVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_ItemsGroup");

            isLoading = false;
        }

        private async Task UpdateItemsGroup(EditContext _formItemsGroupVM, int _IsTypeUpdate)
        {
            itemsGroupVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formItemsGroupVM.Validate()) return;

            isLoading = true;

            if (itemsGroupVM.IsTypeUpdate != 2)
            {
                await inventoryService.UpdateItemsGroup(itemsGroupVM);

                logVM.LogDesc = (itemsGroupVM.IsTypeUpdate == 0 ? "Thêm mới" : "Cập nhật") + " lớp hàng " + itemsGroupVM.IGrpCode + "";
                await sysService.InsertLog(logVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ItemsGroup");
                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                itemsGroupVMs = await inventoryService.GetItemsGroupList();
                itemsVM.IGrpCode = itemsGroupVM.IGrpCode;
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await inventoryService.UpdateItemsGroup(itemsGroupVM);

                    if (affectedRows > 0)
                    {
                        logVM.LogDesc = "Xóa nhóm hàng " + itemsGroupVM.IGrpCode + "";
                        await sysService.InsertLog(logVM);

                        itemsGroupVMs = await inventoryService.GetItemsGroupList();
                        itemsVM.IGrpCode = String.Empty;

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ItemsGroup");
                        await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        itemsGroupVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    itemsGroupVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

        //ItemsUnit
        private async Task InitializeModalUpdate_ItemsUnit(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                itemsUnitVM = new();
            }

            if (_IsTypeUpdate == 1)
            {
                itemsUnitVM = itemsUnitVMs.First(x => x.IUnitCode == itemsVM.IUnitCode);
            }

            itemsUnitVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_ItemsUnit");

            isLoading = false;
        }

        private async Task UpdateItemsUnit(EditContext _formItemsUnitVM, int _IsTypeUpdate)
        {
            itemsUnitVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formItemsUnitVM.Validate()) return;

            isLoading = true;

            if (itemsUnitVM.IsTypeUpdate != 2)
            {
                await inventoryService.UpdateItemsUnit(itemsUnitVM);

                logVM.LogDesc = (itemsUnitVM.IsTypeUpdate == 0 ? "Thêm mới" : "Cập nhật") + " đơn vị tính " + itemsUnitVM.IUnitCode + "";
                await sysService.InsertLog(logVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ItemsUnit");
                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                itemsUnitVMs = await inventoryService.GetItemsUnitList();
                itemsVM.IUnitCode = itemsUnitVM.IUnitCode;
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await inventoryService.UpdateItemsUnit(itemsUnitVM);

                    if (affectedRows > 0)
                    {
                        logVM.LogDesc = "Xóa đơn vị tính " + itemsGroupVM.IGrpCode + "";
                        await sysService.InsertLog(logVM);

                        itemsUnitVMs = await inventoryService.GetItemsUnitList();
                        itemsVM.IUnitCode = String.Empty;

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ItemsUnit");
                        await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        itemsUnitVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    itemsUnitVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

        //Items
        private async Task GetItems()
        {
            isLoading = true;

            filterVM.CategoryName = "Items";

            filterVM.searchValues = String.Empty;

            itemsVM = new();

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalList_Items");

            isLoading = false;
        }

        private Virtualize<ItemsVM>? virtualizeItemsList;
        private async ValueTask<ItemsProviderResult<ItemsVM>> LoadItemsList(ItemsProviderRequest request)
        {
            var numEmployees = Math.Min(request.Count, totalEmployees - request.StartIndex);
            var employees = await EmployeesService.GetEmployeesAsync(request.StartIndex,
                numEmployees, request.CancellationToken);

            return new ItemsProviderResult<Employee>(employees, totalEmployees);
        }

        private async Task SearchItemsList(string value)
        {
            filterVM.searchValues = value;

            await virtualizeItemsList.RefreshDataAsync();
            StateHasChanged();
        }

        private void onclick_Selected(ItemsVM _itemsVM)
        {
            itemsVM = _itemsVM == itemsVM ? new() : _itemsVM;
        }

        private string SetSelected(ItemsVM _itemsVM)
        {
            if (itemsVM.ICode != _itemsVM.ICode)
            {
                return string.Empty;
            }
            return "selected";
        }
        private async Task InitializeModalUpdate_Items(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                itemsVM = new();
                quantitativeItemsVMs = new();

                itemsVM.IURLPicture1 = "/images/_default/no-image.png";
                itemsVM.IClsCode = filterVM.IClsCode;
                itemsVM.IGrpCode = filterVM.IGrpCode;
                itemsVM.IActive = true;
            }

            if (_IsTypeUpdate == 1)
            {
                if (!String.IsNullOrEmpty(itemsVM.IDetail))
                {
                    await QuillHtml.LoadHTMLContent(itemsVM.IDetail);
                }

                quantitativeItemsVMs = await inventoryService.GetQuantitativeItems(itemsVM.ICode);
            }

            itemsVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Items");

            isLoading = false;
        }

        public string onchange_IClsCode
        {
            get
            {
                return itemsVM.IClsCode;
            }
            set
            {
                itemsVM.IClsCode = value;
                itemsVM.IGrpCode = String.Empty;
            }
        }

        MemoryStream memoryStream;
        Stream stream;
        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            isLoading = true;

            var format = "image/png";
            long maxFileSize = 1024 * 1024 * 15;

            var resizedFile = await e.File.RequestImageFileAsync(format, 640, 480); // resize the image file
            stream = resizedFile.OpenReadStream(maxFileSize);
            memoryStream = new();
            await stream.CopyToAsync(memoryStream);

            itemsVM.IURLPicture1 = $"data:{format};base64,{Convert.ToBase64String(memoryStream.ToArray())}";// convert to a base64 string!!

            itemsVM.FileContent = memoryStream.ToArray();

            memoryStream.Close();

            isLoading = false;
        }

        private void FileDefault()
        {
            memoryStream = null;
            itemsVM.IsDelFileUpload = true;
            itemsVM.IURLPicture1 = UrlDirectory.Default_Items;
            itemsVM.FileContent = null;
        }

        private async Task<IEnumerable<ItemsVM>> SearchItemsQI(string _searchItems)
        {
            filterVM.ITypeCode = "HH";
            filterVM.IActive = true;
            filterVM.searchText = _searchItems;
            return await inventoryService.GetItemsList(filterVM);
        }
        private void SelectedItemQI(ItemsVM result)
        {
            if (result != null)
            {
                qi_itemsVM = result;

                quantitativeItemsVM.QI_ICode = qi_itemsVM.ICode;
                quantitativeItemsVM.QI_IName = qi_itemsVM.IName;
                quantitativeItemsVM.QI_IUnitName = itemsUnitVMs.Where(x => x.IUnitCode == qi_itemsVM.IUnitCode).Select(x => x.IUnitName).First();

                quantitativeItemsVMs.Add(quantitativeItemsVM);

                quantitativeItemsVM = new();
            }
        }
        private async Task UpdateItems(EditContext _formItemsVM, int _IsTypeUpdate)
        {
            itemsVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formItemsVM.Validate()) return;

            isLoading = true;

            if (itemsVM.IsTypeUpdate != 2)
            {
                if (itemsVM.ITypeCode != "TP")
                {
                    quantitativeItemsVMs = new();
                }
                else
                {
                    if (quantitativeItemsVMs.Count() == 0)
                    {
                        await js.Toast_Alert("Thành phần, định lượng không được trống!", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }

                    if (quantitativeItemsVMs.Where(x => x.QI_UnitRatio == 0).Count() > 0)
                    {
                        await js.Toast_Alert("Tỷ lệ phải khác 0!", SweetAlertMessageType.warning);

                        isLoading = false;
                        return;
                    }
                }

                itemsVM.IDetail = await QuillHtml.GetHTML();

                itemsVM.ICode = await inventoryService.UpdateItems(itemsVM, quantitativeItemsVMs);

                if (filterVM.CategoryName == "Voucher")
                {
                    //Cập nhật Items từ Voucher
                    if (filterVM.TypeView == 0)
                    {
                        voucherDetailVM = (await SearchItems(itemsVM.ICode)).First();
                        await SelectedItem(voucherDetailVM);
                    }
                }

                if(filterVM.CategoryName == "Items")
                {
                    if (itemsVM.IsTypeUpdate == 0)
                    {
                        itemsVMs.Insert(0, itemsVM);
                        await js.InvokeAsync<object>("updateScrollToTop", "Items");
                    }
                }

                //Cập nhật Items từ tồn kho
                if (filterVM.TypeView == 1 && ReportName == "FIN_Tong_hop_ton_kho")
                {
                    inventoryVMs = await inventoryService.GetInventorys(filterVM);
                }

                logVM.LogDesc = (itemsVM.IsTypeUpdate == 0 ? "Thêm mới" : "Cập nhật") + " hàng hóa " + itemsVM.ICode + "";
                await sysService.InsertLog(logVM);

                await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Items");

                itemsVM.IsTypeUpdate = 1;
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    var result = await inventoryService.UpdateItems(itemsVM, quantitativeItemsVMs);

                    if (result != "Err_NotDel")
                    {
                        logVM.LogDesc = "Xóa hàng hóa " + itemsVM.ICode + "";
                        await sysService.InsertLog(logVM);

                        itemsVMs.Remove(itemsVM);
                        itemsVM = new();

                        await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Items");
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        itemsVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    itemsVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

    }
}
