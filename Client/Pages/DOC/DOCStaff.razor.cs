﻿using Data.Repositories.DOC;
using Data.Repositories.HR;
using Data.Repositories.SYSTEM;
using Model.Entities.HR;
using Model.ViewModels.DOC;
using Model.ViewModels.FIN;
using Model.ViewModels.HR;
using Utilities;
using WebApp.Helpers;
using DevExpress.CodeParser;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.IO;

namespace WebApp.Pages.DOC
{
    partial class DOCStaff
    {
        [Inject] IJSRuntime js { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        [Inject] SysRepository sysRepo { get; set; }
        [Inject] IWebHostEnvironment env { get; set; }
        [Inject] OrganizationalChartService organizationalChartRepo { get; set; }
        [Inject] ProfileRepository profileRepo { get; set; }

        [Inject] DocumentService docRepo { get; set; }

        protected string UserID;

        bool isLoading;

        bool isLoadingPage;

        //Filter
        FilterHrVM filterHrVM = new();
        IEnumerable<DivisionVM> division_filter_list;
        IEnumerable<DepartmentVM> department_filter_list;
        IEnumerable<ProfileVM> eserial_filter_list;

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
            isLoadingPage = true;

            UserID = (await authenticationStateTask).User.GetUserId();

            if (sysRepo.checkPermisFunc(UserID, "DOC_DOCStaff"))
            {
                await sysRepo.insert_LogUserFunc(UserID, "DOC_DOCStaff");
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

            //Initialize Filter
            filterHrVM.UserID = UserID;

            filterHrVM.GroupType = "DOCStaff";

            division_filter_list = await organizationalChartRepo.GetDivisionList(filterHrVM);
            filterHrVM.DivisionID = division_filter_list.Count() > 0 ? division_filter_list.ElementAt(0).DivisionID : string.Empty;

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartRepo.GetDepartmentList(filterHrVM);

            filterHrVM.SectionID = string.Empty;

            filterHrVM.PositionGroupID = string.Empty;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await profileRepo.GetEserialListByID(filterHrVM);

            doctype_filter_list = await docRepo.GetDocTypeList(filterHrVM);

            isLoadingPage = false;
        }

        private async void onchange_filter_division(string value)
        {
            isLoading = true;

            filterHrVM.DivisionID = value;

            filterHrVM.DepartmentID = string.Empty;
            department_filter_list = await organizationalChartRepo.GetDepartmentList(filterHrVM);

            documentVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_department(string value)
        {
            isLoading = true;

            filterHrVM.DepartmentID = value;

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await profileRepo.GetEserialListByID(filterHrVM);

            documentVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private void onchange_filter_eserial(string value)
        {
            isLoading = true;

            filterHrVM.Eserial = value;

            documentVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async void onchange_filter_doctype(int value)
        {
            isLoading = true;

            filterHrVM.DocTypeID = value;

            documentVMs = null;

            isLoading = false;

            StateHasChanged();
        }

        private async Task onchange_filter_profiletype(ChangeEventArgs args)
        {
            isLoading = true;

            filterHrVM.TypeProfile = int.Parse(args.Value.ToString());

            filterHrVM.Eserial = string.Empty;
            eserial_filter_list = await profileRepo.GetEserialListByID(filterHrVM);

            documentVMs = null;

            isLoading = false;
        }

        private async Task onchange_filter_type(ChangeEventArgs args)
        {
            isLoading = true;

            filterHrVM.isTypeSearch = int.Parse(args.Value.ToString());

            documentVMs = null;

            isLoading = false;
        }

        protected async Task GetDOCStaffList()
        {
            isLoading = true;

            documentVMs = await docRepo.GetDocList(filterHrVM);

            isLoading = false;
        }

        //Update Document
        private async Task InitializeModalUpdate_Document(int _isTypeUpdate, DocumentVM _documentVM)
        {
            isLoading = true;

            if (_isTypeUpdate == 0)
            {
                documentVM = new();

                documentVM.DivisionID = filterHrVM.DivisionID;
                documentVM.Eserial = filterHrVM.Eserial;
                documentVM.GroupType = "DOCStaff";
                documentVM.IsDelFileScan = true;
            }

            if (_isTypeUpdate == 1)
            {
                documentVM = _documentVM;
            }

            documentVM.IsTypeUpdate = _isTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_Document");

            isLoading = false;
        }

        public string onchange_DateOfIssue
        {
            get
            {
                return documentVM.DateOfIssue.HasValue ? documentVM.DateOfIssue.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                documentVM.DateOfIssue = LibraryFunc.FormatDateDDMMYYYY(value, documentVM.DateOfIssue);
            }
        }

        public string onchange_ExpDate
        {
            get
            {
                return documentVM.ExpDate.HasValue ? documentVM.ExpDate.Value.ToString("dd/MM/yyyy") : "";
            }
            set
            {
                documentVM.ExpDate = LibraryFunc.FormatDateDDMMYYYY(value, documentVM.ExpDate);
            }
        }

        private async Task UpdateDocument()
        {
            isLoading = true;

            if (documentVM.IsTypeUpdate != 2)
            {
                if (documentVM.IsDelFileScan && !String.IsNullOrEmpty(documentVM.FileScan))
                {
                    LibraryFunc.DelFileFrom(Path.Combine(env.ContentRootPath, $"{UrlDirectory.Upload_DOC_Private}{documentVM.FileScan}"));
                    documentVM.FileScan = String.Empty;
                }

                if (memoryStream != null)
                {
                    var filename = LibraryFunc.RemoveWhitespace(documentVM.DocTypeID + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss"));

                    var path = $"{UrlDirectory.Upload_DOC_Private}{filename}.pdf";

                    File.WriteAllBytes(path, memoryStream.ToArray());

                    documentVM.FileScan = filename + ".pdf";
                }

                await docRepo.UpdateDocument(documentVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Document");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    if (!String.IsNullOrEmpty(documentVM.FileScan))
                    {
                        LibraryFunc.DelFileFrom(Path.Combine(env.ContentRootPath, $"{UrlDirectory.Upload_DOC_Private}{documentVM.FileScan}"));
                    }
                    await docRepo.UpdateDocument(documentVM);

                    await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_Document");
                    await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);

                }
                else
                {
                    documentVM.IsTypeUpdate = 1;
                }
            }

            memoryStream = null;

            await GetDOCStaffList();

            isLoading = false;
        }

        //Upload File
        private long maxFileSize = 99999999;
        MemoryStream memoryStream;
        Stream stream;
        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            isLoading = true;

            var imageFiles = e.GetMultipleFiles();

            foreach (var img in imageFiles)
            {
                documentVM.ContentType = img.ContentType;

                stream = img.OpenReadStream(maxFileSize);
                memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                stream.Close();
            }

            isLoading = false;
        }

        //Update Doctype
        private async Task InitializeModalUpdate_DocType(int _isTypeUpdate)
        {
            isLoading = true;

            if (_isTypeUpdate == 0)
            {
                doctypeVM = new();
                doctypeVM.GroupType = "DOCStaff";
            }

            if (_isTypeUpdate == 1)
            {
                doctypeVM = doctype_filter_list.First(x => x.DocTypeID == documentVM.DocTypeID);
            }

            doctypeVM.IsTypeUpdate = _isTypeUpdate;

            await js.InvokeAsync<object>("ShowModal", "#InitializeModalUpdate_DocType");

            isLoading = false;
        }
        private async Task UpdateDocType()
        {
            isLoading = true;

            if (doctypeVM.IsTypeUpdate != 2)
            {
                await docRepo.UpdateDocType(doctypeVM);

                await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_DocType");
                await js.Toast_Alert("Cập nhật thành công!", SweetAlertMessageType.success);
            }
            else
            {
                if (await js.Swal_Confirm("Xác nhận!", $"Bạn có chắn chắn xóa?", SweetAlertMessageType.question))
                {
                    int affectedRows = await docRepo.UpdateDocType(doctypeVM);

                    if (affectedRows > 0)
                    {
                        doctypeVM.DocTypeID = 0;
                        doctype_filter_list = await docRepo.GetDocTypeList(filterHrVM);

                        await js.InvokeAsync<object>("CloseModal", "#InitializeModalUpdate_DocType");
                        await js.Toast_Alert("Xóa thành công!", SweetAlertMessageType.success);
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

            doctype_filter_list = await docRepo.GetDocTypeList(filterHrVM);

            isLoading = false;
        }

    }
}
