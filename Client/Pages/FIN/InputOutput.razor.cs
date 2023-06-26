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
using D69soft.Shared.Models.Entities.HR;
using D69soft.Client.Pages.HR;
using D69soft.Shared.Models.ViewModels.CRM;
using D69soft.Client.Services.CRM;

namespace D69soft.Client.Pages.FIN
{
    partial class InputOutput
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

        //PermisFunc
        bool FIN_InOut_Update;
        bool FIN_InOut_CancelActive;

        LogVM logVM = new();

        //Filter
        FilterFinVM filterFinVM = new();
        FilterHrVM filterHrVM = new();

        //Division
        IEnumerable<DivisionVM> filter_divisionVMs;

        //VType
        IEnumerable<VTypeVM> filter_vTypeVMs;
        IEnumerable<VSubTypeVM> filter_vSubTypeVMs;

        IEnumerable<VSubTypeVM> vSubTypeVMs;

        //Voucher
        StockVoucherVM stockVoucherVM = new();
        List<StockVoucherVM> stockVoucherVMs;

        //VoucherDetail
        StockVoucherDetailVM stockVoucherDetailVM = new();
        List<StockVoucherDetailVM> stockVoucherDetailVMs;

        //Vendor
        IEnumerable<VendorVM> vendorVMs;

        //Customer
        IEnumerable<CustomerVM> customerVMs;

        //Stock
        IEnumerable<StockVM> stockVMs;

        //VAT
        IEnumerable<VATDefVM> vatDefVMs;

        private BlazoredTypeahead<StockVoucherDetailVM, StockVoucherDetailVM> txtSearchItems;

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

