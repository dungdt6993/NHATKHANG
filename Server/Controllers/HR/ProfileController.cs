using D69soft.Shared.Models.ViewModels.HR;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Data.Infrastructure;
using Dapper;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Shared.Utilities;
using System.Collections;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace D69soft.Server.Controllers.HR
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;
        private readonly IWebHostEnvironment _env;

        public ProfileController(SqlConnectionConfig connConfig, IWebHostEnvironment env)
        {
            _connConfig = connConfig;
            _env = env;
        }

        //Contact
        [HttpGet("GetContacts/{_UserID}")]
        public async Task<ActionResult<List<ProfileVM>>> GetContacts(string _UserID)
        {
            var sql = "select p.UrlAvatar, p.Eserial, p.LastName, p.MiddleName, p.FirstName, p.LastName + ' ' + p.MiddleName + ' ' + p.FirstName as FullName, p.Birthday, p.Mobile, s.EmailCompany, de.DepartmentName, po.PositionName from HR.Profile p ";
            sql += "join (select * from HR.Staff where coalesce(Terminated,0) = 0) s on s.Eserial = p.Eserial ";
            sql += "join (select * from HR.JobHistory where CurrentJobID=1 and DivisionID in (select DivisionID from HR.JobHistory where CurrentJobID=1 and Eserial=@UserID)) jh on jh.Eserial = s.Eserial ";
            sql += "join HR.Department de on de.DepartmentID = jh.DepartmentID ";
            sql += "join HR.Position po on po.PositionID = jh.PositionID ";
            sql += "order by de.DepartmentName, p.FirstName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ProfileVM>(sql, new { UserID = _UserID });
                return result.ToList();
            }
        }

        //Profile
        [HttpPost("GetEserialListByID")]
        public async Task<ActionResult<IEnumerable<EserialVM>>> GetEserialListByID(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@SectionID", _filterHrVM.SectionID);
                parm.Add("@DeptID", _filterHrVM.DepartmentID);
                parm.Add("@arrPos", _filterHrVM.PositionGroupID);
                parm.Add("@isSearch", _filterHrVM.TypeProfile);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryAsync<EserialVM>("HR.Profile_viewEserialMain", parm, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpPost("GetProfileList")]
        public async Task<ActionResult<List<ProfileVM>>> GetProfileList(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@SectionID", _filterHrVM.SectionID);
                parm.Add("@DeptID", _filterHrVM.DepartmentID);
                parm.Add("@arrPos", _filterHrVM.PositionGroupID);
                parm.Add("@Eserial", _filterHrVM.Eserial);
                parm.Add("@isSearch", _filterHrVM.TypeProfile);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryAsync<ProfileVM>("HR.Profile_viewListProfile", parm, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        [HttpPost("GetSearchEmpl")]
        public async Task<ActionResult<IEnumerable<ProfileVM>>> GetSearchEmpl(FilterHrVM _filterHrVM)
        {
            var sql = "select * from HR.Profile p ";
            sql += "join (select * from HR.Staff where coalesce(Terminated,0)=0) s on s.Eserial = p.Eserial ";
            sql += "join (select * from HR.JobHistory where CurrentJobID=1) jh on jh.Eserial = p.Eserial ";
            sql += "join HR.Position po on po.PositionID = jh.PositionID ";
            sql += "where (p.Eserial LIKE CONCAT('%',@searchEmpl,'%') or (p.LastName + ' ' + p.MiddleName + ' ' + p.FirstName LIKE CONCAT('%',@searchEmpl,'%')) or po.PositionName LIKE CONCAT('%',@searchEmpl,'%')) ";
            sql += "order by FirstName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ProfileVM>(sql, _filterHrVM);
                return Ok(result);
            }
        }

        [HttpPost("UpdateProfile")]
        public async Task<ActionResult<string>> UpdateProfile(ProfileVM _profileVM)
        {
            if (_profileVM.IsTypeUpdate == 0)
            {
                _profileVM.User_PassReset = LibraryFunc.RandomNumber(100000, 999999).ToString();

                _profileVM.User_Password = LibraryFunc.GennerateToMD5(_profileVM.User_PassReset);
            }

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@typeView", _profileVM.IsTypeUpdate);

                parm.Add("@ckcontractextension", _profileVM.ckContractExtension);
                parm.Add("@ckjob", _profileVM.ckJob);
                parm.Add("@cksal", _profileVM.ckSal);
                parm.Add("@eserial", _profileVM.Eserial);
                parm.Add("@lastname", _profileVM.LastName);
                parm.Add("@middlename", _profileVM.MiddleName);
                parm.Add("@firstname ", _profileVM.FirstName);
                parm.Add("@birthday ", _profileVM.Birthday);
                parm.Add("@placeofbirthday", _profileVM.PlaceOfBirth);
                parm.Add("@gender", _profileVM.Gender);
                parm.Add("@idcard", _profileVM.IDCard);
                parm.Add("@dateofissue", _profileVM.DateOfIssue);
                parm.Add("@placeofissue", _profileVM.PlaceOfIssue);
                parm.Add("@nationality", _profileVM.CountryCode);
                parm.Add("@ethnic", _profileVM.EthnicID);
                parm.Add("@hometown", _profileVM.Hometown);
                parm.Add("@qualification", _profileVM.Qualification);
                parm.Add("@tel", _profileVM.Mobile);
                parm.Add("@email", _profileVM.Email);
                parm.Add("@resident", _profileVM.Resident);
                parm.Add("@temporarity", _profileVM.Temporarity);
                parm.Add("@pittaxcode", _profileVM.PITTaxCode);
                parm.Add("@taxdept", _profileVM.TaxDept);
                parm.Add("@visanum", _profileVM.VisaNumber);
                parm.Add("@visaexp", _profileVM.VisaExpDate);
                parm.Add("@contact_name", _profileVM.Contact_Name);
                parm.Add("@contact_rela", _profileVM.Contact_Rela);
                parm.Add("@contact_tel", _profileVM.Contact_Tel);
                parm.Add("@contact_address", _profileVM.Contact_Address);
                parm.Add("@joindate", _profileVM.JoinDate);
                parm.Add("@startdayal", _profileVM.StartDayAL);
                parm.Add("@worktype ", _profileVM.WorkTypeID);
                parm.Add("@division", _profileVM.DivisionID);
                parm.Add("@department", _profileVM.DepartmentID);
                parm.Add("@section", _profileVM.SectionID);
                parm.Add("@position", _profileVM.PositionID);
                parm.Add("@startcontractdate", _profileVM.StartContractDate);
                parm.Add("@contracttype", _profileVM.ContractTypeID);
                parm.Add("@endcontractdate", _profileVM.EndContractDate);
                parm.Add("@shift", _profileVM.ShiftID);
                parm.Add("@jobstartdate", _profileVM.JobStartDate);
                parm.Add("@bankaccount", _profileVM.BankAccount);
                parm.Add("@bankcode", _profileVM.BankCode);
                parm.Add("@bankname", _profileVM.BankName);
                parm.Add("@bankbranch", _profileVM.BankBranch);
                parm.Add("@timeattcode", _profileVM.TimeAttCode);
                parm.Add("@socialins", _profileVM.SocialInsNumber);
                parm.Add("@healthins", _profileVM.HealthInsNumber);
                parm.Add("@emailcompany", _profileVM.EmailCompany);
                parm.Add("@basicsalary", _profileVM.BasicSalary);
                parm.Add("@othersalary", _profileVM.OtherSalary);
                parm.Add("@benefit1", _profileVM.Benefit1);
                parm.Add("@benefit2", _profileVM.Benefit2);
                parm.Add("@benefit3", _profileVM.Benefit3);
                parm.Add("@benefit4", _profileVM.Benefit4);
                parm.Add("@benefit5", _profileVM.Benefit5);
                parm.Add("@benefit6", _profileVM.Benefit6);
                parm.Add("@benefit7", _profileVM.Benefit7);
                parm.Add("@benefit8", _profileVM.Benefit8);
                parm.Add("@beginsalary", _profileVM.BeginSalaryDate);
                parm.Add("@ispaybank", _profileVM.SalaryByBank);
                parm.Add("@ispaybymonth", _profileVM.IsPayByMonth);
                parm.Add("@ispaybydate", _profileVM.IsPayByDate);
                parm.Add("@reason", _profileVM.Reason);
                parm.Add("@approvedby", _profileVM.ApprovedBy);
                parm.Add("@imageurl", _profileVM.UrlAvatar == null ? UrlDirectory.Default_Avatar : _profileVM.UrlAvatar);
                parm.Add("@role", _profileVM.PermisId);
                parm.Add("@pass", _profileVM.User_Password);
                parm.Add("@passreset", _profileVM.User_PassReset);

                parm.Add("@Appraiser_Eserial", _profileVM.Appraiser_Eserial);
                parm.Add("@DirectManager_Eserial", _profileVM.DirectManager_Eserial);
                parm.Add("@ControlDept_Eserial", _profileVM.ControlDept_Eserial);
                parm.Add("@Approve_Eserial", _profileVM.Approve_Eserial);

                parm.Add("@UserID", _profileVM.UserID);

                _profileVM.Eserial = await conn.ExecuteScalarAsync<string>("HR.Profile_update", parm, commandType: CommandType.StoredProcedure);

                //Update UrlAvarta
                if (_profileVM.IsDelFileUpload)
                {
                    LibraryFunc.DelFileFrom(Path.Combine(_env.ContentRootPath, $"{UrlDirectory.Upload_HR_Images_Profile_Private}{_profileVM.Eserial}.png"));
                    _profileVM.UrlAvatar = UrlDirectory.Default_Avatar;
                }

                if (_profileVM.FileContent != null)
                {
                    var filename = $"{_profileVM.Eserial}.png";
                    var path = Path.Combine(_env.ContentRootPath, $"{UrlDirectory.Upload_HR_Images_Profile_Private}{filename}");
                    var fs = System.IO.File.Create(path);
                    fs.Write(_profileVM.FileContent, 0, _profileVM.FileContent.Length);
                    fs.Close();

                    _profileVM.UrlAvatar = $"{UrlDirectory.Upload_HR_Images_Profile_Public}{filename}";
                }

                var sql = "Update HR.Profile set UrlAvatar=@UrlAvatar where Eserial=@Eserial";
                await conn.ExecuteAsync(sql, _profileVM);

                return _profileVM.Eserial;
            }
        }

        [HttpGet("GetProfileHistory/{_Eserial}")]
        public async Task<ActionResult<List<ProfileVM>>> GetProfileHistory(string _Eserial)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();

                parm.Add("@Eserial", _Eserial);

                var result = await conn.QueryAsync<ProfileVM>("HR.Profile_viewProfileHistory", parm, commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
        }

        [HttpGet("GetSalaryDef")]
        public async Task<ActionResult<List<SalaryDefVM>>> GetSalaryDef()
        {
            var sql = "select * from HR.SalaryDef";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<SalaryDefVM>(sql);
                return result.ToList();
            }
        }

        [HttpGet("CheckContainsEserial/{_Eserial}")]
        public async Task<ActionResult<bool>> CheckContainsEserial(string _Eserial)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.Profile where Eserial = @Eserial) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { Eserial = _Eserial });
            }
        }

        [HttpGet("GetCountryList")]
        public async Task<ActionResult<IEnumerable<CountryVM>>> GetCountryList()
        {
            var sql = "select CountryCode, CountryName from HR.Country";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<CountryVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("GetEthnicList")]
        public async Task<ActionResult<IEnumerable<EthnicVM>>> GetEthnicList()
        {
            var sql = "select EthnicID, EthnicName from HR.Ethnic";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<EthnicVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("CkUpdateJobHistory/{_Eserial}")]
        public async Task<ActionResult<bool>> CkUpdateJobHistory(string _Eserial)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (select 1 from HR.JobHistory where Eserial=@Eserial group by Eserial having count(Eserial)>1) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { Eserial = _Eserial });
            }
        }

        [HttpGet("CkUpdateSalHistory/{_Eserial}")]
        public async Task<ActionResult<bool>> CkUpdateSalHistory(string _Eserial)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (select 1 from HR.SalaryHistory where Eserial=@Eserial group by Eserial having count(Eserial)>1) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { Eserial = _Eserial });
            }
        }

        [HttpPost("ResetPass")]
        public async Task<ActionResult<bool>> ResetPass(ProfileVM _profileVM)
        {
            _profileVM.User_PassReset = LibraryFunc.RandomNumber(100000, 999999).ToString();

            _profileVM.User_Password = LibraryFunc.GennerateToMD5(_profileVM.User_PassReset);

            _profileVM.User_isChangePass = 0;

            var sql = "Update HR.Profile set User_Password = @User_Password, User_PassReset = @User_PassReset, User_isChangePass = @User_isChangePass where Eserial = @Eserial";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _profileVM);
                return true;
            }
        }

        [HttpPost("DelProfileHistory")]
        public async Task<ActionResult<bool>> DelProfileHistory(ProfileVM _profileVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                //Check del JOB
                var sqlCheckDelJob = "select 1 from HR.JobSalHistory where Eserial=@Eserial and JobID=@JobID and SalID=@SalaryID and (SUBSTRING(RIGHT(AdjustProfileID,5),3,1) > 0 or SUBSTRING(RIGHT(AdjustProfileID,5),1,1) > 0) ";
                if (await conn.ExecuteScalarAsync<int>(sqlCheckDelJob, _profileVM) == 1)
                {
                    var sqlDelJob = "Delete from HR.JobHistory where Eserial=@Eserial and JobID=@JobID ";
                    await conn.ExecuteAsync(sqlDelJob, _profileVM);

                    var sqlUpdateJobActive = "Update HR.JobHistory set CurrentJobID = 1 where Eserial = @Eserial and JobID in (select MAX(JobID) from HR.JobHistory where Eserial = @Eserial)";
                    await conn.ExecuteAsync(sqlUpdateJobActive, _profileVM);
                }

                //Check del SAL
                var sqlCheckDelSal = "select 1 from HR.JobSalHistory where Eserial=@Eserial and SalID=@SalaryID and JobID=@JobID and SUBSTRING(RIGHT(AdjustProfileID,5),5,1) > 0 ";
                if (await conn.ExecuteScalarAsync<int>(sqlCheckDelSal, _profileVM) == 1)
                {
                    var sqlDelSal = "Delete from HR.SalaryHistory where Eserial=@Eserial and SalaryID=@SalaryID";
                    await conn.ExecuteAsync(sqlDelSal, _profileVM);

                    var sqlUpdateSalActive = "Update HR.SalaryHistory set isActive = 1 where Eserial = @Eserial and SalaryID in (select MAX(SalaryID) from HR.SalaryHistory where Eserial = @Eserial)";
                    await conn.ExecuteAsync(sqlUpdateSalActive, _profileVM);
                }

                //Del JobSalHistory
                var sqlDelJobSalHistory = "Delete from HR.JobSalHistory where Eserial=@Eserial and JobID=@JobID and SalID=@SalaryID ";
                await conn.ExecuteAsync(sqlDelJobSalHistory, _profileVM);

                //Del LogPrintAgreementText
                var sqlDelLogPrintAgreementText = "Delete from HR.LogPrintAgreementText where Eserial=@Eserial and JobID=@JobID and SalID=@SalaryID ";
                await conn.ExecuteAsync(sqlDelLogPrintAgreementText, _profileVM);

                return true;
            }
        }

        [HttpGet("DelProfile/{_Eserial}")]
        public async Task<ActionResult<bool>> DelProfile(string _Eserial)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@eserial", _Eserial);

                await conn.ExecuteAsync("HR.Profile_del", parm, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        [HttpPost("TerminateProfile")]
        public async Task<ActionResult<bool>> TerminateProfile(ProfileVM _profileVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@eserial", _profileVM.Eserial);
                parm.Add("@terminatedate", _profileVM.TerminateDate);
                parm.Add("@reasonterminate", _profileVM.ReasonTerminate);
                parm.Add("@UserID", _profileVM.UserID);

                await conn.ExecuteAsync("HR.Profile_terminate", parm, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        [HttpGet("RestoreTerminateProfile/{_Eserial}/{_UserID}")]
        public async Task<ActionResult<bool>> RestoreTerminateProfile(string _Eserial, string _UserID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@eserial", _Eserial);
                parm.Add("@UserID", _UserID);

                await conn.ExecuteAsync("HR.Profile_restoreTerminate", parm, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        //Permis
        [HttpGet("GetFuncGroupPermis/{_Eserial}")]
        public async Task<ActionResult<IEnumerable<FuncVM>>> GetFuncGroupPermis(string _Eserial)
        {
            var sql = "select distinct fg.ModuleID, fg.FGNo, fg.FuncGrpID, fg.FuncGrpName, case when sum(case when coalesce(pf.FuncID,'') != '' then 1 else 0 end) > 0 then 1 else 0 end as IsChecked from SYSTEM.FuncGrp fg ";
            sql += "join (select * from SYSTEM.Func where isActive = 1) f on fg.FuncGrpID = f.FuncGrpID ";
            sql += "left join (select * from SYSTEM.PermissionFunc where UserID=@Eserial) pf on pf.FuncID = f.FuncID ";
            sql += "group by fg.ModuleID, fg.FGNo, fg.FuncGrpID,fg.FuncGrpName order by fg.ModuleID, FGNo ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<FuncVM>(sql, new { Eserial = _Eserial });
                return Ok(result);
            }
        }

        [HttpGet("GetFuncPermis/{_Eserial}")]
        public async Task<ActionResult<IEnumerable<FuncVM>>> GetFuncPermis(string _Eserial)
        {
            var sql = "select f.FuncGrpID, f.FuncID, FuncName, case when coalesce(pf.FuncID,'') != '' then 1 else 0 end as IsChecked from SYSTEM.Func f ";
            sql += "left join (select * from SYSTEM.PermissionFunc where UserID=@Eserial) pf on pf.FuncID = f.FuncID ";
            sql += "where isActive = 1 order by FNo ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<FuncVM>(sql, new { Eserial = _Eserial });
                return Ok(result);
            }
        }

        [HttpGet("GetSubFuncPermis/{_Eserial}")]
        public async Task<ActionResult<IEnumerable<FuncVM>>> GetSubFuncPermis(string _Eserial)
        {
            var sql = "select f.FuncGrpID, sf.FuncID, sf.SubFuncID, SubFuncName, case when coalesce(pfs.SubFuncID,'') != '' then 1 else 0 end as IsChecked from SYSTEM.SubFunc sf ";
            sql += "join (select * from SYSTEM.Func where isActive = 1) f on f.FuncID = sf.FuncID ";
            sql += "left join (select * from SYSTEM.PermissionSubFunc where UserID=@Eserial) pfs on pfs.SubFuncID = sf.SubFuncID order by SubNo ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<FuncVM>(sql, new { Eserial = _Eserial });
                return Ok(result);
            }
        }

        [HttpGet("GetDivisionPermis/{_Eserial}")]
        public async Task<ActionResult<IEnumerable<DepartmentVM>>> GetDivisionPermis(string _Eserial)
        {
            var sql = "select di.DivisionID, di.DivisionName, case when sum(case when coalesce(pd.DepartmentID,'') != '' then 1 else 0 end) > 0 then 1 else 0 end as IsChecked  from HR.Division di ";
            sql += "left join (select * from SYSTEM.PermissionDepartment where UserID=@Eserial) pd on pd.DivisionID = di.DivisionID group by di.DivisionID, di.DivisionName ";
            sql += "order by DivisionID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<DepartmentVM>(sql, new { Eserial = _Eserial });
                return Ok(result);
            }
        }

        [HttpGet("GetDepartmentPermis/{_Eserial}")]
        public async Task<ActionResult<IEnumerable<DepartmentVM>>> GetDepartmentPermis(string _Eserial)
        {
            var sql = "select di.DivisionID, de.DepartmentID, de.DepartmentName, case when coalesce(pd.DepartmentID,'') != '' then 1 else 0 end as IsChecked  from HR.Department de ";
            sql += "join HR.Division di on di.DivisionID = de.DivisionID ";
            sql += "left join (select * from SYSTEM.PermissionDepartment where UserID=@Eserial) pd on pd.DivisionID = di.DivisionID and pd.DepartmentID = de.DepartmentID ";
            sql += "order by di.DivisionID, de.DepartmentID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<DepartmentVM>(sql, new { Eserial = _Eserial });
                return Ok(result);
            }
        }

        [HttpGet("GetSysReportGroupPermis/{_Eserial}")]
        public async Task<ActionResult<IEnumerable<SysRptVM>>> GetSysReportGroupPermis(string _Eserial)
        {
            var sql = "select rg.RptGrpID, rg.RptGrpName, case when sum(case when coalesce(pr.RptID,'') != '' then 1 else 0 end) > 0 then 1 else 0 end as IsChecked from SYSTEM.RptGrp rg ";
            sql += "join (select * from SYSTEM.Rpt) r on rg.RptGrpID = r.RptGrpID ";
            sql += "left join (select * from SYSTEM.PermissionRpt where UserID=@Eserial) pr on pr.RptID = r.RptID ";
            sql += "group by rg.ModuleID, rg.RptGrpID, rg.RptGrpName order by rg.ModuleID, rg.RptGrpID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<SysRptVM>(sql, new { Eserial = _Eserial });
                return Ok(result);
            }
        }

        [HttpGet("GetSysReportPermis/{_Eserial}")]
        public async Task<ActionResult<IEnumerable<SysRptVM>>> GetSysReportPermis(string _Eserial)
        {
            var sql = "select r.RptGrpID, r.RptID, r.RptName, case when coalesce(pr.RptID,'') != '' then 1 else 0 end as IsChecked from SYSTEM.Rpt r ";
            sql += "left join (select * from SYSTEM.PermissionRpt where UserID=@Eserial) pr on pr.RptID = r.RptID  ";
            sql += "order by r.RptGrpID, r.RptName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<SysRptVM>(sql, new { Eserial = _Eserial });
                return Ok(result);
            }
        }

        [HttpPost("UpdatePermis/{_Eserial}")]
        public async Task<ActionResult<bool>> UpdatePermis(ArrayList _arrayList, string _Eserial)
        {
            IEnumerable<FuncVM> _funcVMs = JsonConvert.DeserializeObject<IEnumerable<FuncVM>>(_arrayList[0].ToString());
            IEnumerable<FuncVM> _subFuncVMs = JsonConvert.DeserializeObject<IEnumerable<FuncVM>>(_arrayList[1].ToString());
            IEnumerable<DepartmentVM> _departmentVMs = JsonConvert.DeserializeObject<IEnumerable<DepartmentVM>>(_arrayList[2].ToString());
            IEnumerable<SysRptVM> _sysRptVMs = JsonConvert.DeserializeObject<IEnumerable<SysRptVM>>(_arrayList[3].ToString());

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sqlFunc = "Delete from SYSTEM.PermissionFunc where UserID=@Eserial ";
                foreach (var funcVM in _funcVMs)
                {
                    sqlFunc += "Insert into SYSTEM.PermissionFunc(UserID, FuncID) Values(@Eserial,'" + funcVM.FuncID + "') ";
                }
                await conn.ExecuteAsync(sqlFunc, new { Eserial = _Eserial });

                var sqlSubFunc = "Delete from SYSTEM.PermissionSubFunc where UserID=@Eserial ";
                foreach (var subFuncVM in _subFuncVMs)
                {
                    sqlSubFunc += "Insert into SYSTEM.PermissionSubFunc(UserID, SubFuncID) Values(@Eserial,'" + subFuncVM.SubFuncID + "') ";
                }
                await conn.ExecuteAsync(sqlSubFunc, new { Eserial = _Eserial });

                var sqlDept = "Delete from SYSTEM.PermissionDepartment where UserID=@Eserial ";
                foreach (var departmentVM in _departmentVMs)
                {
                    sqlDept += "Insert into SYSTEM.PermissionDepartment(UserID, DivisionID, DepartmentID) Values(@Eserial,'" + departmentVM.DivisionID + "', '" + departmentVM.DepartmentID + "') ";
                }
                await conn.ExecuteAsync(sqlDept, new { Eserial = _Eserial });

                var sqlRpt = "Delete from SYSTEM.PermissionRpt where UserID=@Eserial ";
                foreach (var _sysRptVM in _sysRptVMs)
                {
                    sqlRpt += "Insert into SYSTEM.PermissionRpt(UserID, RptID) Values(@Eserial,'" + _sysRptVM.RptID + "') ";
                }
                await conn.ExecuteAsync(sqlRpt, new { Eserial = _Eserial });
            }
            return true;
        }

        //EmplTrn
        [HttpGet("GetSalTrnGrp/{_Eserial}")]
        public async Task<ActionResult<IEnumerable<EmployeeTransactionVM>>> GetSalTrnGrp(string _Eserial)
        {
            var sql = "select stg.TrnGroupCode, stg.TrnGroupName, case when sum(coalesce(et.TrnCode,0)) > 0 then 1 else 0 end as IsChecked from HR.SalaryTransactionGroup stg ";
            sql += "join HR.SalaryTransactionCode st  on st.TrnGroupCode = stg.TrnGroupCode ";
            sql += "left join (select * from HR.EmployeeTransaction where Eserial = @Eserial) et on et.SalTrnID = st.SalTrnID ";
            sql += "group by stg.TrnGroupCode, stg.TrnGroupName order by stg.TrnGroupCode ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<EmployeeTransactionVM>(sql, new { Eserial = _Eserial });
                return Ok(result);
            }
        }

        [HttpGet("GetSalTrnCode/{_Eserial}")]
        public async Task<ActionResult<IEnumerable<EmployeeTransactionVM>>> GetSalTrnCode(string _Eserial)
        {
            var sql = "select st.SalTrnID, stg.TrnGroupCode, stg.TrnGroupName, st.TrnCode, st.TrnSubCode, st.TrnName, case when coalesce(et.SalTrnID,0) != 0 then 1 else 0 end as IsChecked from HR.SalaryTransactionCode st ";
            sql += "join HR.SalaryTransactionGroup stg  on stg.TrnGroupCode = st.TrnGroupCode ";
            sql += "left join (select * from HR.EmployeeTransaction where Eserial = @Eserial) et on et.SalTrnID = st.SalTrnID";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<EmployeeTransactionVM>(sql, new { Eserial = _Eserial });
                return Ok(result);
            }
        }

        [HttpPost("UpdateEmplTrn/{_Eserial}")]
        public async Task<ActionResult<bool>> UpdateEmplTrn([FromBody] IEnumerable<EmployeeTransactionVM> _salTrnCodes, string _Eserial)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "Delete from HR.EmployeeTransaction where Eserial=@Eserial ";
                foreach (var _salTrnCode in _salTrnCodes)
                {
                    sql += "Insert into HR.EmployeeTransaction(Eserial, SalTrnID, TrnCode, TrnSubCode) Values(@Eserial," + _salTrnCode.SalTrnID + "," + _salTrnCode.TrnCode + "," + _salTrnCode.TrnSubCode + ") ";
                }
                await conn.ExecuteAsync(sql, new { Eserial = _Eserial });

            }

            return true;
        }

        //ContractType
        [HttpGet("GetContractTypeGroupList")]
        public async Task<ActionResult<IEnumerable<ContractTypeVM>>> GetContractTypeGroupList()
        {
            var sql = "select * from HR.ContractTypeGroup ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ContractTypeVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("GetContractTypeList")]
        public async Task<ActionResult<IEnumerable<ContractTypeVM>>> GetContractTypeList()
        {
            var sql = "select * from HR.ContractType ct join HR.ContractTypeGroup ctg on ctg.ContractTypeGroupID = ct.ContractTypeGroupID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ContractTypeVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("GetNumMonthLC/{_ContractTypeID}")]
        public async Task<ActionResult<int>> GetNumMonthLC(string _ContractTypeID)
        {
            var sql = "select NumMonth from HR.ContractType where ContractTypeID=@ContractTypeID";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<int>(sql, new { ContractTypeID = _ContractTypeID });
            }
        }

        [HttpGet("ContainsContractTypeID/{id}")]
        public async Task<ActionResult<bool>> ContainsContractTypeID(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.ContractType where ContractTypeID = @ContractTypeID) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { ContractTypeID = id });
            }
        }

        [HttpPost("UpdateContractType")]
        public async Task<ActionResult<int>> UpdateContractType(ContractTypeVM _contractTypeVM)
        {
            var sql = "";
            if (_contractTypeVM.IsTypeUpdate == 0)
            {
                sql += "Insert into HR.ContractType (ContractTypeID,ContractTypeName,NumMonth,ContractTypeGroupID) Values (@ContractTypeID,@ContractTypeName,@NumMonth,@ContractTypeGroupID) ";
            }
            if (_contractTypeVM.IsTypeUpdate == 1)
            {
                sql += "Update HR.ContractType set ContractTypeName = @ContractTypeName, NumMonth = @NumMonth, ContractTypeGroupID = @ContractTypeGroupID where ContractTypeID = @ContractTypeID ";
            }
            if (_contractTypeVM.IsTypeUpdate == 2)
            {
                sql += "if not exists (select * from HR.JobHistory where ContractTypeID=@ContractTypeID) ";
                sql += "begin ";
                sql += "delete from HR.ContractType where ContractTypeID=@ContractTypeID ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteAsync(sql, _contractTypeVM);
            }
        }

        //WorkType
        [HttpGet("GetWorkTypeList")]
        public async Task<ActionResult<IEnumerable<WorkTypeVM>>> GetWorkTypeList()
        {
            var sql = "select * from HR.WorkType";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<WorkTypeVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("ContainsWorkTypeID/{id}")]
        public async Task<ActionResult<bool>> ContainsWorkTypeID(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.WorkType where WorkTypeID = @WorkTypeID) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { WorkTypeID = id });
            }
        }

        [HttpPost("UpdateWorkType")]
        public async Task<ActionResult<int>> UpdateWorkType(WorkTypeVM _workTypeVM)
        {
            var sql = "";
            if (_workTypeVM.IsTypeUpdate == 0)
            {
                sql += "Insert into HR.WorkType (WorkTypeID,WorkTypeName,BudgetSATConfig,BudgetSUNConfig, isCalcCLDO) Values (@WorkTypeID,@WorkTypeName,@BudgetSATConfig,@BudgetSUNConfig,@isCalcCLDO) ";
            }
            if (_workTypeVM.IsTypeUpdate == 1)
            {
                sql += "Update HR.WorkType set WorkTypeName = @WorkTypeName, BudgetSATConfig = @BudgetSATConfig, BudgetSUNConfig = @BudgetSUNConfig, isCalcCLDO = @isCalcCLDO where WorkTypeID = @WorkTypeID ";
            }
            if (_workTypeVM.IsTypeUpdate == 2)
            {
                sql += "if not exists (select * from HR.JobHistory where WorkTypeID=@WorkTypeID) ";
                sql += "begin ";
                sql += "delete from HR.WorkType where WorkTypeID=@WorkTypeID ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteAsync(sql, _workTypeVM);
            }
        }

        //ProfileRelationship
        [HttpGet("GetRelationshipList")]
        public async Task<ActionResult<IEnumerable<ProfileRelationshipVM>>> GetRelationshipList()
        {
            var sql = "select * from HR.Relationship ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ProfileRelationshipVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("GetProfileRelationshipList/{_Eserial}")]
        public async Task<ActionResult<IEnumerable<ProfileRelationshipVM>>> GetProfileRelationshipList(string _Eserial)
        {
            var sql = "select * from HR.ProfileRelationship pr join HR.Relationship r on r.RelationshipID = pr.RelationshipID where Eserial=@Eserial ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ProfileRelationshipVM>(sql, new { Eserial = _Eserial });
                return Ok(result);
            }
        }

        [HttpPost("UpdateProfileRelationship")]
        public async Task<ActionResult<bool>> UpdateProfileRelationship(ProfileRelationshipVM _profileRelationshipVM)
        {
            var sql = "";
            if (_profileRelationshipVM.IsTypeUpdate == 0)
            {
                sql += "Insert into HR.ProfileRelationship (Eserial,Rela_FullName,RelationshipID,Rela_Birthday,Rela_ValidTo,Rela_TaxCode,isEmployeeTax,isActive,Rela_Note) Values (@Eserial,@Rela_FullName,@RelationshipID,@Rela_Birthday,@Rela_ValidTo,@Rela_TaxCode,@isEmployeeTax,@isActive,@Rela_Note) ";
            }
            if (_profileRelationshipVM.IsTypeUpdate == 1)
            {
                sql += "Update HR.ProfileRelationship set Rela_FullName = @Rela_FullName, RelationshipID = @RelationshipID, Rela_Birthday = @Rela_Birthday, Rela_ValidTo = @Rela_ValidTo, ";
                sql += "Rela_TaxCode = @Rela_TaxCode, isEmployeeTax = @isEmployeeTax, isActive = @isActive, Rela_Note = @Rela_Note ";
                sql += "where SeqPrRela = @SeqPrRela ";
            }
            if (_profileRelationshipVM.IsTypeUpdate == 2)
            {
                sql += "delete from HR.ProfileRelationship where SeqPrRela=@SeqPrRela ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _profileRelationshipVM);
                return true;
            }
        }

        //Báo cáo biến động nhân sự
        [HttpPost("GetEmplChangeList")]
        public async Task<ActionResult<DataTable>> GetEmplChangeList(FilterHrVM _filterHrVM)
        {
            return ExecuteStoredProcPrmsToDataTable("RPT.HR_Bao_cao_bien_dong_nhan_su",
                "@Y", _filterHrVM.Year,
                "@DivisionID", _filterHrVM.DivisionID);
        }

        public DataTable ExecuteStoredProcPrmsToDataTable(string storedName, params object[] prms)
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = conn;

                cmd.CommandTimeout = 0;

                cmd.CommandText = storedName;
                cmd.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i < prms.Length; i += 2)
                {
                    SqlParameter pa = new SqlParameter(prms[i].ToString(), prms[i + 1]);
                    cmd.Parameters.Add(pa);
                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);

                cmd.Dispose();
            }

            return dt;
        }
    }
}
