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
using D69soft.Client.Services.OP;
using System;

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

        bool isLoading;
        bool isLoadingScreen = true;

        protected string UserID;

        //Para
        [Parameter]
        public string _FuncID { get; set; }

        //PermisFunc
        bool FIN_Voucher_Update;
        bool FIN_Voucher_CancelActive;

        LogVM logVM = new();

        //Filter
        FilterFinVM filterFinVM = new();
        FilterHrVM filterHrVM = new();

        //Division
        IEnumerable<DivisionVM> filter_divisionVMs;

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
        IEnumerable<VendorVM> vendorVMs;

        //Customer
        IEnumerable<CustomerVM> customerVMs;

        //Stock
        IEnumerable<StockVM> stockVMs;

        //VAT
        IEnumerable<VATDefVM> vatDefVMs;

        //RPT
        string ReportName = String.Empty;

        //Inventory
        InventoryVM inventoryVM = new();
        List<InventoryVM> inventoryVMs;

        //InventoryBookDetailVM
        List<InventoryBookDetailVM> inventoryBookDetailVMs;

        //Cash
        List<VoucherDetailVM> cashBooks;

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
            filterHrVM.UserID = UserID = (await authenticationStateTask).User.GetUserId();

            filterFinVM.FuncID = _FuncID;

            if (await sysService.CheckAccessFunc(UserID, filterFinVM.FuncID))
            {
                logVM.LogUser = UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = filterFinVM.FuncID;
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            FIN_Voucher_Update = await sysService.CheckAccessSubFunc(UserID, $"{filterFinVM.FuncID}_Update");
            FIN_Voucher_CancelActive = await sysService.CheckAccessSubFunc(UserID, $"{filterFinVM.FuncID}_CancelActive");

            filter_divisionVMs = await organizationalChartService.GetDivisionList(filterHrVM);
            filterFinVM.DivisionID = filter_divisionVMs.Count() > 0 ? filter_divisionVMs.ElementAt(0).DivisionID : string.Empty;

            filterFinVM.searchActive = 2;

            filterFinVM.StartDate = DateTime.Now;
            filterFinVM.EndDate = DateTime.Now;

            filter_vTypeVMs = await voucherService.GetVTypeVMs(filterFinVM.FuncID);

            await GetVouchers();

            isLoadingScreen = false;
        }

        protected override async Task OnParametersSetAsync()
        {
            await OnInitializedAsync();
        }

        public void OnRangeSelect(DateRange _range)
        {
            filterFinVM.StartDate = _range.Start;
            filterFinVM.EndDate = _range.End;
        }

        private async Task GetVouchers()
        {
            isLoading = true;

            voucherVM = new();
            voucherDetailVMs = new();

            filterFinVM.TypeView = 0;

            voucherVMs = await voucherService.GetVouchers(filterFinVM);

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

            vSubTypeVMs = await voucherService.GetVSubTypeVMs(_vTypeID);

            vendorVMs = await purchasingService.GetVendorList();

            customerVMs = await customerService.GetCustomers();

            stockVMs = await inventoryService.GetStockList();

            vatDefVMs = await voucherService.GetVATDefs();

            if (_IsTypeUpdate == 0)
            {
                voucherVM = new();
                voucherDetailVMs = new();

                voucherVM.VCode = filter_vTypeVMs.Where(x => x.VTypeID == _vTypeID).Select(x => x.VCode).First();

                if (filterFinVM.FuncID != "FIN_Cash" && filterFinVM.FuncID != "FIN_Bank")
                {
                    filterFinVM.ITypeCode = voucherVM.ITypeCode = "HH";
                    voucherVM.PaymentTypeCode = "CASH";
                }
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

            voucherVM.UserID = UserID;

            voucherVM.VTypeID = _vTypeID;

            voucherVM.DivisionID = filterFinVM.DivisionID;
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
                    voucherDetailVMs.ToList().ForEach(x => { x.VATCode = String.Empty; x.VATRate = 0; });

                    voucherVM.InvoiceNumber = 0;
                    voucherVM.InvoiceDate = null;
                }
            }
        }

        private string onchange_VendorCode
        {
            get { return voucherVM.VendorCode; }
            set
            {
                voucherVM.VendorCode = value;

                voucherVM.VDesc = "Mua hàng của " + vendorVMs.Where(x => x.VendorCode == voucherVM.VendorCode).Select(x => x.VendorName).FirstOrDefault();
            }
        }

        private string onchange_CustomerCode
        {
            get { return voucherVM.CustomerCode; }
            set
            {
                voucherVM.CustomerCode = value;

                voucherVM.VDesc = "Bán hàng cho " + customerVMs.Where(x => x.CustomerCode == voucherVM.CustomerCode).Select(x => x.CustomerName).FirstOrDefault() + "";
            }
        }

        private async Task<IEnumerable<VoucherDetailVM>> SearchItems(string _valueSearchItems)
        {
            voucherVM.valueSearchItems = _valueSearchItems;
            return await voucherService.GetSearchItems(voucherVM);
        }

        private async Task SelectedItem(VoucherDetailVM _voucherDetailVM)
        {
            if (_voucherDetailVM != null)
            {
                _voucherDetailVM.SeqVD = voucherDetailVMs.Count == 0 ? 1 : voucherDetailVMs.Select(x => x.SeqVD).Max() + 1;

                //Hóa đơn
                if (!voucherVM.IsInvoice)
                {
                    _voucherDetailVM.VATCode = String.Empty;
                    _voucherDetailVM.VATRate = 0;
                }

                if (voucherVM.VTypeID == "FIN_Trf" || voucherVM.VTypeID == "FIN_Output" || voucherVM.VTypeID == "FIN_Sale")
                {
                    await UpdateInventoryCheck_Qty(_voucherDetailVM);
                }

                voucherDetailVMs.Add(_voucherDetailVM);

                await txtSearchItems.Focus();
            }

            await js.InvokeAsync<object>("updateScrollToBottom");
        }

        private async Task CreateVoucherDetail()
        {
            VoucherDetailVM _voucherDetailVM = new();

            _voucherDetailVM.SeqVD = voucherDetailVMs.Count == 0 ? 1 : voucherDetailVMs.Select(x => x.SeqVD).Max() + 1;

            voucherDetailVMs.Add(_voucherDetailVM);

            await js.InvokeAsync<object>("updateScrollToBottom");
        }

        private void onchange_VDQty(ChangeEventArgs e, VoucherDetailVM _voucherDetailVM)
        {
            _voucherDetailVM.VDQty = float.Parse(e.Value.ToString());

            if (_voucherDetailVM.VDPrice * (decimal)_voucherDetailVM.VDQty != 0)
            {
                _voucherDetailVM.VDDiscountPrice = _voucherDetailVM.VDDiscountPercent * _voucherDetailVM.VDPrice * (decimal)_voucherDetailVM.VDQty / 100;
            }

            StateHasChanged();
        }

        private void onchange_VDPrice(ChangeEventArgs e, VoucherDetailVM _voucherDetailVM)
        {
            _voucherDetailVM.VDPrice = decimal.Parse(e.Value.ToString());

            if (_voucherDetailVM.VDPrice * (decimal)_voucherDetailVM.VDQty != 0)
            {
                _voucherDetailVM.VDDiscountPrice = _voucherDetailVM.VDDiscountPercent * _voucherDetailVM.VDPrice * (decimal)_voucherDetailVM.VDQty / 100;
            }

            StateHasChanged();
        }

        private async void onchange_VDDiscountPrice(ChangeEventArgs e, VoucherDetailVM _voucherDetailVM)
        {
            _voucherDetailVM.VDDiscountPrice = decimal.Parse(e.Value.ToString());

            if (_voucherDetailVM.VDPrice * (decimal)_voucherDetailVM.VDQty != 0)
            {
                _voucherDetailVM.VDDiscountPercent = _voucherDetailVM.VDDiscountPrice / _voucherDetailVM.VDPrice * (decimal)_voucherDetailVM.VDQty * 100;
            }

            StateHasChanged();
        }

        private async void onchange_VDDiscountPercent(ChangeEventArgs e, VoucherDetailVM _voucherDetailVM)
        {
            _voucherDetailVM.VDDiscountPercent = decimal.Parse(e.Value.ToString());

            if (_voucherDetailVM.VDPrice * (decimal)_voucherDetailVM.VDQty != 0)
            {
                _voucherDetailVM.VDDiscountPrice = _voucherDetailVM.VDDiscountPercent * _voucherDetailVM.VDPrice * (decimal)_voucherDetailVM.VDQty / 100;
            }

            StateHasChanged();
        }

        private async void onchange_VAT(string value, VoucherDetailVM _voucherDetailVM)
        {
            isLoading = true;

            _voucherDetailVM.VATCode = value;

            _voucherDetailVM.VATRate = vatDefVMs.Where(x => x.VATCode == _voucherDetailVM.VATCode).Select(x => x.VATRate).FirstOrDefault();

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
                var str = LibraryFunc.RepalceWhiteSpace(voucherVM.DataFromExcel.Trim()).Replace(" \t", "\t").Replace("\t ", "\t").Replace(",", string.Empty).Replace(" ", "\n");
                var lines = Regex.Split(str, "\n");

                foreach (var itemfinger in lines)
                {
                    if (string.IsNullOrWhiteSpace(itemfinger))
                        continue;

                    voucherDetailVM = new();

                    var itemSplit = Regex.Split(itemfinger, "\t").Select(x => x.Trim()).ToArray();

                    filterFinVM.ICode = itemSplit[0].Trim();

                    IEnumerable<VoucherDetailVM> _voucherDetailVMs;

                    _voucherDetailVMs = await SearchItems(filterFinVM.ICode);

                    voucherDetailVM = _voucherDetailVMs.First();

                    if (voucherVM.VTypeID == "FIN_InventoryCheck")
                    {
                        voucherDetailVM.InventoryCheck_ActualQty = float.Parse(itemSplit[1].Trim());
                    }
                    else
                    {
                        voucherDetailVM.VDQty = float.Parse(itemSplit[1].Trim());
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

            if (voucherVM.IsTypeUpdate != 2)
            {
                if (voucherDetailVMs.Count == 0)
                {
                    await js.Swal_Message("Cảnh báo!", "Nhập chi tiết không được trống.", SweetAlertMessageType.warning);
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

                if (voucherVM.VTypeID == "FIN_Cash_Payment" || voucherVM.VTypeID == "FIN_Cash_Receipt" || voucherVM.VTypeID == "FIN_Bank_Credit" || voucherVM.VTypeID == "FIN_Bank_Debit")
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

                        _voucherVM.IsTypeUpdate = 0;

                        _voucherVM.UserID = UserID;

                        _voucherVM.DivisionID = filterFinVM.DivisionID;

                        _voucherVM.VReference = voucherVM.VNumber;

                        _voucherVM.VActive = true;

                        _voucherVM.VDate = voucherVM.VDate;

                        if (voucherVM.VTypeID == "FIN_Purchasing")
                        {
                            _voucherVM.VCode = "PC";
                            _voucherVM.VDesc = "Trả tiền " + voucherVM.VDesc;
                            if (voucherVM.PaymentTypeCode == "CASH")
                            {
                                _voucherVM.VTypeID = "FIN_Cash_Payment";
                                _voucherVM.VSubTypeID = "FIN_Cash_Payment_Vendor";
                            }

                            if (voucherVM.PaymentTypeCode == "BANK")
                            {
                                _voucherVM.VTypeID = "FIN_Bank_Debit";
                                _voucherVM.VSubTypeID = "FIN_Bank_Debit_Vendor";
                            }
                        }

                        if (voucherVM.VTypeID == "FIN_Sale")
                        {
                            _voucherVM.VCode = "PT";
                            _voucherVM.VDesc = "Thu tiền " + voucherVM.VDesc;
                            if (voucherVM.PaymentTypeCode == "CASH")
                            {
                                _voucherVM.VTypeID = "FIN_Cash_Receipt";
                                _voucherVM.VSubTypeID = "FIN_Cash_Receipt_Customer";
                            }

                            if (voucherVM.PaymentTypeCode == "BANK")
                            {
                                _voucherVM.VTypeID = "FIN_Bank_Credit";
                                _voucherVM.VSubTypeID = "FIN_Bank_Credit_Customer";
                            }
                        }

                        _voucherDetailVM.VDDesc = _voucherVM.VDesc;
                        _voucherDetailVM.VDPrice = voucherDetailVMs.Select(x => x.VDPrice * (decimal)x.VDQty - x.VDDiscountPrice + (x.VDPrice * (decimal)x.VDQty - x.VDDiscountPrice) * (decimal)x.VATRate).Sum();

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

        //RPT
        protected async Task ViewRPT(string _ReportName)
        {
            filterFinVM.TypeView = 1;

            ReportName = _ReportName;

            if (_ReportName == "FIN_So_chi_tiet_quy_tien_mat")
            {
                cashBooks = await voucherService.GetCashBooks(filterFinVM);
            }

            if (_ReportName == "FIN_Tong_hop_ton_kho")
            {
                inventoryVMs = await inventoryService.GetInventorys(filterFinVM);
            }
        }

    }
}
