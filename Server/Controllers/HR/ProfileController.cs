using D69soft.Shared.Models.ViewModels.HR;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Data.Infrastructure;
using Dapper;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Shared.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace D69soft.Server.Controllers.HR
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public ProfileController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        //Contact
        [HttpGet("GetContacts/{_UserID}")]
        public async Task<ActionResult<List<ProfileManagamentVM>>> GetContacts(string _UserID)
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

                var result = await conn.QueryAsync<ProfileManagamentVM>(sql, new { UserID = _UserID });
                return result.ToList();
            }
        }

        //Profile
        [HttpPost("GetEserialListByID")]
        public async Task<IEnumerable<ProfileVM>> GetEserialListByID(FilterHrVM _filterHrVM)
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

                var result = await conn.QueryAsync<ProfileVM>("HR.Profile_viewEserialMain", parm, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        [HttpPost("GetProfileList/{_UserID}")]
        public async Task<List<ProfileManagamentVM>> GetProfileList(FilterHrVM _filterHrVM, string _UserID)
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
                parm.Add("@UserID", _UserID);

                var result = await conn.QueryAsync<ProfileManagamentVM>("HR.Profile_viewListProfile", parm, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        [HttpPost("GetProfileByEserial")]
        public async Task<ProfileManagamentVM> GetProfileByEserial(FilterHrVM _filterHrVM)
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
                parm.Add("@Eserial", _filterHrVM.selectedEserial);
                parm.Add("@isSearch", _filterHrVM.TypeProfile);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryFirstAsync<ProfileManagamentVM>("HR.Profile_viewListProfile", parm, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        [HttpPost("GetSearchEmpl")]
        public async Task<IEnumerable<ProfileManagamentVM>> GetSearchEmpl(FilterHrVM _filterHrVM)
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

                var result = await conn.QueryAsync<ProfileManagamentVM>(sql, _filterHrVM);
                return result;
            }
        }

        [HttpPost("UpdateProfile")]
        public async Task<string> UpdateProfile(ProfileManagamentVM _profileManagamentVM)
        {
            if (_profileManagamentVM.isTypeSave == 0)
            {
                _profileManagamentVM.User_PassReset = LibraryFunc.RandomNumber(100000, 999999).ToString();

                _profileManagamentVM.User_Password = LibraryFunc.GennerateToMD5(_profileManagamentVM.User_PassReset);
            }

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@typeView", _profileManagamentVM.isTypeSave);

                parm.Add("@ckcontractextension", _profileManagamentVM.ckContractExtension);
                parm.Add("@ckjob", _profileManagamentVM.ckJob);
                parm.Add("@cksal", _profileManagamentVM.ckSal);
                parm.Add("@eserial", _profileManagamentVM.Eserial);
                parm.Add("@lastname", _profileManagamentVM.LastName);
                parm.Add("@middlename", _profileManagamentVM.MiddleName);
                parm.Add("@firstname ", _profileManagamentVM.FirstName);
                parm.Add("@birthday ", _profileManagamentVM.Birthday);
                parm.Add("@placeofbirthday", _profileManagamentVM.PlaceOfBirth);
                parm.Add("@gender", _profileManagamentVM.Gender);
                parm.Add("@idcard", _profileManagamentVM.IDCard);
                parm.Add("@dateofissue", _profileManagamentVM.DateOfIssue);
                parm.Add("@placeofissue", _profileManagamentVM.PlaceOfIssue);
                parm.Add("@nationality", _profileManagamentVM.CountryCode);
                parm.Add("@ethnic", _profileManagamentVM.EthnicID);
                parm.Add("@hometown", _profileManagamentVM.Hometown);
                parm.Add("@qualification", _profileManagamentVM.Qualification);
                parm.Add("@tel", _profileManagamentVM.Mobile);
                parm.Add("@email", _profileManagamentVM.Email);
                parm.Add("@resident", _profileManagamentVM.Resident);
                parm.Add("@temporarity", _profileManagamentVM.Temporarity);
                parm.Add("@pittaxcode", _profileManagamentVM.PITTaxCode);
                parm.Add("@taxdept", _profileManagamentVM.TaxDept);
                parm.Add("@visanum", _profileManagamentVM.VisaNumber);
                parm.Add("@visaexp", _profileManagamentVM.VisaExpDate);
                parm.Add("@contact_name", _profileManagamentVM.Contact_Name);
                parm.Add("@contact_rela", _profileManagamentVM.Contact_Rela);
                parm.Add("@contact_tel", _profileManagamentVM.Contact_Tel);
                parm.Add("@contact_address", _profileManagamentVM.Contact_Address);
                parm.Add("@joindate", _profileManagamentVM.JoinDate);
                parm.Add("@startdayal", _profileManagamentVM.StartDayAL);
                parm.Add("@worktype ", _profileManagamentVM.WorkTypeID);
                parm.Add("@division", _profileManagamentVM.DivisionID);
                parm.Add("@department", _profileManagamentVM.DepartmentID);
                parm.Add("@section", _profileManagamentVM.SectionID);
                parm.Add("@position", _profileManagamentVM.PositionID);
                parm.Add("@startcontractdate", _profileManagamentVM.StartContractDate);
                parm.Add("@contracttype", _profileManagamentVM.ContractTypeID);
                parm.Add("@endcontractdate", _profileManagamentVM.EndContractDate);
                parm.Add("@shift", _profileManagamentVM.ShiftID);
                parm.Add("@jobstartdate", _profileManagamentVM.JobStartDate);
                parm.Add("@bankaccount", _profileManagamentVM.BankAccount);
                parm.Add("@bankcode", _profileManagamentVM.BankCode);
                parm.Add("@bankname", _profileManagamentVM.BankName);
                parm.Add("@bankbranch", _profileManagamentVM.BankBranch);
                parm.Add("@timeattcode", _profileManagamentVM.TimeAttCode);
                parm.Add("@socialins", _profileManagamentVM.SocialInsNumber);
                parm.Add("@healthins", _profileManagamentVM.HealthInsNumber);
                parm.Add("@emailcompany", _profileManagamentVM.EmailCompany);
                parm.Add("@basicsalary", _profileManagamentVM.BasicSalary);
                parm.Add("@othersalary", _profileManagamentVM.OtherSalary);
                parm.Add("@benefit1", _profileManagamentVM.Benefit1);
                parm.Add("@benefit2", _profileManagamentVM.Benefit2);
                parm.Add("@benefit3", _profileManagamentVM.Benefit3);
                parm.Add("@benefit4", _profileManagamentVM.Benefit4);
                parm.Add("@benefit5", _profileManagamentVM.Benefit5);
                parm.Add("@benefit6", _profileManagamentVM.Benefit6);
                parm.Add("@benefit7", _profileManagamentVM.Benefit7);
                parm.Add("@benefit8", _profileManagamentVM.Benefit8);
                parm.Add("@beginsalary", _profileManagamentVM.BeginSalaryDate);
                parm.Add("@ispaybank", _profileManagamentVM.SalaryByBank);
                parm.Add("@ispaybymonth", _profileManagamentVM.IsPayByMonth);
                parm.Add("@ispaybydate", _profileManagamentVM.IsPayByDate);
                parm.Add("@reason", _profileManagamentVM.Reason);
                parm.Add("@approvedby", _profileManagamentVM.ApprovedBy);
                parm.Add("@imageurl", _profileManagamentVM.UrlAvatar == null ? UrlDirectory.Default_Avatar : _profileManagamentVM.UrlAvatar);
                parm.Add("@role", _profileManagamentVM.PermisId);
                parm.Add("@pass", _profileManagamentVM.User_Password);
                parm.Add("@passreset", _profileManagamentVM.User_PassReset);

                parm.Add("@Appraiser_Eserial", _profileManagamentVM.Appraiser_Eserial);
                parm.Add("@DirectManager_Eserial", _profileManagamentVM.DirectManager_Eserial);
                parm.Add("@ControlDept_Eserial", _profileManagamentVM.ControlDept_Eserial);
                parm.Add("@Approve_Eserial", _profileManagamentVM.Approve_Eserial);

                parm.Add("@UserID", _profileManagamentVM.UserID);

                var eserial = await conn.ExecuteScalarAsync<string>("HR.Profile_update", parm, commandType: CommandType.StoredProcedure);

                return eserial;
            }
        }

        [HttpGet("UpdateUrlAvatar")]
        public async Task<bool> UpdateUrlAvatar(string _Eserial, string _UrlAvatar)
        {
            var sql = "Update HR.Profile set UrlAvatar=@UrlAvatar where Eserial=@Eserial";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, new { Eserial = _Eserial, UrlAvatar = _UrlAvatar });
                return true;
            }
        }

        [HttpGet("GetProfileHistory")]
        public async Task<List<ProfileManagamentVM>> GetProfileHistory(string _Eserial)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();

                parm.Add("@Eserial", _Eserial);

                var result = await conn.QueryAsync<ProfileManagamentVM>("HR.Profile_viewProfileHistory", parm, commandType: CommandType.StoredProcedure);

                return result.AsList();
            }
        }

        [HttpPost("GetSalaryDef")]
        public async Task<List<SalaryDefVM>> GetSalaryDef()
        {
            var sql = "select * from HR.SalaryDef";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<SalaryDefVM>(sql);
                return result.AsList();
            }
        }

        [HttpGet("CheckContainsEserial/{_Eserial}")]
        public async Task<bool> CheckContainsEserial(string _Eserial)
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
        public async Task<IEnumerable<CountryVM>> GetCountryList()
        {
            var sql = "select CountryCode, CountryName from HR.Country";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<CountryVM>(sql);
                return result;
            }
        }

        [HttpGet("GetEthnicList")]
        public async Task<IEnumerable<EthnicVM>> GetEthnicList()
        {
            var sql = "select EthnicID, EthnicName from HR.Ethnic";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<EthnicVM>(sql);
                return result;
            }
        }

        [HttpGet("CkUpdateJobHistory/{_Eserial}")]
        public async Task<bool> CkUpdateJobHistory(string _Eserial)
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
        public async Task<bool> CkUpdateSalHistory(string _Eserial)
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
        public async Task<bool> ResetPass(ProfileManagamentVM _profileManagamentVM)
        {
            _profileManagamentVM.User_PassReset = LibraryFunc.RandomNumber(100000, 999999).ToString();

            _profileManagamentVM.User_Password = LibraryFunc.GennerateToMD5(_profileManagamentVM.User_PassReset);

            _profileManagamentVM.User_isChangePass = 0;

            var sql = "Update HR.Profile set User_Password = @User_Password, User_PassReset = @User_PassReset, User_isChangePass = @User_isChangePass where Eserial = @Eserial";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _profileManagamentVM);
                return true;
            }
        }

        [HttpPost("DelProfileHistory")]
        public async Task<bool> DelProfileHistory(ProfileManagamentVM _profileManagamentVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                //Check del JOB
                var sqlCheckDelJob = "select 1 from HR.JobSalHistory where Eserial=@Eserial and JobID=@JobID and SalID=@SalaryID and (SUBSTRING(RIGHT(AdjustProfileID,5),3,1) > 0 or SUBSTRING(RIGHT(AdjustProfileID,5),1,1) > 0) ";
                if (await conn.ExecuteScalarAsync<int>(sqlCheckDelJob, _profileManagamentVM) == 1)
                {
                    var sqlDelJob = "Delete from HR.JobHistory where Eserial=@Eserial and JobID=@JobID ";
                    await conn.ExecuteAsync(sqlDelJob, _profileManagamentVM);

                    var sqlUpdateJobActive = "Update HR.JobHistory set CurrentJobID = 1 where Eserial = @Eserial and JobID in (select MAX(JobID) from HR.JobHistory where Eserial = @Eserial)";
                    await conn.ExecuteAsync(sqlUpdateJobActive, _profileManagamentVM);
                }

                //Check del SAL
                var sqlCheckDelSal = "select 1 from HR.JobSalHistory where Eserial=@Eserial and SalID=@SalaryID and JobID=@JobID and SUBSTRING(RIGHT(AdjustProfileID,5),5,1) > 0 ";
                if (await conn.ExecuteScalarAsync<int>(sqlCheckDelSal, _profileManagamentVM) == 1)
                {
                    var sqlDelSal = "Delete from HR.SalaryHistory where Eserial=@Eserial and SalaryID=@SalaryID";
                    await conn.ExecuteAsync(sqlDelSal, _profileManagamentVM);

                    var sqlUpdateSalActive = "Update HR.SalaryHistory set isActive = 1 where Eserial = @Eserial and SalaryID in (select MAX(SalaryID) from HR.SalaryHistory where Eserial = @Eserial)";
                    await conn.ExecuteAsync(sqlUpdateSalActive, _profileManagamentVM);
                }

                //Del JobSalHistory
                var sqlDelJobSalHistory = "Delete from HR.JobSalHistory where Eserial=@Eserial and JobID=@JobID and SalID=@SalaryID ";
                await conn.ExecuteAsync(sqlDelJobSalHistory, _profileManagamentVM);

                //Del LogPrintAgreementText
                var sqlDelLogPrintAgreementText = "Delete from HR.LogPrintAgreementText where Eserial=@Eserial and JobID=@JobID and SalID=@SalaryID ";
                await conn.ExecuteAsync(sqlDelLogPrintAgreementText, _profileManagamentVM);

                return true;
            }
        }

        [HttpGet("DelProfile")]
        public async Task<bool> DelProfile(string _Eserial)
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
        public async Task<bool> TerminateProfile(ProfileManagamentVM _profileManagamentVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@eserial", _profileManagamentVM.Eserial);
                parm.Add("@terminatedate", _profileManagamentVM.TerminateDate);
                parm.Add("@reasonterminate", _profileManagamentVM.ReasonTerminate);
                parm.Add("@UserID", _profileManagamentVM.UserID);

                await conn.ExecuteAsync("HR.Profile_terminate", parm, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        [HttpGet("RestoreTerminateProfile/{_Eserial}/{_UserID}")]
        public async Task<bool> RestoreTerminateProfile(string _Eserial, string _UserID)
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
        public async Task<IEnumerable<FuncVM>> GetFuncGroupPermis(string _Eserial)
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
                return result;
            }
        }

        [HttpGet("GetFuncPermis/{_Eserial}")]
        public async Task<IEnumerable<FuncVM>> GetFuncPermis(string _Eserial)
        {
            var sql = "select f.FuncGrpID, f.FuncID, FuncName, case when coalesce(pf.FuncID,'') != '' then 1 else 0 end as IsChecked from SYSTEM.Func f ";
            sql += "left join (select * from SYSTEM.PermissionFunc where UserID=@Eserial) pf on pf.FuncID = f.FuncID ";
            sql += "where isActive = 1 order by FNo ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<FuncVM>(sql, new { Eserial = _Eserial });
                return result;
            }
        }

        [HttpGet("GetSubFuncPermis/{_Eserial}")]
        public async Task<IEnumerable<FuncVM>> GetSubFuncPermis(string _Eserial)
        {
            var sql = "select f.FuncGrpID, sf.FuncID, sf.SubFuncID, SubFuncName, case when coalesce(pfs.SubFuncID,'') != '' then 1 else 0 end as IsChecked from SYSTEM.SubFunc sf ";
            sql += "join (select * from SYSTEM.Func where isActive = 1) f on f.FuncID = sf.FuncID ";
            sql += "left join (select * from SYSTEM.PermissionSubFunc where UserID=@Eserial) pfs on pfs.SubFuncID = sf.SubFuncID order by SubNo ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<FuncVM>(sql, new { Eserial = _Eserial });
                return result;
            }
        }

        [HttpGet("GetDivisionPermis/{_Eserial}")]
        public async Task<IEnumerable<DepartmentVM>> GetDivisionPermis(string _Eserial)
        {
            var sql = "select di.DivisionID, di.DivisionName, case when sum(case when coalesce(pd.DepartmentID,'') != '' then 1 else 0 end) > 0 then 1 else 0 end as IsChecked  from HR.Division di ";
            sql += "left join (select * from SYSTEM.PermissionDepartment where UserID=@Eserial) pd on pd.DivisionID = di.DivisionID group by di.DivisionID, di.DivisionName ";
            sql += "order by DivisionID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<DepartmentVM>(sql, new { Eserial = _Eserial });
                return result;
            }
        }

        [HttpGet("GetDepartmentPermis/{_Eserial}")]
        public async Task<IEnumerable<DepartmentVM>> GetDepartmentPermis(string _Eserial)
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
                return result;
            }
        }

        [HttpGet("GetRptGrpPermis/{_Eserial}")]
        public async Task<IEnumerable<SysRptVM>> GetSysReportGroupPermis(string _Eserial)
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
                return result;
            }
        }

        [HttpGet("GetRptPermis/{_Eserial}")]
        public async Task<IEnumerable<SysRptVM>> GetSysReportPermis(string _Eserial)
        {
            var sql = "select r.RptGrpID, r.RptID, r.RptName, case when coalesce(pr.RptID,'') != '' then 1 else 0 end as IsChecked from SYSTEM.Rpt r ";
            sql += "left join (select * from SYSTEM.PermissionRpt where UserID=@Eserial) pr on pr.RptID = r.RptID  ";
            sql += "order by r.RptGrpID, r.RptName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<SysRptVM>(sql, new { Eserial = _Eserial });
                return result;
            }
        }

        [HttpGet("UpdatePermis/{_funcVMs}/{_subFuncVMs}/{_departmentVMs}/{_sysRptVMs}/{_Eserial}")]
        public async Task<bool> UpdatePermis(IEnumerable<FuncVM> _funcVMs, [FromRouteAttribute] IEnumerable<FuncVM> _subFuncVMs, [FromRouteAttribute] IEnumerable<DepartmentVM> _departmentVMs, [FromRouteAttribute] IEnumerable<SysRptVM> _sysRptVMs, string _Eserial)
        {
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
        public async Task<IEnumerable<EmployeeTransactionVM>> GetSalTrnGrp(string _Eserial)
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
                return result;
            }
        }

        [HttpGet("GetSalTrnCode/{_Eserial}")]
        public async Task<IEnumerable<EmployeeTransactionVM>> GetSalTrnCode(string _Eserial)
        {
            var sql = "select st.SalTrnID, stg.TrnGroupCode, stg.TrnGroupName, st.TrnCode, st.TrnSubCode, st.TrnName, case when coalesce(et.SalTrnID,0) != 0 then 1 else 0 end as IsChecked from HR.SalaryTransactionCode st ";
            sql += "join HR.SalaryTransactionGroup stg  on stg.TrnGroupCode = st.TrnGroupCode ";
            sql += "left join (select * from HR.EmployeeTransaction where Eserial = @Eserial) et on et.SalTrnID = st.SalTrnID";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<EmployeeTransactionVM>(sql, new { Eserial = _Eserial });
                return result;
            }
        }

        [HttpGet("UpdateEmplTrn/{_salTrnCodes}/{_Eserial}")]
        public async Task<bool> UpdateEmplTrn(IEnumerable<EmployeeTransactionVM> _salTrnCodes, string _Eserial)
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
        public async Task<IEnumerable<ContractTypeVM>> GetContractTypeGroupList()
        {
            var sql = "select * from HR.ContractTypeGroup ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ContractTypeVM>(sql);
                return result;
            }
        }

        [HttpGet("GetContractTypeList")]
        public async Task<IEnumerable<ContractTypeVM>> GetContractTypeList()
        {
            var sql = "select * from HR.ContractType ct join HR.ContractTypeGroup ctg on ctg.ContractTypeGroupID = ct.ContractTypeGroupID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ContractTypeVM>(sql);
                return result;
            }
        }

        [HttpGet("GetNumMonthLC/{_ContractTypeID}")]
        public async Task<int> GetNumMonthLC(string _ContractTypeID)
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
        public async Task<bool> ContainsContractTypeID(string id)
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
        public async Task<int> UpdateContractType(ContractTypeVM _contractTypeVM)
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
        public async Task<IEnumerable<WorkTypeVM>> GetWorkTypeList()
        {
            var sql = "select * from HR.WorkType";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<WorkTypeVM>(sql);
                return result;
            }
        }

        [HttpGet("ContainsWorkTypeID/{id}")]
        public async Task<bool> ContainsWorkTypeID(string id)
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
        public async Task<int> UpdateWorkType(WorkTypeVM _workTypeVM)
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
        public async Task<IEnumerable<ProfileRelationshipVM>> GetRelationshipList()
        {
            var sql = "select * from HR.Relationship ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ProfileRelationshipVM>(sql);
                return result;
            }
        }

        [HttpGet("GetProfileRelationshipList/{_Eserial}")]
        public async Task<IEnumerable<ProfileRelationshipVM>> GetProfileRelationshipList(string _Eserial)
        {
            var sql = "select * from HR.ProfileRelationship pr join HR.Relationship r on r.RelationshipID = pr.RelationshipID where Eserial=@Eserial ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ProfileRelationshipVM>(sql, new { Eserial = _Eserial });
                return result;
            }
        }

        [HttpPost("UpdateProfileRelationship")]
        public async Task<bool> UpdateProfileRelationship(ProfileRelationshipVM _profileRelationshipVM)
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
        public async Task<DataTable> GetEmplChangeList(FilterHrVM _filterHrVM)
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
