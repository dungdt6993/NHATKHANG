using Data.Infrastructure;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Model.ViewModels.OP;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.OP;
using D69soft.Shared.Models.ViewModels.SYSTEM;

namespace D69soft.Server.Controllers.OP
{
    [Route("api/[controller]")]
    [ApiController]
    public class OPController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public OPController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        [HttpPost("GetTenders")]
        public async Task<ActionResult<IEnumerable<TenderVM>>> GetTenders(FilterVM _filterVM)
        {
            var sql = "select * from OP.Tender where DivisionID=@DivisionID order by TenderCode ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<TenderVM>(sql, _filterVM);
                return Ok(result);
            }
        }

        //CruiseSchedule
        [HttpPost("GetCruiseSchedules")]
        public async Task<ActionResult<List<CruiseScheduleVM>>> GetCruiseSchedules(FilterVM _filterVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@M", _filterVM.Month);
                parm.Add("@Y", _filterVM.Year);
                parm.Add("@DivisionID", _filterVM.DivisionID);

                var result = await conn.QueryAsync<CruiseScheduleVM>("OP.CruiseSchedule_view", parm, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        [HttpGet("GetCruiseStatus")]
        public async Task<ActionResult<IEnumerable<CruiseStatusVM>>> GetCruiseStatus()
        {
            var sql = "select * from OP.CruiseStatus order by NumDay ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<CruiseStatusVM>(sql);
                return Ok(result);
            }
        }

        [HttpPost("UpdateCruiseSchedule")]
        public async Task<ActionResult<bool>> UpdateCruiseSchedule(CruiseScheduleVM _cruiseScheduleVM)
        {
            var sql = "Declare @sNumDay int ";
            sql += "select @sNumDay = NumDay from OP.CruiseStatus where CruiseStatusCode=@CruiseStatusCode ";
            sql += "Update OP.CruiseSchedule set isCI = 1, CruiseStatusCode = 'NC', GuestNumber=0, BudgetFoodCost=0 where CruiseCode=@CruiseCode and dDate>@dDate and dDate<DATEADD(d,@NumDay-1,format(@dDate,'yyyy-MM-dd')) ";
            sql += "Update OP.CruiseSchedule set isCI = 1, CruiseStatusCode = @CruiseStatusCode, GuestNumber=@GuestNumber, BudgetFoodCost=@BudgetFoodCost where CruiseCode=@CruiseCode and dDate=@dDate ";
            sql += "Update OP.CruiseSchedule set isCI = 0, CruiseStatusCode = @CruiseStatusCode, GuestNumber=@GuestNumber where CruiseCode=@CruiseCode and dDate>@dDate and dDate<DATEADD(d,@sNumDay-1,format(@dDate,'yyyy-MM-dd')) ";

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _cruiseScheduleVM);
            }
            return true;
        }

        //TenderSchedule
        [HttpPost("GetTenderSchedules")]
        public async Task<ActionResult<IEnumerable<TenderScheduleVM>>> GetTenderSchedules(FilterVM _filterVM)
        {
            var sql = "select * from OP.TenderSchedule ts join OP.Tender t on t.TenderCode = ts.TenderCode where dDate=format(@dDate,'yyyy-MM-dd') ";
            sql += "order by ShiftID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<TenderScheduleVM>(sql, _filterVM);
                return Ok(result);
            }
        }

        [HttpPost("UpdateTenderShift")]
        public async Task<ActionResult<bool>> UpdateTenderShift(TenderScheduleVM _tenderScheduleVM)
        {
            var sql = "Update OP.TenderSchedule set ShiftID = @ShiftID where TenderCode = @TenderCode and dDate=format(@dDate,'yyyy-MM-dd') ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _tenderScheduleVM);
            }
            return true;
        }

        [HttpPost("UpdateTenderStatus")]
        public async Task<ActionResult<bool>> UpdateTenderStatus(TenderScheduleVM _tenderScheduleVM)
        {
            var sql = "Update OP.TenderSchedule set TenderStatus = @TenderStatus, TenderStatusTimeUpdate=GETDATE() where TenderCode = @TenderCode and dDate=format(@dDate,'yyyy-MM-dd') ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _tenderScheduleVM);
            }
            return true;
        }

    }
}
