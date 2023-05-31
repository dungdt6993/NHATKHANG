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
using D69soft.Client.Extension;
using Microsoft.AspNetCore.Components.Forms;
using D69soft.Shared.Models.Entities.HR;

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



        bool isLoading;
        bool isLoadingScreen = true;

        protected string UserID;

        //PermisFunc
        bool STOCK_InOut_Update;
        bool STOCK_InOut_CancelActive;

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

        //Stock
        IEnumerable<StockVM> stockVMs;

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

            if (await sysService.CheckAccessFunc(UserID, "STOCK_InOut"))
            {
                logVM.LogUser = UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "STOCK_InOut";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            STOCK_InOut_Update = await sysService.CheckAccessSubFunc(UserID, "STOCK_InOut_Update");
            STOCK_InOut_CancelActive = await sysService.CheckAccessSubFunc(UserID, "STOCK_InOut_CancelActive");

            filter_divisionVMs = await organizationalChartService.GetDivisionList(filterHrVM);
            filterFinVM.DivisionID = filter_divisionVMs.Count() > 0 ? filter_divisionVMs.ElementAt(0).DivisionID : string.Empty;

            filterFinVM.FuncID = "STOCK_InOut";

            filterFinVM.searchActive = 2;

            filterFinVM.StartDate = DateTime.Now;
            filterFinVM.EndDate = DateTime.Now;

            filter_vTypeVMs = await voucherService.GetVTypeVMs("STOCK_InOut");
            filter_vSubTypeVMs = await voucherService.GetVSubTypeVMs("STOCK_InOut");

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

            stockVMs = await inventoryService.GetStockList();

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

                if (stockVoucherVM.VSubTypeID == "STOCK_Output_SalePOS" && !String.IsNullOrEmpty(stockVoucherVM.Reference_VNumber))
                {
                    result.FromStockCode = stockVoucherVM.Reference_StockCode;
                    result.FromStockName = stockVMs.Where(x => x.StockCode == stockVoucherVM.Reference_StockCode).Select(x => x.StockName).First();
                }

                if (stockVoucherVM.VTypeID == "STOCK_Output" || stockVoucherVM.VTypeID == "STOCK_Transfer")
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

                    if (stockVoucherVM.VSubTypeID == "STOCK_Output_SalePOS" && !String.IsNullOrEmpty(stockVoucherVM.Reference_VNumber))
                    {
                        stockVoucherDetailVM.FromStockCode = stockVoucherVM.Reference_StockCode;
                        stockVoucherDetailVM.FromStockName = stockVMs.Where(x => x.StockCode == stockVoucherVM.Reference_StockCode).Select(x => x.StockName).First();
                    }

                    if (stockVoucherVM.VTypeID == "STOCK_Output" || stockVoucherVM.VTypeID == "STOCK_Transfer")
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

                if (stockVoucherVM.VTypeID == "STOCK_Input")
                {
                    stockVoucherDetailVMs.ToList().ForEach(x => x.FromStockCode = String.Empty);

                    if (stockVoucherVM.VSubTypeID == "STOCK_Input_Purchasing")
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

                    if (stockVoucherDetailVMs.Where(x => x.ToStockCode == String.Empty).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Nhập kho không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }
                }

                if (stockVoucherVM.VTypeID == "STOCK_Output")
                {
                    stockVoucherDetailVMs.ToList().ForEach(x => x.ToStockCode = String.Empty);
                    stockVoucherDetailVMs.ToList().ForEach(x => x.VendorCode = String.Empty);

                    if (stockVoucherDetailVMs.Where(x => x.FromStockCode == String.Empty).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Xuất kho không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }
                }

                if (stockVoucherVM.VTypeID == "STOCK_Transfer")
                {
                    if (stockVoucherVM.VSubTypeID == "STOCK_Transfer_Purchasing")
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

                    if (stockVoucherDetailVMs.Where(x => x.FromStockCode == String.Empty).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Xuất kho không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }

                    if (stockVoucherDetailVMs.Where(x => x.ToStockCode == String.Empty).Count() > 0)
                    {
                        await js.Swal_Message("Cảnh báo!", "Nhập kho không được trống.", SweetAlertMessageType.warning);
                        isLoading = false;
                        return;
                    }
                }

                stockVoucherVM.VNumber = await voucherService.UpdateVoucher(stockVoucherVM, stockVoucherDetailVMs);

                stockVoucherVM.IsTypeUpdate = 3;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    await voucherService.UpdateVoucher(stockVoucherVM, stockVoucherDetailVMs);

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Voucher");
                    await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);

                    stockVoucherVM = new();
                    await GetVouchers();
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
                    if (stockVoucherVM.VTypeID == "STOCK_Output" || stockVoucherVM.VTypeID == "STOCK_Transfer")
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

                await js.Toast_Alert("" + question + " thành công!", SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        string ReportName = String.Empty;

        protected async Task PrintVoucher()
        {
            ReportName = "Phieu_xuat_chuyen_kho_noi_bo?VNumber=" + stockVoucherVM.VNumber + "";
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
