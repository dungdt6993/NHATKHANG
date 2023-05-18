using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Services.HR;
using D69soft.Client.Services.KPI;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.KPI;
using D69soft.Client.Helpers;

namespace D69soft.Client.Pages.KPI
{
    partial class KPIs
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] KPIService kpiService { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingScreen = true;

        //Filter
        FilterHrVM filterHrVM = new();
        IEnumerable<PeriodVM> year_filter_list;
        IEnumerable<PeriodVM> month_filter_list;
        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;
        IEnumerable<PositionVM> position_filter_list;
        IEnumerable<ProfileVM> eserial_filter_list;

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
            

            UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(UserID, "KPI_KPIs"))
            {
                await sysService.InsertLogUserFunc(UserID, "KPI_KPIs");
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            filterHrVM.UserID = UserID;

            month_filter_list = await kpiService.GetMonthFilter(filterHrVM);
            filterHrVM.Month = month_filter_list.OrderByDescending(x => x.Month).ToList().ElementAt(0).Month;

            year_filter_list = await kpiService.GetYearFilter(filterHrVM);
            filterHrVM.Year = year_filter_list.OrderByDescending(x => x.Year).ToList().ElementAt(0).Year;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            division_filter_list = await kpiService.GetDivisions(filterHrVM);
            filterHrVM.DivisionID = division_filter_list.Count() > 0 ? division_filter_list.ElementAt(0).DivisionID : string.Empty;

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await kpiService.GetDepartments(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartService.GetPositionList();

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await kpiService.GetEserials(filterHrVM);

            await GetKPIs();

            isLoadingScreen = false;
        }

        private async void onchange_filter_month(int value)
        {
            isLoading = true;

            filterHrVM.Month = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await kpiService.GetEserials(filterHrVM);

            await GetKPIs();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_year(int value)
        {
            isLoading = true;

            filterHrVM.Year = value;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            month_filter_list = await kpiService.GetMonthFilter(filterHrVM);
            filterHrVM.Month = month_filter_list.OrderByDescending(x => x.Month).ToList().ElementAt(0).Month;

            filterHrVM.Period = filterHrVM.Year * 100 + filterHrVM.Month;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await kpiService.GetEserials(filterHrVM);

            await GetKPIs();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterHrVM.DivisionID = value;

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await kpiService.GetDepartments(filterHrVM);

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await kpiService.GetEserials(filterHrVM);

            await GetKPIs();

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
            eserial_filter_list = await kpiService.GetEserials(filterHrVM);

            await GetKPIs();

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

                kPIVMs = null;

                isLoading = false;
            }
        }

        private async void reload_filter_eserial()
        {
            isLoading = true;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await kpiService.GetEserials(filterHrVM);

            await GetKPIs();

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_eserial(string value)
        {
            isLoading = true;

            filterHrVM.Eserial = value;

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

            if (!String.IsNullOrEmpty(filterHrVM.Eserial))
            {
                kPIVMs = await kpiService.GetKPIs(filterHrVM);
                rankVM = await kpiService.GetRank(filterHrVM);
            }
            else
            {
                rankVMs = await kpiService.GetRanks(filterHrVM);
            }

            isLoading = false;
        }

        private async void onchange_StaffScore(ChangeEventArgs e, KPIVM _kpiVM)
        {
            _kpiVM.StaffScore = float.Parse(e.Value.ToString());
            await kpiService.UpdateStaffScore(_kpiVM);

            rankVM = await kpiService.GetRank(filterHrVM);

            await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);

            StateHasChanged();
        }

        private async void onchange_FinalScore(ChangeEventArgs e, KPIVM _kpiVM)
        {
            _kpiVM.FinalScore = float.Parse(e.Value.ToString());
            await kpiService.UpdateFinalScore(_kpiVM);

            rankVM = await kpiService.GetRank(filterHrVM);

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
                await kpiService.InitializeKPI(filterHrVM, _Eserial);

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

            if(type == "SendKPI")
            {
                str = value ? "Gửi duyệt" : "Hủy gửi duyệt";
            }

            if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn "+ str + "?", SweetAlertMessageType.question))
            {

                if (type== "SendKPI")
                {
                    rankVM.isSendKPI = value;
                    rankVM.TimeSendKPI = DateTime.Now;
                }

                if (type == "SendAppraiser")
                {
                    if (kPIVMs.Where(x => x.FinalScore != x.Score && String.IsNullOrEmpty(x.ManagerComment)).Count() > 0)
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
                    if (kPIVMs.Where(x => x.FinalScore != x.Score && String.IsNullOrEmpty(x.ManagerComment)).Count() > 0)
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
                    if (kPIVMs.Where(x => x.FinalScore != x.Score && String.IsNullOrEmpty(x.ManagerComment)).Count() > 0)
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

                await js.Toast_Alert("" + str + " thành công!", SweetAlertMessageType.success);
            }

            isLoading = false;
        }

    }
}
