using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Client.Services;
using D69soft.Shared.Utilities;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Extensions;

namespace D69soft.Client.Pages.HR
{
    partial class DOCBoat
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject] SysService sysService { get; set; }
        [Inject] OrganizationalChartService organizationalChartService { get; set; }
        [Inject] DocumentService documentService { get; set; }

        bool isLoading;
        bool isLoadingScreen = true;

        //Log
        LogVM logVM = new();

        //Filter
        FilterVM filterVM = new();

        //PermisFunc
        bool DOC_DOCBoat_Update;

        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;

        DocumentTypeVM doctypeVM = new();
        IEnumerable<DocumentTypeVM> doctype_filter_list;

        DocumentVM documentVM = new();
        List<DocumentVM> documentVMs;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("bootrap_select");
            }
            await js.InvokeAsync<object>("bootrap_select_refresh");

            await js.InvokeAsync<object>("maskDate");
        }

        protected override async Task OnInitializedAsync()
        {
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            if (await sysService.CheckAccessFunc(filterVM.UserID, "DOC_DOCBoat"))
            {
                logVM.LogUser = filterVM.UserID;
                logVM.LogType = "FUNC";
                logVM.LogName = "DOC_DOCBoat";
                await sysService.InsertLog(logVM);
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            DOC_DOCBoat_Update = await sysService.CheckAccessSubFunc(filterVM.UserID, "DOC_DOCBoat_Update");

            //Initialize Filter
            filterVM.GroupType = "DocBoat";

            division_filter_list = await organizationalChartService.GetDivisionList(filterVM);
            filterVM.DivisionID = (await sysService.GetInfoUser(filterVM.UserID)).DivisionID;

            filterVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterVM);

            doctype_filter_list = await documentService.GetDocTypes(filterVM);

            isLoadingScreen = false;
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterVM.DivisionID = value;

            filterVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartService.GetDepartmentList(filterVM);

            documentVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private void onchange_filter_department(string value)
        {
            isLoading = true;

            filterVM.DepartmentID = value;

            documentVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_doctype(int value)
        {
            isLoading = true;

            filterVM.DocTypeID = value;

            documentVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async Task onchange_filter_type(ChangeEventArgs args)
        {
            isLoading = true;

            filterVM.IsTypeSearch = int.Parse(args.Value.ToString());

            documentVMs = null;

            isLoading = false;
        }

        protected async Task GetDocBoatList()
        {
            isLoading = true;

            memoryStream = null;

            documentVMs = await documentService.GetDocs(filterVM);

            isLoading = false;
        }

        //Update Document
        private async Task InitializeModalUpdate_Document(int _IsTypeUpdate, DocumentVM _documentVM)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                documentVM = new();

                documentVM.DivisionID = filterVM.DivisionID;
                documentVM.GroupType = "DOCBoat";
                documentVM.IsDelFileScan = true;
            }

            if (_IsTypeUpdate == 1)
            {
                documentVM = _documentVM;
            }

            documentVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Document");

            isLoading = false;
        }

        private async Task UpdateDocument(EditContext _formDocumentVM ,int _IsTypeUpdate)
        {
            documentVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formDocumentVM.Validate()) return;

            isLoading = true;

            if (documentVM.IsTypeUpdate != 2)
            {
                await documentService.UpdateDocument(documentVM);

                logVM.LogDesc = "Cập nhật giấy tờ tàu thành công!";
                await sysService.InsertLog(logVM);

                await GetDocBoatList();

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Document");
                await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    documentVM.IsDelFileScan = true;

                    await documentService.UpdateDocument(documentVM);

                    logVM.LogDesc = "Xoá giấy tờ tàu thành công!";
                    await sysService.InsertLog(logVM);

                    await GetDocBoatList();

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Document");
                    await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                }
                else
                {
                    documentVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

        //Upload File
        private long maxFileSize = 99999999;
        MemoryStream memoryStream;
        Stream stream;
        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            isLoading = true;

            var Files = e.GetMultipleFiles();

            foreach (var file in Files)
            {
                stream = file.OpenReadStream(maxFileSize);
                memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                stream.Close();

                documentVM.FileName = file.Name;
                documentVM.FileContent= memoryStream.ToArray();
                documentVM.FileType = file.ContentType;
                memoryStream.Close();
            }

            isLoading = false;
        }

        //Update Doctype
        private async Task InitializeModalUpdate_DocType(int _IsTypeUpdate)
        {
            isLoading = true;

            if (_IsTypeUpdate == 0)
            {
                doctypeVM = new();
                doctypeVM.GroupType = "DocBoat";
            }

            if (_IsTypeUpdate == 1)
            {
                doctypeVM = doctype_filter_list.First(x => x.DocTypeID == documentVM.DocTypeID);
            }

            doctypeVM.IsTypeUpdate = _IsTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_DocType");

            isLoading = false;
        }
        private async Task UpdateDocType(EditContext _formDocTypeVM, int _IsTypeUpdate)
        {
            doctypeVM.IsTypeUpdate = _IsTypeUpdate;

            if (!_formDocTypeVM.Validate()) return;

            isLoading = true;

            if (doctypeVM.IsTypeUpdate != 2)
            {
                await documentService.UpdateDocType(doctypeVM);

                logVM.LogDesc = "Cập nhật loại tài liệu thành công!";
                await sysService.InsertLog(logVM);

                doctype_filter_list = await documentService.GetDocTypes(filterVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_DocType");
                await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await documentService.UpdateDocType(doctypeVM);

                    if (affectedRows > 0)
                    {
                        logVM.LogDesc = "Xóa loại tài liệu thành công!";
                        await sysService.InsertLog(logVM);

                        documentVM.DocTypeID = 0;
                        doctype_filter_list = await documentService.GetDocTypes(filterVM);

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_DocType");
                        await js.Toast_Alert(logVM.LogDesc, SweetAlertMessageType.success);
                    }
                    else
                    {
                        await js.Swal_Message("Xóa không thành công!", "Có dữ liệu liên quan.", SweetAlertMessageType.error);
                        doctypeVM.IsTypeUpdate = 1;
                    }
                }
                else
                {
                    doctypeVM.IsTypeUpdate = 1;
                }
            }

            isLoading = false;
        }

    }
}
