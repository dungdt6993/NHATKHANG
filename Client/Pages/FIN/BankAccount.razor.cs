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
using D69soft.Shared.Models.Entities.FIN;


namespace D69soft.Client.Pages.FIN
{
    partial class BankAccount
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] MoneyService moneyService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        //Bank
        List<BankVM> bankVMs;

        //BankAccount
        BankAccountVM bankAccountVM = new();
        List<BankAccountVM> bankAccountVMs;

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

            if (await sysService.CheckAccessFunc(filterVM.UserID, "FIN_BankAccount"))
            {
                logVM.LogUser = filterVM.UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "FIN_BankAccount";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            bankVMs = (await moneyService.GetBankList()).ToList();
            await GetBankAccounts();

            isLoadingScreen = false;
        }

        private async Task GetBankAccounts()
        {
            isLoading = true;

            bankAccountVM = new();

            bankAccountVMs = (await moneyService.GetBankAccountList()).ToList();

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

                logVM.LogDesc = (bankAccountVM.IsTypeUpdate == 0 ? "Thêm mới" : "Cập nhật") + " tài khoản ngân hàng " + bankAccountVM.BankAccount + " - "+ bankVMs.Where(x=>x.SwiftCode == bankAccountVM.SwiftCode).Select(x => x.BankShortName).First() +"";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                bankAccountVM.IsTypeUpdate = 1;
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    await moneyService.UpdateBankAccount(bankAccountVM);

                    logVM.LogDesc = "Xóa tài khoản ngân hàng " + bankAccountVM.BankAccount + " - " + bankVMs.Where(x => x.SwiftCode == bankAccountVM.SwiftCode).Select(x => x.BankShortName).First() + "";
                    await sysService.InsertLog(logVM);

                    await GetBankAccounts();

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_BankAccount");
                    await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                }
                else
                {
                    bankAccountVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

    }
}
