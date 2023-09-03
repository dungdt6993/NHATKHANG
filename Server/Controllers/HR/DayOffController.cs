using Data.Infrastructure;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Utilities.AmLich;

namespace D69soft.Server.Controllers.HR
{
    [Route("api/[controller]")]
    [ApiController]
    public class DayOffController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public DayOffController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        [HttpPost("GetDayOffList")]
        public async Task<ActionResult<List<DayOffVM>>> GetDayOffList(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Y", _filterHrVM.Year);
                parm.Add("@M", _filterHrVM.Month);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@SectionID", _filterHrVM.SectionID);
                parm.Add("@DeptID", _filterHrVM.DepartmentID);
                parm.Add("@arrPos", _filterHrVM.PositionGroupID);
                parm.Add("@Eserial", _filterHrVM.Eserial);
                parm.Add("@DayOffType", _filterHrVM.ShiftID);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryAsync<DayOffVM>("HR.DayOff_viewMain", parm, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        [HttpGet("GetDayOffTypeList")]
        public async Task<ActionResult<IEnumerable<ShiftVM>>> GetDayOffTypeList()
        {
            var sql = "select * from HR.ShiftType where isOFF=1 and ShiftTypeID in ('AL','DO','PH') order by ShiftTypeID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ShiftVM>(sql);
                return Ok(result);
            }
        }

        [HttpPost("DayOff_calcAL")]
        public async Task<ActionResult<bool>> DayOff_calcAL(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Y", _filterHrVM.Year);
                parm.Add("@M", _filterHrVM.Month);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@Eserial", _filterHrVM.Eserial);

                await conn.ExecuteAsync("HR.DayOff_calcAL", parm, commandType: CommandType.StoredProcedure);
            }
            return true;
        }

