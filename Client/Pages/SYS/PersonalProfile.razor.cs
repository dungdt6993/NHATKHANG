using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Client.Extensions;

namespace D69soft.Client.Pages.SYS
{
    partial class PersonalProfile
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] PayrollService payrollService { get; set; }

        protected string UserID;

        bool isLoadingScreen = true;

        LogVM logVM = new();

        //Filter
        FilterHrVM filterHrVM = new();

        UserVM userInfo = new UserVM();

        IEnumerable<PayslipVM> payslipUsers;

        //Monthly Income
        List<MonthlyIncomeTrnOtherVM> monthlyIncomeTrnOtherVMs;

        protected override async Task OnInitializedAsync()
        {
            UserID = (await authenticationStateTask).User.GetUserId();

            logVM.LogType = "FUNC";
            logVM.LogName = "HR_PersonalProfile";
            logVM.LogUser = UserID;
            await sysService.InsertLog(logVM);

            userInfo = await sysService.GetInfoUser(UserID);

            payslipUsers = await payrollService.GetPayslipUser(UserID);

            isLoadingScreen = false;
        }

        private async Task InitializeModalList_SalTrn(int _period)
        {
            filterHrVM.Month = int.Parse(_period.ToString().Substring(4, 2));
            filterHrVM.Year = int.Parse(_period.ToString().Substring(0, 4));
            filterHrVM.DivisionID = userInfo.DivisionID;
            filterHrVM.DepartmentID = string.Empty;
            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };
            filterHrVM.SectionID = string.Empty;
            filterHrVM.Eserial = UserID;
            filterHrVM.TrnCode = 0;
            filterHrVM.TrnSubCode = 0;

            monthlyIncomeTrnOtherVMs = await payrollService.GetMonthlyIncomeTrnOtherList(filterHrVM);

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalList_SalTrn");
        }

        string ReportName = String.Empty;
        private async Task viewPayslip(string _id)
        {
            ReportName = "Payslip?ID=" + _id + "";
            await js.InvokeAsync<object>("ShowModal", "#InitializeModalView_Rpt");
        }

        private async Task UpdateSalaryQuestion(int type, PayslipVM _payslipVM)
        {
            _payslipVM.TypeUpdateSalaryQuestion = type;

            if ((_payslipVM.SalaryQuestion ?? string.Empty) == string.Empty)
            {
                await js.Swal_Message("Cảnh báo!", "Câu hỏi không được trống.", SweetAlertMessageType.warning);
            }
            else
            {
                if (await payrollService.UpdateSalaryQuestion(_payslipVM))
                {
                    if (type == 0)
                    {
                        _payslipVM.TypeUpdateSalaryQuestion = 1;

                        await js.Swal_Message("Thông báo!", "Gửi câu hỏi thành công.", SweetAlertMessageType.success);
                    }
                    else
                    {
                        if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn hủy?", SweetAlertMessageType.question))
                        {
                            _payslipVM.TypeUpdateSalaryQuestion = 0;
                            _payslipVM.SalaryQuestion = string.Empty;

                            await js.Swal_Message("Thông báo!", "Hủy thành công.", SweetAlertMessageType.success);
                        }
                    }
                }
            }
        }

    }
}
