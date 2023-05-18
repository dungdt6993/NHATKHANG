using Data.Repositories.SYSTEM;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Data.Repositories.HR;
using Model.ViewModels.HR;
using Model.ViewModels.SYSTEM;
using WebApp.Helpers;

namespace WebApp.Pages.SYS
{
    partial class PersonalProfile
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysRepository sysRepo { get; set; }

        [Inject] UserRepository userRepo { get; set; }
        [Inject] PayrollService payrollRepo { get; set; }

        protected string UserID;

        bool isLoadingPage;

        UserSysLogVM userSysLogVM = new UserSysLogVM();

        UserVM userInfo = new UserVM();

        IEnumerable<PayslipVM> payslipUsers;

        //Filter
        FilterHrVM filterHrVM = new();

        //Monthly Income
        List<MonthlyIncomeTrnOtherVM> monthlyIncomeTrnOtherVMs;

        protected override async Task OnInitializedAsync()
        {
            isLoadingPage = true;

            UserID = (await authenticationStateTask).User.GetUserId();

            userSysLogVM.UserSysLog = UserID;
            userSysLogVM.UrlFuncLog = "/HR/Profile";
            userSysLogVM.FuncPermisID = "HR_Profile";
            userSysLogVM.isShowNotifi = 1;

            userInfo = await userRepo.GetInfoUser(UserID);

            payslipUsers = await userRepo.GetPayslipUser(UserID);

            isLoadingPage = false;
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

            monthlyIncomeTrnOtherVMs = await payrollRepo.GetMonthlyIncomeTrnOtherList(filterHrVM);

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
                if (await payrollRepo.UpdateSalaryQuestion(_payslipVM))
                {
                    if (type == 0)
                    {
                        _payslipVM.TypeUpdateSalaryQuestion = 1;

                        userSysLogVM.DescUserSysLog = "Mã NV " + UserID + " gửi thắc mắc lương!";

                        await sysRepo.insert_UserSysLog(userSysLogVM);
                        await sysRepo.notifi_UserSysLogByPermis(userSysLogVM);

                        await js.Swal_Message("Thông báo!", "Gửi câu hỏi thành công.", SweetAlertMessageType.success);
                    }
                    else
                    {
                        if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn hủy?", SweetAlertMessageType.question))
                        {
                            _payslipVM.TypeUpdateSalaryQuestion = 0;
                            _payslipVM.SalaryQuestion = string.Empty;

                            userSysLogVM.DescUserSysLog = "Mã NV " + UserID + " hủy gửi thắc mắc lương!";

                            await sysRepo.insert_UserSysLog(userSysLogVM);
                            await sysRepo.notifi_UserSysLogByPermis(userSysLogVM);

                            await js.Swal_Message("Thông báo!", "Hủy thành công.", SweetAlertMessageType.success);
                        }
                    }
                }
            }
        }

    }
}