        [HttpPost("DayOff_calcDO")]
        public async Task<ActionResult<bool>> DayOff_calcDO(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Y", _filterHrVM.Year);
                parm.Add("@M", _filterHrVM.Month);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@Eserial", _filterHrVM.Eserial);

                await conn.ExecuteAsync("HR.DayOff_calcDO", parm, commandType: CommandType.StoredProcedure);
            }
            return true;
        }

        [HttpPost("DayOff_calcPH")]
        public async Task<ActionResult<bool>> DayOff_calcPH(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Y", _filterHrVM.Year);
                parm.Add("@M", _filterHrVM.Month);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@Eserial", _filterHrVM.Eserial);

                await conn.ExecuteAsync("HR.DayOff_calcPH", parm, commandType: CommandType.StoredProcedure);
            }
            return true;
        }

        [HttpPost("DayOff_calcControlDAYOFF")]
        public async Task<ActionResult<bool>> DayOff_calcControlDAYOFF(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Y", _filterHrVM.Year);
                parm.Add("@M", _filterHrVM.Month);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@Eserial", _filterHrVM.Eserial);

                await conn.ExecuteAsync("HR.DayOff_calcControlDAYOFF", parm, commandType: CommandType.StoredProcedure);
            }
            return true;
        }

        [HttpPost("DayOff_calcDODefault/{_typeCalc}")]
        public async Task<ActionResult<bool>> DayOff_calcDODefault(FilterHrVM _filterHrVM, int _typeCalc)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Y", _filterHrVM.Year);
                parm.Add("@M", _filterHrVM.Month);
                parm.Add("@typeCalc", _typeCalc);

                await conn.ExecuteAsync("HR.DayOff_calcDODefault", parm, commandType: CommandType.StoredProcedure);
            }
            return true;
        }

        [HttpPost("DayOff_calcPHDefault/{_typeCalc}")]
        public async Task<ActionResult<bool>> DayOff_calcPHDefault(FilterHrVM _filterHrVM, int _typeCalc)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = string.Empty;

                if(_typeCalc == 1)
                {
                    sql += "delete from HR.PublicHolidayTransfer where PHYear=@Year ";
                }

                sql += "IF NOT EXISTS (select * from HR.PublicHolidayTransfer where PHYear=@Year) ";
                sql += "BEGIN ";

                sql += "delete from HR.PublicHolidayTransfer where PHYear=@Year ";

                sql += "Insert into HR.PublicHolidayTransfer (PHYear,PHDateLunar,NumDay) ";
                sql += "select @Year,cast(case when PHMonth = 12 then @Year-1 else @Year end as varchar) +'-'+cast(PHMonth as varchar)+'-'+cast(PHDay as varchar), NumDay from HR.PublicHolidayDef where isLunar=1 ";

                sql += "Insert into HR.PublicHolidayTransfer (PHYear,PHDateSolar,PHDate,NumDay) ";
                sql += "select @Year,cast(@Year as varchar) +'-'+cast(PHMonth as varchar)+'-'+cast(PHDay as varchar),cast(@Year as varchar) +'-'+cast(PHMonth as varchar)+'-'+cast(PHDay as varchar), NumDay from HR.PublicHolidayDef where isLunar=0 ";

                sql += "select format(PHDateLunar,'dd') as ddPHLunar, format(PHDateLunar,'MM') as MMPHLunar, format(PHDateLunar,'yyyy') as yyyyPHLunar, PHDateLunar from HR.PublicHolidayTransfer where PHYear=@Year and coalesce(PHDateLunar,'') <> '' order by PHDateLunar ";

                sql += "END ";

                IEnumerable<DayOffVM> lunars;

                lunars = await conn.QueryAsync<DayOffVM>(sql, _filterHrVM);
                if(lunars.Count() != 0)
                {
                    string sqlUpdateLunarToSolar = "";
                    foreach (var lunar in lunars)
                    {
                        Lunar l = new Lunar();
                        l.lunarDay = Int32.Parse(lunar.ddPHLunar);
                        l.lunarMonth = Int32.Parse(lunar.MMPHLunar);
                        l.lunarYear = Int32.Parse(lunar.yyyyPHLunar);
                        l.isleap = false;

                        Solar s = LunarSolarConverter.LunarToSolar(l);

                        var solarString = s.solarYear + "-" + s.solarMonth + "-" + s.solarDay;

                        sqlUpdateLunarToSolar += "Update HR.PublicHolidayTransfer set PHDate = '" + solarString + "' where PHDateLunar = '" + lunar.PHDateLunar.ToString("yyyy-MM-dd") + "' ";
                    }
                    await conn.ExecuteAsync(sqlUpdateLunarToSolar);
                }
            }
            return true;
        }

        [HttpPost("UpdateAddBalance")]
        public async Task<ActionResult<bool>> UpdateAddBalance(DayOffVM _shiftVM)
        {
            var sql = "";
            if (_shiftVM.dayOffType == "AL")
            {
                sql += "Update HR.ALMonth set ALAddBalance = @ALAddBalance, ALNoteAddBalance = @ALNoteAddBalance, ALUserAddBalance = @UserID, ALTimeAddBalance = GETDATE() where Period=@Period and Eserial=@Eserial ";
            }
            if (_shiftVM.dayOffType == "DO")
            {
                sql += "Update HR.CLDOMonth set CLDOAddBalance = @CLDOAddBalance, CLDONoteAddBalance = @CLDONoteAddBalance, CLDOUserAddBalance=@UserID, CLDOTimeAddBalance=GETDATE() where Period=@Period and Eserial=@Eserial ";
            }
            if (_shiftVM.dayOffType == "PH")
            {
                sql += "Update HR.CLPHMonth set CLPHAddBalance = @CLPHAddBalance, CLPHNoteAddBalance = @CLPHNoteAddBalance, CLPHUserAddBalance=@UserID, CLPHTimeAddBalance=GETDATE() where Period=@Period and Eserial=@Eserial ";
            }

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _shiftVM);
                return true;
            }
        }

        [HttpGet("GetDayOffDetail/{_Period}/{_Eserial}")]
        public async Task<ActionResult<IEnumerable<DayOffVM>>> GetDayOffDetail(int _Period, string _Eserial)
        {
            var sql = "select N'Nghỉ tuần (DO)' as dayOffType, DOCurrent, DOTaken, DOBalance from HR.DOMonth where Period=@Period and Eserial=@Eserial ";
            sql += "union all ";
            sql += "select N'Nghỉ bù tuần (CLDO)', CLDOTotal, CLDOTaken, CLDOBalance from HR.CLDOMonth where Period=@Period and Eserial=@Eserial ";
            sql += "union all ";
            sql += "select N'Nghỉ bù lễ tết (CLPH)', CLPHTotal, CLPHTaken, CLPHBalance from HR.CLPHMonth where Period=@Period and Eserial=@Eserial ";
            sql += "union all ";
            sql += "select N'Nghỉ phép năm (AL)', ALTotal, ALTaken, ALBalance from HR.ALMonth where Period=@Period and Eserial=@Eserial ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<DayOffVM>(sql, new {Period = _Period, Eserial = _Eserial});
                return Ok(result);
            }
        }

        //SpecialDayOff
        [HttpPost("GetSpecialDayOffList")]
        public async Task<ActionResult<List<DayOffVM>>> GetSpecialDayOffList(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Y", _filterHrVM.Year);
                parm.Add("@M", _filterHrVM.Month);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@SectionID", _filterHrVM.SectionID);
                parm.Add("@DeptID", _filterHrVM.DepartmentID);
                parm.Add("@arrPos", _filterHrVM.PositionGroupID);
                parm.Add("@Eserial", _filterHrVM.Eserial);
                parm.Add("@DayOffType", _filterHrVM.ShiftID);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryAsync<DayOffVM>("HR.DayOff_viewSpecialDayOffList", parm, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        //PH
        [HttpGet("GetPublicHolidayList")]
        public async Task<ActionResult<IEnumerable<PublicHolidayDefVM>>> GetPublicHolidayList()
        {
            var sql = "select * from HR.PublicHolidayDef order by PHName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<PublicHolidayDefVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("ContainsPublicHoliday/{_PHDay}/{_PHMonth}/{_isLunar}")]
        public async Task<ActionResult<bool>> ContainsPublicHoliday(int _PHDay, int _PHMonth, bool _isLunar)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.PublicHolidayDef where PHDay = @PHDay and PHMonth = @PHMonth and isLunar = @isLunar) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { PHDay = _PHDay, PHMonth = _PHMonth, isLunar = _isLunar });
            }
        }

        [HttpPost("UpdatePublicHoliday")]
        public async Task<ActionResult<bool>> UpdatePublicHoliday(PublicHolidayDefVM _publicHolidayDefVM)
        {
            var sql = "";
            if (_publicHolidayDefVM.IsTypeUpdate == 0)
            {
                sql += "Insert into HR.PublicHolidayDef(PHName,PHDay,PHMonth,NumDay,isLunar) Values (@PHName,@PHDay,@PHMonth,1,@isLunar) ";
            }
            if (_publicHolidayDefVM.IsTypeUpdate == 1)
            {
                sql += "Update HR.PublicHolidayDef set PHName = @PHName, PHDay = @PHDay, isLunar = @isLunar where PHDefID = @PHDefID ";
            }
            if (_publicHolidayDefVM.IsTypeUpdate == 2)
            {
                sql += "delete from HR.PublicHolidayDef where PHDefID = @PHDefID ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _publicHolidayDefVM);
                return true;
            }
        }

    }
}
