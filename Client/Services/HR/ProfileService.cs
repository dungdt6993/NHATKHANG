using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Newtonsoft.Json;
using System.Collections;
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
        public async Task<List<ProfileVM>> GetContacts(string _UserID)
        {
            return await _httpClient.GetFromJsonAsync<List<ProfileVM>>($"api/Profile/GetContacts/{_UserID}");
        }

        //Profile
        public async Task<DataTable> dtEmplChange(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/dtEmplChange", _filterVM);

            return JsonConvert.DeserializeObject<DataTable>(await response.Content.ReadAsStringAsync());
        }

        public async Task<IEnumerable<EserialVM>> GetEserialListByID(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/GetEserialListByID", _filterVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<EserialVM>>();
        }

        public async Task<List<ProfileVM>> GetProfileList(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/GetProfileList", _filterVM);

            return await response.Content.ReadFromJsonAsync<List<ProfileVM>>();
        }

        public async Task<IEnumerable<ProfileVM>> GetSearchEmpl(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/GetSearchEmpl", _filterVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<ProfileVM>>();
        }

        public async Task<string> UpdateProfile(ProfileVM _profileVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/UpdateProfile", _profileVM);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<List<ProfileVM>> GetProfileHistory(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<List<ProfileVM>>($"api/Profile/GetProfileHistory/{_Eserial}");
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

        public async Task<string> ResetPass(ProfileVM _profileVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/ResetPass", _profileVM);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> DelProfileHistory(ProfileVM _profileVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/DelProfileHistory", _profileVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DelProfile(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Profile/DelProfile/{_Eserial}");
        }

        public async Task<bool> TerminateProfile(ProfileVM _profileVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/TerminateProfile", _profileVM);

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

        public async Task<IEnumerable<RptVM>> GetSysReportGroupPermis(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<RptVM>>($"api/Profile/GetSysReportGroupPermis/{_Eserial}");
        }

        public async Task<IEnumerable<RptVM>> GetSysReportPermis(string _Eserial)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<RptVM>>($"api/Profile/GetSysReportPermis/{_Eserial}");
        }

        public async Task<bool> UpdatePermis(IEnumerable<FuncVM> _funcVMs, IEnumerable<FuncVM> _subFuncVMs, IEnumerable<DepartmentVM> _departmentVMs, IEnumerable<RptVM> _sysRptVMs, string _Eserial)
        {
            ArrayList arrayList = new ArrayList();

            arrayList.Add(_funcVMs);
            arrayList.Add(_subFuncVMs);
            arrayList.Add(_departmentVMs);
            arrayList.Add(_sysRptVMs);

            var response = await _httpClient.PostAsJsonAsync($"api/Profile/UpdatePermis/{_Eserial}", arrayList);

            return await response.Content.ReadFromJsonAsync<bool>();
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
            var response = await _httpClient.PostAsJsonAsync($"api/Profile/UpdateEmplTrn/{_Eserial}", _salTrnCodes);

            return await response.Content.ReadFromJsonAsync<bool>();
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

    }
}