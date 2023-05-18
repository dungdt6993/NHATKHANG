using BlazorDateRangePicker;
using Blazored.TextEditor;
using Data.Repositories.HR;
using Data.Repositories.SYSTEM;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Model.ViewModels.HR;
using WebApp.Helpers;
using Model.Entities.HR;
using Model.ViewModels.DOC;
using Utilities;
using Model.ViewModels.KPI;

namespace WebApp.Pages.HR
{
    partial class WorkPlan
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        [Inject] SysRepository sysRepo { get; set; }
        [Inject] OrganizationalChartService organizationalChartRepo { get; set; }

        [Inject] DutyRosterService dutyRosterRepo { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingPage;

        //Filter
        FilterHrVM filterHrVM = new();

        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<PositionVM> position_filter_list;

        //WorkPlan
        DutyRosterVM workPlanVM = new();
        IEnumerable<DutyRosterVM> workPlanVMs;

        BlazoredTextEditor QuillHtml = new BlazoredTextEditor();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");

            await js.InvokeAsync<object>("maskDate");

            if (!String.IsNullOrEmpty(workPlanVM.WorkPlanDesc))
            {
                await QuillHtml.LoadHTMLContent(workPlanVM.WorkPlanDesc);
            }

        }

        protected override async Task OnInitializedAsync()
        {
            isLoadingPage = true;

            UserID = (await authenticationStateTask).User.GetUserId();

            if (sysRepo.checkPermisFunc(UserID, "HR_WorkPlan"))
            {
                await sysRepo.insert_LogUserFunc(UserID, "HR_WorkPlan");
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            filterHrVM.UserID = workPlanVM.UserID = UserID;

            filterHrVM.dDate = DateTime.Now;

            division_filter_list = await organizationalChartRepo.GetDivisionList(filterHrVM);
            filterHrVM.DivisionID = division_filter_list.Count() > 0 ? division_filter_list.ElementAt(0).DivisionID : string.Empty;

            filterHrVM.PositionGroupID = string.Empty;
            position_filter_list = await organizationalChartRepo.GetPositionList();

            await GetWorkPlans();

            isLoadingPage = false;
        }

        public async Task OnRangeSelect_dDate(DateRange _range)
        {
            filterHrVM.dDate = _range.Start;

            await GetWorkPlans();

        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterHrVM.DivisionID = value;

            filterHrVM.PositionGroupID = string.Empty;
            filterHrVM.arrPositionID = new string[] { };

            position_filter_list = await organizationalChartRepo.GetPositionList();

            await GetWorkPlans();

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

                isLoading = false;
            }
        }

        private async Task GetWorkPlans()
        {
            isLoading = true;

            workPlanVMs = await dutyRosterRepo.GetWorkPlans(filterHrVM);

            isLoading = false;
        }

        private async Task InitializeModalUpdate_WorkPlan(int _isTypeUpdate, string _PositionGroupID, DutyRosterVM _workPlanVM)
        {
            isLoading = true;

            if (_isTypeUpdate == 0)
            {
                workPlanVM = new();

                workPlanVM.PositionGroupID = _PositionGroupID;
                workPlanVM.WorkPlanStartDate = DateTime.Now.Date;

                workPlanVM.UserCreated = UserID;
            }

            if (_isTypeUpdate == 1)
            {
                workPlanVM = _workPlanVM;
            }

            workPlanVM.IsTypeUpdate = _isTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_WorkPlan");

            isLoading = false;
        }

        public string onchange_WorkPlanStartDate
        {
            get
            {
                return workPlanVM.WorkPlanStartDate.HasValue ? workPlanVM.WorkPlanStartDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                workPlanVM.WorkPlanStartDate = LibraryFunc.FormatDateDDMMYYYY(value, workPlanVM.WorkPlanStartDate);
            }
        }

        public string onchange_WorkPlanDeadline
        {
            get
            {
                return workPlanVM.WorkPlanDeadline.HasValue ? workPlanVM.WorkPlanDeadline.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                workPlanVM.WorkPlanDeadline = LibraryFunc.FormatDateDDMMYYYY(value, workPlanVM.WorkPlanDeadline);
            }
        }

        private async Task UpdateWorkPlan()
        {
            isLoading = true;

            if (workPlanVM.IsTypeUpdate != 2)
            {
                workPlanVM.WorkPlanDesc = await QuillHtml.GetHTML();

                await dutyRosterRepo.UpdateWorkPlan(workPlanVM);
                workPlanVM.IsTypeUpdate = 1;

                await GetWorkPlans();

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_WorkPlan");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await dutyRosterRepo.UpdateWorkPlan(workPlanVM);

                    if (affectedRows > 0)
                    {
                        await GetWorkPlans();

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_WorkPlan");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        workPlanVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    workPlanVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

        private async void onchange_WorkPlanIsDone(ChangeEventArgs e, DutyRosterVM _workPlanVM)
        {
            isLoading = true;

            if ((bool)e.Value)
            {
                _workPlanVM.WorkPlanIsDone = true;

                await dutyRosterRepo.UpdateWorkPlanIsDone(_workPlanVM);

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có muốn bỏ hoàn thành công việc này?", SweetAlertMessageType.question))
                {
                    _workPlanVM.WorkPlanIsDone = false;

                    await dutyRosterRepo.UpdateWorkPlanIsDone(_workPlanVM);

                    await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
                }
            }

            await GetWorkPlans();

            isLoading = false;

            StateHasChanged();
        }

    }
}
