using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.Data;
using D69soft.Client.Services;
using D69soft.Server.Services.HR;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Helpers;
using D69soft.Shared.Utilities;

namespace D69soft.Client.Pages.HR
{
    public partial class Profile
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] AuthService authService { get; set; }
        [Inject] ProfileService profileService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] DutyRosterService dutyRosterService { get; set; }

        protected string UserID;

        bool isLoading;
        bool isLoadingScreen = true;

        bool disabled_btnUpdateProfile = true;

        private string hidden_adjustProfile = "hidden_adjustProfile";
        bool disable_ckContractExtension = false;
        bool disable_ckJob = false;
        bool disable_ckSal = false;

        bool disable_Eserial;
        string placeholder_Eserial;

        bool disabled_JoinDate; bool disabled_StartContractDate; bool disabled_ContractTypeID;
        bool disabled_EndContractDate; bool disabled_JobStartDate;
        bool disabled_BasicSalary; bool disabled_Benefit4; bool disabled_OtherSalary; bool disabled_Benefit5; bool disabled_Benefit1; bool disabled_Benefit6; bool disabled_Benefit2; bool disabled_Benefit7;
        bool disabled_Benefit3; bool disabled_Benefit8; bool disabled_Reason; bool disabled_ApprovedBy; bool disabled_BeginSalaryDate; bool disabled_SalaryByBank; bool disabled_IsPayBy;

        LogVM logVM = new();

        FilterHrVM filterHrVM = new();
        DivisionVM divisionSelected = new();
        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;
        IEnumerable<SectionVM> section_filter_list;
        IEnumerable<PositionVM> position_filter_list;
        IEnumerable<ProfileVM> eserial_filter_list;

        ProfileManagamentVM profileManagament = new();
        List<ProfileManagamentVM> profileVMs = new();

        IEnumerable<DepartmentVM> departments;
        IEnumerable<SectionVM> sections;
        IEnumerable<PositionVM> positions;
        IEnumerable<CountryVM> countrys;
        IEnumerable<EthnicVM> ethnics;
        IEnumerable<WorkTypeVM> workTypes;
        IEnumerable<ShiftVM> shifts;
        IEnumerable<ContractTypeVM> laborContractTypes;
        IEnumerable<PermissionUserVM> permissionUsers;
        List<ProfileManagamentVM> profileHistorys;
        List<SalaryDefVM> salaryDefs;

        //KPI
        IEnumerable<ProfileManagamentVM> empls;

        //Báo cáo biến động nhân sự
        DataTable dtEmplChange;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
            await js.InvokeAsync<object>("maskCurrency");
            await js.InvokeAsync<object>("maskPeriod");
            await js.InvokeAsync<object>("maskDate");
            await js.InvokeAsync<object>("datepicker");
        }

        protected override async Task OnInitializedAsync()
        {


            UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "HR_Profile"))
            {
                logVM.LogType = "FUNC";
                logVM.LogName = "HR_Profile";
                logVM.LogUser = UserID;
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            filterHrVM.UserID = UserID;

            division_filter_list = await organizationalChartService.GetDivisionList(filterHrVM);
            filterHrVM.DivisionID = division_filter_list.Count() > 0 ? division_filter_list.ElementAt(0).DivisionID : string.Empty;

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterHrVM);

            filterHrVM.SectionID = string.Empty;
            section_filter_list = await organizationalChartService.GetSectionList();

            filterHrVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartService.GetPositionList();

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await profileService.GetEserialListByID(filterHrVM);

            //Báo cáo biên động nhân sự
            filterHrVM.Year = DateTime.Now.Year;
            dtEmplChange = await profileService.GetEmplChangeList(filterHrVM);

            isLoadingScreen = false;
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterHrVM.DivisionID = value;

            filterHrVM.TypeProfile = 0;

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await profileService.GetEserialListByID(filterHrVM);

            filterHrVM.selectedEserial = string.Empty;
            profileVMs = null;

            dtEmplChange = await profileService.GetEmplChangeList(filterHrVM);

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_department(string value)
        {
            isLoading = true;

            filterHrVM.DepartmentID = value;

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await profileService.GetEserialListByID(filterHrVM);

            filterHrVM.selectedEserial = string.Empty;
            profileVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_section(string value)
        {
            isLoading = true;

            filterHrVM.SectionID = value;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await profileService.GetEserialListByID(filterHrVM);

            filterHrVM.selectedEserial = string.Empty;
            profileVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private string[] onchange_filter_positiongroup
        {
            get
            {
                return filterHrVM.arrPositionID;
            }
            set
            {
                isLoading = true;

                filterHrVM.arrPositionID = (string[])value;

                filterHrVM.PositionGroupID = string.Join(",", (string[])value);

                reload_filter_eserial();

                profileVMs = null;

                isLoading = false;
            }
        }

        private async Task onchange_filter_typeprofile(ChangeEventArgs args)
        {
            isLoading = true;

            filterHrVM.TypeProfile = int.Parse(args.Value.ToString());

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await profileService.GetEserialListByID(filterHrVM);

            filterHrVM.selectedEserial = string.Empty;
            profileVMs = null;

            isLoading = false;
        }

        private void onchange_filter_eserial(string value)
        {
            isLoading = true;

            filterHrVM.Eserial = value;

            filterHrVM.selectedEserial = string.Empty;
            profileVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void reload_filter_eserial()
        {
            filterHrVM.selectedEserial = string.Empty;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await profileService.GetEserialListByID(filterHrVM);

            StateHasChanged();
        }

        protected async Task GetProfileList()
        {
            isLoading = true;

            profileVMs = await profileService.GetProfileList(filterHrVM, UserID);

            dtEmplChange = await profileService.GetEmplChangeList(filterHrVM);

            await virtualizeProfileList.RefreshDataAsync();
            StateHasChanged();

            isLoading = false;
        }

        private Virtualize<ProfileManagamentVM> virtualizeProfileList { get; set; }

        private ValueTask<ItemsProviderResult<ProfileManagamentVM>> LoadProfileList(ItemsProviderRequest request)
        {
            return new(new ItemsProviderResult<ProfileManagamentVM>(
                profileVMs.Skip(request.StartIndex).Take(request.Count),
                profileVMs.Count));
        }

        private void onclick_selectedEserial(ProfileManagamentVM _profileVM)
        {
            filterHrVM.selectedEserial = filterHrVM.selectedEserial == _profileVM.Eserial ? String.Empty : _profileVM.Eserial;
        }

        private void onchange_department(string value)
        {
            isLoading = true;

            profileManagament.DepartmentID = value;

            isLoading = false;

            StateHasChanged();
        }

        public string onchange_Birthday
        {
            get
            {
                return profileManagament.Birthday.HasValue ? profileManagament.Birthday.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileManagament.Birthday = LibraryFunc.FormatDateDDMMYYYY(value, profileManagament.Birthday);
            }
        }

        public string onchange_DateOfIssue
        {
            get
            {
                return profileManagament.DateOfIssue.HasValue ? profileManagament.DateOfIssue.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileManagament.DateOfIssue = LibraryFunc.FormatDateDDMMYYYY(value, profileManagament.DateOfIssue);
            }
        }

        public string onchange_VisaExpDate
        {
            get
            {
                return profileManagament.VisaExpDate.HasValue ? profileManagament.VisaExpDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileManagament.VisaExpDate = LibraryFunc.FormatDateDDMMYYYY(value, profileManagament.VisaExpDate);
            }
        }

        public string onchange_JoinDate
        {
            get
            {
                return profileManagament.JoinDate.HasValue ? profileManagament.JoinDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileManagament.JoinDate = LibraryFunc.FormatDateDDMMYYYY(value, profileManagament.JoinDate);
            }
        }

        public string onchange_StartDayAL
        {
            get
            {
                return profileManagament.StartDayAL.HasValue ? profileManagament.StartDayAL.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileManagament.StartDayAL = LibraryFunc.FormatDateDDMMYYYY(value, profileManagament.StartDayAL);
            }
        }

        public string onchange_StartContractDate
        {
            get
            {
                return profileManagament.StartContractDate.HasValue ? profileManagament.StartContractDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                isLoading = true;

                profileManagament.StartContractDate = LibraryFunc.FormatDateDDMMYYYY(value, profileManagament.StartContractDate);

                if (profileManagament.StartContractDate == null)
                {
                    disabled_ContractTypeID = true;
                    disabled_EndContractDate = true;
                }
                else
                {
                    disabled_ContractTypeID = false;
                }
                profileManagament.ContractTypeID = string.Empty;
                profileManagament.EndContractDate = null;

                isLoading = false;
            }
        }

        private async Task onchange_contracttype(string value)
        {
            isLoading = true;

            profileManagament.ContractTypeID = value;

            int NumMonth = await profileService.GetNumMonthLC(profileManagament.ContractTypeID);
            if (NumMonth != 0)
            {
                profileManagament.EndContractDate = Convert.ToDateTime(profileManagament.StartContractDate).AddMonths(NumMonth).AddDays(-1);
                disabled_EndContractDate = true;
            }
            else
            {
                disabled_EndContractDate = false;
                profileManagament.EndContractDate = null;
            }

            isLoading = false;

        }

        public string onchange_EndContractDate
        {
            get
            {
                return profileManagament.EndContractDate.HasValue ? profileManagament.EndContractDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileManagament.EndContractDate = LibraryFunc.FormatDateDDMMYYYY(value, profileManagament.EndContractDate);
            }
        }

        public string onchange_JobStartDate
        {
            get
            {
                return profileManagament.JobStartDate.HasValue ? profileManagament.JobStartDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileManagament.JobStartDate = LibraryFunc.FormatDateDDMMYYYY(value, profileManagament.JobStartDate);
            }
        }

        public string onchange_TerminateDate
        {
            get
            {
                return profileManagament.TerminateDate.HasValue ? profileManagament.TerminateDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileManagament.TerminateDate = LibraryFunc.FormatDateDDMMYYYY(value, profileManagament.TerminateDate);
            }
        }

        public string onchange_BeginSalaryDate
        {
            get
            {
                return profileManagament.BeginSalaryDate.HasValue ? profileManagament.BeginSalaryDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileManagament.BeginSalaryDate = LibraryFunc.FormatDateDDMMYYYY(value, profileManagament.BeginSalaryDate);
            }
        }

        public decimal onchange_basicsalary
        {
            get { return profileManagament.BasicSalary; }
            set
            {
                isLoading = true;

                profileManagament.BasicSalary = value;

                profileManagament.TotalSalary = profileManagament.BasicSalary + profileManagament.OtherSalary + profileManagament.Benefit1 + profileManagament.Benefit2 + profileManagament.Benefit3 + profileManagament.Benefit4
                                                + profileManagament.Benefit5 + profileManagament.Benefit6 + profileManagament.Benefit7 + profileManagament.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_othersalary
        {
            get { return profileManagament.OtherSalary; }
            set
            {
                isLoading = true;

                profileManagament.OtherSalary = value;

                profileManagament.TotalSalary = profileManagament.BasicSalary + profileManagament.OtherSalary + profileManagament.Benefit1 + profileManagament.Benefit2 + profileManagament.Benefit3 + profileManagament.Benefit4
                                                + profileManagament.Benefit5 + profileManagament.Benefit6 + profileManagament.Benefit7 + profileManagament.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_benefit1
        {
            get { return profileManagament.Benefit1; }
            set
            {
                isLoading = true;

                profileManagament.Benefit1 = value;

                profileManagament.TotalSalary = profileManagament.BasicSalary + profileManagament.OtherSalary + profileManagament.Benefit1 + profileManagament.Benefit2 + profileManagament.Benefit3 + profileManagament.Benefit4
                                                + profileManagament.Benefit5 + profileManagament.Benefit6 + profileManagament.Benefit7 + profileManagament.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_benefit2
        {
            get { return profileManagament.Benefit2; }
            set
            {
                isLoading = true;

                profileManagament.Benefit2 = value;

                profileManagament.TotalSalary = profileManagament.BasicSalary + profileManagament.OtherSalary + profileManagament.Benefit1 + profileManagament.Benefit2 + profileManagament.Benefit3 + profileManagament.Benefit4
                                                + profileManagament.Benefit5 + profileManagament.Benefit6 + profileManagament.Benefit7 + profileManagament.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_benefit3
        {
            get { return profileManagament.Benefit3; }
            set
            {
                isLoading = true;

                profileManagament.Benefit3 = value;

                profileManagament.TotalSalary = profileManagament.BasicSalary + profileManagament.OtherSalary + profileManagament.Benefit1 + profileManagament.Benefit2 + profileManagament.Benefit3 + profileManagament.Benefit4
                                                + profileManagament.Benefit5 + profileManagament.Benefit6 + profileManagament.Benefit7 + profileManagament.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_benefit4
        {
            get { return profileManagament.Benefit4; }
            set
            {
                isLoading = true;

                profileManagament.Benefit4 = value;

                profileManagament.TotalSalary = profileManagament.BasicSalary + profileManagament.OtherSalary + profileManagament.Benefit1 + profileManagament.Benefit2 + profileManagament.Benefit3 + profileManagament.Benefit4
                                                + profileManagament.Benefit5 + profileManagament.Benefit6 + profileManagament.Benefit7 + profileManagament.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_benefit5
        {
            get { return profileManagament.Benefit5; }
            set
            {
                isLoading = true;

                profileManagament.Benefit5 = value;

                profileManagament.TotalSalary = profileManagament.BasicSalary + profileManagament.OtherSalary + profileManagament.Benefit1 + profileManagament.Benefit2 + profileManagament.Benefit3 + profileManagament.Benefit4
                                                + profileManagament.Benefit5 + profileManagament.Benefit6 + profileManagament.Benefit7 + profileManagament.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_benefit6
        {
            get { return profileManagament.Benefit6; }
            set
            {
                isLoading = true;

                profileManagament.Benefit6 = value;

                profileManagament.TotalSalary = profileManagament.BasicSalary + profileManagament.OtherSalary + profileManagament.Benefit1 + profileManagament.Benefit2 + profileManagament.Benefit3 + profileManagament.Benefit4
                                                + profileManagament.Benefit5 + profileManagament.Benefit6 + profileManagament.Benefit7 + profileManagament.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_benefit7
        {
            get { return profileManagament.Benefit7; }
            set
            {
                isLoading = true;

                profileManagament.Benefit7 = value;

                profileManagament.TotalSalary = profileManagament.BasicSalary + profileManagament.OtherSalary + profileManagament.Benefit1 + profileManagament.Benefit2 + profileManagament.Benefit3 + profileManagament.Benefit4
                                                + profileManagament.Benefit5 + profileManagament.Benefit6 + profileManagament.Benefit7 + profileManagament.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_benefit8
        {
            get { return profileManagament.Benefit8; }
            set
            {
                isLoading = true;

                profileManagament.Benefit8 = value;

                profileManagament.TotalSalary = profileManagament.BasicSalary + profileManagament.OtherSalary + profileManagament.Benefit1 + profileManagament.Benefit2 + profileManagament.Benefit3 + profileManagament.Benefit4
                                                + profileManagament.Benefit5 + profileManagament.Benefit6 + profileManagament.Benefit7 + profileManagament.Benefit8;

                isLoading = false;
            }
        }

        private void onchange_salarybybank(ChangeEventArgs args)
        {
            isLoading = true;

            profileManagament.SalaryByBank = int.Parse(args.Value.ToString());

            isLoading = false;
        }

        private void onchange_ispayby(ChangeEventArgs args)
        {
            isLoading = true;

            int ck = int.Parse(args.Value.ToString());

            if (ck == 1)
            {
                profileManagament.IsPayByMonth = 1;
                profileManagament.IsPayByDate = 0;
            }

            if (ck == 2)
            {
                profileManagament.IsPayByMonth = 0;
                profileManagament.IsPayByDate = 1;
            }

            isLoading = false;
        }

        private void onchange_ckContractExtension(ChangeEventArgs args)
        {
            isLoading = true;

            profileManagament.ckContractExtension = int.Parse(args.Value.ToString());

            disable_ckContractExtension = true;

            if (profileManagament.ckContractExtension == 1)
            {
                disabled_ContractTypeID = false;
                disabled_EndContractDate = false;

                profileManagament.StartContractDate = Convert.ToDateTime(profileManagament.EndContractDate).AddDays(1);

                profileManagament.ContractTypeID = string.Empty;
                profileManagament.EndContractDate = null;
            }

            if (profileManagament.ckContractExtension == 2)
            {
                disabled_StartContractDate = false;

                profileManagament.StartContractDate = null;
                profileManagament.ContractTypeID = string.Empty;
                profileManagament.EndContractDate = null;
            }

            disabled_btnUpdateProfile = false;

            isLoading = false;
        }

        private void onchange_ckJob(ChangeEventArgs args)
        {
            isLoading = true;

            profileManagament.ckJob = int.Parse(args.Value.ToString());

            disable_ckContractExtension = true;
            disable_ckJob = true;

            if (profileManagament.ckContractExtension != 0)
            {
                profileManagament.JobStartDate = profileManagament.StartContractDate;
                disabled_JobStartDate = true;
            }
            else
            {
                profileManagament.JobStartDate = null;
                disabled_JobStartDate = false;
            }

            disabled_btnUpdateProfile = false;

            isLoading = false;
        }

        private void onchange_ckSal(ChangeEventArgs args)
        {
            isLoading = true;

            profileManagament.ckSal = int.Parse(args.Value.ToString());

            disable_ckContractExtension = true;
            disable_ckJob = true;
            disable_ckSal = true;

            disabled_BasicSalary = false;
            disabled_Benefit4 = false;
            disabled_OtherSalary = false;
            disabled_Benefit5 = false;
            disabled_Benefit1 = false;
            disabled_Benefit6 = false;
            disabled_Benefit2 = false;
            disabled_Benefit7 = false;
            disabled_Benefit3 = false;
            disabled_Benefit8 = false;
            disabled_Reason = false;
            disabled_ApprovedBy = false;
            disabled_BeginSalaryDate = false;
            disabled_SalaryByBank = false;
            disabled_IsPayBy = false;

            profileManagament.Reason = string.Empty;
            profileManagament.ApprovedBy = string.Empty;

            if (profileManagament.ckContractExtension != 0)
            {
                profileManagament.BeginSalaryDate = profileManagament.StartContractDate;
                disabled_BeginSalaryDate = true;
            }

            if (profileManagament.ckContractExtension == 0 && profileManagament.ckJob != 0)
            {
                profileManagament.BeginSalaryDate = profileManagament.JobStartDate;
                disabled_BeginSalaryDate = true;
            }

            disabled_btnUpdateProfile = false;

            isLoading = false;
        }

        private async Task Initialize_ModalProfile(int typeView)
        {
            isLoading = true;

            disabled_btnUpdateProfile = false;

            disable_Eserial = true;
            placeholder_Eserial = "Mã NV";

            divisionSelected = division_filter_list.First(x => x.DivisionID == filterHrVM.DivisionID);

            countrys = await profileService.GetCountryList();
            ethnics = await profileService.GetEthnicList();

            sections = await organizationalChartService.GetSectionList();
            departments = await organizationalChartService.GetDepartmentList(filterHrVM);
            positions = await organizationalChartService.GetPositionList();

            laborContractTypes = await profileService.GetContractTypeList();
            workTypes = await profileService.GetWorkTypeList();
            shifts = await dutyRosterService.GetShiftList();
            shifts = shifts.Where(x => x.isOFF == 0);

            salaryDefs = await profileService.GetSalaryDef();

            permissionUsers = await authService.GetPermissionUser(filterHrVM.selectedEserial, UserID);

            if (typeView == 0 || typeView == 4)
            {
                disabled_ContractTypeID = true;
                disabled_EndContractDate = true;

                profileManagament = new();

                profileHistorys = null;


                profileManagament.UrlAvatar = UrlDirectory.Default_Avatar;

                profileManagament.CountryCode = "VN";

                profileManagament.EthnicID = 1;

                profileManagament.DivisionID = filterHrVM.DivisionID;

                profileManagament.PermisId = 4;

                profileManagament.IsPayByMonth = 1;

                if (typeView == 4)
                {
                    profileManagament = await profileService.GetProfileByEserial(filterHrVM);
                    profileManagament.Eserial = null;
                    profileManagament.JoinDate = null;
                    profileManagament.StartDayAL = null;
                    profileManagament.StartContractDate = null;
                    profileManagament.ContractTypeID = String.Empty;
                    profileManagament.EndContractDate = null;
                    profileManagament.JobStartDate = null;
                    profileManagament.User_isChangePass = 0;
                    profileManagament.BeginSalaryDate = null;
                    profileManagament.Reason = String.Empty;
                    profileManagament.ApprovedBy = String.Empty;
                }

                profileManagament.isTypeSave = typeView == 4 ? 0 : 0;

                profileManagament.isAutoEserial = divisionSelected.isAutoEserial;

                if (!profileManagament.isAutoEserial)
                {
                    disable_Eserial = false;
                }
                else
                {
                    placeholder_Eserial = "Mã NV tự động";
                }
            }
            else
            {
                profileManagament = await profileService.GetProfileByEserial(filterHrVM);

                profileManagament.IsUpdateUrlAvatar = false;

                profileHistorys = await profileService.GetProfileHistory(profileManagament.Eserial);

                profileManagament.isTypeSave = typeView;

                if (!await profileService.CkUpdateJobHistory(profileManagament.Eserial))
                {
                    disabled_StartContractDate = true;
                    disabled_ContractTypeID = true;
                    disabled_EndContractDate = true;
                    disabled_JobStartDate = true;
                }

                if (!await profileService.CkUpdateSalHistory(profileManagament.Eserial))
                {
                    disabled_BasicSalary = true;
                    disabled_Benefit4 = true;
                    disabled_OtherSalary = true;
                    disabled_Benefit5 = true;
                    disabled_Benefit1 = true;
                    disabled_Benefit6 = true;
                    disabled_Benefit2 = true;
                    disabled_Benefit7 = true;
                    disabled_Benefit3 = true;
                    disabled_Benefit8 = true;
                    disabled_Reason = true;
                    disabled_ApprovedBy = true;
                    disabled_BeginSalaryDate = true;
                    disabled_SalaryByBank = true;
                    disabled_IsPayBy = true;
                }

                disabled_JoinDate = true;

                if (typeView == 2)
                {
                    hidden_adjustProfile = string.Empty;
                    disabled_btnUpdateProfile = true;

                    disabled_StartContractDate = true;
                    disabled_ContractTypeID = true;
                    disabled_EndContractDate = true;
                    disabled_JobStartDate = true;

                    disabled_BasicSalary = true;
                    disabled_Benefit4 = true;
                    disabled_OtherSalary = true;
                    disabled_Benefit5 = true;
                    disabled_Benefit1 = true;
                    disabled_Benefit6 = true;
                    disabled_Benefit2 = true;
                    disabled_Benefit7 = true;
                    disabled_Benefit3 = true;
                    disabled_Benefit8 = true;
                    disabled_Reason = true;
                    disabled_ApprovedBy = true;
                    disabled_BeginSalaryDate = true;
                    disabled_SalaryByBank = true;
                    disabled_IsPayBy = true;
                }
            }

            isLoading = false;
        }

        private void Close_ModalProfile()
        {
            hidden_adjustProfile = "hidden_adjustProfile";

            disable_ckContractExtension = false;
            disable_ckJob = false;
            disable_ckSal = false;

            disabled_JoinDate = false;
            disabled_StartContractDate = false;
            disabled_ContractTypeID = false;
            disabled_EndContractDate = false;
            disabled_JobStartDate = false;
            disabled_BasicSalary = false;
            disabled_Benefit4 = false;
            disabled_OtherSalary = false;
            disabled_Benefit5 = false;
            disabled_Benefit1 = false;
            disabled_Benefit6 = false;
            disabled_Benefit2 = false;
            disabled_Benefit7 = false;
            disabled_Benefit3 = false;
            disabled_Benefit8 = false;
            disabled_Reason = false;
            disabled_ApprovedBy = false;
            disabled_BeginSalaryDate = false;
            disabled_SalaryByBank = false;
            disabled_IsPayBy = false;

            disabled_btnUpdateProfile = false;
        }

        MemoryStream memoryStream;
        Stream stream;
        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            isLoading = true;

            var format = "image/png";
            long maxFileSize = 1024 * 1024 * 15;

            var resizedFile = await e.File.RequestImageFileAsync(format, 640, 480); // resize the image file
            stream = resizedFile.OpenReadStream(maxFileSize);

            memoryStream = new();
            await stream.CopyToAsync(memoryStream);

            profileManagament.UrlAvatar = $"data:{format};base64,{Convert.ToBase64String(memoryStream.ToArray())}";// convert to a base64 string!!

            profileManagament.IsUpdateUrlAvatar = true;

            isLoading = false;
        }

        private void FileDefault()
        {
            profileManagament.UrlAvatar = UrlDirectory.Default_Avatar;
            memoryStream = null;
            profileManagament.IsUpdateUrlAvatar = true;
        }

        private async Task SaveProfile()
        {
            isLoading = true;

            disabled_btnUpdateProfile = true;

            profileManagament.UserID = UserID;

            profileManagament.Eserial = await profileService.UpdateProfile(profileManagament);

            if (profileManagament.IsUpdateUrlAvatar)
            {
                LibraryFunc.DelFileFrom(Path.Combine(Directory.GetCurrentDirectory(), $"{UrlDirectory.Upload_HR_Images_Profile_Private}{profileManagament.Eserial}.png"));

                if (memoryStream != null)
                {
                    var path = $"{UrlDirectory.Upload_HR_Images_Profile_Private}{profileManagament.Eserial}.png";

                    File.WriteAllBytes(path, memoryStream.ToArray());

                    profileManagament.UrlAvatar = $"{UrlDirectory.Upload_HR_Images_Profile_Public}{profileManagament.Eserial}.png";
                }

                await profileService.UpdateUrlAvatar(profileManagament.Eserial, profileManagament.UrlAvatar);
            }

            if (profileManagament.isTypeSave == 1)
            {
                await js.Swal_Message("Thông báo!", "Cập nhật dữ liệu thành công.", SweetAlertMessageType.success);

                disabled_btnUpdateProfile = false;

           }

            if (profileManagament.isTypeSave == 2)
            {
                await js.Swal_Message("Thông báo!", "Cập nhật dữ liệu thành công.", SweetAlertMessageType.success);

                disabled_btnUpdateProfile = true;
                disable_ckContractExtension = true;
                disable_ckJob = true;
                disable_ckSal = true;
            }

            if (profileManagament.isTypeSave == 0)
            {
                await js.Swal_Message("Thông báo!", "Thêm mới dữ liệu thành công.", SweetAlertMessageType.success);

                profileManagament.isTypeSave = 1;
                disabled_btnUpdateProfile = false;
            }

            profileHistorys = await profileService.GetProfileHistory(profileManagament.Eserial);

            await GetProfileList();
            eserial_filter_list = await profileService.GetEserialListByID(filterHrVM);

            isLoading = false;
        }

        private async Task ResetPass()
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Đặt lại mật khẩu đăng nhập hệ thống?", SweetAlertMessageType.question))
            {
                await profileService.ResetPass(profileManagament);
                await js.Swal_Message("Thông báo!", "Đặt lại mật khẩu đăng nhập thành công.", SweetAlertMessageType.success);
            }
        }

        private async Task DelProfileHistory(ProfileManagamentVM profilehistory)
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
            {
                if (await profileService.DelProfileHistory(profilehistory))
                {
                    await js.Swal_Message("Thông báo!", "Xóa thành công.", SweetAlertMessageType.success);

                    Close_ModalProfile();
                    await Initialize_ModalProfile(1);

                    await GetProfileList();
                }

            }
        }

        private async Task DelProfile()
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa nhân viên mã " + filterHrVM.selectedEserial + "?", SweetAlertMessageType.question))
            {
                if (await profileService.DelProfile(filterHrVM.selectedEserial))
                {
                    await js.Swal_Message("Thông báo!", "Xóa thành công.", SweetAlertMessageType.success);

                    await GetProfileList();

                    reload_filter_eserial();
                }
            }

        }

        private async Task InitializeModal_TerminateProfile()
        {
            isLoading = true;

            disabled_btnUpdateProfile = false;

            profileManagament = new ProfileManagamentVM();

            profileManagament = await profileService.GetProfileByEserial(filterHrVM);

            profileManagament.isTypeSave = 4;

            profileManagament.UserID = UserID;

            isLoading = false;
        }

        private async Task TerminateProfile()
        {
            if (await profileService.TerminateProfile(profileManagament))
            {
                await js.Swal_Message("Thông báo!", "Chấm dứt hợp đồng thành công.", SweetAlertMessageType.success);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModal_TerminateProfile");

                await GetProfileList();

                reload_filter_eserial();
            }
            disabled_btnUpdateProfile = true;
        }

        private async Task RestoreTerminateProfile()
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắc chắn hủy chấm dứt hợp đồng?", SweetAlertMessageType.question))
            {
                if (await profileService.RestoreTerminateProfile(filterHrVM.selectedEserial, UserID))
                {
                    await js.Swal_Message("Thông báo!", "Hủy chấm dứt hợp đồng thành công.", SweetAlertMessageType.success);
                    await js.InvokeAsync<object>("CloseModal", "#InitializeModal_TerminateProfile");

                    await GetProfileList();

                    reload_filter_eserial();
                }

            }

        }

        //Permis
        IEnumerable<FuncVM> permis_funcGroups;
        IEnumerable<FuncVM> permis_funcs;
        IEnumerable<FuncVM> permis_subFuncs;

        IEnumerable<DepartmentVM> permis_divs;
        IEnumerable<DepartmentVM> permis_depts;

        IEnumerable<SysRptVM> permis_rptgrps;
        IEnumerable<SysRptVM> permis_rpts;

        private async Task InitializeModal_Permis()
        {
            isLoading = true;

            permis_funcGroups = await profileService.GetFuncGroupPermis(profileManagament.Eserial);
            permis_funcs = await profileService.GetFuncPermis(profileManagament.Eserial);
            permis_subFuncs = await profileService.GetSubFuncPermis(profileManagament.Eserial);

            permis_divs = await profileService.GetDivisionPermis(profileManagament.Eserial);
            permis_depts = await profileService.GetDepartmentPermis(profileManagament.Eserial);

            permis_rptgrps = await profileService.GetSysReportGroupPermis(profileManagament.Eserial);
            permis_rpts = await profileService.GetSysReportPermis(profileManagament.Eserial);

            isLoading = false;
        }

        private void CheckAll_GrpFunc(object checkValue, string funcGrpID)
        {
            bool isChecked = (bool)checkValue;

            permis_funcs.Where(x => x.FuncGrpID == funcGrpID).ToList().ForEach(e => e.IsChecked = isChecked);
            permis_subFuncs.Where(x => x.FuncGrpID == funcGrpID).ToList().ForEach(e => e.IsChecked = isChecked);
        }

        private void CheckAll_Func(object checkValue, FuncVM funcVM, FuncVM funcGrpVM)
        {
            bool isChecked = (bool)checkValue;

            funcVM.IsChecked = isChecked;

            permis_subFuncs.Where(x => x.FuncID == funcVM.FuncID).ToList().ForEach(e => e.IsChecked = isChecked);

            funcGrpVM.IsChecked = permis_funcs.Where(x => x.FuncGrpID == funcGrpVM.FuncGrpID && x.IsChecked).Count() > 0 ? true : false;
        }

        private void Check_SubFunc(object checkValue, FuncVM subFuncVM, FuncVM funcVM, FuncVM funcGrpVM)
        {
            bool isChecked = (bool)checkValue;

            subFuncVM.IsChecked = isChecked;

            funcVM.IsChecked = permis_subFuncs.Where(x => x.FuncID == funcVM.FuncID && x.IsChecked).Count() > 0 ? true : false;

            funcGrpVM.IsChecked = permis_funcs.Where(x => x.FuncGrpID == funcGrpVM.FuncGrpID && x.IsChecked).Count() > 0 ? true : false;
        }

        private void CheckAll_Division(object checkValue, string divisionID)
        {
            bool isChecked = (bool)checkValue;

            permis_depts.Where(x => x.DivisionID == divisionID).ToList().ForEach(e => e.IsChecked = isChecked);
        }

        private void Check_Department(object checkValue, DepartmentVM department, DepartmentVM division)
        {
            bool isChecked = (bool)checkValue;

            department.IsChecked = isChecked;

            division.IsChecked = permis_depts.Where(x => x.DivisionID == division.DivisionID && x.IsChecked).Count() > 0 ? true : false;

        }

        private void CheckAll_RptGrp(object checkValue, int rptgrpID)
        {
            bool isChecked = (bool)checkValue;

            permis_rpts.Where(x => x.RptGrpID == rptgrpID).ToList().ForEach(e => e.IsChecked = isChecked);
        }

        private void Check_Rpt(object checkValue, SysRptVM rpt, SysRptVM rptVM)
        {
            bool isChecked = (bool)checkValue;

            rpt.IsChecked = isChecked;

            rptVM.IsChecked = permis_rpts.Where(x => x.RptGrpID == rptVM.RptGrpID && x.IsChecked).Count() > 0 ? true : false;

        }

        private async Task UpdatePermis()
        {
            await profileService.UpdatePermis(permis_funcs.Where(x => x.IsChecked), permis_subFuncs.Where(x => x.IsChecked), permis_depts.Where(x => x.IsChecked), permis_rpts.Where(x => x.IsChecked), filterHrVM.selectedEserial);

            await js.Swal_Message("Thông báo!", "Cập nhật phân quyền thành công.", SweetAlertMessageType.success);
        }

        //EmployeeTransaction
        IEnumerable<EmployeeTransactionVM> salTrnGrps;
        IEnumerable<EmployeeTransactionVM> salTrnCodes;
        private async Task InitializeModal_EmployeeTransaction()
        {
            isLoading = true;

            salTrnGrps = await profileService.GetSalTrnGrp(profileManagament.Eserial);
            salTrnCodes = await profileService.GetSalTrnCode(profileManagament.Eserial);

            isLoading = false;
        }

        private void CheckAll_EmplTrn(object _checkValue, int _trnGroupCode)
        {
            bool isChecked = (bool)_checkValue;

            salTrnCodes.Where(x => x.TrnGroupCode == _trnGroupCode).ToList().ForEach(e => e.IsChecked = isChecked);
        }

        private void Check_EmplTrn(object checkValue, EmployeeTransactionVM _trnGroup, EmployeeTransactionVM _trn)
        {
            bool isChecked = (bool)checkValue;

            _trn.IsChecked = isChecked;

            _trnGroup.IsChecked = salTrnCodes.Where(x => x.TrnGroupCode == _trnGroup.TrnGroupCode && x.IsChecked).Count() > 0 ? true : false;

        }

        private async Task UpdateEmplTrn()
        {
            await profileService.UpdateEmplTrn(salTrnCodes.Where(x => x.IsChecked), filterHrVM.selectedEserial);

            await js.Swal_Message("Thông báo!", "Cập nhật thiết lập giao dịch lương thành công.", SweetAlertMessageType.success);
        }

        //ContractType
        IEnumerable<ContractTypeVM> contractTypeGroupVMs;
        IEnumerable<ContractTypeVM> contractTypeVMs;
        ContractTypeVM contractTypeVM = new();
        private async Task InitializeModal_ContractType()
        {
            isLoading = true;

            contractTypeVMs = await profileService.GetContractTypeList();

            isLoading = false;
        }

        private async Task InitializeModalUpdate_ContractType(int _isTypeUpdate, ContractTypeVM _contractTypeVM)
        {
            isLoading = true;

            contractTypeVM = new();

            contractTypeGroupVMs = await profileService.GetContractTypeGroupList();

            if (_isTypeUpdate == 1)
            {
                contractTypeVM = _contractTypeVM;
            }

            contractTypeVM.IsTypeUpdate = _isTypeUpdate;

            isLoading = false;
        }

        private async Task UpdateContractType()
        {
            isLoading = true;

            if (contractTypeVM.IsTypeUpdate != 2)
            {
                await profileService.UpdateContractType(contractTypeVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ContractType");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await profileService.UpdateContractType(contractTypeVM);

                    if (affectedRows > 0)
                    {
                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ContractType");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu hồ sơ nhân viên liên quan.", SweetAlertMessageType.error);
                        contractTypeVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    contractTypeVM.IsTypeUpdate = 1;
                }
            }

            contractTypeVMs = await profileService.GetContractTypeList();

            isLoading = false;
        }

        private async Task CloseModalUpdate_ContractType()
        {
            isLoading = true;

            contractTypeVMs = await profileService.GetContractTypeList();

            isLoading = false;
        }

        //WorkType
        IEnumerable<WorkTypeVM> workTypeVMs;
        WorkTypeVM workTypeVM = new();
        private async Task InitializeModalList_WorkType()
        {
            isLoading = true;

            workTypeVMs = await profileService.GetWorkTypeList();

            isLoading = false;
        }
        private void InitializeModalUpdate_WorkType(int _isTypeUpdate, WorkTypeVM _workTypeVM)
        {
            isLoading = true;

            workTypeVM = new();

            if (_isTypeUpdate == 1)
            {
                workTypeVM = _workTypeVM;
            }

            workTypeVM.IsTypeUpdate = _isTypeUpdate;

            isLoading = false;
        }

        private void onchange_budgetSAT(ChangeEventArgs args)
        {
            isLoading = true;

            workTypeVM.BudgetSATConfig = decimal.Parse(args.Value.ToString());

            isLoading = false;
        }

        private void onchange_budgetSUN(ChangeEventArgs args)
        {
            isLoading = true;

            workTypeVM.BudgetSUNConfig = decimal.Parse(args.Value.ToString());

            isLoading = false;
        }

        private async Task UpdateWorkType()
        {
            isLoading = true;

            if (workTypeVM.IsTypeUpdate != 2)
            {
                await profileService.UpdateWorkType(workTypeVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_WorkType");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await profileService.UpdateWorkType(workTypeVM);

                    if (affectedRows > 0)
                    {
                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_WorkType");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu hồ sơ nhân viên liên quan.", SweetAlertMessageType.error);
                        workTypeVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    workTypeVM.IsTypeUpdate = 1;
                }
            }

            workTypeVMs = await profileService.GetWorkTypeList();

            isLoading = false;
        }

        private async Task CloseModalUpdate_WorkType()
        {
            isLoading = true;

            workTypeVMs = await profileService.GetWorkTypeList();

            isLoading = false;
        }

        //ProfileRelationship
        IEnumerable<ProfileRelationshipVM> profileRelationshipVMs;
        IEnumerable<ProfileRelationshipVM> relationshipVMs;
        ProfileRelationshipVM profileRelationshipVM = new();
        private async Task InitializeModal_ProfileRelationship()
        {
            isLoading = true;

            profileRelationshipVMs = await profileService.GetProfileRelationshipList(filterHrVM.selectedEserial);

            await js.InvokeAsync<object>("ShowModal", "#InitializeModal_ProfileRelationship");

            isLoading = false;
        }

        private async Task InitializeModalUpdate_ProfileRelationship(int _isTypeUpdate, ProfileRelationshipVM _profileRelationshipVM)
        {
            isLoading = true;

            profileRelationshipVM = new();

            relationshipVMs = await profileService.GetRelationshipList();
            profileRelationshipVM.RelationshipID = relationshipVMs.ElementAt(0).RelationshipID;

            profileRelationshipVM.Eserial = filterHrVM.selectedEserial;

            profileRelationshipVM.isActive = true;

            if (_isTypeUpdate == 1)
            {
                profileRelationshipVM = _profileRelationshipVM;
            }

            profileRelationshipVM.IsTypeUpdate = _isTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_ProfileRelationship");

            isLoading = false;
        }

        public string onchange_Rela_Birthday
        {
            get
            {
                return profileRelationshipVM.Rela_Birthday.HasValue ? profileRelationshipVM.Rela_Birthday.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileRelationshipVM.Rela_Birthday = LibraryFunc.FormatDateDDMMYYYY(value, profileRelationshipVM.Rela_Birthday);

                if (profileRelationshipVM.Rela_Birthday != null)
                {
                    if (profileRelationshipVM.RelationshipID == 9)
                    {
                        profileRelationshipVM.Rela_ValidTo = ((profileRelationshipVM.Rela_Birthday.Value.Year + 18) * 100 + profileRelationshipVM.Rela_Birthday.Value.Month).ToString();
                    }
                    else
                    {
                        profileRelationshipVM.Rela_ValidTo = "299912";
                    }
                }
                else
                {
                    profileRelationshipVM.Rela_ValidTo = String.Empty;
                }

            }
        }

        private async Task UpdateProfileRelationship()
        {
            isLoading = true;

            if (profileRelationshipVM.IsTypeUpdate != 2)
            {
                if (!profileRelationshipVM.isEmployeeTax)
                {
                    profileRelationshipVM.Rela_TaxCode = string.Empty;
                    profileRelationshipVM.Rela_ValidTo = string.Empty;
                }

                await profileService.UpdateProfileRelationship(profileRelationshipVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ProfileRelationship");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    await profileService.UpdateProfileRelationship(profileRelationshipVM);

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ProfileRelationship");
                    await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                }
                else
                {
                    profileRelationshipVM.IsTypeUpdate = 1;
                }
            }

            profileRelationshipVMs = await profileService.GetProfileRelationshipList(filterHrVM.selectedEserial);

            isLoading = false;
        }

        private async Task CloseModalUpdate_ProfileRelationship()
        {
            isLoading = true;

            profileRelationshipVMs = await profileService.GetProfileRelationshipList(filterHrVM.selectedEserial);

            isLoading = false;
        }

        //KPI
        private async void onchange_SearchEmpl(ChangeEventArgs e)
        {
            await Task.Delay(2000);

            filterHrVM.searchEmpl = e.Value.ToString();

            if (!String.IsNullOrEmpty(filterHrVM.searchEmpl))
            {
                empls = await profileService.GetSearchEmpl(filterHrVM);
            }
            else
            {
                empls = null;
            }
            StateHasChanged();
        }

    }
}
