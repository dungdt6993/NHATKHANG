using D69soft.Shared.Models.ViewModels.DOC;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Net.Http.Json;

namespace D69soft.Client.Services.HR
{
    public class OrganizationalChartService
    {
        private readonly HttpClient _httpClient;

        public OrganizationalChartService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        //Division
        public async Task<IEnumerable<DivisionVM>> GetDivisionList(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OrganizationalChart/GetDivisionList", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<DivisionVM>>();
        }
        public async Task<bool> CheckContainsDivisionID(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/OrganizationalChart/CheckContainsDivisionID/{id}");
        }
        public async Task<int> UpdateDivision(DivisionVM _divisionVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OrganizationalChart/UpdateDivision", _divisionVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }

        //Department
        public async Task<IEnumerable<DepartmentVM>> GetDepartmentList(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OrganizationalChart/GetDepartmentList", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<DepartmentVM>>();
        }
        public async Task<bool> CheckContainsDepartmentID(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/OrganizationalChart/CheckContainsDepartment/{id}");
        }
        public async Task<int> UpdateDepartment(DepartmentVM _departmentVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OrganizationalChart/UpdateDepartment", _departmentVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }

        //DepartmentGroup
        public async Task<IEnumerable<DepartmentGroupVM>> GetDepartmentGroupList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DepartmentGroupVM>>($"api/OrganizationalChart/GetDepartmentGroupList");
        }
        public async Task<bool> CheckContainsDepartmentGroupID(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/OrganizationalChart/CheckContainsDepartmentGroupID/{id}");
        }
        public async Task<int> UpdateDepartmentGroup(DepartmentGroupVM _departmentGroupVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OrganizationalChart/UpdateDepartmentGroup", _departmentGroupVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }

        //Section
        public async Task<IEnumerable<SectionVM>> GetSectionList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<SectionVM>>($"api/OrganizationalChart/GetSectionList");
        }
        public async Task<bool> CheckContainsSectionID(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/OrganizationalChart/CheckContainsSectionID/{id}");
        }
        public async Task<int> UpdateSection(SectionVM _sectionVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OrganizationalChart/UpdateSection", _sectionVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }

        //Position
        public async Task<List<PositionVM>> GetPositionList()
        {
            return await _httpClient.GetFromJsonAsync<List<PositionVM>>($"api/OrganizationalChart/GetPositionList");
        }
        public async Task<bool> CheckContainsPositionID(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/OrganizationalChart/CheckContainsPositionID/{id}");
        }
        public async Task<int> UpdatePosition(PositionVM _positionVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OrganizationalChart/UpdatePosition", _positionVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }

        //PositionGroup
        public async Task<IEnumerable<PositionGroupVM>> GetPositionGroupList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PositionGroupVM>>($"api/GetPositionList/GetPositionGroupList");
        }
        public async Task<bool> CheckContainsPositionGroupID(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/OrganizationalChart/CheckContainsPositionGroupID/{id}");
        }
        public async Task<int> UpdatePositionGroup(PositionGroupVM _positionGroupVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/OrganizationalChart/UpdatePositionGroup", _positionGroupVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }
    }
}
