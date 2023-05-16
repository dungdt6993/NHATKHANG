using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using D69soft.Client.Helpers;
using System.Net.Http.Json;

namespace D69soft.Client.Shared
{
    partial class NavMenu
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] HttpClient httpClient { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        protected string UserID;

        UserVM userVM = new();

        IEnumerable<FuncVM> modules;

        IEnumerable<FuncVM> funcMenuGrps;

        IEnumerable<FuncVM> funcMenus;

        bool ckViewFuncMenuRpt = false;

        protected override async Task OnInitializedAsync()
        {
            UserID = (await authenticationStateTask).User.GetUserId();

            userVM = await httpClient.GetFromJsonAsync<UserVM>($"api/Sys/GetInfoUser/{UserID}");

            modules = await httpClient.GetFromJsonAsync<IEnumerable<FuncVM>>($"api/Sys/GetModuleMenu/{UserID}");

            funcMenuGrps = await httpClient.GetFromJsonAsync<IEnumerable<FuncVM>>($"api/Sys/GetFuncMenuGroup/{UserID}");

            funcMenus = await httpClient.GetFromJsonAsync<IEnumerable<FuncVM>>($"api/Sys/GetFuncMenu/{UserID}");

            ckViewFuncMenuRpt = await httpClient.GetFromJsonAsync<bool>($"api/Sys/CheckViewFuncMenuRpt/{UserID}");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await js.InvokeAsync<object>("menu_treeview");

            if (firstRender)
            {
                //if (!userRepo.CheckChangePassDefault(UserID))
                //{
                //    await js.InvokeAsync<object>("ShowModal", "#InitializeModal_ChangePass");
                //}
            }

        }

        private void ClickMenuFunc(string _urlFunc)
        {
            navigationManager.NavigateTo(_urlFunc, true);
        }
    }
}