            await js.InvokeAsync<object>("keyPressNextInput");
        }

        protected override async Task OnInitializedAsync()
        {
            filterHrVM.UserID = UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "FIN_InOut"))
            {
                logVM.LogUser = UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "FIN_InOut";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            FIN_InOut_Update = await sysService.CheckAccessSubFunc(UserID, "FIN_InOut_Update");
            FIN_InOut_CancelActive = await sysService.CheckAccessSubFunc(UserID, "FIN_InOut_CancelActive");

            filter_divisionVMs = await organizationalChartService.GetDivisionList(filterHrVM);
            filterFinVM.DivisionID = filter_divisionVMs.Count() > 0 ? filter_divisionVMs.ElementAt(0).DivisionID : string.Empty;

            filterFinVM.FuncID = "FIN_InOut";

            filterFinVM.searchActive = 2;

            filterFinVM.StartDate = DateTime.Now;
            filterFinVM.EndDate = DateTime.Now;

            filter_vTypeVMs = await voucherService.GetVTypeVMs("FIN_InOut");
            filter_vSubTypeVMs = await voucherService.GetVSubTypeVMs("FIN_InOut");

            await GetVouchers();

            isLoadingScreen = false;
        }

        public void OnRangeSelect(DateRange _range)
        {
            filterFinVM.StartDate = _range.Start;
            filterFinVM.EndDate = _range.End;
        }

        private async Task GetVouchers()
        {
            isLoading = true;

            ReportName = "CustomNewReport";

            stockVoucherVM = new();
            stockVoucherDetailVMs = new();

            stockVoucherVMs = await voucherService.GetStockVouchers(filterFinVM);

            isLoading = false;
        }

        private void onclick_Selected(StockVoucherVM _stockVoucherVM)
        {
            stockVoucherVM = _stockVoucherVM == stockVoucherVM ? new() : _stockVoucherVM;
        }

        private string SetSelected(StockVoucherVM _stockVoucherVM)
        {
            if (stockVoucherVM.VNumber != _stockVoucherVM.VNumber)
            {
                return string.Empty;
            }
            return "selected";
        }

        private async Task InitializeModalUpdate_Voucher(string _vTypeID, int _IsTypeUpdate)
        {
            isLoading = true;

            vendorVMs = await purchasingService.GetVendorList();

            customerVMs = await customerService.GetCustomers();

            stockVMs = await inventoryService.GetStockList();

            vatDefVMs = await voucherService.GetVATDefs();

            if (_IsTypeUpdate == 0)
            {
                stockVoucherVM = new();
                stockVoucherDetailVMs = new();
            }

            if (_IsTypeUpdate != 0)
            {
                stockVoucherDetailVMs = await voucherService.GetStockVoucherDetails(stockVoucherVM.VNumber);
            }

            if (_IsTypeUpdate == 0 || _IsTypeUpdate == 5)
            {
                stockVoucherVM.VDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                stockVoucherVM.VActive = false;
            }

            if (_IsTypeUpdate == 5)
            {
                foreach (var _stockVoucherDetailVM in stockVoucherDetailVMs)
                {
                    _stockVoucherDetailVM.InventoryCheck_Qty = await inventoryService.GetInventoryCheck_Qty(stockVoucherVM.VDate.Value, _stockVoucherDetailVM);
                }

                stockVoucherDetailVMs.ForEach(e => e.Price = e.IPrice);

                stockVoucherDetailVMs.ForEach(e => { e.FromStockCode = e.StockDefault; e.FromStockName = stockVMs.Where(x => x.StockCode == e.StockDefault).Select(x => x.StockName).FirstOrDefault(); });

                stockVoucherDetailVMs.ForEach(e => { e.ToStockCode = e.StockDefault; e.ToStockName = stockVMs.Where(x => x.StockCode == e.StockDefault).Select(x => x.StockName).FirstOrDefault(); });

                stockVoucherDetailVMs.ForEach(e => { e.VendorCode = e.VendorDefault; e.VendorName = vendorVMs.Where(x => x.VendorCode == e.VendorDefault).Select(x => x.VendorName).FirstOrDefault(); });
            }

            stockVoucherVM.UserID = UserID;

            stockVoucherVM.VTypeID = _vTypeID;
            vSubTypeVMs = filter_vSubTypeVMs.Where(x => x.VTypeID == _vTypeID);

            if (_IsTypeUpdate == 0)
            {
                stockVoucherVM.VSubTypeID = vSubTypeVMs.ElementAt(0).VSubTypeID;
            }

            stockVoucherVM.DivisionID = filterFinVM.DivisionID;
            stockVoucherVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Voucher");

            await js.InvokeAsync<object>("bootrap_select_refresh");

            isLoading = false;
        }

        private string onchange_VendorCode
        {
            get { return stockVoucherVM.VendorCode; }
            set
            {
                stockVoucherVM.VendorCode = value;
                if (String.IsNullOrEmpty(value))
                {
                    stockVoucherVM.VDesc = "Mua hàng";
                }
                else
                {
                    stockVoucherVM.VDesc = "Mua hàng của " + vendorVMs.Where(x => x.VendorCode == stockVoucherVM.VendorCode).Select(x => x.VendorName).First() + "";
                }
            }
        }

        private string onchange_CustomerCode
        {
            get { return stockVoucherVM.CustomerCode; }
            set
            {
                stockVoucherVM.CustomerCode = value;
                if (String.IsNullOrEmpty(value))
                {
                    stockVoucherVM.VDesc = "Xuất kho bán hàng";
                }
                else
                {
                    stockVoucherVM.VDesc = "Xuất kho bán hàng cho " + customerVMs.Where(x => x.CustomerCode == stockVoucherVM.CustomerCode).Select(x => x.CustomerName).First() + "";
                }
            }
        }

        private async Task<IEnumerable<StockVoucherDetailVM>> SearchItems(string searchText)
        {
            filterFinVM.searchText = searchText;
            filterFinVM.IActive = true;
            return await voucherService.GetSearchItems(filterFinVM);
        }

        private async Task SelectedItem(StockVoucherDetailVM result)
        {
            if (result != null)
            {
                result.SeqVD = stockVoucherDetailVMs.Count == 0 ? 1 : stockVoucherDetailVMs.Select(x => x.SeqVD).Max() + 1;

                if (stockVoucherVM.VSubTypeID == "FIN_Output_SalePOS" && !String.IsNullOrEmpty(stockVoucherVM.Reference_VNumber))
                {
                    result.FromStockCode = stockVoucherVM.Reference_StockCode;
                    result.FromStockName = stockVMs.Where(x => x.StockCode == stockVoucherVM.Reference_StockCode).Select(x => x.StockName).First();
                }

                if (stockVoucherVM.VTypeID == "FIN_Output" || stockVoucherVM.VTypeID == "FIN_Transfer")
                {
                    await UpdateInventoryCheck_Qty(result);
                }

                stockVoucherDetailVMs.Add(result);

                await txtSearchItems.Focus();
            }

            await js.InvokeAsync<object>("updateScrollToBottom");
        }

        private void UpdateItem(StockVoucherDetailVM result, StockVoucherDetailVM _stockVoucherDetailVM)
        {
            if (result != null)
            {
                _stockVoucherDetailVM.IsUpdateItem = 0;
                _stockVoucherDetailVM.ICode = result.ICode;
                _stockVoucherDetailVM.IName = result.IName;
                _stockVoucherDetailVM.IUnitName = result.IUnitName;
                _stockVoucherDetailVM.Price = result.Price;
                _stockVoucherDetailVM.FromStockCode = result.FromStockCode;
                _stockVoucherDetailVM.FromStockName = result.FromStockName;
                _stockVoucherDetailVM.ToStockCode = result.ToStockCode;
                _stockVoucherDetailVM.ToStockName = result.ToStockName;
            }
            else
            {
                _stockVoucherDetailVM.IsUpdateItem = 0;
            }

            StateHasChanged();
        }

        private async void onchange_FromStockCode(string value, StockVoucherDetailVM _stockVoucherDetailVM)
        {
            isLoading = true;

            _stockVoucherDetailVM.FromStockCode = value;

            await UpdateInventoryCheck_Qty(_stockVoucherDetailVM);

            isLoading = false;

            StateHasChanged();
        }

        private async Task onclick_copyFromStockCode(StockVoucherDetailVM _stockVoucherDetailVM)
        {
            isLoading = true;

            stockVoucherDetailVMs.Where(x => x.SeqVD > _stockVoucherDetailVM.SeqVD).ToList().ForEach(e => { e.FromStockCode = _stockVoucherDetailVM.FromStockCode; e.FromStockName = stockVMs.Where(x => x.StockCode == _stockVoucherDetailVM.FromStockCode).Select(x => x.StockName).FirstOrDefault(); });

            foreach (var _stockVoucherDetailVM_0 in stockVoucherDetailVMs.Where(x => x.SeqVD > _stockVoucherDetailVM.SeqVD))
            {
                _stockVoucherDetailVM_0.InventoryCheck_Qty = await inventoryService.GetInventoryCheck_Qty(stockVoucherVM.VDate.Value, _stockVoucherDetailVM_0);
            }

            isLoading = false;
        }

        private async void onchange_VAT(string value, StockVoucherDetailVM _stockVoucherDetailVM)
        {
            isLoading = true;

            _stockVoucherDetailVM.VATCode = value;

            _stockVoucherDetailVM.VATRate = vatDefVMs.Where(x => x.VATCode == _stockVoucherDetailVM.VATCode).Select(x => x.VATRate).FirstOrDefault();

            isLoading = false;

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
                var str = LibraryFunc.RepalceWhiteSpace(stockVoucherVM.DataFromExcel.Trim()).Replace(" \t", "\t").Replace("\t ", "\t").Replace(",", string.Empty).Replace(" ", "\n");
                var lines = Regex.Split(str, "\n");

                foreach (var itemfinger in lines)
                {
                    if (string.IsNullOrWhiteSpace(itemfinger))
                        continue;

                    stockVoucherDetailVM = new();

                    var itemSplit = Regex.Split(itemfinger, "\t").Select(x => x.Trim()).ToArray();

                    filterFinVM.ICode = itemSplit[0].Trim();

                    IEnumerable<StockVoucherDetailVM> _stockVoucherDetailVMs;

                    _stockVoucherDetailVMs = await SearchItems(filterFinVM.ICode);

                    stockVoucherDetailVM = _stockVoucherDetailVMs.First();

                    stockVoucherDetailVM.Qty = float.Parse(itemSplit[1].Trim());

                    stockVoucherDetailVM.SeqVD = stockVoucherDetailVMs.Count == 0 ? 1 : stockVoucherDetailVMs.Select(x => x.SeqVD).Max() + 1;

                    if (stockVoucherVM.VSubTypeID == "FIN_Output_SalePOS" && !String.IsNullOrEmpty(stockVoucherVM.Reference_VNumber))
                    {
                        stockVoucherDetailVM.FromStockCode = stockVoucherVM.Reference_StockCode;
                        stockVoucherDetailVM.FromStockName = stockVMs.Where(x => x.StockCode == stockVoucherVM.Reference_StockCode).Select(x => x.StockName).First();
                    }

                    if (stockVoucherVM.VTypeID == "FIN_Output" || stockVoucherVM.VTypeID == "FIN_Transfer")
                    {
                        await UpdateInventoryCheck_Qty(stockVoucherDetailVM);
                    }

                    stockVoucherDetailVMs.Add(stockVoucherDetailVM);

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_PostExcel");
                }

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            catch (Exception)
            {
                await js.Swal_Message("Thông báo!", "Dữ liệu không hợp lệ.", SweetAlertMessageType.error);
            }

            stockVoucherVM.DataFromExcel = string.Empty;

            isLoading = false;
        }

        private async Task UpdateVoucher(EditContext formStockVoucherVM, int _IsTypeUpdate)
        {
            stockVoucherVM.IsTypeUpdate = _IsTypeUpdate;

            if (!formStockVoucherVM.Validate()) return;

            isLoading = true;

            stockVoucherVM.IsTypeUpdate = stockVoucherVM.IsTypeUpdate == 5 ? 0 : stockVoucherVM.IsTypeUpdate;

            if (stockVoucherVM.IsTypeUpdate != 2)
            {
                if (stockVoucherDetailVMs.Count == 0)
                {
                    await js.Swal_Message("Cảnh báo!", "Dữ liệu mặt hàng không được trống.", SweetAlertMessageType.warning);
                    isLoading = false;
                    return;
                }

                if (stockVoucherVM.VTypeID == "FIN_Input")
                {
                    stockVoucherDetailVMs.ToList().ForEach(x => x.FromStockCode = String.Empty);

                    if (stockVoucherVM.VSubTypeID == "FIN_Input_Purchasing")
                    {
                        stockVoucherDetailVMs.ToList().ForEach(x => x.VendorCode = stockVoucherVM.VendorCode);
                    }
                    else
                    {
                        stockVoucherDetailVMs.ToList().ForEach(x => x.VendorCode = String.Empty);
                    }

                    if (stockVoucherDetailVMs.Where(x => String.IsNullOrEmpty(x.ToStockCode)).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Kho nhập không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }
                }

                if (stockVoucherVM.VTypeID == "FIN_Output")
                {
                    stockVoucherDetailVMs.ToList().ForEach(x => x.ToStockCode = String.Empty);
                    stockVoucherDetailVMs.ToList().ForEach(x => x.VendorCode = String.Empty);

                    if (stockVoucherVM.VSubTypeID == "FIN_Output_SalePOS")
                    {
                        stockVoucherDetailVMs.ToList().ForEach(x => x.CustomerCode = stockVoucherVM.CustomerCode);
                    }
                    else
                    {
                        stockVoucherDetailVMs.ToList().ForEach(x => x.CustomerCode = String.Empty);
                    }

                    if (stockVoucherDetailVMs.Where(x => String.IsNullOrEmpty(x.FromStockCode)).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Kho xuất không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }
                }

                if (stockVoucherVM.VTypeID == "FIN_Transfer")
                {
                    if (stockVoucherVM.VSubTypeID == "FIN_Transfer_Purchasing")
                    {
                        if (stockVoucherVM.IsMultipleInvoice)
                        {
                            stockVoucherVM.VendorCode = String.Empty;
                        }
                        else
                        {
                            stockVoucherDetailVMs.ToList().ForEach(x => x.VendorCode = stockVoucherVM.VendorCode);
                        }
                    }
                    else
                    {
                        stockVoucherDetailVMs.ToList().ForEach(x => x.VendorCode = String.Empty);
                    }

                    if (stockVoucherDetailVMs.Where(x => String.IsNullOrEmpty(x.FromStockCode)).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Kho xuất không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }

                    if (stockVoucherDetailVMs.Where(x => x.ToStockCode == String.Empty).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Kho nhập không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }
                }

                stockVoucherVM.VNumber = await voucherService.UpdateVoucher(stockVoucherVM, stockVoucherDetailVMs);

                stockVoucherVM.IsTypeUpdate = 3;


                logVM.LogDesc = "Cập nhật chứng từ " + stockVoucherVM.VNumber + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    await voucherService.UpdateVoucher(stockVoucherVM, stockVoucherDetailVMs);

                    logVM.LogDesc = "Xóa chứng từ " + stockVoucherVM.VNumber + "";
                    await sysService.InsertLog(logVM);

                    await GetVouchers();

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Voucher");
                    await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                }
                else
                {
                    stockVoucherVM.IsTypeUpdate = 1;
                }
            }

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
                    if (stockVoucherVM.VTypeID == "FIN_Output" || stockVoucherVM.VTypeID == "FIN_Transfer")
                    {
                        foreach (var _stockVoucherDetailVM in stockVoucherDetailVMs)
                        {
                            _stockVoucherDetailVM.InventoryCheck_Qty = await inventoryService.GetInventoryCheck_Qty(stockVoucherVM.VDate.Value, _stockVoucherDetailVM);
                        }

                        //if (stockVoucherDetailVMs.GroupBy(x => new { x.ICode, x.FromStockCode, x.InventoryCheck_Qty }).Select(x => new { InventoryCheck_Qty = x.Max(x => x.InventoryCheck_Qty), sumQty = x.Sum(x => x.Qty) }).Where(x => x.sumQty > x.InventoryCheck_Qty).Count() > 0)
                        //{
                        //    await js.Swal_Message("Cảnh báo!", "Số lượng xuất kho vượt quá số lượng tồn trong kho.", SweetAlertMessageType.warning);
                        //    isLoading = false;
                        //    return;
                        //}
                    }
                }

                stockVoucherVM.IsTypeUpdate = _IsTypeUpdate;
                stockVoucherVM.VActive = _VActive;

                await voucherService.UpdateVoucher(stockVoucherVM, stockVoucherDetailVMs);

                stockVoucherVM.IsTypeUpdate = 3;

                logVM.LogDesc = question + " chứng từ " + stockVoucherVM.VNumber + "";
                await sysService.InsertLog(logVM);

                await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        string ReportName = String.Empty;

        protected async Task PrintVoucher(string _ReportName)
        {
            ReportName = $"{_ReportName}?VNumber={stockVoucherVM.VNumber}";
            await js.InvokeAsync<object>("ShowModal", "#InitializeModalView_Rpt");
        }

        private async Task UpdateAllInventoryCheck_Qty()
        {
            isLoading = true;

            foreach (var _stockVoucherDetailVM in stockVoucherDetailVMs)
            {
                _stockVoucherDetailVM.InventoryCheck_Qty = await inventoryService.GetInventoryCheck_Qty(stockVoucherVM.VDate.Value, _stockVoucherDetailVM);
            }

            isLoading = false;
        }

        private async Task UpdateInventoryCheck_Qty(StockVoucherDetailVM _stockVoucherDetailVM)
        {
            _stockVoucherDetailVM.InventoryCheck_Qty = await inventoryService.GetInventoryCheck_Qty(stockVoucherVM.VDate.Value, _stockVoucherDetailVM);
        }

    }
}
