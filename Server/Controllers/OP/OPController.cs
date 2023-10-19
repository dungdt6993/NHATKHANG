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
            var sql = "select * from OP.CruiseStatus order by CruiseStatusCode ";
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

        //Vehicle
        [HttpPost("GetVehicles")]
        public async Task<ActionResult<IEnumerable<VehicleVM>>> GetVehicles(FilterVM _filterVM)
        {
            var sql = "select * from OP.Vehicle v join HR.Department de on de.DepartmentID = v.DepartmentID where de.DivisionID=@DivisionID order by VehicleCode ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<VehicleVM>(sql, _filterVM);
                return Ok(result);
            }
        }

        [HttpGet("CheckContainsVehicleCode/{id}")]
        public async Task<bool> CheckContainsVehicleCode(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM OP.Vehicle where VehicleCode = @VehicleCode) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { VehicleCode = id });
            }
        }

        [HttpPost("UpdateVehicle")]
        public async Task<ActionResult<int>> UpdateVehicle(VehicleVM _vehicleVM)
        {
            var sql = "";
            if (_vehicleVM.IsTypeUpdate == 0)
            {
                sql = "Insert into OP.Vehicle (VehicleCode, VehicleName, DepartmentID, VehicleActive) Values (@VehicleCode,@VehicleName, @DepartmentID, @VehicleActive) ";
            }
            if (_vehicleVM.IsTypeUpdate == 1)
            {
                sql = "Update OP.Vehicle set VehicleName = @VehicleName, DepartmentID = @DepartmentID, VehicleActive = @VehicleActive where VehicleCode = @VehicleCode ";
            }
            if (_vehicleVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from DOC.Document where DepartmentID=@VehicleCode) ";
                sql += "begin ";
                sql += "delete from OP.Vehicle where VehicleCode = @VehicleCode ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteAsync(sql, _vehicleVM); ;
            }
        }

        //VehicleSchedule
        [HttpPost("GetVehicleSchedules")]
        public async Task<ActionResult<IEnumerable<VehicleScheduleVM>>> GetVehicleSchedules(FilterVM _filterVM)
        {
            var sql = "select * from OP.VehicleSchedule ts join OP.Vehicle t on t.VehicleCode = ts.VehicleCode where dDate=format(@dDate,'yyyy-MM-dd') ";
            sql += "order by ShiftID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<VehicleScheduleVM>(sql, _filterVM);
                return Ok(result);
            }
        }

        [HttpPost("UpdateVehicleShift")]
        public async Task<ActionResult<bool>> UpdateVehicleShift(VehicleScheduleVM _vehicleScheduleVM)
        {
            var sql = "Update OP.VehicleSchedule set ShiftID = @ShiftID where VehicleCode = @VehicleCode and dDate=format(@dDate,'yyyy-MM-dd') ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _vehicleScheduleVM);
            }
            return true;
        }

        [HttpPost("UpdateVehicleStatus")]
        public async Task<ActionResult<bool>> UpdateVehicleStatus(VehicleScheduleVM _vehicleScheduleVM)
        {
            var sql = "Update OP.VehicleSchedule set VehicleStatus = @VehicleStatus, VehicleStatusTimeUpdate=GETDATE() where VehicleCode = @VehicleCode and dDate=format(@dDate,'yyyy-MM-dd') ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _vehicleScheduleVM);
            }
            return true;
        }

    }
}
