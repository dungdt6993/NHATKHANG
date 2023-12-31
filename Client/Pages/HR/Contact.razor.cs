﻿using D69soft.Client.Extensions;
using D69soft.Client.Services;
using D69soft.Server.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace D69soft.Client.Pages.HR
{
    partial class Contact
	{
		[CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] SysService sysService { get; set; }

        [Inject] ProfileService profileService { get; set; }

        bool isLoadingScreen = true;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        List<ProfileVM> contacts;
        List<ProfileVM> search_contacts;

        protected override async Task OnInitializedAsync()
        {
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            logVM.LogUser = filterVM.UserID;
            logVM.LogType = "FUNC";
            logVM.LogName = "HR_Contact";
            await sysService.InsertLog(logVM);

            search_contacts = contacts = await profileService.GetContacts(filterVM.UserID);

            isLoadingScreen = false;
        }

        private string onchange_SearchValues
        {
            get { return filterVM.searchValues; }
            set
            {
                filterVM.searchValues = value;
                search_contacts = contacts.Where(x => x.FullName.ToUpper().Contains(filterVM.searchValues.ToUpper())).ToList();
            }
        }

    }
}
