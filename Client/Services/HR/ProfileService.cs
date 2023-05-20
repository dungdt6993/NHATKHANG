using D69soft.Shared.Models.ViewModels.DOC;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Data;
using System.Net.Http.Json;

namespace D69soft.Server.Services.HR
{
    public class ProfileService
    {
        private readonly HttpClient _httpClient;

        public ProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //Contact
        public async Task<List<ProfileManagamentVM>> GetContacts(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<List<ProfileManagamentVM>>($"api/Profile/GetContacts/{_UserID}");
        }

        //Profile
        public async Task<IEnumerable<ProfileVM>> GetEserialListByID(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/GetEserialListByID", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<ProfileVM>>();
        }

        public async Task<List<ProfileManagamentVM>> GetProfileList(FilterHrVM _filterHrVM, string _UserID)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/GetProfileList/{_UserID}", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<List<ProfileManagamentVM>>();
        }

        public async Task<ProfileManagamentVM> GetProfileByEserial(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/GetProfileByEserial", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<ProfileManagamentVM>();
        }

        public async Task<IEnumerable<ProfileManagamentVM>> GetSearchEmpl(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/GetSearchEmpl", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<ProfileManagamentVM>>();
        }

        public async Task<string> UpdateProfile(ProfileManagamentVM _profileManagamentVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/UpdateProfile", _profileManagamentVM);

            return await response.Content.ReadFromJsonAsync<string>();
        }

        public async Task<bool> UpdateUrlAvatar(string _Eserial, string _UrlAvatar)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Profile/UpdateUrlAvatar/{_Eserial}/{_UrlAvatar}");
        }

        public async Task<List<ProfileManagamentVM>> GetProfileHistory(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<List<ProfileManagamentVM>>($"api/Profile/GetProfileHistory/{_Eserial}");
        }

        public async Task<List<SalaryDefVM>> GetSalaryDef()
        {
            return await _httpClient.GetFromJsonAsync<List<SalaryDefVM>>($"api/Profile/GetSalaryDef");
        }

        public async Task<bool> CheckContainsEserial(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Profile/CheckContainsEserial/{_Eserial}");
        }

        public async Task<IEnumerable<CountryVM>> GetCountryList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CountryVM>>($"api/Profile/GetCountryList");
        }

        public async Task<IEnumerable<EthnicVM>> GetEthnicList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<EthnicVM>>($"api/Profile/GetEthnicList");
        }

        public async Task<bool> CkUpdateJobHistory(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Profile/CkUpdateJobHistory/{_Eserial}");
        }

        public async Task<bool> CkUpdateSalHistory(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Profile/CkUpdateSalHistory/{_Eserial}");
        }

        public async Task<bool> ResetPass(ProfileManagamentVM _profileManagamentVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/ResetPass", _profileManagamentVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DelProfileHistory(ProfileManagamentVM _profileManagamentVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/DelProfileHistory", _profileManagamentVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DelProfile(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Profile/DelProfile/{_Eserial}");
        }

        public async Task<bool> TerminateProfile(ProfileManagamentVM _profileManagamentVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/TerminateProfile", _profileManagamentVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> RestoreTerminateProfile(string _Eserial, string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Profile/RestoreTerminateProfile/{_Eserial}/{_UserID}");
        }

        //Permis
        public async Task<IEnumerable<FuncVM>> GetFuncGroupPermis(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<FuncVM>>($"api/Profile/GetFuncGroupPermis/{_Eserial}");
        }

        public async Task<IEnumerable<FuncVM>> GetFuncPermis(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<FuncVM>>($"api/Profile/GetFuncPermis/{_Eserial}");
        }

        public async Task<IEnumerable<FuncVM>> GetSubFuncPermis(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<FuncVM>>($"api/Profile/GetSubFuncPermis/{_Eserial}");
        }

        public async Task<IEnumerable<DepartmentVM>> GetDivisionPermis(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DepartmentVM>>($"api/Profile/GetDivisionPermis/{_Eserial}");
        }

        public async Task<IEnumerable<DepartmentVM>> GetDepartmentPermis(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DepartmentVM>>($"api/Profile/GetDepartmentPermis/{_Eserial}");
        }

        public async Task<IEnumerable<SysRptVM>> GetSysReportGroupPermis(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<SysRptVM>>($"api/Profile/GetSysReportGroupPermis/{_Eserial}");
        }

        public async Task<IEnumerable<SysRptVM>> GetSysReportPermis(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<SysRptVM>>($"api/Profile/GetSysReportPermis/{_Eserial}");
        }

        public async Task<bool> UpdatePermis(IEnumerable<FuncVM> _funcVMs, IEnumerable<FuncVM> _subFuncVMs, IEnumerable<DepartmentVM> _departmentVMs, IEnumerable<SysRptVM> _sysRptVMs, string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Profile/UpdatePermis/{_funcVMs}/{_subFuncVMs}/{_departmentVMs}/{_sysRptVMs}/{_Eserial}");
        }

        //EmplTrn
        public async Task<IEnumerable<EmployeeTransactionVM>> GetSalTrnGrp(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<EmployeeTransactionVM>>($"api/Profile/GetSalTrnGrp/{_Eserial}");
        }

        public async Task<IEnumerable<EmployeeTransactionVM>> GetSalTrnCode(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<EmployeeTransactionVM>>($"api/Profile/GetSalTrnCode/{_Eserial}");
        }

        public async Task<bool> UpdateEmplTrn(IEnumerable<EmployeeTransactionVM> _salTrnCodes, string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Profile/UpdateEmplTrn/{_salTrnCodes}/{_Eserial}");
        }

        //ContractType
        public async Task<IEnumerable<ContractTypeVM>> GetContractTypeGroupList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ContractTypeVM>>($"api/Profile/GetContractTypeGroupList");
        }
        public async Task<IEnumerable<ContractTypeVM>> GetContractTypeList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ContractTypeVM>>($"api/Profile/GetContractTypeList");
        }

        public async Task<int> GetNumMonthLC(string _ContractTypeID)
        {
            return await _httpClient.GetFromJsonAsync<int>($"api/Profile/GetNumMonthLC/{_ContractTypeID}");
        }

        public async Task<bool> ContainsContractTypeID(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Profile/ContainsContractTypeID/{id}");
        }

        public async Task<int> UpdateContractType(ContractTypeVM _contractTypeVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/UpdateContractType", _contractTypeVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }

        //WorkType
        public async Task<IEnumerable<WorkTypeVM>> GetWorkTypeList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<WorkTypeVM>>($"api/Profile/GetWorkTypeList");
        }

        public async Task<bool> ContainsWorkTypeID(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Profile/ContainsWorkTypeID/{id}");
        }

        public async Task<int> UpdateWorkType(WorkTypeVM _workTypeVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/UpdateWorkType", _workTypeVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }

        //ProfileRelationship
        public async Task<IEnumerable<ProfileRelationshipVM>> GetRelationshipList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ProfileRelationshipVM>>($"api/Profile/GetRelationshipList");
        }

        public async Task<IEnumerable<ProfileRelationshipVM>> GetProfileRelationshipList(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ProfileRelationshipVM>>($"api/Profile/GetProfileRelationshipList/{_Eserial}");
        }

        public async Task<bool> UpdateProfileRelationship(ProfileRelationshipVM _profileRelationshipVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/UpdateProfileRelationship", _profileRelationshipVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        //Báo cáo biến động nhân sự
        public async Task<DataTable> GetEmplChangeList(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/GetEmplChangeList", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<DataTable>();
        }

    }
}