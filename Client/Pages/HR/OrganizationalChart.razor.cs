using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Blazored.TextEditor;
using D69soft.Client.Services;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using D69soft.Shared.Models.Entities.HR;
using D69soft.Client.Services.FIN;
using D69soft.Shared.Models.ViewModels.FIN;

namespace D69soft.Client.Pages.HR
{
    partial class OrganizationalChart
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] DutyRosterService dutyRosterService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        //Division
        DivisionVM divisionVM = new();
        IEnumerable<DivisionVM> divisionVMs;

        //Department
        DepartmentVM departmentVM = new();
        IEnumerable<DepartmentVM> departmentVMs;
        DepartmentGroupVM departmentGroupVM = new();
        IEnumerable<DepartmentGroupVM> departmentGroupVMs;

        //Section
        SectionVM sectionVM = new();
        IEnumerable<SectionVM> sectionVMs;

        //Position
        PositionVM positionVM = new();
        List<PositionVM> positionVMs;
        PositionGroupVM positionGroupVM = new();
        IEnumerable<PositionGroupVM> positionGroups;

        //Shift
        IEnumerable<ShiftVM> shiftVMs;

        BlazoredTextEditor QuillHtml = new BlazoredTextEditor();

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

            if (await sysService.CheckAccessFunc(filterVM.UserID, "HR_OrganizationalChart"))
            {
                logVM.LogUser = filterVM.UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "HR_OrganizationalChart";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }



            filterVM.searchValues = string.Empty;

            filterVM.UserID = String.Empty;
            divisionVMs = await organizationalChartService.GetDivisionList(filterVM);

            departmentGroupVMs = await organizationalChartService.GetDepartmentGroupList();

            sectionVMs = await organizationalChartService.GetSectionList();

            positionVMs = await organizationalChartService.GetPositionList();
            positionGroups = await organizationalChartService.GetPositionGroupList();

            shiftVMs = await dutyRosterService.GetShiftList();

            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            isLoadingScreen = false;
        }

        //Division
        private async Task onclick_selectedDivision(DivisionVM _divisionVM)
        {
            isLoading = true;

            filterVM.DivisionID = filterVM.DivisionID == _divisionVM.DivisionID ? String.Empty : _divisionVM.DivisionID;

            divisionVM = _divisionVM;

            await GetDepartmentList();

            isLoading = false;
        }

        private async Task InitializeModalUpdate_Division(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                divisionVM = new();

                divisionVM.isActive = true;
                divisionVM.is2625 = 0;

                divisionVM.IsTypeUpdate = 0;

                filterVM.DivisionID = string.Empty;
            }

            divisionVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Division");

            isLoading = false;
        }

        private async Task UpdateDivision(EditContext _formDivisionVM, int _IsTypeUpdate)
        {
            divisionVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formDivisionVM.Validate()) return;

            isLoading = true;

            if (divisionVM.IsTypeUpdate != 2)
            {
                await organizationalChartService.UpdateDivision(divisionVM);
                divisionVM.IsTypeUpdate = 1;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await organizationalChartService.UpdateDivision(divisionVM);

                    if (affectedRows > 0)
                    {
                        await GetDivisionList();

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Division");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        divisionVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    divisionVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

        private async Task GetDivisionList()
        {
            isLoading = true;

            filterVM.DivisionID = String.Empty;
            divisionVMs = await organizationalChartService.GetDivisionList(filterVM);

            isLoading = false;
        }

        //Department
        private void onclick_selectedDepartment(DepartmentVM _departmentVM)
        {
            isLoading = true;

            filterVM.DepartmentID = filterVM.DepartmentID == _departmentVM.DepartmentID ? String.Empty : _departmentVM.DepartmentID;

            departmentVM = _departmentVM;

            isLoading = false;
        }

        private async Task InitializeModalUpdate_Department(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                departmentVM = new();

                departmentVM.DivisionID = filterVM.DivisionID;
                departmentVM.isActive = true;
            }

            departmentVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Department");

            isLoading = false;
        }

        private async Task UpdateDepartment(EditContext _formDepartmentVM, int _IsTypeUpdate)
        {
            departmentVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formDepartmentVM.Validate()) return;

            isLoading = true;

            if (departmentVM.IsTypeUpdate != 2)
            {
                await organizationalChartService.UpdateDepartment(departmentVM);
                departmentVM.IsTypeUpdate = 1;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await organizationalChartService.UpdateDepartment(departmentVM);

                    if (affectedRows > 0)
                    {
                        await GetDepartmentList();

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Department");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        departmentVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    departmentVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

        private async Task GetDepartmentList()
        {
            isLoading = true;

            filterVM.DepartmentID = String.Empty;
            departmentVMs = await organizationalChartService.GetDepartmentList(filterVM);

            isLoading = false;
        }

        //DepartmentGroup
        private async Task InitializeModalUpdate_DepartmentGroup(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                departmentGroupVM = new();
            }

            if (_IsTypeUpdate == 1)
            {
                departmentGroupVM = departmentGroupVMs.First(x => x.DepartmentGroupID == departmentVM.DepartmentGroupID);
            }

            departmentGroupVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_DepartmentGroup");

            isLoading = false;
        }
        private async Task UpdateDepartmentGroup(EditContext _formDepartmentGroupVM, int _IsTypeUpdate)
        {
            departmentGroupVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formDepartmentGroupVM.Validate()) return;

            isLoading = true;

            if (departmentGroupVM.IsTypeUpdate != 2)
            {
                await organizationalChartService.UpdateDepartmentGroup(departmentGroupVM);
                departmentGroupVM.IsTypeUpdate = 1;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await organizationalChartService.UpdateDepartmentGroup(departmentGroupVM);

                    if (affectedRows > 0)
                    {
                        departmentGroupVM.DepartmentGroupID = String.Empty;
                        await GetDepartmentGroupList();

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_DepartmentGroup");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        departmentGroupVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    departmentGroupVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }
        private async Task GetDepartmentGroupList()
        {
            isLoading = true;

            departmentGroupVMs = await organizationalChartService.GetDepartmentGroupList();

            isLoading = false;
        }

        //Section
        private void onclick_selectedSection(SectionVM _sectionVM)
        {
            isLoading = true;

            filterVM.SectionID = filterVM.SectionID == _sectionVM.SectionID ? String.Empty : _sectionVM.SectionID;

            sectionVM = _sectionVM;

            isLoading = false;
        }
        private async Task InitializeModalUpdate_Section(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                sectionVM = new();
                sectionVM.isActive = true;
            }

            sectionVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Section");

            isLoading = false;
        }

        private async Task UpdateSection(EditContext _formSectionVM, int _IsTypeUpdate)
        {
            sectionVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formSectionVM.Validate()) return;

            isLoading = true;

            if (sectionVM.IsTypeUpdate != 2)
            {
                await organizationalChartService.UpdateSection(sectionVM);
                sectionVM.IsTypeUpdate = 1;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await organizationalChartService.UpdateSection(sectionVM);

                    if (affectedRows > 0)
                    {
                        sectionVM.SectionID = String.Empty;
                        await GetSectionList();

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Section");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        sectionVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    sectionVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }
        private async Task GetSectionList()
        {
            isLoading = true;

            filterVM.SectionID = String.Empty;
            sectionVMs = await organizationalChartService.GetSectionList();

            isLoading = false;
        }

        //Position
        private void onclick_selectedPosition(PositionVM _positionVM)
        {
            isLoading = true;

            filterVM.PositionID = filterVM.PositionID == _positionVM.PositionID ? String.Empty : _positionVM.PositionID;

            positionVM = _positionVM;

            isLoading = false;
        }

        private async Task InitializeModalUpdate_Position(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                positionVM = new();

                positionVM.isActive = true;
            }

            if (_IsTypeUpdate == 1)
            {
                if (!String.IsNullOrEmpty(positionVM.JobDesc))
                {
                    await QuillHtml.LoadHTMLContent(positionVM.JobDesc);
                }
            }

            positionVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Position");

            isLoading = false;
        }

        private async Task UpdatePosition(EditContext _formPositionVM, int _IsTypeUpdate)
        {
            positionVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formPositionVM.Validate()) return;

            isLoading = true;

            if (positionVM.IsTypeUpdate != 2)
            {
                positionVM.JobDesc = await QuillHtml.GetHTML();

                await organizationalChartService.UpdatePosition(positionVM);
                positionVM.IsTypeUpdate = 1;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await organizationalChartService.UpdatePosition(positionVM);

                    if (affectedRows > 0)
                    {
                        await GetPositionList();

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Position");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        positionVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    positionVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

        protected void FilterPosition(ChangeEventArgs args)
        {
            isLoading = true;

            filterVM.searchValues = args.Value.ToString();

            isLoading = false;
        }

        private async Task GetPositionList()
        {
            isLoading = true;

            filterVM.PositionID = String.Empty;
            positionVMs = await organizationalChartService.GetPositionList();

            isLoading = false;
        }

        //PositionGroup
        private async Task InitializeModalUpdate_PositionGroup(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                positionGroupVM = new();
            }

            if (_IsTypeUpdate == 1)
            {
                positionGroupVM = positionGroups.First(x => x.PositionGroupID == positionVM.PositionGroupID);
            }

            positionGroupVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_PositionGroup");

            isLoading = false;
        }
        private async Task UpdatePositionGroup(EditContext _formPositionGroupVM, int _IsTypeUpdate)
        {
            positionGroupVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formPositionGroupVM.Validate()) return;

            isLoading = true;

            if (positionGroupVM.IsTypeUpdate != 2)
            {
                await organizationalChartService.UpdatePositionGroup(positionGroupVM);
                positionGroupVM.IsTypeUpdate = 1;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await organizationalChartService.UpdatePositionGroup(positionGroupVM);

                    if (affectedRows > 0)
                    {
                        positionGroupVM.PositionGroupID = String.Empty;
                        await LoadPositionGroupList();

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_PositionGroup");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        positionGroupVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    positionGroupVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }
        private async Task LoadPositionGroupList()
        {
            isLoading = true;

            positionGroups = await organizationalChartService.GetPositionGroupList();

            isLoading = false;
        }

    }
}
