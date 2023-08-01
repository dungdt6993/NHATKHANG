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
using D69soft.Shared.Utilities;
using D69soft.Client.Extensions;

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

        bool isLoading;
        bool isLoadingScreen = true;

        protected string UserID;

        //PermisFunc
        bool HR_Profile_EditSal;
        bool HR_Profile_ChangeSal;
        bool HR_Profile_ViewSal;
        bool HR_Profile_User_Permis;
        bool HR_Profile_User_ResetPass;
        bool HR_Profile_EmployeeTransaction;
        bool HR_Profile_UpdateHistory;
        bool HR_Profile_UpdateContractType;
        bool HR_Profile_UpdateWorkType;
        bool HR_Profile_Edit;
        bool HR_Profile_New;
        bool HR_Profile_Del;
        bool HR_Profile_Adjust;
        bool HR_Profile_Terminate;

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
        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;
        IEnumerable<SectionVM> section_filter_list;
        IEnumerable<PositionVM> position_filter_list;

        ProfileVM profileVM = new();
        private List<ProfileVM> profileVMs { get; set; } = new();

        private Virtualize<ProfileVM> virtualizeProfileList { get; set; }

        IEnumerable<CountryVM> countryVMs;
        IEnumerable<EthnicVM> ethnicVMs;
        IEnumerable<PermissionUserVM> permissionUserVMs;
        List<ProfileVM> profileHistorys;
        List<SalaryDefVM> salaryDefVMs;

        //KPI
        IEnumerable<ProfileVM> empls;

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
            filterHrVM.UserID = UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "HR_Profile"))
            {
                logVM.LogUser = UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "HR_Profile";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            HR_Profile_EditSal = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_EditSal");
            HR_Profile_ChangeSal = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_ChangeSal");
            HR_Profile_ViewSal = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_ViewSal");
            HR_Profile_User_Permis = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_User_Permis");
            HR_Profile_User_ResetPass = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_User_ResetPass");
            HR_Profile_EmployeeTransaction = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_EmployeeTransaction");
            HR_Profile_UpdateHistory = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_UpdateHistory");
            HR_Profile_UpdateContractType = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_UpdateContractType");
            HR_Profile_UpdateWorkType = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_UpdateWorkType");
            HR_Profile_Edit = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_Edit");
            HR_Profile_New = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_New");
            HR_Profile_Del = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_Del");
            HR_Profile_Adjust = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_Adjust");
            HR_Profile_Terminate = await sysService.CheckAccessSubFunc(UserID, "HR_Profile_Terminate");

            //Initialize Filter
            division_filter_list = await organizationalChartService.GetDivisionList(filterHrVM);
            filterHrVM.DivisionID = division_filter_list.Count() > 0 ? division_filter_list.ElementAt(0).DivisionID : string.Empty;

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterHrVM);

            filterHrVM.SectionID = string.Empty;
            section_filter_list = await organizationalChartService.GetSectionList();

            filterHrVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartService.GetPositionList();

            filterHrVM.Eserial = string.Empty;

            filterHrVM.searchValues = string.Empty;

            filterHrVM.Year = DateTime.Now.Year;

            await GetProfileList();

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

            await GetProfileList();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_department(string value)
        {
            isLoading = true;

            filterHrVM.DepartmentID = value;

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };

            await GetProfileList();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_section(string value)
        {
            isLoading = true;

            filterHrVM.SectionID = value;

            await GetProfileList();

            isLoading = false;

            StateHasChanged();
        }

        private async Task onchange_filter_positiongroup(string[] value)
        {
            isLoading = true;

            filterHrVM.arrPositionID = value;

            filterHrVM.PositionGroupID = string.Join(",", value);

            await GetProfileList();

            isLoading = false;
        }

        private async Task onchange_filter_typeprofile(ChangeEventArgs args)
        {
            isLoading = true;

            filterHrVM.TypeProfile = int.Parse(args.Value.ToString());

            await GetProfileList();

            isLoading = false;
        }

        private async Task SearchProfile(string value)
        {
            filterHrVM.searchValues = value;

            await virtualizeProfileList.RefreshDataAsync();
            StateHasChanged();
        }

        //Profile
        protected async Task GetProfileList()
        {
            isLoading = true;

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

            profileVM = new();

            //Bien dong nhan su
            dtEmplChange = await profileService.dtEmplChange(filterHrVM);

            //Load profiles
            profileVMs = await profileService.GetProfileList(filterHrVM);

            if(empls == null)
            {
                empls = profileVMs;
            }

            await virtualizeProfileList.RefreshDataAsync();
            StateHasChanged();

            isLoading = false;
        }

        private ValueTask<ItemsProviderResult<ProfileVM>> LoadProfileList(ItemsProviderRequest request)
        {
            return new(new ItemsProviderResult<ProfileVM>(
                    profileVMs.Where(x => x.Eserial.Contains(filterHrVM.searchValues) || x.FullName.ToUpper().Contains(filterHrVM.searchValues.ToUpper())).Skip(request.StartIndex).Take(request.Count),
                    profileVMs.Where(x => x.Eserial.Contains(filterHrVM.searchValues) || x.FullName.ToUpper().Contains(filterHrVM.searchValues.ToUpper())).Count()
                ));
        }

        private void onclick_selectedEserial(ProfileVM _profileVM)
        {
            profileVM = _profileVM == profileVM ? new() : _profileVM;
        }

        public string onchange_Birthday
        {
            get
            {
                return profileVM.Birthday.HasValue ? profileVM.Birthday.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileVM.Birthday = LibraryFunc.FormatDateDDMMYYYY(value, profileVM.Birthday);
            }
        }

        public string onchange_DateOfIssue
        {
            get
            {
                return profileVM.DateOfIssue.HasValue ? profileVM.DateOfIssue.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileVM.DateOfIssue = LibraryFunc.FormatDateDDMMYYYY(value, profileVM.DateOfIssue);
            }
        }

        public string onchange_VisaExpDate
        {
            get
            {
                return profileVM.VisaExpDate.HasValue ? profileVM.VisaExpDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileVM.VisaExpDate = LibraryFunc.FormatDateDDMMYYYY(value, profileVM.VisaExpDate);
            }
        }

        public string onchange_JoinDate
        {
            get
            {
                return profileVM.JoinDate.HasValue ? profileVM.JoinDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileVM.JoinDate = LibraryFunc.FormatDateDDMMYYYY(value, profileVM.JoinDate);
            }
        }

        public string onchange_StartDayAL
        {
            get
            {
                return profileVM.StartDayAL.HasValue ? profileVM.StartDayAL.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileVM.StartDayAL = LibraryFunc.FormatDateDDMMYYYY(value, profileVM.StartDayAL);
            }
        }

        public string onchange_StartContractDate
        {
            get
            {
                return profileVM.StartContractDate.HasValue ? profileVM.StartContractDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                isLoading = true;

                profileVM.StartContractDate = LibraryFunc.FormatDateDDMMYYYY(value, profileVM.StartContractDate);

                if (profileVM.StartContractDate == null)
                {
                    disabled_ContractTypeID = true;
                    disabled_EndContractDate = true;
                }
                else
                {
                    disabled_ContractTypeID = false;
                }
                profileVM.ContractTypeID = string.Empty;
                profileVM.EndContractDate = null;

                isLoading = false;
            }
        }

        private async Task onchange_ContractType(string value)
        {
            isLoading = true;

            profileVM.ContractTypeID = value;

            int NumMonth = await profileService.GetNumMonthLC(profileVM.ContractTypeID);
            if (NumMonth != 0)
            {
                profileVM.EndContractDate = Convert.ToDateTime(profileVM.StartContractDate).AddMonths(NumMonth).AddDays(-1);
                disabled_EndContractDate = true;
            }
            else
            {
                disabled_EndContractDate = false;
                profileVM.EndContractDate = null;
            }

            isLoading = false;

        }

        public string onchange_EndContractDate
        {
            get
            {
                return profileVM.EndContractDate.HasValue ? profileVM.EndContractDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileVM.EndContractDate = LibraryFunc.FormatDateDDMMYYYY(value, profileVM.EndContractDate);
            }
        }

        public string onchange_JobStartDate
        {
            get
            {
                return profileVM.JobStartDate.HasValue ? profileVM.JobStartDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileVM.JobStartDate = LibraryFunc.FormatDateDDMMYYYY(value, profileVM.JobStartDate);
            }
        }

        public string onchange_TerminateDate
        {
            get
            {
                return profileVM.TerminateDate.HasValue ? profileVM.TerminateDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileVM.TerminateDate = LibraryFunc.FormatDateDDMMYYYY(value, profileVM.TerminateDate);
            }
        }

        public string onchange_BeginSalaryDate
        {
            get
            {
                return profileVM.BeginSalaryDate.HasValue ? profileVM.BeginSalaryDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                profileVM.BeginSalaryDate = LibraryFunc.FormatDateDDMMYYYY(value, profileVM.BeginSalaryDate);
            }
        }

        public decimal onchange_BasicSalary
        {
            get { return profileVM.BasicSalary; }
            set
            {
                isLoading = true;

                profileVM.BasicSalary = value;

                profileVM.TotalSalary = profileVM.BasicSalary + profileVM.OtherSalary + profileVM.Benefit1 + profileVM.Benefit2 + profileVM.Benefit3 + profileVM.Benefit4
                                                + profileVM.Benefit5 + profileVM.Benefit6 + profileVM.Benefit7 + profileVM.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_OtherSalary
        {
            get { return profileVM.OtherSalary; }
            set
            {
                isLoading = true;

                profileVM.OtherSalary = value;

                profileVM.TotalSalary = profileVM.BasicSalary + profileVM.OtherSalary + profileVM.Benefit1 + profileVM.Benefit2 + profileVM.Benefit3 + profileVM.Benefit4
                                                + profileVM.Benefit5 + profileVM.Benefit6 + profileVM.Benefit7 + profileVM.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_Benefit1
        {
            get { return profileVM.Benefit1; }
            set
            {
                isLoading = true;

                profileVM.Benefit1 = value;

                profileVM.TotalSalary = profileVM.BasicSalary + profileVM.OtherSalary + profileVM.Benefit1 + profileVM.Benefit2 + profileVM.Benefit3 + profileVM.Benefit4
                                                + profileVM.Benefit5 + profileVM.Benefit6 + profileVM.Benefit7 + profileVM.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_Benefit2
        {
            get { return profileVM.Benefit2; }
            set
            {
                isLoading = true;

                profileVM.Benefit2 = value;

                profileVM.TotalSalary = profileVM.BasicSalary + profileVM.OtherSalary + profileVM.Benefit1 + profileVM.Benefit2 + profileVM.Benefit3 + profileVM.Benefit4
                                                + profileVM.Benefit5 + profileVM.Benefit6 + profileVM.Benefit7 + profileVM.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_Benefit3
        {
            get { return profileVM.Benefit3; }
            set
            {
                isLoading = true;

                profileVM.Benefit3 = value;

                profileVM.TotalSalary = profileVM.BasicSalary + profileVM.OtherSalary + profileVM.Benefit1 + profileVM.Benefit2 + profileVM.Benefit3 + profileVM.Benefit4
                                                + profileVM.Benefit5 + profileVM.Benefit6 + profileVM.Benefit7 + profileVM.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_Benefit4
        {
            get { return profileVM.Benefit4; }
            set
            {
                isLoading = true;

                profileVM.Benefit4 = value;

                profileVM.TotalSalary = profileVM.BasicSalary + profileVM.OtherSalary + profileVM.Benefit1 + profileVM.Benefit2 + profileVM.Benefit3 + profileVM.Benefit4
                                                + profileVM.Benefit5 + profileVM.Benefit6 + profileVM.Benefit7 + profileVM.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_Benefit5
        {
            get { return profileVM.Benefit5; }
            set
            {
                isLoading = true;

                profileVM.Benefit5 = value;

                profileVM.TotalSalary = profileVM.BasicSalary + profileVM.OtherSalary + profileVM.Benefit1 + profileVM.Benefit2 + profileVM.Benefit3 + profileVM.Benefit4
                                                + profileVM.Benefit5 + profileVM.Benefit6 + profileVM.Benefit7 + profileVM.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_Benefit6
        {
            get { return profileVM.Benefit6; }
            set
            {
                isLoading = true;

                profileVM.Benefit6 = value;

                profileVM.TotalSalary = profileVM.BasicSalary + profileVM.OtherSalary + profileVM.Benefit1 + profileVM.Benefit2 + profileVM.Benefit3 + profileVM.Benefit4
                                                + profileVM.Benefit5 + profileVM.Benefit6 + profileVM.Benefit7 + profileVM.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_Benefit7
        {
            get { return profileVM.Benefit7; }
            set
            {
                isLoading = true;

                profileVM.Benefit7 = value;

                profileVM.TotalSalary = profileVM.BasicSalary + profileVM.OtherSalary + profileVM.Benefit1 + profileVM.Benefit2 + profileVM.Benefit3 + profileVM.Benefit4
                                                + profileVM.Benefit5 + profileVM.Benefit6 + profileVM.Benefit7 + profileVM.Benefit8;

                isLoading = false;
            }
        }

        public decimal onchange_Benefit8
        {
            get { return profileVM.Benefit8; }
            set
            {
                isLoading = true;

                profileVM.Benefit8 = value;

                profileVM.TotalSalary = profileVM.BasicSalary + profileVM.OtherSalary + profileVM.Benefit1 + profileVM.Benefit2 + profileVM.Benefit3 + profileVM.Benefit4
                                                + profileVM.Benefit5 + profileVM.Benefit6 + profileVM.Benefit7 + profileVM.Benefit8;

                isLoading = false;
            }
        }

        private void onchange_SalaryByBank(ChangeEventArgs args)
        {
            isLoading = true;

            profileVM.SalaryByBank = int.Parse(args.Value.ToString());

            isLoading = false;
        }

        private void onchange_IsPayBy(ChangeEventArgs args)
        {
            isLoading = true;

            int ck = int.Parse(args.Value.ToString());

            if (ck == 1)
            {
                profileVM.IsPayByMonth = 1;
                profileVM.IsPayByDate = 0;
            }

            if (ck == 2)
            {
                profileVM.IsPayByMonth = 0;
                profileVM.IsPayByDate = 1;
            }

            isLoading = false;
        }

        private void onchange_ckContractExtension(ChangeEventArgs args)
        {
            isLoading = true;

            profileVM.ckContractExtension = int.Parse(args.Value.ToString());

            disable_ckContractExtension = true;

            if (profileVM.ckContractExtension == 1)
            {
                disabled_ContractTypeID = false;
                disabled_EndContractDate = false;

                profileVM.StartContractDate = Convert.ToDateTime(profileVM.EndContractDate).AddDays(1);

                profileVM.ContractTypeID = string.Empty;
                profileVM.EndContractDate = null;
            }

            if (profileVM.ckContractExtension == 2)
            {
                disabled_StartContractDate = false;

                profileVM.StartContractDate = null;
                profileVM.ContractTypeID = string.Empty;
                profileVM.EndContractDate = null;
            }

            disabled_btnUpdateProfile = false;

            isLoading = false;
        }

        private void onchange_ckJob(ChangeEventArgs args)
        {
            isLoading = true;

            profileVM.ckJob = int.Parse(args.Value.ToString());

            disable_ckContractExtension = true;
            disable_ckJob = true;

            if (profileVM.ckContractExtension != 0)
            {
                profileVM.JobStartDate = profileVM.StartContractDate;
                disabled_JobStartDate = true;
            }
            else
            {
                profileVM.JobStartDate = null;
                disabled_JobStartDate = false;
            }

            disabled_btnUpdateProfile = false;

            isLoading = false;
        }

        private void onchange_ckSal(ChangeEventArgs args)
        {
            isLoading = true;

            profileVM.ckSal = int.Parse(args.Value.ToString());

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

            profileVM.Reason = string.Empty;
            profileVM.ApprovedBy = string.Empty;

            if (profileVM.ckContractExtension != 0)
            {
                profileVM.BeginSalaryDate = profileVM.StartContractDate;
                disabled_BeginSalaryDate = true;
            }

            if (profileVM.ckContractExtension == 0 && profileVM.ckJob != 0)
            {
                profileVM.BeginSalaryDate = profileVM.JobStartDate;
                disabled_BeginSalaryDate = true;
            }

            disabled_btnUpdateProfile = false;

            isLoading = false;
        }

        private async Task Initialize_ModalProfile(int _IsTypeUpdate)
        {
            isLoading = true;

            profileVM.IsTypeUpdate = _IsTypeUpdate;

            disabled_btnUpdateProfile = false;

            disable_Eserial = true;
            placeholder_Eserial = "Mã NV";

            countryVMs = await profileService.GetCountryList();
            ethnicVMs = await profileService.GetEthnicList();

            contractTypeVMs = await profileService.GetContractTypeList();
            workTypeVMs = await profileService.GetWorkTypeList();

            permissionUserVMs = await authService.GetPermissionUser(UserID);

            salaryDefVMs = await profileService.GetSalaryDef();

            if (profileVM.IsTypeUpdate == 0)
            {
                disabled_ContractTypeID = true;
                disabled_EndContractDate = true;

                if (profileVM.IsTypeUpdate == 0)
                {
                    profileVM = new();

                    profileVM.CountryCode = "VN";

                    profileVM.EthnicID = 1;

                    profileVM.DivisionID = filterHrVM.DivisionID;

                    profileVM.PermisId = 4;

                    profileVM.IsPayByMonth = 1;

                    profileVM.IsTypeUpdate = _IsTypeUpdate;
                }

                profileVM.UrlAvatar = UrlDirectory.Default_Avatar;

                profileHistorys = null;

                profileVM.isAutoEserial = division_filter_list.Where(x => x.DivisionID == filterHrVM.DivisionID).Select(x => x.isAutoEserial).First();

                if (!profileVM.isAutoEserial)
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
                profileHistorys = await profileService.GetProfileHistory(profileVM.Eserial);

                if (!await profileService.CkUpdateJobHistory(profileVM.Eserial))
                {
                    disabled_StartContractDate = true;
                    disabled_ContractTypeID = true;
                    disabled_EndContractDate = true;
                    disabled_JobStartDate = true;
                }

                if (!await profileService.CkUpdateSalHistory(profileVM.Eserial))
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

                if (profileVM.IsTypeUpdate == 2)
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
            stream.Close();

            profileVM.UrlAvatar = $"data:{format};base64,{Convert.ToBase64String(memoryStream.ToArray())}";// convert to a base64 string!!

            profileVM.FileContent = memoryStream.ToArray();

            memoryStream.Close();

            isLoading = false;
        }

        private void FileDefault()
        {
            memoryStream = null;
            profileVM.IsDelFileUpload = true;
            profileVM.UrlAvatar = UrlDirectory.Default_Avatar;
            profileVM.FileContent = null;
        }

        private async Task UpdateProfile()
        {
            isLoading = true;

            disabled_btnUpdateProfile = true;

            profileVM.UserID = UserID;

            profileVM.Eserial = await profileService.UpdateProfile(profileVM);

            if (profileVM.IsTypeUpdate == 1)
            {

                logVM.LogDesc = "Cập nhật hồ sơ " + profileVM.Eserial + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                disabled_btnUpdateProfile = false;
            }

            if (profileVM.IsTypeUpdate == 2)
            {
                logVM.LogDesc = "Điều chỉnh hồ sơ " + profileVM.Eserial + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                disabled_btnUpdateProfile = true;
                disable_ckContractExtension = true;
                disable_ckJob = true;
                disable_ckSal = true;
            }

            if (profileVM.IsTypeUpdate == 0)
            {
                logVM.LogDesc = "Thêm mới hồ sơ " + profileVM.Eserial + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                profileVM.IsTypeUpdate = 1;
                disabled_btnUpdateProfile = false;
            }

            profileHistorys = await profileService.GetProfileHistory(profileVM.Eserial);

            isLoading = false;
        }

        private async Task ResetPass()
        {
            if (await js.Swal_Confirm("Xác nhận!", $"Đặt lại mật khẩu đăng nhập hệ thống?", SweetAlertMessageType.question))
            {
                profileVM.User_isChangePass = 0;
                profileVM.User_PassReset = await profileService.ResetPass(profileVM);

                logVM.LogDesc = "Đặt lại mật khẩu đăng nhập " + profileVM.Eserial + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);
            }
        }

        private async Task DelProfileHistory(ProfileVM profilehistory)
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
            {
                if (await profileService.DelProfileHistory(profilehistory))
                {
                    filterHrVM.Eserial = profileVM.Eserial;

                    profileVM = (await profileService.GetProfileList(filterHrVM)).First();

                    profileHistorys = await profileService.GetProfileHistory(profileVM.Eserial);

                    profileVM.IsTypeUpdate = 1;

                    filterHrVM.Eserial = String.Empty;

                    logVM.LogDesc = "Xóa lịch sử hồ sơ " + profileVM.Eserial + "";
                    await sysService.InsertLog(logVM);

                    await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);
                }
            }

            isLoading = false;
        }

        private async Task DelProfile()
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa nhân viên mã " + profileVM.Eserial + "?", SweetAlertMessageType.question))
            {
                if (await profileService.DelProfile(profileVM.Eserial))
                {
                    logVM.LogDesc = "Xóa hồ sơ " + profileVM.Eserial + "";
                    await sysService.InsertLog(logVM);

                    await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                    await GetProfileList();
                }
            }

            isLoading = false;
        }

        private void InitializeModal_TerminateProfile()
        {
            isLoading = true;

            disabled_btnUpdateProfile = false;

            profileVM.IsTypeUpdate = 4;

            profileVM.UserID = UserID;

            isLoading = false;
        }

        private async Task TerminateProfile()
        {
            isLoading = true;

            if (await profileService.TerminateProfile(profileVM))
            {
                logVM.LogDesc = "Chấm dứt hợp đồng " + profileVM.Eserial + "";
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModal_TerminateProfile");

                await GetProfileList();
            }
            disabled_btnUpdateProfile = true;

            isLoading = false;
        }

        private async Task RestoreTerminateProfile()
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắc chắn hủy chấm dứt hợp đồng?", SweetAlertMessageType.question))
            {
                if (await profileService.RestoreTerminateProfile(profileVM.Eserial, UserID))
                {
                    logVM.LogDesc = "Hủy chấm dứt hợp đồng " + profileVM.Eserial + "";
                    await sysService.InsertLog(logVM);

                    await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);

                    await GetProfileList();
                }
            }

            isLoading = false;
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

            permis_funcGroups = await profileService.GetFuncGroupPermis(profileVM.Eserial);
            permis_funcs = await profileService.GetFuncPermis(profileVM.Eserial);
            permis_subFuncs = await profileService.GetSubFuncPermis(profileVM.Eserial);

            permis_divs = await profileService.GetDivisionPermis(profileVM.Eserial);
            permis_depts = await profileService.GetDepartmentPermis(profileVM.Eserial);

            permis_rptgrps = await profileService.GetSysReportGroupPermis(profileVM.Eserial);
            permis_rpts = await profileService.GetSysReportPermis(profileVM.Eserial);

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
            await profileService.UpdatePermis(permis_funcs.Where(x => x.IsChecked), permis_subFuncs.Where(x => x.IsChecked), permis_depts.Where(x => x.IsChecked), permis_rpts.Where(x => x.IsChecked), profileVM.Eserial);

            logVM.LogDesc = "Cập nhật phân quyền " + profileVM.Eserial + "";
            await sysService.InsertLog(logVM);

            await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);
        }

        //EmployeeTransaction
        IEnumerable<EmployeeTransactionVM> salTrnGrps;
        IEnumerable<EmployeeTransactionVM> salTrnCodes;
        private async Task InitializeModal_EmployeeTransaction()
        {
            isLoading = true;

            salTrnGrps = await profileService.GetSalTrnGrp(profileVM.Eserial);
            salTrnCodes = await profileService.GetSalTrnCode(profileVM.Eserial);

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
            await profileService.UpdateEmplTrn(salTrnCodes.Where(x => x.IsChecked), profileVM.Eserial);

            logVM.LogDesc = "Cập nhật thiết lập giao dịch lương " + profileVM.Eserial + "";
            await sysService.InsertLog(logVM);

            await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);
        }

        //ContractType
        IEnumerable<ContractTypeVM> contractTypeGroupVMs;
        IEnumerable<ContractTypeVM> contractTypeVMs;
        ContractTypeVM contractTypeVM = new();
        private async Task InitializeModalList_ContractType()
        {
            isLoading = true;

            contractTypeVMs = await profileService.GetContractTypeList();

            isLoading = false;
        }

        private async Task InitializeModalUpdate_ContractType(int _IsTypeUpdate, ContractTypeVM _contractTypeVM)
        {
            isLoading = true;

            contractTypeVM = new();

            contractTypeGroupVMs = await profileService.GetContractTypeGroupList();

            if (_IsTypeUpdate == 1)
            {
                contractTypeVM = _contractTypeVM;
            }

            contractTypeVM.IsTypeUpdate = _IsTypeUpdate;

            isLoading = false;
        }

        private async Task UpdateContractType(EditContext _formContractTypeVM, int _IsTypeUpdate)
        {
            contractTypeVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formContractTypeVM.Validate()) return;

            isLoading = true;

            if (contractTypeVM.IsTypeUpdate != 2)
            {
                await profileService.UpdateContractType(contractTypeVM);

                await InitializeModalList_ContractType();

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
                        await InitializeModalList_ContractType();

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
        private void InitializeModalUpdate_WorkType(int _IsTypeUpdate, WorkTypeVM _workTypeVM)
        {
            isLoading = true;

            workTypeVM = new();

            if (_IsTypeUpdate == 1)
            {
                workTypeVM = _workTypeVM;
            }

            workTypeVM.IsTypeUpdate = _IsTypeUpdate;

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

        private async Task UpdateWorkType(EditContext _formWorkTypeVM, int _IsTypeUpdate)
        {
            workTypeVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formWorkTypeVM.Validate()) return;

            isLoading = true;

            if (workTypeVM.IsTypeUpdate != 2)
            {
                await profileService.UpdateWorkType(workTypeVM);

                await InitializeModalList_WorkType();

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
                        await InitializeModalList_WorkType();

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

            isLoading = false;
        }

        //ProfileRelationship
        IEnumerable<ProfileRelationshipVM> profileRelationshipVMs;
        IEnumerable<ProfileRelationshipVM> relationshipVMs;
        ProfileRelationshipVM profileRelationshipVM = new();
        private async Task InitializeModalList_ProfileRelationship()
        {
            isLoading = true;

            profileRelationshipVMs = await profileService.GetProfileRelationshipList(profileVM.Eserial);

            await js.InvokeAsync<object>("ShowModal", "#InitializeModal_ProfileRelationship");

            isLoading = false;
        }

        private async Task InitializeModalUpdate_ProfileRelationship(int _IsTypeUpdate, ProfileRelationshipVM _profileRelationshipVM)
        {
            isLoading = true;

            profileRelationshipVM = new();

            relationshipVMs = await profileService.GetRelationshipList();
            profileRelationshipVM.RelationshipID = relationshipVMs.ElementAt(0).RelationshipID;

            profileRelationshipVM.Eserial = profileVM.Eserial;

            profileRelationshipVM.isActive = true;

            if (_IsTypeUpdate == 1)
            {
                profileRelationshipVM = _profileRelationshipVM;
            }

            profileRelationshipVM.IsTypeUpdate = _IsTypeUpdate;

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

        private async Task UpdateProfileRelationship(EditContext _formProfileRelationshipVM, int _IsTypeUpdate)
        {
            profileRelationshipVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formProfileRelationshipVM.Validate()) return;

            isLoading = true;

            if (profileRelationshipVM.IsTypeUpdate != 2)
            {
                if (!profileRelationshipVM.isEmployeeTax)
                {
                    profileRelationshipVM.Rela_TaxCode = string.Empty;
                    profileRelationshipVM.Rela_ValidTo = string.Empty;
                }

                await profileService.UpdateProfileRelationship(profileRelationshipVM);

                await InitializeModalList_ProfileRelationship();

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ProfileRelationship");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    await profileService.UpdateProfileRelationship(profileRelationshipVM);

                    await InitializeModalList_ProfileRelationship();

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_ProfileRelationship");
                    await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                }
                else
                {
                    profileRelationshipVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

    }
}
