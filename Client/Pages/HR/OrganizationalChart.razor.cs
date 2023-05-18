using Data.Repositories.HR;
using Data.Repositories.SYSTEM;
using Model.Entities.HR;
using Model.ViewModels.DOC;
using Model.ViewModels.HR;
using WebApp.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using DevExpress.XtraRichEdit.Model;
using Blazored.TextEditor;

namespace WebApp.Pages.HR
{
    partial class OrganizationalChart
    {
        [Inject] IJSRuntime js { get; set; }

        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        [Inject] SysRepository sysRepo { get; set; }
        [Inject] UserRepository userRepo { get; set; }

        [Inject] OrganizationalChartService organizationalChartRepo { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingPage;

        FilterHrVM filterHrVM = new();

        //Division
        DivisionVM divisionVM = new();
        IEnumerable<DivisionVM> divisionVMs;

        //Department
        DepartmentVM departmentVM = new();
        IEnumerable<DepartmentVM> departmentVMs;
        //DepartmentGroup
        DepartmentGroupVM departmentGroupVM = new();
        IEnumerable<DepartmentGroupVM> departmentGroupVMs;

        //Section
        SectionVM sectionVM = new();
        IEnumerable<SectionVM> sectionVMs;

        //Position
        PositionVM positionVM = new();
        List<PositionVM> positionVMs;
        //PositionGroup
        PositionGroupVM positionGroupVM = new();
        IEnumerable<PositionGroupVM> positionGroups;

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
            isLoadingPage = true;

            UserID = (await authenticationStateTask).User.GetUserId();

            if (sysRepo.checkPermisFunc(UserID, "HR_OrganizationalChart"))
            {
                await sysRepo.insert_LogUserFunc(UserID, "HR_OrganizationalChart");
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            filterHrVM.UserID = String.Empty;

            filterHrVM.searchValues = string.Empty;

            divisionVMs = await organizationalChartRepo.GetDivisionList(filterHrVM);

            departmentGroupVMs = await organizationalChartRepo.GetDepartmentGroupList();

            sectionVMs = await organizationalChartRepo.GetSectionList();

            positionVMs = await organizationalChartRepo.GetPositionList();
            positionGroups = await organizationalChartRepo.GetPositionGroupList();

            isLoadingPage = false;
        }

        //Division
        private async Task onclick_selectedDivision(DivisionVM _divisionVM)
        {
            isLoading = true;

            filterHrVM.DivisionID = filterHrVM.DivisionID == _divisionVM.DivisionID ? String.Empty : _divisionVM.DivisionID;

            divisionVM = _divisionVM;

            await LoadDepartmentList();

            isLoading = false;
        }

        private async Task InitializeModalUpdate_Division(int _isTypeUpdate)
        {
            isLoading = true;

            if (_isTypeUpdate == 0)
            {
                divisionVM = new();

                divisionVM.isActive = true;
                divisionVM.is2625 = 0;

                divisionVM.IsTypeUpdate = 0;

                filterHrVM.DivisionID = string.Empty;
            }

            divisionVM.IsTypeUpdate = _isTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Division");

            isLoading = false;
        }

        private async Task UpdateDivision()
        {
            isLoading = true;

            if (divisionVM.IsTypeUpdate != 2)
            {
                await organizationalChartRepo.UpdateDivision(divisionVM);
                divisionVM.IsTypeUpdate = 1;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await organizationalChartRepo.UpdateDivision(divisionVM);

                    if (affectedRows > 0)
                    {
                        await LoadDivisionList();

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

        private async Task LoadDivisionList()
        {
            isLoading = true;

            filterHrVM.DivisionID = String.Empty;
            divisionVMs = await organizationalChartRepo.GetDivisionList(filterHrVM);

            isLoading = false;
        }

        //Department
        private void onclick_selectedDepartment(DepartmentVM _departmentVM)
        {
            isLoading = true;

            filterHrVM.DepartmentID = filterHrVM.DepartmentID == _departmentVM.DepartmentID ? String.Empty : _departmentVM.DepartmentID;

            departmentVM = _departmentVM;

            isLoading = false;
        }

        private async Task InitializeModalUpdate_Department(int _isTypeUpdate)
        {
            isLoading = true;

            if (_isTypeUpdate == 0)
            {
                departmentVM = new();

                departmentVM.DivisionID = filterHrVM.DivisionID;
                departmentVM.isActive = true;
            }

            departmentVM.IsTypeUpdate = _isTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Department");

            isLoading = false;
        }

        private async Task UpdateDepartment()
        {
            isLoading = true;

            if (departmentVM.IsTypeUpdate != 2)
            {
                await organizationalChartRepo.UpdateDepartment(departmentVM);
                departmentVM.IsTypeUpdate = 1;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await organizationalChartRepo.UpdateDepartment(departmentVM);

                    if (affectedRows > 0)
                    {
                        await LoadDepartmentList();

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

        private async Task LoadDepartmentList()
        {
            isLoading = true;

            filterHrVM.DepartmentID = String.Empty;
            departmentVMs = await organizationalChartRepo.GetDepartmentList(filterHrVM);

            isLoading = false;
        }

        //DepartmentGroup
        private async Task InitializeModalUpdate_DepartmentGroup(int _isTypeUpdate)
        {
            isLoading = true;

            if (_isTypeUpdate == 0)
            {
                departmentGroupVM = new();
            }

            if (_isTypeUpdate == 1)
            {
                departmentGroupVM = departmentGroupVMs.First(x => x.DepartmentGroupID == departmentVM.DepartmentGroupID);
            }

            departmentGroupVM.IsTypeUpdate = _isTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_DepartmentGroup");

            isLoading = false;
        }
        private async Task UpdateDepartmentGroup()
        {
            isLoading = true;

            if (departmentGroupVM.IsTypeUpdate != 2)
            {
                await organizationalChartRepo.UpdateDepartmentGroup(departmentGroupVM);
                departmentGroupVM.IsTypeUpdate = 1;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await organizationalChartRepo.UpdateDepartmentGroup(departmentGroupVM);

                    if (affectedRows > 0)
                    {
                        departmentGroupVM.DepartmentGroupID = String.Empty;
                        await LoadDepartmentGroupList();

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
        private async Task LoadDepartmentGroupList()
        {
            isLoading = true;

            departmentGroupVMs = await organizationalChartRepo.GetDepartmentGroupList();

            isLoading = false;
        }

        //Section
        private void onclick_selectedSection(SectionVM _sectionVM)
        {
            isLoading = true;

            filterHrVM.SectionID = filterHrVM.SectionID == _sectionVM.SectionID ? String.Empty : _sectionVM.SectionID;

            sectionVM = _sectionVM;

            isLoading = false;
        }
        private async Task InitializeModalUpdate_Section(int _isTypeUpdate)
        {
            isLoading = true;

            if (_isTypeUpdate == 0)
            {
                sectionVM = new();
                sectionVM.isActive = true;
            }

            sectionVM.IsTypeUpdate = _isTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Section");

            isLoading = false;
        }
        private async Task UpdateSection()
        {
            isLoading = true;

            if (sectionVM.IsTypeUpdate != 2)
            {
                await organizationalChartRepo.UpdateSection(sectionVM);
                sectionVM.IsTypeUpdate = 1;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await organizationalChartRepo.UpdateSection(sectionVM);

                    if (affectedRows > 0)
                    {
                        sectionVM.SectionID = String.Empty;
                        await LoadSectionList();

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
        private async Task LoadSectionList()
        {
            isLoading = true;

            filterHrVM.SectionID = String.Empty;
            sectionVMs = await organizationalChartRepo.GetSectionList();

            isLoading = false;
        }

        //Position
        private void onclick_selectedPosition(PositionVM _positionVM)
        {
            isLoading = true;

            filterHrVM.PositionID = filterHrVM.PositionID == _positionVM.PositionID ? String.Empty : _positionVM.PositionID;

            positionVM = _positionVM;

            isLoading = false;
        }

        private async Task InitializeModalUpdate_Position(int _isTypeUpdate)
        {
            isLoading = true;

            if (_isTypeUpdate == 0)
            {
                positionVM = new();

                positionVM.isActive = true;
            }

            if (_isTypeUpdate == 1)
            {
                if (!String.IsNullOrEmpty(positionVM.JobDesc))
                {
                    await QuillHtml.LoadHTMLContent(positionVM.JobDesc);
                }
            }

            positionVM.IsTypeUpdate = _isTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Position");

            isLoading = false;
        }

        private async Task UpdatePosition()
        {
            isLoading = true;

            if (positionVM.IsTypeUpdate != 2)
            {
                positionVM.JobDesc = await QuillHtml.GetHTML();

                await organizationalChartRepo.UpdatePosition(positionVM);
                positionVM.IsTypeUpdate = 1;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await organizationalChartRepo.UpdatePosition(positionVM);

                    if (affectedRows > 0)
                    {
                        await LoadPositionList();

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

            filterHrVM.searchValues = args.Value.ToString();

            isLoading = false;
        }

        private async Task LoadPositionList()
        {
            isLoading = true;

            filterHrVM.PositionID = String.Empty;
            positionVMs = await organizationalChartRepo.GetPositionList();

            isLoading = false;
        }

        //PositionGroup
        private async Task InitializeModalUpdate_PositionGroup(int _isTypeUpdate)
        {
            isLoading = true;

            if (_isTypeUpdate == 0)
            {
                positionGroupVM = new();
            }

            if (_isTypeUpdate == 1)
            {
                positionGroupVM = positionGroups.First(x => x.PositionGroupID == positionVM.PositionGroupID);
            }

            positionGroupVM.IsTypeUpdate = _isTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_PositionGroup");

            isLoading = false;
        }
        private async Task UpdatePositionGroup()
        {
            isLoading = true;

            if (positionGroupVM.IsTypeUpdate != 2)
            {
                await organizationalChartRepo.UpdatePositionGroup(positionGroupVM);
                positionGroupVM.IsTypeUpdate = 1;

                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await organizationalChartRepo.UpdatePositionGroup(positionGroupVM);

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

            positionGroups = await organizationalChartRepo.GetPositionGroupList();

            isLoading = false;
        }

    }
}
