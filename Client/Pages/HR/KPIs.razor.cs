using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;

namespace D69soft.Client.Pages.HR
{
    partial class KPIs
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] KPIService kpiService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        //PermisFunc
        bool KPI_KPIs_Management;

        IEnumerable<PeriodVM> year_filter_list;
        IEnumerable<PeriodVM> month_filter_list;
        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;
        IEnumerable<PositionVM> position_filter_list;
        IEnumerable<EserialVM> eserial_filter_list;

        //KPIs
        KPIVM kPIVM = new();
        IEnumerable<KPIVM> kPIVMs;

        //Rank
        RankVM rankVM = new();
        List<RankVM> rankVMs;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");
        }

        protected override async Task OnInitializedAsync()
        {
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(filterVM.UserID, "KPI_KPIs"))
            {
                logVM.LogUser = filterVM.UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "KPI_KPIs";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            KPI_KPIs_Management = await sysService.CheckAccessSubFunc(filterVM.UserID, "KPI_KPIs_Management");

            //Initialize Filter
            month_filter_list = await kpiService.GetMonthFilter(filterVM);
            filterVM.Month = month_filter_list.OrderByDescending(x => x.Month).ToList().ElementAt(0).Month;

            year_filter_list = await kpiService.GetYearFilter(filterVM);
            filterVM.Year = year_filter_list.OrderByDescending(x => x.Year).ToList().ElementAt(0).Year;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            division_filter_list = await kpiService.GetDivisions(filterVM);
            filterVM.DivisionID = division_filter_list.Count() > 0 ? division_filter_list.ElementAt(0).DivisionID : string.Empty;

            filterVM.DepartmentID = string.Empty;
            department_filter_list = await kpiService.GetDepartments(filterVM);

            filterVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartService.GetPositionList();

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await kpiService.GetEserials(filterVM);

            await GetKPIs();

            isLoadingScreen = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterVM.Month = value;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await kpiService.GetEserials(filterVM);

            await GetKPIs();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            filterVM.Year = value;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            month_filter_list = await kpiService.GetMonthFilter(filterVM);
            filterVM.Month = month_filter_list.OrderByDescending(x => x.Month).ToList().ElementAt(0).Month;

            filterVM.Period = filterVM.Year * 100 + filterVM.Month;

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await kpiService.GetEserials(filterVM);

            await GetKPIs();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterVM.DivisionID = value;

            filterVM.DepartmentID = string.Empty;
            department_filter_list = await kpiService.GetDepartments(filterVM);

            filterVM.PositionGroupID = string.Empty;
            filterVM.arrPositionID = new string[] { };

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await kpiService.GetEserials(filterVM);

            await GetKPIs();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_department(string value)
        {
            isLoading = true;

            filterVM.DepartmentID = value;

            filterVM.PositionGroupID = string.Empty;
            filterVM.arrPositionID = new string[] { };

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await kpiService.GetEserials(filterVM);

            await GetKPIs();

            isLoading = false;

            StateHasChanged();
        }

        private string[] onchange_filter_positiongroup
        {
            get
            {
                return filterVM.arrPositionID;
            }
            set
            {
                isLoading = true;

                filterVM.arrPositionID = value;

                filterVM.PositionGroupID = string.Join(",", value);

                reload_filter_eserial();

                kPIVMs = null;

                isLoading = false;
            }
        }

        private async void reload_filter_eserial()
        {
            isLoading = true;

            filterVM.Eserial = string.Empty;
            eserial_filter_list = await kpiService.GetEserials(filterVM);

            await GetKPIs();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_eserial(string value)
        {
            isLoading = true;

            filterVM.Eserial = value;

            await GetKPIs();

            isLoading = false;

            StateHasChanged();
        }

        private async Task GetKPIs()
        {
            isLoading = true;

            kPIVMs = null;
            rankVM = new();
            rankVMs = null;

            if (!string.IsNullOrEmpty(filterVM.Eserial))
            {
                kPIVMs = await kpiService.GetKPIs(filterVM);
                rankVM = await kpiService.GetRank(filterVM);
            }
            else
            {
                rankVMs = await kpiService.GetRanks(filterVM);
            }

            isLoading = false;
        }

        private async void onchange_StaffScore(ChangeEventArgs e, KPIVM _kpiVM)
        {
            _kpiVM.StaffScore = float.Parse(e.Value.ToString());
            await kpiService.UpdateStaffScore(_kpiVM);

            rankVM = await kpiService.GetRank(filterVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            StateHasChanged();
        }

        private async void onchange_FinalScore(ChangeEventArgs e, KPIVM _kpiVM)
        {
            _kpiVM.FinalScore = float.Parse(e.Value.ToString());
            await kpiService.UpdateFinalScore(_kpiVM);

            rankVM = await kpiService.GetRank(filterVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            StateHasChanged();
        }

        private async void onchange_ActualDescription(ChangeEventArgs e, KPIVM _kpiVM)
        {
            _kpiVM.ActualDescription = e.Value.ToString();
            await kpiService.UpdateActualDescription(_kpiVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            StateHasChanged();
        }

        private async void onchange_ManagerComment(ChangeEventArgs e, KPIVM _kpiVM)
        {
            _kpiVM.ManagerComment = e.Value.ToString();
            await kpiService.UpdateManagerComment(_kpiVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            StateHasChanged();
        }

        private async void onchange_StaffNote(ChangeEventArgs e, RankVM _rankVM)
        {
            _rankVM.StaffNote = e.Value.ToString();
            await kpiService.UpdateStaffNote(_rankVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            StateHasChanged();
        }

        private async void onchange_ManagerNote(ChangeEventArgs e, RankVM _rankVM)
        {
            _rankVM.ManagerNote = e.Value.ToString();
            await kpiService.UpdateManagerNote(_rankVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            StateHasChanged();
        }

        private async void onchange_Target(ChangeEventArgs e, KPIVM _kpiVM)
        {
            _kpiVM.DescriptionName = e.Value.ToString();
            await kpiService.UpdateTarget(_kpiVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            StateHasChanged();
        }

        private async Task InitializeKPI(string _Eserial)
        {
            isLoading = true;

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn cập nhật lại Form KPI?", SweetAlertMessageType.question))
            {
                await kpiService.InitializeKPI(filterVM, _Eserial);

                kPIVMs = null;
                rankVM = new();

                StateHasChanged();

                await GetKPIs();

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }

            isLoading = false;
        }

        private async Task SendApproveKPI(bool value, string type, RankVM _rankVM)
        {
            rankVM = _rankVM;
            await SendKPI(value, type);
        }

        private async Task SendKPI(bool value, string type)
        {
            isLoading = true;

            var str = value ? "Duyệt" : "Hủy duyệt";

            if (type == "SendKPI")
            {
                str = value ? "Gửi duyệt" : "Hủy gửi duyệt";
            }

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn " + str + "?", SweetAlertMessageType.question))
            {

                if (type == "SendKPI")
                {
                    rankVM.isSendKPI = value;
                    rankVM.TimeSendKPI = DateTime.Now;
                }

                if (type == "SendAppraiser")
                {
                    if (kPIVMs.Where(x => x.FinalScore != x.Score && string.IsNullOrEmpty(x.ManagerComment)).Count() > 0)
                    {
                        await js.Toast_Alert("Khi tăng/giảm điểm đánh giá, ý kiến người đánh giá không được trống!", SweetAlertMessageType.error);

                        isLoading = false;

                        return;
                    }

                    rankVM.isSendAppraiser = value;
                    rankVM.TimeSendAppraiser = DateTime.Now;
                }

                if (type == "SendDirectManager")
                {
                    if (kPIVMs.Where(x => x.FinalScore != x.Score && string.IsNullOrEmpty(x.ManagerComment)).Count() > 0)
                    {
                        await js.Toast_Alert("Khi tăng/giảm điểm đánh giá, ý kiến người đánh giá không được trống!", SweetAlertMessageType.error);

                        isLoading = false;

                        return;
                    }

                    rankVM.isSendDirectManager = value;
                    rankVM.TimeSendDirectManager = DateTime.Now;
                }

                if (type == "SendControlDept")
                {
                    if (kPIVMs.Where(x => x.FinalScore != x.Score && string.IsNullOrEmpty(x.ManagerComment)).Count() > 0)
                    {
                        await js.Toast_Alert("Khi tăng/giảm điểm đánh giá, ý kiến người đánh giá không được trống!", SweetAlertMessageType.error);

                        isLoading = false;

                        return;
                    }

                    rankVM.isSendControlDept = value;
                    rankVM.TimeSendControlDept = DateTime.Now;
                }

                if (type == "SendApprove")
                {
                    rankVM.isSendApprove = value;
                    rankVM.TimeSendApprove = DateTime.Now;
                }

                await kpiService.SendKPI(rankVM, type);

                logVM.LogDesc = "" + str + " KPI " + rankVM.Eserial;
                await sysService.InsertLog(logVM);

                await js.Swal_Message("Thông báo!", logVM.LogDesc, SweetAlertMessageType.success);
            }

            isLoading = false;
        }

    }
}
