using D69soft.Client.Helpers;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace D69soft.Client.Pages.Auth
{
    partial class Login
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] HttpClient httpClient { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [Inject] AuthenticationStateProvider authenticationStateProvider { get; set; }


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

            var response = await httpClient.PostAsJsonAsync($"api/Auth/LoginRequest", userVM);

            var result = await response.Content.ReadAsStringAsync();

            if (int.Parse(result) > 0)
            {
                await ((CustomAuthenticationStateProvider)authenticationStateProvider).UpdateAuthenticationState(userVM.Eserial);

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
