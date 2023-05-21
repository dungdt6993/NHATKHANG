using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using BlazorDateRangePicker;
using D69soft.Client.Services;
using D69soft.Client.Services.HR;
using D69soft.Client.Services.FIN;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.EA;
using D69soft.Client.Helpers;
using D69soft.Shared.Models.ViewModels.SYSTEM;

namespace D69soft.Client.Pages.EA
{
    partial class Request
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] AuthService authService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] VoucherService voucherService { get; set; }
        [Inject] RequestService requestService { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingScreen = true;

        LogVM logVM = new();

        //Filter
        FilterFinVM filterFinVM = new();
        FilterHrVM filterHrVM = new();

        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;

        //Request
        List<RequestVM> requestVMs;
        List<RequestVM> search_requestVMs;

        //Voucher
        StockVoucherVM stockVoucherVM = new();
        List<StockVoucherDetailVM> stockVoucherDetailVMs;

        bool permisSubFunc_EA_Request_Handover;

        bool userRoleAdmin;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
            await js.InvokeAsync<object>("tooltip");
        }

        protected override async Task OnInitializedAsync()
        {
            UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "EA_Request"))
            {
                logVM.LogType = "FUNC";
                logVM.LogName = "EA_Request";
                logVM.LogUser = UserID;
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            filterFinVM.UserID = filterHrVM.UserID = UserID;

            division_filter_list = await organizationalChartService.GetDivisionList(filterHrVM);
            filterFinVM.DivisionID = filterHrVM.DivisionID = division_filter_list.Count() > 0 ? division_filter_list.ElementAt(0).DivisionID : string.Empty;

            filterFinVM.DepartmentID = filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterHrVM);

            filterFinVM.EndDate = new DateTime(DateTime.MaxValue.Year, DateTime.MaxValue.Month, DateTime.MaxValue.Day);

            permisSubFunc_EA_Request_Handover = await sysService.CheckAccessSubFunc(UserID, "EA_Request_Handover");

            if (await authService.GetRole(UserID) <= 2)
            {
                userRoleAdmin = true;
            }

            if (permisSubFunc_EA_Request_Handover)
            {
                filterFinVM.isHandover = true;
            }

            filterFinVM.ShowEntity = 100;
            filterFinVM.Status = "all";

            requestVMs = search_requestVMs = await requestService.GetRequest(filterFinVM);

            isLoadingScreen = false;
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterFinVM.DivisionID = filterHrVM.DivisionID = value;

            filterFinVM.DepartmentID = filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterHrVM);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_department(string value)
        {
            isLoading = true;

            filterFinVM.DepartmentID = filterHrVM.DepartmentID = value;

            requestVMs = search_requestVMs = await requestService.GetRequest(filterFinVM);

            await FilterRequest(filterFinVM.Status);

            isLoading = false;

            StateHasChanged();
        }

        private async void OnRangeSelect(DateRange _range)
        {
            isLoading = true;

            filterFinVM.StartDate = _range.Start;
            filterFinVM.EndDate = _range.End;

            requestVMs = search_requestVMs = await requestService.GetRequest(filterFinVM);

            await FilterRequest(filterFinVM.Status);

            isLoading = false;

            StateHasChanged();
        }

        private async Task SendApprove(bool value, string type, RequestVM _requestVM)
        {
            isLoading = true;

            var str = String.Empty;

            if (type == "SendDirectManager" || type == "SendControlDept")
            {
                str = value ? "Gửi duyệt" : "Hủy gửi duyệt";
            }

            if (type == "SendDirectManager")
            {
                str = value ? "Duyệt" : "Hủy duyệt";
            }

            if (type == "SendDelRequest")
            {
                str = "Hủy";
            }

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn " + str + "?", SweetAlertMessageType.question))
            {
                if (type == "SendDirectManager")
                {
                    _requestVM.isSendDirectManager = value;
                    _requestVM.TimeSendDirectManager = DateTime.Now;
                }

                if (type == "SendControlDept")
                {
                    _requestVM.isSendControlDept = value;
                    _requestVM.TimeSendControlDept = DateTime.Now;
                }

                if (type == "SendApprove")
                {
                    _requestVM.isSendApprove = value;
                    _requestVM.TimeSendApprove = DateTime.Now;

                    if (_requestVM.isSendApprove)
                    {
                        if (search_requestVMs.Where(x => x.RequestCode == _requestVM.RequestCode && x.QtyApproved > 0).Count() > 0)
                        {
                            //Tu dong tao phieu xuat chuyen kho noi bo
                            stockVoucherVM = new();
                            stockVoucherDetailVMs = new();

                            stockVoucherVM.UserID = UserID;
                            stockVoucherVM.DivisionID = _requestVM.DivisionID;

                            stockVoucherVM.IsTypeUpdate = 0;
                            stockVoucherVM.VTypeID = "STOCK_Transfer";
                            stockVoucherVM.VSubTypeID = "STOCK_Transfer_Internal";
                            stockVoucherVM.Reference_VNumber = _requestVM.RequestCode;
                            stockVoucherVM.VDesc = $"Yêu cầu cấp hàng số {_requestVM.RequestCode} - {_requestVM.ReasonOfRequest} ";
                            stockVoucherVM.VDate = _requestVM.TimeSendApprove;

                            stockVoucherDetailVMs = await requestService.GetRequestDetailToStockVoucherDetail(_requestVM.RequestCode);

                            await voucherService.UpdateVoucher(stockVoucherVM, stockVoucherDetailVMs);
                        }
                    }
                }

                await requestService.SendApprove(_requestVM, type);

                if (type == "SendDelRequest")
                {
                    requestVMs = search_requestVMs = await requestService.GetRequest(filterFinVM);
                }

                await FilterRequest(filterFinVM.Status);

                ReportName = "CustomNewReport";

                await js.Toast_Alert("" + str + " thành công!", SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        private async void onchange_QtyApproved(ChangeEventArgs e, RequestVM _requestVM)
        {
            _requestVM.QtyApproved = float.Parse(e.Value.ToString());
            await requestService.UpdateQtyApproved(_requestVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            ReportName = "CustomNewReport";

            StateHasChanged();
        }

        private async void onchange_RDNote(ChangeEventArgs e, RequestVM _requestVM)
        {
            _requestVM.RDNote = e.Value.ToString();
            await requestService.UpdateRDNote(_requestVM);

            _requestVM.IsUpdateRDNote = false;

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            ReportName = "CustomNewReport";

            StateHasChanged();
        }

        string ReportName = String.Empty;
        protected async Task PrintRequest(String _requestCode)
        {
            ReportName = "EA_Request_PrintRequest?RequestCode=" + _requestCode + "";
            await js.InvokeAsync<object>("ShowModal", "#InitializeModalView_Rpt");
        }

        protected async Task PrintHandover(String _requestCode)
        {
            ReportName = "EA_Request_PrintHandover?RequestCode=" + _requestCode + "";
            await js.InvokeAsync<object>("ShowModal", "#InitializeModalView_Rpt");
        }

        //Load Request
        private Virtualize<RequestVM> RequestContainer { get; set; }
        protected async Task FilterRequest(String _filterRequest)
        {
            isLoading = true;

            filterFinVM.Status = _filterRequest;

            if (filterFinVM.Status == "all")
            {
                search_requestVMs = requestVMs;

                await RequestContainer.RefreshDataAsync();
                StateHasChanged();
            }

            if (filterFinVM.Status == "pending")
            {
                search_requestVMs = requestVMs.Where(x => !x.isSendApprove).ToList();

                await RequestContainer.RefreshDataAsync();
                StateHasChanged();
            }

            if (filterFinVM.Status == "approved")
            {
                search_requestVMs = requestVMs.Where(x => x.isSendApprove).ToList();

                await RequestContainer.RefreshDataAsync();
                StateHasChanged();
            }

            if (filterFinVM.Status == "nothandover")
            {
                search_requestVMs = requestVMs.Where(x => x.isSendApprove && !x.VActive).ToList();

                await RequestContainer.RefreshDataAsync();
                StateHasChanged();
            }

            isLoading = false;
        }

        private async ValueTask<ItemsProviderResult<RequestVM>> LoadRequestVMs(ItemsProviderRequest request)
        {
            return new ItemsProviderResult<RequestVM>(search_requestVMs.DistinctBy(x => new { x.RequestCode }).Skip(request.StartIndex).Take(request.Count), search_requestVMs.DistinctBy(x => new { x.RequestCode }).Count());
        }

        private async Task ShowEntity(ChangeEventArgs e)
        {
            isLoading = true;

            filterFinVM.ShowEntity = int.Parse(e.Value.ToString());
            requestVMs = search_requestVMs = await requestService.GetRequest(filterFinVM);

            await FilterRequest(filterFinVM.Status);

            isLoading = false;
        }

    }
}
