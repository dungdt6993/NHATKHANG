using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using BlazorDateRangePicker;
using D69soft.Client.Services;
using D69soft.Client.Services.HR;
using D69soft.Client.Services.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;
using D69soft.Client.Services.OP;
using D69soft.Shared.Models.ViewModels.OP;
using D69soft.Shared.Models.ViewModels.FIN;

namespace D69soft.Client.Pages.OP
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

        bool isLoading;
        bool isLoadingScreen = true;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;

        //Request
        List<RequestVM> requestVMs = new();
        private Virtualize<RequestVM> virtualizeRequests { get; set; }

        //Voucher
        VoucherVM voucherVM = new();
        List<VoucherDetailVM> voucherDetailVMs;

        //PermisFunc
        bool permisFunc_FIN_Stock;

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
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            logVM.LogUser = filterVM.UserID;
            logVM.LogType = "FUNC";
            logVM.LogName = "EA_Request";
            await sysService.InsertLog(logVM);

            permisFunc_FIN_Stock = await sysService.CheckAccessFunc(filterVM.UserID, "FIN_Stock");

            division_filter_list = await organizationalChartService.GetDivisionList(filterVM);
            filterVM.DivisionID = (await sysService.GetInfoUser(filterVM.UserID)).DivisionID;

            filterVM.DepartmentID = filterVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterVM);

            filterVM.EndDate = new DateTime(DateTime.MaxValue.Year, DateTime.MaxValue.Month, DateTime.MaxValue.Day);

            if (permisFunc_FIN_Stock)
            {
                filterVM.isHandover = true;
            }

            filterVM.ShowEntity = 50;
            filterVM.RequestStatus = "all";

            await GetRequests();

            isLoadingScreen = false;
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterVM.DivisionID = filterVM.DivisionID = value;

            filterVM.DepartmentID = filterVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterVM);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_department(string value)
        {
            isLoading = true;

            filterVM.DepartmentID = filterVM.DepartmentID = value;

            await GetRequests();

            isLoading = false;

            StateHasChanged();
        }

        private async void OnRangeSelect(DateRange _range)
        {
            isLoading = true;

            filterVM.StartDate = _range.Start;
            filterVM.EndDate = _range.End;

            await GetRequests();

            isLoading = false;

            StateHasChanged();
        }

        //GetRequest
        protected async Task GetRequests()
        {
            isLoading = true;

            ReportName = "CustomNewReport";

            requestVMs = await requestService.GetRequests(filterVM);

            await virtualizeRequests.RefreshDataAsync();
            StateHasChanged();

            isLoading = false;
        }

        //Load Request
        protected async Task FilterRequest(String _filterRequest)
        {
            isLoading = true;

            filterVM.RequestStatus = _filterRequest;

            await GetRequests();

            isLoading = false;
        }

        private async ValueTask<ItemsProviderResult<RequestVM>> LoadRequestVMs(ItemsProviderRequest request)
        {
            return new ItemsProviderResult<RequestVM>(requestVMs.DistinctBy(x => new { x.RequestCode }).Skip(request.StartIndex).Take(request.Count), requestVMs.DistinctBy(x => new { x.RequestCode }).Count());
        }

        private async Task GetMoreRequests()
        {
            isLoading = true;

            filterVM.ShowEntity = filterVM.ShowEntity + 50;

            await GetRequests();

            isLoading = false;
        }

        private async Task SendApprove(bool value, string type, RequestVM _requestVM)
        {
            isLoading = true;

            var str = String.Empty;

            if (type == "SendDirectManager" || type == "SendControlDept")
            {
                str = value ? "Gửi duyệt" : "Hủy gửi duyệt";
            }

            if (type == "SendApprove")
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
                        if (requestVMs.Where(x => x.RequestCode == _requestVM.RequestCode && x.QtyApproved > 0).Count() > 0)
                        {
                            //Tu dong tao phieu xuat kho
                            voucherVM = new();
                            voucherDetailVMs = new();

                            voucherVM.UserID = filterVM.UserID;
                            voucherVM.DivisionID = _requestVM.DivisionID;

                            voucherVM.IsTypeUpdate = 0;
                            voucherVM.VTypeID = "FIN_Output";
                            voucherVM.ITypeCode = "HH";
                            voucherVM.VCode = "XK";
                            voucherVM.VDesc = $"Xuất kho theo yêu cầu số {_requestVM.RequestCode} - {_requestVM.ReasonOfRequest} ";
                            voucherVM.VDate = _requestVM.TimeSendApprove;

                            voucherVM.VReference = _requestVM.RequestCode;

                            voucherVM.VActive = false;

                            voucherDetailVMs = await requestService.GetRequestDetailVoucherDetail(_requestVM.RequestCode);

                            await voucherService.UpdateVoucher(voucherVM, voucherDetailVMs);
                        }
                    }
                }

                await requestService.SendApprove(_requestVM, type);

                logVM.LogDesc = str + " đơn số " + _requestVM.RequestCode;
                await sysService.InsertLog(logVM);

                await GetRequests();

                await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
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

    }
}
