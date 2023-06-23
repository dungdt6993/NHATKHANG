using Blazored.LocalStorage;
using D69soft.Client.Components;
using D69soft.Client.Extensions;
using D69soft.Client.Services;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Globalization;

namespace D69soft.Client.Pages.Auth
{
    partial class Login
    {
        [Inject] ILocalStorageService localStorage { get; set; }
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [Inject] AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] SysService sysService { get; set; }
        [Inject] AuthService authService { get; set; }

        LogVM logVM = new();

        UserVM userVM = new();

        bool btnLoading = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
        }

        CultureInfo Culture
        {
            get => CultureInfo.CurrentCulture;
            set
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    localStorage.SetItemAsStringAsync("culture", value.Name);
                    navigationManager.NavigateTo(navigationManager.Uri, forceLoad: true);
                }
            }
        }

        public async Task HandleLogin()
        {
            btnLoading = true;

            LoginResponseVM loginResponseVM = await authService.Login(userVM);
            if (loginResponseVM.Successful)
            {
                logVM.LogType = "AUTH";
                logVM.LogName = "Login";
                logVM.LogUser = userVM.Eserial;
                await sysService.InsertLog(logVM);

                navigationManager.NavigateTo("/", true);
            }
            else
            {
                await js.Swal_Message("Cảnh báo!", loginResponseVM.Error, SweetAlertMessageType.error);
            }

            btnLoading = false;
        }
    }
}
