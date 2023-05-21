﻿using D69soft.Client.Extension;
using D69soft.Client.Helpers;
using D69soft.Client.Services;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace D69soft.Client.Pages.Auth
{
    partial class Login
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [Inject] AuthenticationStateProvider authenticationStateProvider { get; set; }

        [Inject] AuthService authService { get; set; }
        [Inject] SysService sysService { get; set; }

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

        private async Task LoginRequest()
        {
            btnLoading = true;

            if (await authService.LoginRequest(userVM) > 0)
            {
                await ((CustomAuthenticationStateProvider)authenticationStateProvider).UpdateAuthenticationState(userVM.Eserial);

                logVM.LogType = "AUTH";
                logVM.LogName = "Login";
                logVM.LogUser = userVM.Eserial;
                await sysService.InsertLog(logVM);

                await js.InvokeAsync<object>("Cookie_User", "Cookie_UserID", userVM.Eserial, 100);

                navigationManager.NavigateTo("/", true);
            }
            else
            {
                await js.Swal_Message("Cảnh báo!", "Tài khoản hoặc mật khẩu không đúng.", SweetAlertMessageType.error);
            }

            btnLoading = false;
        }
    }
}
