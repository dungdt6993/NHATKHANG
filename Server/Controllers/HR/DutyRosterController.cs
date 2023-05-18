using Data.Infrastructure;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.HR;

namespace Data.Repositories.HR
{
    [Route("api/[controller]")]
    [ApiController]
    public class DutyRosterController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public DutyRosterController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        [HttpPost("GetEserialByID/{_UserID}")]
        public async Task<ActionResult<IEnumerable<ProfileVM>>> GetEserialByID(FilterHrVM _filterHrVM, string _UserID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@sPeriod", _filterHrVM.Year * 100 + _filterHrVM.Month);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@SectionID", _filterHrVM.SectionID);
                parm.Add("@DeptID", _filterHrVM.DepartmentID);
                parm.Add("@arrPos", _filterHrVM.PositionGroupID);
                parm.Add("@Eserial", _filterHrVM.Eserial);
                parm.Add("@UserID", _UserID);

                var result = await conn.QueryAsync<ProfileVM>("HR.DutyRoster_viewEserialMain", parm, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpPost("GetDutyRosterList")]
        public async Task<ActionResult<List<DutyRosterVM>>> GetDutyRosterList(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Y", _filterHrVM.Year);
                parm.Add("@M", _filterHrVM.Month);
                parm.Add("@D", _filterHrVM.Day);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@SectionID", _filterHrVM.SectionID);
                parm.Add("@DeptID", _filterHrVM.DepartmentID);
                parm.Add("@arrPos", _filterHrVM.PositionGroupID);
                parm.Add("@Eserial", _filterHrVM.Eserial);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryAsync<DutyRosterVM>("HR.DutyRoster_viewDutyRosterList", parm, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        [HttpPost("GetDutyRosterByDay/{_dutyRosterVM}")]
        public async Task<ActionResult<DutyRosterVM>> GetDutyRosterByDay(FilterHrVM _filterHrVM, DutyRosterVM _dutyRosterVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Y", _filterHrVM.Year);
                parm.Add("@M", _filterHrVM.Month);
                parm.Add("@D", _dutyRosterVM.dDate.Day);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@SectionID", _filterHrVM.SectionID);
                parm.Add("@DeptID", _filterHrVM.DepartmentID);
                parm.Add("@arrPos", _filterHrVM.PositionGroupID);
                parm.Add("@Eserial", _dutyRosterVM.Eserial);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryFirstAsync<DutyRosterVM>("HR.DutyRoster_viewDutyRosterList", parm, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        [HttpPost("GetLockDutyRoster")]
        public async Task<ActionResult<IEnumerable<LockDutyRosterVM>>> GetLockDutyRoster(FilterHrVM _filterHrVM)
        {
            var sql = "select de.DepartmentID, de.DepartmentName, rld.LockFrom, rld.LockTo from HR.Department de left join (select * from HR.LockDutyRoster where Period=@Period) rld on rld.DepartmentID = de.DepartmentID where de.isActive = 1 and de.DivisionID=@DivisionID ";

            if (_filterHrVM.DepartmentID != string.Empty)
            {
                sql += " and de.DepartmentID=@DepartmentID ";
            }

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<LockDutyRosterVM>(sql, new { Period = _filterHrVM.Year * 100 + _filterHrVM.Month, DivisionID = _filterHrVM.DivisionID, DepartmentID = _filterHrVM.DepartmentID });
                return result;
            }
        }

        [HttpPost("LockDutyRoster/{_UserID}")]
        public async Task<ActionResult<bool>> LockDutyRoster(LockDutyRosterVM _filterHrVM, string _UserID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sqlDel = "Delete from HR.LockDutyRoster where Period=@Period and DivisionID=@DivisionID ";
                if (_filterHrVM.DepartmentID != string.Empty)
                {
                    sqlDel += " and DepartmentID=@DepartmentID ";
                }
                else
                {
                    sqlDel += " and DepartmentID in (select DepartmentID from SYSTEM.PermissionDepartment where UserID='" + _UserID + "' and DivisionID=@DivisionID) ";
                }

                await conn.ExecuteAsync(sqlDel, _filterHrVM);

                if (_filterHrVM.IsTypeUpdate == 1)
                {
                    var sqlInsert = string.Empty;
                    if (_filterHrVM.DepartmentID != string.Empty)
                    {
                        sqlInsert += "Insert into HR.LockDutyRoster values(@Period,@DivisionID,@DepartmentID,@LockFrom,@LockTo,'" + _UserID + "',GETDATE()) ";
                    }
                    else
                    {
                        sqlInsert += "Insert into HR.LockDutyRoster select @Period,DivisionID, DepartmentID,@LockFrom,@LockTo,'" + _UserID + "',GETDATE() from SYSTEM.PermissionDepartment where UserID='" + UserID + "' and DivisionID=@DivisionID ";
                    }
                    await conn.ExecuteAsync(sqlInsert, _filterHrVM);
                }
            }
            return true;
        }

        [HttpPost("InitializeAttendanceRecordDutyRoster")]
        public async Task<ActionResult<bool>> InitializeAttendanceRecordDutyRoster(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@sPeriod", _filterHrVM.Year * 100 + _filterHrVM.Month);

                await conn.ExecuteAsync("HR.DutyRoster_insertEserialNotPeriod", parm, commandType: CommandType.StoredProcedure);
                await conn.ExecuteAsync("HR.AttendanceRecord_insertEserialNotPeriod", parm, commandType: CommandType.StoredProcedure);
                await conn.ExecuteAsync("HR.DutyRoster_calcDutyRosterWihtJoinDateTerminateDate", parm, commandType: CommandType.StoredProcedure);
            }
            return true;
        }

        [HttpPost("UpdateShiftWork")]
        public async Task<ActionResult<string>> UpdateShiftWork(DutyRosterVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@dDate", _filterHrVM.dDate);
                parm.Add("@Eserial", _filterHrVM.Eserial);
                parm.Add("@WorkShift", _filterHrVM.WorkShift);
                parm.Add("@FirstShiftID", _filterHrVM.FirstShiftID);
                parm.Add("@SecondShiftID", _filterHrVM.SecondShiftID);

                parm.Add("@UserID", _filterHrVM.UserID);

                var eserial = await conn.ExecuteScalarAsync<string>("HR.DutyRoster_updateShift", parm, commandType: CommandType.StoredProcedure);

                return eserial;
            }
        }

