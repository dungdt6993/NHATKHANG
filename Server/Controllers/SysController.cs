using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using Data.Infrastructure;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using Newtonsoft.Json;

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

        //DataTable
        [HttpPost("ExecuteSQLQueryToDataTable")]
        public async Task<ActionResult<string>> ExecuteSQLQueryToDataTable([FromBody]string _sql)
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(_sql, conn);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(dt);
            }

            return JsonConvert.SerializeObject(dt);
        }

        //Log
        [HttpPost("InsertLog")]
        public async Task<ActionResult<bool>> InsertLog(LogVM _logVM)
        {
            var sql = "Insert into SYSTEM.Log (LogType, LogName, LogDesc, LogTime, LogNote, LogUser) ";
            sql += "Values (@LogType,@LogName,@LogDesc,GETDATE(),@LogNote,@LogUser)";

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _logVM);
            }
            return true;
        }

        [HttpGet("GetLog")]
        public async Task<ActionResult<List<LogVM>>> GetLog()
        {
            var sql = "select top 100 * from SYSTEM.Log order by LogTime desc ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<LogVM>(sql);
                return result.ToList();
            }
        }

        //Info User
        [HttpGet("GetInfoUser/{_UserID}")]
        public async Task<ActionResult<UserVM>> GetInfoUser(string _UserID)
        {
            var sql = "Select p.Eserial, LastName, MiddleName, FirstName, UrlAvatar, di.DivisionID, di.DivisionName, di.DivisionShortName, de.DepartmentName, po.PositionName, po.isLeader, s.JoinDate from HR.Profile p ";
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

        [HttpGet("CheckAccessSubFunc/{_UserID}/{_SubFuncID}")]
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
        public async Task<ActionResult<IEnumerable<PeriodVM>>> GetDayFilter(FilterVM _filterVM)
        {
            var date = _filterVM.Year + "-" + _filterVM.Month + "-" + "01";

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
        [HttpGet("GetRptList/{_RptID}/{_UserID}")]
        public async Task<ActionResult<IEnumerable<RptVM>>> GetRptList(int _RptID, string _UserID)
        {
            var sql = "select mo.ModuleID, mo.ModuleName, fg.FuncGrpIcon, fg.FuncGrpID, fg.FuncGrpName, r.RptID, r.RptName, r.RptUrl from SYSTEM.Rpt r ";
            sql += "join SYSTEM.FuncGrp fg on fg.FuncGrpID = r.FuncGrpID ";
            sql += "join SYSTEM.Module mo on mo.ModuleID = fg.ModuleID ";
            sql += "join SYSTEM.PermissionRpt pr on pr.RptID = r.RptID ";
            sql += "where UserID=@UserID and (r.RptID=@RptID or @RptID=0) ";
            sql += "order by mo.ModuleID, fg.FGNo, r.RptName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<RptVM>(sql, new { RptID = _RptID, UserID = _UserID});
                return Ok(result);
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

        [HttpGet("GetSysRptList")]
        public async Task<ActionResult<IEnumerable<RptVM>>> GetSysRptList()
        {
            var sql = "select * from SYSTEM.Rpt where coalesce(FuncGrpID,'') = '' order by RptName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<RptVM>(sql);
                return Ok(result);
            }
        }

    }
}
