using D69soft.Client.Extensions;
using D69soft.Client.Services.FIN;
using D69soft.Client.Services.HR;
using D69soft.Client.Services;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using D69soft.Shared.Models.ViewModels.HR;


namespace D69soft.Client.Pages.FIN
{
    partial class Bank
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] MoneyService moneyService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        protected string UserID;

        LogVM logVM = new();

        //Bank
        BankVM bankVM = new();
        List<BankVM> bankVMs;

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

            if (await sysService.CheckAccessFunc(UserID, "FIN_Bank"))
            {
                logVM.LogUser = UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "FIN_Bank";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            await GetBanks();

            isLoadingScreen = false;
        }

        private async Task GetBanks()
        {
            isLoading = true;

            bankVM = new();

            bankVMs = (await moneyService.GetBankList()).ToList();

            isLoading = false;
        }

        private void onclick_Selected(BankVM _bankVM)
        {
            bankVM = _bankVM == bankVM ? new() : _bankVM;
        }

        private string SetSelected(BankVM _bankVM)
        {
            if (bankVM.SwiftCode != _bankVM.SwiftCode)
            {
                return string.Empty;
            }
            return "selected";
        }

        private async Task InitializeModalUpdate_Bank(int _IsTypeUpdate)
        {
            isLoading = true;

            bankVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Bank");

            isLoading = false;
        }

        private async Task UpdateBank(EditContext _formBankVM, int _IsTypeUpdate)
        {
            bankVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formBankVM.Validate()) return;
            isLoading = true;

            if (bankVM.IsTypeUpdate != 2)
            {
                await moneyService.UpdateBank(bankVM);

                logVM.LogDesc = (bankVM.IsTypeUpdate == 0 ? "Thêm mới" : "Cập nhật") + " ngân hàng " + bankVM.BankShortName + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                bankVM.IsTypeUpdate = 1;
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    await moneyService.UpdateBank(bankVM);

                    logVM.LogDesc = "Xóa ngân hàng " + bankVM.BankShortName + "";
                    await sysService.InsertLog(logVM);

                    await GetBanks();

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Bank");
                    await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                }
                else
                {
                    bankVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

    }
}