        //Shift
        [HttpGet("GetShiftTypeList")]
        public async Task<ActionResult<IEnumerable<ShiftTypeVM>>> GetShiftTypeList()
        {
            var sql = "select * from HR.ShiftType order by ShiftTypeID";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ShiftTypeVM>(sql);
                return result;
            }
        }

        [HttpGet("GetShiftList")]
        public async Task<ActionResult<IEnumerable<ShiftVM>>> GetShiftList()
        {
            var sql = "select * from HR.Shift s join HR.ShiftType st on st.ShiftTypeID = s.ShiftTypeID ";
            sql += "order by st.isOFF, s.ShiftName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ShiftVM>(sql);
                return result;
            }
        }

        [HttpGet("ContainsShiftID/{id}")]
        public async Task<bool> ContainsShiftID(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.Shift where ShiftID = @ShiftID) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return conn.ExecuteScalar<bool>(sql, new { ShiftID = id });
            }
        }

        [HttpPost("UpdateShift")]
        public async Task<ActionResult<int>> UpdateShift(ShiftVM _shiftVM)
        {
            var sql = "BEGIN TRANSACTION ";
            if (_shiftVM.IsTypeUpdate == 0)
            {
                sql += "Insert into HR.Shift (ShiftID,ShiftName,ShiftTypeID,BeginTime,EndTime,isNight,isSplit,ColorHEX,isActive) Values (UPPER(@ShiftID),@ShiftName,@ShiftTypeID,@BeginTime,@EndTime,@isNight,@isSplit,@ColorHEX,@isActive) ";
                sql += "Update HR.Shift set isEndTimeOfNextDay = case when EndTime<BeginTime then 1 else 0 end where ShiftID=@ShiftID ";
            }
            if (_shiftVM.IsTypeUpdate == 1)
            {
                sql += "Update HR.Shift set ShiftName = @ShiftName, ShiftTypeID = @ShiftTypeID, BeginTime = @BeginTime, EndTime = @EndTime, isNight = @isNight, isSplit = @isSplit, ColorHEX = @ColorHEX, isActive = @isActive where ShiftID = @ShiftID ";
                sql += "Update HR.Shift set isEndTimeOfNextDay = case when EndTime<BeginTime then 1 else 0 end where ShiftID=@ShiftID ";
            }
            if (_shiftVM.IsTypeUpdate == 2)
            {
                sql += "if not exists (select * from HR.DutyRoster where FirstShiftID=@ShiftID) ";
                sql += "begin ";
                sql += "delete from HR.Shift where ShiftID=@ShiftID ";
                sql += "end ";
            }
            sql += "IF @@ERROR>0 BEGIN IF (@@TRANCOUNT>0) ROLLBACK TRANSACTION END ELSE BEGIN COMMIT TRANSACTION; END ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var affectedRows = await conn.ExecuteAsync(sql, _shiftVM);
                return affectedRows;
            }
        }

        //Att
        [HttpPost("GetProfileUser")]
        public async Task<ActionResult<ProfileManagamentVM>> GetProfileUser(FilterHrVM _filterHrVM)
        {
            var sql = " select p.Eserial, p.LastName, p.MiddleName, p.FirstName, jh.DivisionID, jh.DepartmentID from HR.JobHistory jh ";
            sql += "join HR.Profile p on p.Eserial = jh.Eserial ";
            sql += "join HR.Position po on po.PositionID = jh.PositionID ";
            sql += "where JobID in(select MAX(JobID) from HR.JobHistory where Eserial=@UserID and YEAR(JobStartDate)*100+MONTH(JobStartDate)<=@Year*100+@Month) ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.QueryFirstAsync<ProfileManagamentVM>(sql, _filterHrVM);
            }
        }

        [HttpPost("GetAttendanceRecordList")]
        public async Task<ActionResult<IEnumerable<DutyRosterVM>>> GetAttendanceRecordList(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Y", _filterHrVM.Year);
                parm.Add("@M", _filterHrVM.Month);
                parm.Add("@D", _filterHrVM.Day);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@SectionID", _filterHrVM.SectionID);
                parm.Add("@DeptID", _filterHrVM.DepartmentID);
                parm.Add("@arrPos", _filterHrVM.PositionGroupID);
                parm.Add("@Eserial", _filterHrVM.Eserial);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryAsync<DutyRosterVM>("HR.DutyRoster_viewDutyRosterList", parm, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        [HttpPost("UpdateExplain")]
        public async Task<ActionResult<bool>> UpdateExplain(DutyRosterVM _dutyRosterVM)
        {
            var sql = "Update HR.AttendanceRecord set Explain = @Explain, EserialExplain = @UserID,ExplainTime = GETDATE() where ARID = @ARID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _dutyRosterVM);
            }
            return true;
        }

        [HttpPost("UpdateExplainHOD")]
        public async Task<ActionResult<bool>> UpdateExplainHOD(DutyRosterVM _dutyRosterVM)
        {
            var sql = "Update HR.AttendanceRecord set ExplainHOD = @ExplainHOD, EserialExplainHOD = @UserID,ExplainHODTime = GETDATE() where ARID = @ARID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _dutyRosterVM);
            }
            return true;
        }

        [HttpPost("UpdateExplainHR")]
        public async Task<ActionResult<bool>> UpdateExplainHR(DutyRosterVM _dutyRosterVM)
        {
            var sql = "Update HR.AttendanceRecord set ExplainHR = @ExplainHR, EserialExplainHR = @UserID,ExplainHRTime = GETDATE() where ARID = @ARID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _dutyRosterVM);
            }
            return true;
        }

        [HttpPost("ConfirmExplain")]
        public async Task<ActionResult<bool>> ConfirmExplain(DutyRosterVM _dutyRosterVM)
        {
            var sql = "Update HR.AttendanceRecord set isConfirmExplain = @isConfirmExplain, EserialConfirmExplain = @UserID,TimeConfirmExplain = GETDATE() where ARID = @ARID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _dutyRosterVM);
            }
            return true;
        }

        [HttpPost("ConfirmLateSoon")]
        public async Task<ActionResult<bool>> ConfirmLateSoon(DutyRosterVM _dutyRosterVM)
        {
            var sql = "Update HR.AttendanceRecord set isConfirmLateSoon = @isConfirmLateSoon, EserialConfirmLateSoon = @UserID,TimeConfirmLateSoon = GETDATE() where ARID = @ARID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _dutyRosterVM);
            }
            return true;
        }

        [HttpPost("CalcFingerData")]
        public async Task<ActionResult<bool>> CalcFingerData(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Y", _filterHrVM.Year);
                parm.Add("@M", _filterHrVM.Month);
                parm.Add("@DivsID", _filterHrVM.DivisionID);

                await conn.ExecuteAsync("HR.AttendanceRecord_calcFingerData", parm, commandType: CommandType.StoredProcedure);
            }
            return true;
        }

        //EmplTrf
        [HttpPost("GetEmplTrfList")]
        public async Task<ActionResult<IEnumerable<DutyRosterVM>>> GetEmplTrfList(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@dDate", _filterHrVM.dDate);
                parm.Add("@DivisionID", _filterHrVM.DivisionID);
                parm.Add("@ShiftID", _filterHrVM.ShiftID);
                parm.Add("@PositionGroupID", _filterHrVM.PositionGroupID);

                var result = await conn.QueryAsync<DutyRosterVM>("OP.EmplTrf_view", parm, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        [HttpPost("GetDutyRosterByEserial/{_dutyRosterVM}")]
        public async Task<ActionResult<DutyRosterVM>> GetDutyRosterByEserial(FilterHrVM _filterHrVM, DutyRosterVM _dutyRosterVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@Y", _filterHrVM.Year);
                parm.Add("@M", _filterHrVM.Month);
                parm.Add("@D", _dutyRosterVM.dDate.Day);
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@SectionID", _filterHrVM.SectionID);
                parm.Add("@DeptID", _filterHrVM.DepartmentID);
                parm.Add("@arrPos", string.Empty);
                parm.Add("@Eserial", _dutyRosterVM.Eserial);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryFirstAsync<DutyRosterVM>("HR.DutyRoster_viewDutyRosterList", parm, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        [HttpPost("UpdatePositionWork")]
        public async Task<ActionResult<bool>> UpdatePositionWork(DutyRosterVM _dutyRosterVM)
        {
            var sql = "Update HR.DutyRoster set PositionWorkID = @PositionID where dDate=@dDate and Eserial = @Eserial ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _dutyRosterVM);
            }
            return true;
        }

        [HttpPost("GetDutyRosterNotes")]
        public async Task<ActionResult<IEnumerable<DutyRosterVM>>> GetDutyRosterNotes(FilterHrVM _filterHrVM)
        {
            var sql = "select * from HR.DutyRosterNote where dDate=format(@dDate,'yyyy-MM-dd') ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<DutyRosterVM>(sql, _filterHrVM);
                return result;
            }
        }

        [HttpPost("UpdateDutyRosterNote")]
        public async Task<ActionResult<bool>> UpdateDutyRosterNote(DutyRosterVM _dutyRosterVM)
        {
            var sql = "delete from HR.DutyRosterNote where dDate=format(@dDate,'yyyy-MM-dd') and ShiftID=@ShiftID and PositionGroupID=@PositionGroupID ";
            if(!String.IsNullOrEmpty(_dutyRosterVM.DutyRosterNote)) {
                sql += "Insert into HR.DutyRosterNote (dDate, PositionGroupID, ShiftID, DutyRosterNote, UserUpdate, TimeUpdate) ";
                sql += "Values (format(@dDate,'yyyy-MM-dd'), @PositionGroupID, @ShiftID, @DutyRosterNote, @UserID, GETDATE()) ";
            }

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _dutyRosterVM);
            }
            return true;
        }

        //WorkPlan
        [HttpPost("GetWorkPlans")]
        public async Task<ActionResult<IEnumerable<DutyRosterVM>>> GetWorkPlans(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@dDate", _filterHrVM.dDate);
                parm.Add("@DivisionID", _filterHrVM.DivisionID);

                var result = await conn.QueryAsync<DutyRosterVM>("HR.WorkPlan_viewList", parm, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        [HttpPost("UpdateWorkPlan")]
        public async Task<ActionResult<int>> UpdateWorkPlan(DutyRosterVM _workPlanVM)
        {
            var sql = "";
            if (_workPlanVM.IsTypeUpdate == 0)
            {
                sql += "Insert into HR.WorkPlan (PositionGroupID,WorkPlanName,WorkPlanDesc,WorkPlanStartDate,WorkPlanDeadline,WorkPlanNote,WorkPlanIsDone,UserCreated,TimeCreated) Values (@PositionGroupID,@WorkPlanName,@WorkPlanDesc,@WorkPlanStartDate,@WorkPlanDeadline,@WorkPlanNote,@WorkPlanIsDone,@UserCreated,GETDATE()) ";
            }
            if (_workPlanVM.IsTypeUpdate == 1)
            {
                sql += "Update HR.WorkPlan set WorkPlanName = @WorkPlanName,WorkPlanDesc = @WorkPlanDesc, WorkPlanStartDate = @WorkPlanStartDate, WorkPlanDeadline = @WorkPlanDeadline, WorkPlanNote = @WorkPlanNote, WorkPlanIsDone = @WorkPlanIsDone where WorkPlanSeq = @WorkPlanSeq ";
            }
            if (_workPlanVM.IsTypeUpdate == 2)
            {
                sql += "delete from HR.WorkPlan where WorkPlanSeq = @WorkPlanSeq ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var affectedRows = await conn.ExecuteAsync(sql, _workPlanVM);
                return affectedRows;
            }
        }

        [HttpPost("UpdateWorkPlanIsDone")]
        public async Task<ActionResult<bool>> UpdateWorkPlanIsDone(DutyRosterVM _workPlanVM)
        {
            var sql = "Update HR.WorkPlan set WorkPlanIsDone = @WorkPlanIsDone, WorkPlanDoneDate = GETDATE() where WorkPlanSeq = @WorkPlanSeq ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _workPlanVM);
            }
            return true;
        }

    }
}
