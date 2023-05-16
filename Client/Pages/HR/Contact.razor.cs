using D69soft.Client.Helpers;
using D69soft.Shared.Models.ViewModels.HR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace D69soft.Client.Pages.HR
{
	partial class Contact
	{
		[CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] HttpClient httpClient { get; set; }

        bool isLoadingScreen = true;

        protected string UserID;

        List<ProfileManagamentVM> contacts;

        List<ProfileManagamentVM> search_contacts;

        FilterHrVM filterHrVM = new();

        protected override async Task OnInitializedAsync()
        {
            UserID = (await authenticationStateTask).User.GetUserId();

            search_contacts = contacts = await httpClient.GetFromJsonAsync<List<ProfileManagamentVM>>($"api/Profile/GetContacts/{UserID}");

            isLoadingScreen = false;
        }

        private string onchange_SearchValues
        {
            get { return filterHrVM.searchValues; }
            set
            {
                filterHrVM.searchValues = value;
                search_contacts = contacts.Where(x => x.FullName.ToUpper().Contains(filterHrVM.searchValues.ToUpper())).ToList();
            }
        }

    }
}
