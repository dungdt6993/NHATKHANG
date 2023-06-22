using D69soft.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace D69soft.Client.Shared
{
    partial class NavBar
    {
        [Inject] AuthenticationStateProvider authenticationStateProvider { get; set; }

        protected string UserID;

        //List<LogNotificationVM> notifications;

        //protected override async Task OnInitializedAsync()
        //{
        //    UserID = (await authenticationStateTask).User.GetUserId();

        //    notifications = await sysService.GetNotifications(UserID);
        //}

        private async Task Logout()
        {
            await ((CustomAuthenticationStateProvider)authenticationStateProvider).UpdateAuthenticationState(null);
        }

        //private async Task SeenNotificationsALL()
        //{
        //    await sysService.SeenNotificationsALL(UserID);
        //    notifications = await sysService.GetNotifications(UserID);
        //}
    }
}
