using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Services.FIN;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Utilities;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;
using D69soft.Shared.Models.ViewModels.HR;
using Newtonsoft.Json.Linq;

namespace D69soft.Client.Pages.FIN
{
    partial class Items
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] VoucherService voucherService { get; set; }
        [Inject] PurchasingService purchasingService { get; set; }
        [Inject] InventoryService inventoryService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        //ItemsType
        IEnumerable<ItemsTypeVM> itemsTypeVMs;

        //ItemsClass
        IEnumerable<ItemsClassVM> itemsClassVMs;

        //ItemsGroup
        ItemsGroupVM itemsGroupVM = new();
        IEnumerable<ItemsGroupVM> itemsGroupVMs;

        //Items
        ItemsVM itemsVM = new();
        List<ItemsVM> itemsVMs;

        //Unit
        ItemsUnitVM itemsUnitVM = new();
        IEnumerable<ItemsUnitVM> itemsUnitVMs;

        //VAT
        IEnumerable<VATDefVM> vatDefVMs;

        //Stock
        IEnumerable<StockVM> stockVMs;

        //Vendor
        IEnumerable<VendorVM> vendorVMs;

        //QuantitativeItems
        ItemsVM qi_itemsVM = new();
        QuantitativeItemsVM quantitativeItemsVM = new();
        List<QuantitativeItemsVM> quantitativeItemsVMs;

        //Sort table
        private bool isSortedAscending;
        private string activeSortColumn;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");

            await js.InvokeAsync<object>("maskCurrency");
        }

        protected override async Task OnInitializedAsync()
        {
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(filterVM.UserID, "FIN_Items"))
            {
                logVM.LogUser = filterVM.UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "FIN_Items";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            itemsTypeVMs = await inventoryService.GetItemsTypes();

            itemsClassVMs = await inventoryService.GetItemsClassList();

            itemsGroupVMs = await inventoryService.GetItemsGroupList();

            vatDefVMs = await voucherService.GetVATDefs();

            filterVM.IActive = true;

            await GetItems();

            isLoadingScreen = false;
        }

        private async Task GetItems()
        {
            isLoading = true;

            itemsVM = new();

            itemsVMs = await inventoryService.GetItemsList(filterVM);

            isLoading = false;
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

        private async void onchange_filter_IClsCode(string value)
        {
            isLoading = true;

            filterVM.IClsCode = value;

            filterVM.IGrpCode = String.Empty;

            filterVM.searchText = String.Empty;

            await GetItems();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_IGrpCode(string value)
        {
            isLoading = true;

            filterVM.IGrpCode = value;

            itemsGroupVM = String.IsNullOrEmpty(value) ? new() : itemsGroupVMs.First(x => x.IGrpCode == value);

            filterVM.searchText = String.Empty;

            await GetItems();

            isLoading = false;

            StateHasChanged();
        }

        protected async void FilterItems(ChangeEventArgs args)
        {
            filterVM.searchText = args.Value.ToString();

            await GetItems();

            StateHasChanged();
        }

        private async Task onchange_filter_IActive(ChangeEventArgs args)
        {
            isLoading = true;

            filterVM.IActive = bool.Parse(args.Value.ToString());

            filterVM.searchText = String.Empty;

            await GetItems();

            isLoading = false;
        }

        private async Task InitializeModalUpdate_ItemsGroup(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                itemsGroupVM = new();

                itemsGroupVM.IClsCode = filterVM.IClsCode;
            }

            itemsGroupVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_ItemsGroup");

            isLoading = false;
        }

        private async Task UpdateItemsGroup()
        {
            isLoading = true;

            if (itemsGroupVM.IsTypeUpdate != 2)
            {
                await inventoryService.UpdateItemsGroup(itemsGroupVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ItemsGroup");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await inventoryService.UpdateItemsGroup(itemsGroupVM);

                    if (affectedRows > 0)
                    {
                        filterVM.IGrpCode = String.Empty;
                        itemsVMs = await inventoryService.GetItemsList(filterVM);

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ItemsGroup");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu hàng hóa liên quan.", SweetAlertMessageType.error);
                        itemsGroupVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    itemsGroupVM.IsTypeUpdate = 1;
                }
            }

            itemsGroupVMs = await inventoryService.GetItemsGroupList();

            isLoading = false;
        }

        private async Task InitializeModalUpdate_Items(int _IsTypeUpdate)
        {
            isLoading = true;

            itemsUnitVMs = await inventoryService.GetItemsUnitList();

            stockVMs = await inventoryService.GetStockList();

            vendorVMs = await purchasingService.GetVendorList();

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

        private async Task<IEnumerable<ItemsVM>> SearchItems(string searchText)
        {
            return await inventoryService.GetItemsForQuantitative(searchText, itemsVM.ICode);
        }

        private void SelectedItem(ItemsVM result)
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

                itemsVM.ICode = await inventoryService.UpdateItems(itemsVM, quantitativeItemsVMs);
                itemsVM.IsTypeUpdate = 1;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    var result = await inventoryService.UpdateItems(itemsVM, quantitativeItemsVMs);

                    if (result != "Err_NotDel")
                    {
                        await GetItems();

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Items");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu hàng hóa liên quan.", SweetAlertMessageType.error);
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

        private async Task InitializeModalUpdate_ItemsUnit(int _IsTypeUpdate)
        {
            isLoading = true;

            itemsUnitVMs = await inventoryService.GetItemsUnitList();

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

        private async Task UpdateItemsUnit()
        {
            isLoading = true;

            if (itemsUnitVM.IsTypeUpdate != 2)
            {
                await inventoryService.UpdateItemsUnit(itemsUnitVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ItemsUnit");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await inventoryService.UpdateItemsUnit(itemsUnitVM);

                    if (affectedRows > 0)
                    {
                        itemsVM.IUnitCode = String.Empty;
                        itemsUnitVMs = await inventoryService.GetItemsUnitList();

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ItemsUnit");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu hàng hóa liên quan.", SweetAlertMessageType.error);
                        itemsUnitVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    itemsUnitVM.IsTypeUpdate = 1;
                }
            }

            itemsUnitVMs = await inventoryService.GetItemsUnitList();

            isLoading = false;
        }
    }
}
