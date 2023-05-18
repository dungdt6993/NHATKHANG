using D69soft.Shared.Models.ViewModels.DOC;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using System.Data;
using System.Net.Http.Json;

namespace Data.Repositories.HR
{
    public class PayrollService
    {
        private readonly HttpClient _httpClient;

        public PayrollService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //Monthly Income
        public async Task<List<MonthlyIncomeTrnOtherVM>> GetMonthlyIncomeTrnOtherList(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/GetMonthlyIncomeTrnOtherList", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<List<MonthlyIncomeTrnOtherVM>>();
        }

        public async Task<bool> UpdateMITrnOther(MonthlyIncomeTrnOtherVM _monthlyIncomeTrnOtherVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/UpdateMITrnOther", _monthlyIncomeTrnOtherVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> GetDataMITrnOtherFromExcel(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/GetDataMITrnOtherFromExcel", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        //Payroll
        public async Task<IEnumerable<SalaryTransactionGroupVM>> GetTrnGroupCodeList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<SalaryTransactionGroupVM>>($"api/Payroll/GetTrnGroupCodeList");
        }

        public async Task<IEnumerable<SalaryTransactionCodeVM>> GetTrnCodeList(int _TrnGroupCode)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<SalaryTransactionCodeVM>>($"api/Payroll/GetTrnGroupCodeList/{_TrnGroupCode}");
        }

        public async Task<bool> CalcSalary(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/CalcSalary", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> CancelCalcSalary(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/CancelCalcSalary", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> LockSalary(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/LockSalary", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> CancelLockSalary(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/CancelLockSalary", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<DataTable> GetPayrollList(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/GetPayrollList", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<DataTable>();
        }

        public async Task<LockSalaryVM> GetLockSalary(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/GetLockSalary", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<LockSalaryVM>();
        }

        public async Task<bool> IsOpenFunc(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/IsOpenFunc", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        //SalaryDef
        public async Task<IEnumerable<SalaryDefVM>> GetSalaryDefList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<SalaryDefVM>>($"api/Payroll/GetSalaryDefList");
        }

        public async Task<bool> UpdateSalaryDef(SalaryDefVM _salaryDefVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/UpdateSalaryDef", _salaryDefVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        //SalTrnCode
        public async Task<IEnumerable<SalaryTransactionCodeVM>> GetSalTrnCodeList()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<SalaryTransactionCodeVM>>($"api/Payroll/GetSalTrnCodeList");
        }

        public async Task<bool> ContainsTrnCodeID(int _TrnCode, int _TrnSubCode)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Payroll/ContainsTrnCodeID/{_TrnCode}/{_TrnSubCode}");
        }

        public async Task<bool> UpdateSalTrnCode(SalaryTransactionCodeVM _salaryTransactionCodeVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/UpdateSalTrnCode", _salaryTransactionCodeVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        //WDDefaut
        public async Task<IEnumerable<WDDefaultVM>> GetWDDefautList(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/GetWDDefautList", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<IEnumerable<WDDefaultVM>>();
        }

        //Payslip
        public async Task<List<PayslipVM>> GetPayslipList(FilterHrVM _filterHrVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/GetPayslipList", _filterHrVM);

            return await response.Content.ReadFromJsonAsync<List<PayslipVM>>();
        }

        public async Task<bool> UpdateSalaryReply(PayslipVM _payslipVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/UpdateSalaryReply", _payslipVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateSalaryQuestion(PayslipVM _payslipVM)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Payroll/UpdateSalaryQuestion", _payslipVM);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

    }
}
