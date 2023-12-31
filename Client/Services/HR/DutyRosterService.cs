﻿using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Collections;
using System.Net.Http.Json;

namespace D69soft.Client.Services.HR
{
    public class DutyRosterService
    {
        private readonly HttpClient _httpClient;

        public DutyRosterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<EserialVM>> GetEserialByID(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/GetEserialByID", _filterVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<EserialVM>>();
        }

        public async Task<List<DutyRosterVM>> GetDutyRosterList(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/GetDutyRosterList", _filterVM);

            return await response.Content.ReadFromJsonAsync<List<DutyRosterVM>>();
        }

        public async Task<DutyRosterVM> GetDutyRosterByDay(FilterVM _filterVM, DutyRosterVM _dutyRosterVM)
        {
            ArrayList arrayList = new ArrayList();

            arrayList.Add(_filterVM);
            arrayList.Add(_dutyRosterVM);

            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/GetDutyRosterByDay", arrayList);

            return await response.Content.ReadFromJsonAsync<DutyRosterVM>();
        }

        public async Task<IEnumerable<LockDutyRosterVM>> GetLockDutyRoster(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/GetLockDutyRoster", _filterVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<LockDutyRosterVM>>();
        }

        public async Task<bool> LockDutyRoster(LockDutyRosterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/LockDutyRoster", _filterVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> InitializeAttendanceRecordDutyRoster(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/InitializeAttendanceRecordDutyRoster", _filterVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<string> UpdateShiftWork(DutyRosterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/UpdateShiftWork", _filterVM);

            return await response.Content.ReadAsStringAsync();
        }

        //Shift
        public async Task<IEnumerable<ShiftTypeVM>> GetShiftTypeList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ShiftTypeVM>>($"api/DutyRoster/GetShiftTypeList");
        }

        public async Task<IEnumerable<ShiftVM>> GetShiftList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ShiftVM>>($"api/DutyRoster/GetShiftList");
        }

        public async Task<bool> ContainsShiftID(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/DutyRoster/ContainsShiftID/{id}");
        }

        public async Task<int> UpdateShift(ShiftVM _shiftVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/UpdateShift", _shiftVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }

        //Att
        public async Task<ProfileVM> GetProfileUser(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/GetProfileUser", _filterVM);

            return await response.Content.ReadFromJsonAsync<ProfileVM>();
        }

        public async Task<IEnumerable<DutyRosterVM>> GetAttendanceRecordList(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/GetAttendanceRecordList", _filterVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<DutyRosterVM>>();
        }

        public async Task<bool> UpdateExplain(DutyRosterVM _dutyRosterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/UpdateExplain", _dutyRosterVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateExplainHOD(DutyRosterVM _dutyRosterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/UpdateExplainHOD", _dutyRosterVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateExplainHR(DutyRosterVM _dutyRosterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/UpdateExplainHR", _dutyRosterVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> ConfirmExplain(DutyRosterVM _dutyRosterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/ConfirmExplain", _dutyRosterVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> ConfirmLateSoon(DutyRosterVM _dutyRosterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/ConfirmLateSoon", _dutyRosterVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> CalcFingerData(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/CalcFingerData", _filterVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        //EmplTrf
        public async Task<IEnumerable<DutyRosterVM>> GetEmplTrfList(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/GetEmplTrfList", _filterVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<DutyRosterVM>>();
        }

        public async Task<bool> UpdatePositionWork(DutyRosterVM _dutyRosterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/UpdatePositionWork", _dutyRosterVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<IEnumerable<DutyRosterVM>> GetDutyRosterNotes(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/GetDutyRosterNotes", _filterVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<DutyRosterVM>>();
        }

        public async Task<bool> UpdateDutyRosterNote(DutyRosterVM _dutyRosterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/UpdateDutyRosterNote", _dutyRosterVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        //WorkPlan
        public async Task<IEnumerable<DutyRosterVM>> GetWorkPlans(FilterVM _filterVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/GetWorkPlans", _filterVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<DutyRosterVM>>();
        }

        public async Task<int> UpdateWorkPlan(DutyRosterVM _workPlanVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/UpdateWorkPlan", _workPlanVM);

            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<bool> UpdateWorkPlanIsDone(DutyRosterVM _workPlanVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/DutyRoster/UpdateWorkPlanIsDone", _workPlanVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

    }
}
