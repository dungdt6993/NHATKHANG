using D69soft.Client.Extensions;
using D69soft.Client.Services.FIN;
using D69soft.Client.Services.HR;
using D69soft.Client.Services;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace D69soft.Client.Pages.FIN
{
    partial class Customer
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] CustomerService customerService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        protected string UserID;

        LogVM logVM = new();

        //Filter
        FilterFinVM filterFinVM = new();
        FilterHrVM filterHrVM = new();

        //Division
        IEnumerable<DivisionVM> filter_divisionVMs;

        //Customer
        CustomerVM customerVM = new();
        List<CustomerVM> customerVMs;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
            await js.InvokeAsync<object>("tooltip");

            await js.InvokeAsync<object>("maskDate");
        }

        protected override async Task OnInitializedAsync()
        {
            filterHrVM.UserID = UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "FIN_Customer"))
            {
                logVM.LogUser = UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "FIN_Customer";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            filter_divisionVMs = await organizationalChartService.GetDivisionList(filterHrVM);
            filterFinVM.DivisionID = (await sysService.GetInfoUser(UserID)).DivisionID;

            await GetCustomers();

            isLoadingScreen = false;
        }

        private async Task GetCustomers()
        {
            isLoading = true;

            customerVM = new();

            customerVMs = (await customerService.GetCustomers()).ToList();

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

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                customerVM.IsTypeUpdate = 1;
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    await customerService.UpdateCustomer(customerVM);

                    logVM.LogDesc = "Xóa khách hàng " + customerVM.CustomerCode + "";
                    await sysService.InsertLog(logVM);

                    await GetCustomers();

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Customer");
                    await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                }
                else
                {
                    customerVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

    }
}
