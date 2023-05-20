using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using Data.Infrastructure;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;

namespace D69soft.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SysController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public SysController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        //Log
        [HttpGet("InsertLogUserFunc/{_UserID}/{_FuncID}")]
        public async Task<ActionResult<bool>> InsertLogUserFunc(string _UserID, string _FuncID)
        {
            var sql = "Insert into SYSTEM.UserFuncLog(UserID, FuncID, TimeUserd) values(@UserID, @FuncID,GetDate()) ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, new { UserID = _UserID, FuncID = _FuncID });
            }
            return true;
        }

        //ErrorLog
        [HttpPost("ErrorLog")]
        public async Task<ActionResult<bool>> ErrorLog(ErrorLogVM _errorLogVM)
        {
            var sql = "Insert into SYSTEM.ErrorLog (ErrType, ErrMessage, ErrTime, ErrNote) ";
            sql += "Values (@ErrType,@ErrMessage,@ErrTime,@ErrNote)";

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _errorLogVM);
            }
            return true;
        }

        //Info User
        [HttpGet("GetInfoUser/{_UserID}")]
        public async Task<ActionResult<UserVM>> GetInfoUser(string _UserID)
        {
            var sql = "Select p.Eserial, LastName, MiddleName, FirstName, UrlAvatar, di.DivisionID, di.DivisionName, de.DepartmentName, po.PositionName, s.JoinDate from HR.Profile p ";
            sql += "left join HR.Staff s on s.Eserial = p.Eserial ";
            sql += "left join (select * from HR.JobHistory where CurrentJobID=1) jh on jh.Eserial = p.Eserial ";
            sql += "left join HR.Division di on di.DivisionID = jh.DivisionID ";
            sql += "left join HR.Department de on de.DepartmentID = jh.DepartmentID ";
            sql += "left join HR.Position po on po.PositionID = jh.PositionID ";
            sql += "where p.Eserial = @UserID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryFirstOrDefaultAsync<UserVM>(sql, new { UserID = _UserID });
                return result;
            }
        }

        //Menu Func
        [HttpGet("GetModuleMenu/{_UserID}")]
        public async Task<ActionResult<IEnumerable<FuncVM>>> GetModuleMenu(string _UserID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@UserID", _UserID);
                parm.Add("@typeView", 1);

                var result = await conn.QueryAsync<FuncVM>("SYSTEM.viewFunc", parm, commandType: CommandType.StoredProcedure);

                return Ok(result);
            }
        }

        [HttpGet("GetFuncMenuGroup/{_UserID}")]
        public async Task<ActionResult<IEnumerable<FuncVM>>> GetFuncMenuGroup(string _UserID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@UserID", _UserID);
                parm.Add("@typeView", 2);

                var result = await conn.QueryAsync<FuncVM>("SYSTEM.viewFunc", parm, commandType: CommandType.StoredProcedure);

                return Ok(result);
            }
        }

        [HttpGet("GetFuncMenu/{_UserID}")]
        public async Task<ActionResult<IEnumerable<FuncVM>>> GetFuncMenu(string _UserID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@UserID", _UserID);
                parm.Add("@typeView", 3);

                var result = await conn.QueryAsync<FuncVM>("SYSTEM.viewFunc", parm, commandType: CommandType.StoredProcedure);

                return Ok(result);
            }
        }

        [HttpGet("CheckViewFuncMenuRpt/{_UserID}")]
        public async Task<ActionResult<bool>> CheckViewFuncMenuRpt(string _UserID)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (select 1 from SYSTEM.PermissionRpt where UserID=@UserID) THEN 1 ELSE 0 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { UserID = _UserID });
            }
        }

        [HttpGet("CheckAccessFunc/{_UserID}/{_FuncID}")]
        public async Task<ActionResult<bool>> CheckAccessFunc(string _UserID, string _FuncID)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (select 1 from SYSTEM.PermissionFunc where UserID = @UserID and FuncID=@FuncID) THEN 1 ELSE 0 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { UserID = _UserID, FuncID = _FuncID });
            }
        }

        [HttpGet("CheckPermisFunc/{_UserID}/{_SubFuncID}")]
        public async Task<ActionResult<bool>> CheckAccessSubFunc(string _UserID, string _SubFuncID)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (select 1 from SYSTEM.PermissionSubFunc where UserID = @UserID and SubFuncID=@SubFuncID) THEN 1 ELSE 0 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { UserID = _UserID, SubFuncID = _SubFuncID });
            }
        }

        //GlobalParameter
        [HttpGet("GetGlobalParameterList/{_FunID}")]
        public async Task<ActionResult<IEnumerable<GlobalParameterVM>>> GetGlobalParameterList(string _FunID)
        {
            var sql = "select * from SYSTEM.GlobalParameter where FuncID=@FuncID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<GlobalParameterVM>(sql, new { FuncID = _FunID });
                return Ok(result);
            }
        }

        [HttpPost("UpdateGlobalParameter")]
        public async Task<ActionResult<bool>> UpdateGlobalParameter(GlobalParameterVM _globalParameterVM)
        {
            var sql = "";
            if (_globalParameterVM.IsTypeUpdate == 1)
            {
                sql += "Update SYSTEM.GlobalParameter set ParaValues = @ParaValues, ParaStatus = @ParaStatus where ParaId = @ParaId ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _globalParameterVM);
                return true;
            }
        }

        //Common
        [HttpGet("GetPeriodFilter")]
        public async Task<ActionResult<IEnumerable<PeriodVM>>> GetPeriodFilter()
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@typeView", 3);

                var result = await conn.QueryAsync<PeriodVM>("HR.viewMonthYearPeriod", parm, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpGet("GetMonthFilter")]
        public async Task<ActionResult<IEnumerable<PeriodVM>>> GetMonthFilter()
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@typeView", 1);

                var result = await conn.QueryAsync<PeriodVM>("HR.viewMonthYearPeriod", parm, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpGet("GetYearFilter")]
        public async Task<ActionResult<IEnumerable<PeriodVM>>> GetYearFilter()
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@typeView", 2);

                var result = await conn.QueryAsync<PeriodVM>("HR.viewMonthYearPeriod", parm, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpPost("GetDayFilter")]
        public async Task<ActionResult<IEnumerable<PeriodVM>>> GetDayFilter(FilterHrVM _filterHrVM)
        {
            var date = _filterHrVM.Year + "-" + _filterHrVM.Month + "-" + "01";

            var sql = "WITH nums AS (SELECT 1 AS value UNION ALL SELECT value + 1 AS value FROM nums WHERE nums.value <= (SELECT Day(DATEADD(d,-1, DATEADD(mm, DATEDIFF(mm, 0 ,'" + date + "')+1, 0))))-1) SELECT value as [Day] FROM nums";

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<PeriodVM>(sql);
                return Ok(result);
            }
        }

        //RPT
        [HttpGet("GetModuleRpt/{_UserID}")]
        public async Task<ActionResult<List<SysRptVM>>> GetModuleRpt(string _UserID)
        {
            var sql = "Declare @roleUser int ";
            sql += "select @roleUser = User_Role from HR.Profile where Eserial=@UserID ";
            sql += "if (@roleUser>2) ";
            sql += "begin ";

            sql += "select distinct mo.ModuleID, mo.ModuleName from SYSTEM.Rpt r ";
            sql += "join SYSTEM.RptGrp rg on rg.RptGrpID = r.RptGrpID ";
            sql += "join (select * from SYSTEM.PermissionRpt where UserID=@UserID) pr on pr.RptID = r.RptID ";
            sql += "join SYSTEM.Module mo on mo.ModuleID = rg.ModuleID ";
            sql += "order by mo.ModuleName ";

            sql += "end ";

            sql += "begin ";

            sql += "select * from SYSTEM.Module order by ModuleName ";

            sql += "end ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<SysRptVM>(sql, new { UserID = _UserID });
                return result.ToList();
            }
        }

        [HttpGet("GetRptGrpByID/{_ModuleID}/{_UserID}")]
        public async Task<ActionResult<List<SysRptVM>>> GetRptGrpByID(string _ModuleID, string _UserID)
        {
            var sql = "Declare @roleUser int ";
            sql += "select @roleUser = User_Role from HR.Profile where Eserial=@UserID ";
            sql += "if (@roleUser>2) ";
            sql += "begin ";

            sql += "select distinct rg.RptGrpID,rg.RptGrpName from SYSTEM.Rpt r ";
            sql += "join (select * from SYSTEM.RptGrp where ModuleID=@ModuleID) rg on rg.RptGrpID = r.RptGrpID ";
            sql += "join (select * from SYSTEM.PermissionRpt where UserID=@UserID) pr on pr.RptID = r.RptID ";
            sql += "order by RptGrpName ";

            sql += "end ";

            sql += "begin ";

            sql += "select * from SYSTEM.RptGrp where ModuleID=@ModuleID order by RptGrpName ";

            sql += "end ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<SysRptVM>(sql, new { UserID = _UserID, ModuleID = _ModuleID });
                return result.ToList();
            }
        }

        [HttpGet("GetRptList/{_ModuleID}/{_RptGrptID}/{_UserID}")]
        public async Task<ActionResult<IEnumerable<SysRptVM>>> GetRptList(string _ModuleID, int _RptGrptID, string _UserID)
        {
            var sql = "select r.RptID, r.RptName, r.RptUrl, rg.RptGrpID, rg.RptGrpName from SYSTEM.Rpt r ";
            sql += "join SYSTEM.RptGrp rg on rg.RptGrpID = r.RptGrpID  ";
            sql += "join SYSTEM.PermissionRpt pr on pr.RptID = r.RptID ";
            sql += "where UserID=@UserID and rg.ModuleID=@ModuleID ";
            if (_RptGrptID != 0)
            {
                sql += " and rg.RptGrpID=@RptGrpID ";
            }
            sql += "order by rg.RptGrpID, r.RptName";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<SysRptVM>(sql, new { UserID = _UserID, ModuleID = _ModuleID, RptGrpID = _RptGrptID });
                return Ok(result);
            }
        }

        [HttpGet("GetRpt/{_RptID}")]
        public async Task<ActionResult<RptVM>> GetRpt(int _RptID)
        {
            var sql = "select rg.RptGrpID, rg.RptGrpName, r.RptID, r.RptName, r.RptUrl, r.PassUserID from SYSTEM.Rpt r ";
            sql += "join SYSTEM.RptGrp rg on rg.RptGrpID = r.RptGrpID ";
            sql += "where r.RptID=@RptID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryFirstAsync<RptVM>(sql, new { RptID = _RptID });
                return result;
            }
        }

        [HttpPost("UpdateRpt")]
        public async Task<ActionResult<bool>> UpdateRpt(RptVM _rptVM)
        {
            var sql = "";
            if (_rptVM.IsTypeUpdate == 0)
            {
                sql += "Insert into SYSTEM.Rpt (RptGrpID, RptName, RptUrl, PassUserID) Values (@RptGrpID,@RptName,@RptUrl,@PassUserID) ";
                sql += "Insert into SYSTEM.PermissionRpt (UserID, RptID) select @UserID, MAX(RptID) from SYSTEM.Rpt ";
            }
            else
            {
                sql += "Update SYSTEM.Rpt set RptGrpID = @RptGrpID, ";
                sql += "RptName = @RptName, RptUrl = @RptUrl, PassUserID = @PassUserID ";
                sql += "where RptID = @RptID ";
            }

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _rptVM);
                return true;
            }
        }

        [HttpGet("DelRpt/{_RptID}")]
        public async Task<ActionResult<bool>> DelRpt(int _RptID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "delete from SYSTEM.Rpt where RptID=@RptID ";
                await conn.ExecuteAsync(sql, new { RptID = _RptID });
                return true;
            }
        }

        [HttpGet("GetRptGrp/{_RptGrpID}")]
        public async Task<ActionResult<RptGrpVM>> GetRptGrp(int _RptGrpID)
        {
            var sql = "select * from SYSTEM.RptGrp ";
            sql += "where RptGrpID=@RptGrpID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryFirstAsync<RptGrpVM>(sql, new { RptGrpID = _RptGrpID });
                return result;
            }
        }

        [HttpPost("UpdateRptGrp")]
        public async Task<ActionResult<string>> UpdateRptGrp(RptGrpVM _rptGrpVM)
        {
            var sql = "";
            if (_rptGrpVM.IsTypeUpdate == 0)
            {
                sql += "Insert into SYSTEM.RptGrp (RptGrpName, ModuleID) Values (@RptGrpName,@ModuleID) ";
            }
            else
            {
                sql += "Update SYSTEM.RptGrp set RptGrpName = @RptGrpName, ModuleID = @ModuleID ";
                sql += "where RptGrpID = @RptGrpID ";
            }

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<string>(sql, _rptGrpVM);
            }
        }

        [HttpGet("DelRptGrp/{_RptGrpID}")]
        public async Task<ActionResult<bool>> DelRptGrp(int _RptGrpID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "delete from SYSTEM.RptGrp where RptGrpID=@RptGrpID ";
                await conn.ExecuteAsync(sql, new { RptGrpID = _RptGrpID });

                return true;
            }
        }

        [HttpGet("CheckPermisRpt/{_UserID}")]
        public async Task<ActionResult<bool>> CheckPermisRpt(string _UserID)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (select 1 from SYSTEM.PermissionRpt where UserID=@UserID) THEN 1 ELSE 0 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return Ok(await conn.ExecuteScalarAsync<bool>(sql, new { UserID = _UserID }));
            }
        }

    }
}
