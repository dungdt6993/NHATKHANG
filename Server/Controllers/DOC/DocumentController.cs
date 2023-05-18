﻿using Data.Infrastructure;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.DOC;
using D69soft.Shared.Models.ViewModels.HR;

namespace Data.Repositories.DOC
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public DocumentController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        [HttpPost("GetDocTypes")]
        public async Task<ActionResult<IEnumerable<DocumentTypeVM>>> GetDocTypes(FilterHrVM _filterHrVM)
        {
            var sql = "select * from DOC.DocumentType where GroupType = @GroupType order by DocTypeName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<DocumentTypeVM>(sql, _filterHrVM);
                return Ok(result);
            }
        }

        [HttpPost("GetDocs")]
        public async Task<ActionResult<List<DocumentVM>>> GetDocs(FilterHrVM _filterHrVM)
        {
            var sql = "select *, case when DATEDIFF(d,GETDATE(),ExpDate) - NumExpDate <=0 and NumExpDate <> 0 then 1 else 0 end as IsExpDoc, case when coalesce(FileScan,'') = '' then 1 else 0 end as IsDelFileScan from DOC.Document do ";
            sql += "join DOC.DocumentType dot on dot.DocTypeID = do.DocTypeID ";

            if (_filterHrVM.GroupType == "DocBoat")
            {
                sql += "join (select * from HR.Department where isActive=1) de on de.DepartmentID = do.DepartmentID ";
                sql += "where dot.GroupType = @GroupType and do.DivisionID=@DivisionID and (do.DepartmentID=@DepartmentID or coalesce(@DepartmentID,'')='') ";
            }

            if (_filterHrVM.GroupType == "DocTender")
            {
                sql += "join (select *, TenderName as DepartmentName from OP.Tender where TenderActive=1) cr on cr.TenderCode = do.DepartmentID ";
                sql += "where dot.GroupType = @GroupType and do.DivisionID=@DivisionID and (do.DepartmentID=@DepartmentID or coalesce(@DepartmentID,'')='') ";
            }

            if (_filterHrVM.GroupType == "DOCLegal")
            {
                sql += "where dot.GroupType = @GroupType and do.DivisionID=@DivisionID ";
            }

            if (_filterHrVM.GroupType == "DOCForm")
            {
                sql += "where dot.GroupType = @GroupType and do.DivisionID=@DivisionID ";
            }

            if (_filterHrVM.GroupType == "DOCStaff")
            {
                sql += "join (select Eserial, LastName, MiddleName, FirstName from HR.Profile) p on p.Eserial = do.Eserial ";
                sql += "join (select * from HR.Staff where coalesce(Terminated,0)=@TypeProfile) s on s.Eserial = p.Eserial ";
                sql += "join (select * from HR.JobHistory where CurrentJobID=1) jh on jh.Eserial = s.Eserial ";
                sql += "join HR.Department de on de.DepartmentID = jh.DepartmentID ";
                sql += "where dot.GroupType = @GroupType and do.DivisionID=@DivisionID and (jh.DepartmentID=@DepartmentID or coalesce(@DepartmentID,'')='') and (p.Eserial=@Eserial or coalesce(@Eserial,'')='') ";
            }

            sql += "and (do.DocTypeID=@DocTypeID or coalesce(@DocTypeID,0)=0) ";
            sql += "and ((DATEDIFF(d,GETDATE(),ExpDate) - NumExpDate <=0 and NumExpDate <> 0 and @isTypeSearch=1) or (coalesce(FileScan,'') = '' and @isTypeSearch=2) or @isTypeSearch=0) ";

            if (_filterHrVM.GroupType == "DocBoat")
            {
                sql += "order by de.DepartmentName, dot.DocTypeName ";
            }

            if (_filterHrVM.GroupType == "DOCStaff")
            {
                sql += "order by de.DepartmentName, p.Eserial ";
            }

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<DocumentVM>(sql, _filterHrVM);
                return result.ToList();
            }
        }

        [HttpPost("UpdateDocument")]
        public async Task<ActionResult<bool>> UpdateDocument(DocumentVM _documentVM)
        {
            var sql = string.Empty;
            if (_documentVM.IsTypeUpdate == 0)
            {
                sql = "Insert into DOC.Document (DocName,DivisionID,DepartmentID,Eserial,DocTypeID,TextNumber,DateOfIssue,ExpDate,DocNote,FileScan) ";
                sql += "Values (@DocName,@DivisionID,@DepartmentID,@Eserial,@DocTypeID,@TextNumber,@DateOfIssue,@ExpDate,@DocNote,@FileScan) ";
            }
            if (_documentVM.IsTypeUpdate == 1)
            {
                sql = "Update DOC.Document set DocName = @DocName, TextNumber = @TextNumber, DateOfIssue = @DateOfIssue, ExpDate = @ExpDate, DocNote = @DocNote, FileScan = @FileScan where DocID = @DocID ";
            }
            if (_documentVM.IsTypeUpdate == 2)
            {
                sql += "delete from DOC.Document where DocID=@DocID ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _documentVM);
                return true;
            }
        }

        [HttpPost("UpdateDocType")]
        public async Task<ActionResult<bool>> UpdateDocType(DocumentTypeVM _documentTypeVM)
        {
            var sql = "";
            if (_documentTypeVM.IsTypeUpdate == 0)
            {
                sql = "Insert into DOC.DocumentType (DocTypeName, NumExpDate, GroupType) Values (@DocTypeName,@NumExpDate, @GroupType) ";
            }
            if (_documentTypeVM.IsTypeUpdate == 1)
            {
                sql = "Update DOC.DocumentType set DocTypeName = @DocTypeName, NumExpDate = @NumExpDate where DocTypeID = @DocTypeID ";
            }
            if (_documentTypeVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from DOC.Document where DocTypeID=@DocTypeID) ";
                sql += "begin ";
                sql += "delete from DOC.DocumentType where DocTypeID = @DocTypeID ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _documentTypeVM);
                return true;
            }
        }
    }
}
