using Data.Infrastructure;
using Dapper;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using System.Data.SqlClient;

namespace D69soft.Server.Controllers.HR
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public KPIController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        //Filter
        [HttpPost("GetMonthFilter")]
        public async Task<ActionResult<IEnumerable<PeriodVM>>> GetMonthFilter(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@typeView", 1);
                parm.Add("@Year", _filterHrVM.Year);

                var result = await conn.QueryAsync<PeriodVM>("KPI.viewMonthYearPeriodKPI", parm, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpPost("GetYearFilter")]
        public async Task<ActionResult<IEnumerable<PeriodVM>>> GetYearFilter(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@typeView", 2);
                parm.Add("@Year", _filterHrVM.Year);

                var result = await conn.QueryAsync<PeriodVM>("KPI.viewMonthYearPeriodKPI", parm, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpPost("GetDivisions")]
        public async Task<ActionResult<IEnumerable<DivisionVM>>> GetDivisions(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@sPeriod", _filterHrVM.Period);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryAsync<DivisionVM>("KPI.KPIs_viewDivision", parm, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpPost("GetDepartments")]
        public async Task<ActionResult<IEnumerable<DepartmentVM>>> GetDepartments(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@sPeriod", _filterHrVM.Period);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryAsync<DepartmentVM>("KPI.KPIs_viewDepartment", parm, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpPost("GetEserials")]
        public async Task<ActionResult<IEnumerable<EserialVM>>> GetEserials(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@sPeriod", _filterHrVM.Period);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@DeptID", _filterHrVM.DepartmentID);
                parm.Add("@arrPos", _filterHrVM.PositionGroupID);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryAsync<EserialVM>("KPI.KPIs_viewEserial", parm, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        //End Filter
        [HttpPost("GetKPIs")]
        public async Task<ActionResult<IEnumerable<KPIVM>>> GetKPIs(FilterHrVM _filterHrVM)
        {
            var sql = "select * from KPI.Management where Eserial=@Eserial and Period=@Period order by CriteriaGroupName, KPINo";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<KPIVM>(sql, _filterHrVM);
                return Ok(result);
            }
        }

        [HttpPost("GetRank")]
        public async Task<ActionResult<RankVM>> GetRank(FilterHrVM _filterHrVM)
        {
            var sql = "select r.Period, r.Eserial, p.LastName, p.MiddleName, p.FirstName, r.PositionID, p.LastName + ' ' + p.MiddleName + ' ' + p.FirstName as FullName, ";
            sql += "pAppraiser.LastName + ' ' + pAppraiser.MiddleName + ' ' + pAppraiser.FirstName as Appraiser_FullName, ";
            sql += "pDirectManager.LastName + ' ' + pDirectManager.MiddleName + ' ' + pDirectManager.FirstName as DirectManager_FullName, ";
            sql += "pControlDept.LastName + ' ' + pControlDept.MiddleName + ' ' + pControlDept.FirstName as ControlDept_FullName, ";
            sql += "pApprove.LastName + ' ' + pApprove.MiddleName + ' ' + pApprove.FirstName as Approve_FullName, ";
            sql += "po.PositionName, poAppraiser.PositionName as Appraiser_PositionName, ";
            sql += "poDirectManager.PositionName as DirectManager_PositionName, ";
            sql += "poControlDept.PositionName as ControlDept_PositionName, ";
            sql += "poApprove.PositionName as Approve_PositionName, r.isSendKPI, r.isSendAppraiser, r.isSendDirectManager, r.isSendControlDept, r.isSendApprove, ";
            sql += "r.TimeSendKPI, r. TimeSendAppraiser, r.TimeSendDirectManager, r.TimeSendControlDept, r.TimeSendApprove, ";
            sql += "r.StaffRanking, r.FinalRanking, r.TotalGrandStaffScore, r.TotalGrandFinalScore, r.StaffNote, r.ManagerNote, ";
            sql += "r.Appraiser_Eserial, r.DirectManager_Eserial, r.ControlDept_Eserial, r.Approve_Eserial ";
            sql += "from KPI.Rank r ";
            sql += "join HR.Profile p on p.Eserial = r.Eserial ";
            sql += "left join HR.Profile pAppraiser on pAppraiser.Eserial = r.Appraiser_Eserial ";
            sql += "left join HR.Profile pDirectManager on pDirectManager.Eserial = r.DirectManager_Eserial ";
            sql += "left join HR.Profile pControlDept on pControlDept.Eserial = r.ControlDept_Eserial ";
            sql += "left join HR.Profile pApprove on pApprove.Eserial = r.Approve_Eserial ";
            sql += "left join HR.Position po on po.PositionID = r.PositionID ";
            sql += "left join HR.Position poAppraiser on poAppraiser.PositionID = r.Appraiser_PositionID ";
            sql += "left join HR.Position poDirectManager on poDirectManager.PositionID = r.DirectManager_PositionID ";
            sql += "left join HR.Position poControlDept on poControlDept.PositionID = r.ControlDept_PositionID ";
            sql += "left join HR.Position poApprove on poApprove.PositionID = r.Approve_PositionID ";
            sql += "where r.Eserial=@Eserial and r.Period=@Period ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryFirstAsync<RankVM>(sql, _filterHrVM);
                return result;
            }
        }

        [HttpPost("GetRanks")]
        public async Task<ActionResult<List<RankVM>>> GetRanks(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@sPeriod", _filterHrVM.Period);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@DeptID", _filterHrVM.DepartmentID);
                parm.Add("@arrPos", _filterHrVM.PositionGroupID);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryAsync<RankVM>("KPI.KPIs_viewRanks", parm, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        [HttpPost("UpdateStaffScore")]
        public async Task<ActionResult<bool>> UpdateStaffScore(KPIVM _kpiVM)
        {
            var sql = "Update KPI.Management set StaffScore=@StaffScore where KPI_ID=@KPI_ID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _kpiVM);
            }

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Period", _kpiVM.Period);
                parm.Add("@Eserial", _kpiVM.Eserial);

                await conn.QueryAsync<DutyRosterVM>("KPI.KPIs_updateRank", parm, commandType: CommandType.StoredProcedure);

                await conn.QueryAsync(sql, _kpiVM);
                return true;
            }
        }

        [HttpPost("UpdateFinalScore")]
        public async Task<ActionResult<bool>> UpdateFinalScore(KPIVM _kpiVM)
        {
            var sql = "Update KPI.Management set FinalScore=@FinalScore where KPI_ID=@KPI_ID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _kpiVM);
            }

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Period", _kpiVM.Period);
                parm.Add("@Eserial", _kpiVM.Eserial);

                await conn.QueryAsync<DutyRosterVM>("KPI.KPIs_updateRank", parm, commandType: CommandType.StoredProcedure);

                await conn.QueryAsync(sql, _kpiVM);
                return true;
            }
        }

        [HttpPost("UpdateActualDescription")]
        public async Task<ActionResult<bool>> UpdateActualDescription(KPIVM _kpiVM)
        {
            var sql = "Update KPI.Management set ActualDescription=@ActualDescription where KPI_ID=@KPI_ID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _kpiVM);
            }

            return true;
        }

        [HttpPost("UpdateManagerComment")]
        public async Task<ActionResult<bool>> UpdateManagerComment(KPIVM _kpiVM)
        {
            var sql = "Update KPI.Management set ManagerComment=@ManagerComment where KPI_ID=@KPI_ID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _kpiVM);
            }

            return true;
        }

        [HttpPost("UpdateStaffNote")]
        public async Task<ActionResult<bool>> UpdateStaffNote(RankVM _rankVM)
        {
            var sql = "Update KPI.Rank set StaffNote=@StaffNote where Period=@Period and Eserial=@Eserial ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _rankVM);
            }

            return true;
        }

        [HttpPost("UpdateManagerNote")]
        public async Task<ActionResult<bool>> UpdateManagerNote(RankVM _rankVM)
        {
            var sql = "Update KPI.Rank set ManagerNote=@ManagerNote where Period=@Period and Eserial=@Eserial ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _rankVM);
            }

            return true;
        }

        [HttpPost("UpdateTarget")]
        public async Task<ActionResult<bool>> UpdateTarget(KPIVM _kpiVM)
        {
            var sql = "Update KPI.Management set DescriptionName=@DescriptionName where KPI_ID=@KPI_ID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _kpiVM);
            }

            return true;
        }

        [HttpPost("InitializeKPI/{_Eserial}")]
        public async Task<ActionResult<bool>> InitializeKPI(FilterHrVM _filterHrVM, string _Eserial)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@sPeriod", _filterHrVM.Period);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@Eserial", _Eserial);
                parm.Add("@UserID", _filterHrVM.UserID);

                await conn.QueryAsync<EserialVM>("KPI.KPIs_open", parm, commandType: CommandType.StoredProcedure);
            }

            return true;
        }

        [HttpPost("SendKPI/{_type}")]
        public async Task<ActionResult<bool>> SendKPI(RankVM _rankVM, string _type)
        {
            var sql = "";

            if (_type == "SendKPI")
            {
                sql = "Update KPI.Rank set isSendKPI=@isSendKPI, TimeSendKPI=@TimeSendKPI where Period=@Period and Eserial=@Eserial ";
            }

            if (_type == "SendAppraiser")
            {
                sql = "Update KPI.Rank set isSendAppraiser=@isSendAppraiser, TimeSendAppraiser=@TimeSendAppraiser where Period=@Period and Eserial=@Eserial ";
            }

            if (_type == "SendDirectManager")
            {
                sql = "Update KPI.Rank set isSendDirectManager=@isSendDirectManager, TimeSendDirectManager=@TimeSendDirectManager where Period=@Period and Eserial=@Eserial ";
            }

            if (_type == "SendControlDept")
            {
                sql = "Update KPI.Rank set isSendControlDept=@isSendControlDept, TimeSendControlDept=@TimeSendControlDept where Period=@Period and Eserial=@Eserial ";
            }

            if (_type == "SendApprove")
            {
                sql = "Update KPI.Rank set isSendApprove=@isSendApprove, TimeSendApprove=@TimeSendApprove where Period=@Period and Eserial=@Eserial ";
            }

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _rankVM);
            }

            return true;
        }

    }
}
