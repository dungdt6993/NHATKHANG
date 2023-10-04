using Data.Infrastructure;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Utilities;
using Newtonsoft.Json;

namespace D69soft.Server.Controllers.HR
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public PayrollController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        //Monthly Income
        [HttpPost("GetMonthlyIncomeTrnOtherList")]
        public async Task<ActionResult<List<MonthlyIncomeTrnOtherVM>>> GetMonthlyIncomeTrnOtherList(FilterHrVM _filterHrVM)
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
                parm.Add("@TrnCode", _filterHrVM.TrnCode);
                parm.Add("@TrnSubCode", _filterHrVM.TrnSubCode);
                parm.Add("@IsTypeSearch", _filterHrVM.IsTypeSearch);

                var result = await conn.QueryAsync<MonthlyIncomeTrnOtherVM>("HR.MonthlyIncome_viewMain", parm, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        [HttpPost("UpdateMITrnOther")]
        public async Task<ActionResult<bool>> UpdateMITrnOther(MonthlyIncomeTrnOtherVM _monthlyIncomeTrnOtherVM)
        {
            if (_monthlyIncomeTrnOtherVM.strSeqMITrnOther != string.Empty)
            {
                _monthlyIncomeTrnOtherVM.strSeqMITrnOther = "," + _monthlyIncomeTrnOtherVM.strSeqMITrnOther + ",";
            }

            var sql = "BEGIN TRANSACTION ";
            if (_monthlyIncomeTrnOtherVM.IsTypeUpdate == 0)
            {
                sql += "Insert into HR.MonthlyIncomeTrnOther (Eserial,Period,TrnCode,TrnSubCode,UnitPrice,Qty,Note,Author,DateUpdate) ";
                sql += "Values(@Eserial,@Period,@TrnCode,@TrnSubCode,@UnitPrice,@Qty,@Note,@UserID,GETDATE()) ";
            }
            if (_monthlyIncomeTrnOtherVM.IsTypeUpdate == 1)
            {
                sql += "Update HR.MonthlyIncomeTrnOther set UnitPrice = @UnitPrice, Qty = @Qty, Note = @Note, Author = @UserID, DateUpdate = GETDATE() ";
                sql += "Where SeqMITrnOther=@SeqMITrnOther ";
            }
            //Del by IsCheck
            if (_monthlyIncomeTrnOtherVM.IsTypeUpdate == 2)
            {
                sql += "delete from HR.MonthlyIncomeTrnOther where CHARINDEX(',' +CONVERT(VARCHAR(MAX), SeqMITrnOther) + ',',@strSeqMITrnOther)>0 ";
            }
            //Update by IsCheck
            if (_monthlyIncomeTrnOtherVM.IsTypeUpdate == 3)
            {
                sql += "Update HR.MonthlyIncomeTrnOther set TrnCode = @TrnCode, TrnSubCode = @TrnSubCode, Note = @Note, Author = @UserID, DateUpdate = GETDATE() where CHARINDEX(',' +CONVERT(VARCHAR(MAX), SeqMITrnOther) + ',',@strSeqMITrnOther)>0 ";
            }
            sql += "IF @@ERROR>0 BEGIN IF (@@TRANCOUNT>0) ROLLBACK TRANSACTION END ELSE BEGIN COMMIT TRANSACTION; END ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _monthlyIncomeTrnOtherVM);
                return true;
            }
        }

        [HttpPost("GetDataMITrnOtherFromExcel")]
        public async Task<ActionResult<bool>> GetDataMITrnOtherFromExcel(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string str = LibraryFunc.RepalceWhiteSpace(_filterHrVM.strDataFromExcel.Trim()).Replace(" \t", "\t").Replace("\t ", "\t").Replace(",", string.Empty).Replace(" ", "\n");

                var sql = string.Empty;
                try
                {
                    var lines = Regex.Split(str, "\n");

                    foreach (var itemfinger in lines)
                    {
                        if (string.IsNullOrWhiteSpace(itemfinger))
                            continue;

                        var itemSplit = Regex.Split(itemfinger, "\t").Select(x => x.Trim()).ToArray();

                        sql = "Insert into HR.MonthlyIncomeTrnOther (Eserial,Period,TrnCode,TrnSubCode,UnitPrice,Qty,Note,Author,DateUpdate,isPostFromExcel) ";
                        sql += "Values(";

                        sql += "'" + itemSplit[0].Trim() + "',@Period,0,0, ABS(" + itemSplit[1].Trim() + "),1,'',@UserID,GETDATE(),1";

                        sql += ") ";

                        await conn.ExecuteAsync(sql, _filterHrVM);
                    }
                }
                catch (Exception)
                {
                    sql = "delete from HR.MonthlyIncomeTrnOther where isPostFromExcel=1 ";
                    return false;
                }

            }
            return true;
        }

        //Payroll
        [HttpGet("GetTrnGroupCodeList")]
        public async Task<ActionResult<IEnumerable<SalaryTransactionGroupVM>>> GetTrnGroupCodeList()
        {
            var sql = "select * from HR.SalaryTransactionGroup where TrnGroupCode not in (100,500,520,620) order by TrnGroupCode";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<SalaryTransactionGroupVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("GetTrnCodeList/{_TrnGroupCode}")]
        public async Task<ActionResult<IEnumerable<SalaryTransactionCodeVM>>> GetTrnCodeList(int _TrnGroupCode)
        {
            var sql = "select * from HR.SalaryTransactionCode ";
            sql += "where TrnGroupCode = @TrnGroupCode and TrnGroupCode <> 100 order by TrnCode, TrnSubCode ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<SalaryTransactionCodeVM>(sql, new { TrnGroupCode = _TrnGroupCode });
                return Ok(result);
            }
        }

        [HttpPost("CalcSalary")]
        public async Task<ActionResult<bool>> CalcSalary(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sqlBeginLog = "Update HR.LockSalary set isSalCalc=1, StatusSalCalc = 0 where Period=" + _filterHrVM.Period + " and DivisionID='" + _filterHrVM.DivisionID + "' ";
                await conn.ExecuteAsync(sqlBeginLog);

                /***************Lấy thông tin dữ liệu lương***************/
                DynamicParameters parmSal = new DynamicParameters();
                parmSal.Add("@Y", _filterHrVM.Year);
                parmSal.Add("@M", _filterHrVM.Month);
                parmSal.Add("@DivsID", _filterHrVM.DivisionID);
                await conn.ExecuteAsync("HR.Payroll_calcSalaryActiveMonth", parmSal, commandType: CommandType.StoredProcedure);

                /***************Cập nhật ngày tính lương active cho những người thay đổi trong tháng***************/
                string sqlSal = "select Eserial from HR.SalaryActiveMonth where Period=" + _filterHrVM.Period + " and Eserial in (select distinct et.Eserial from HR.EmployeeTransaction et join HR.JobHistory jh on jh.Eserial = et.Eserial where CurrentJobID = 1 and jh.DivisionID='" + _filterHrVM.DivisionID + "') group by Eserial having count(Eserial) >1 ";

                var _eserialSals = await conn.QueryAsync<string>(sqlSal);

                if (_eserialSals.ToList().Count() > 0)
                {
                    for (int i = 0; i < _eserialSals.ToList().Count; i++)
                    {
                        var _eserialSal = _eserialSals.ToList()[i];

                        string sqlEserialSal = "select BeginSalaryDate from HR.SalaryActiveMonth where Period=" + _filterHrVM.Period + " and Eserial='" + _eserialSal + "' order by BeginSalaryDate";

                        var _beginSalaryDates = await conn.QueryAsync<string>(sqlEserialSal);

                        for (int j = 0; j < _beginSalaryDates.ToList().Count - 1; j++)
                        {
                            var _beginSalaryDate_fisrt = _beginSalaryDates.ToList()[j];

                            var _beginSalaryDate_last = _beginSalaryDates.ToList()[j + 1];

                            string sqlUpdateEndSalaryDateActive = "Update HR.SalaryActiveMonth set EndSalaryDateActive = DATEADD(day,-1,CONVERT(datetime,'" + _beginSalaryDate_last + "')) where Period=" + _filterHrVM.Period + " and Eserial='" + _eserialSal + "' and BeginSalaryDate = '" + _beginSalaryDate_fisrt + "' ";

                            await conn.ExecuteAsync(sqlUpdateEndSalaryDateActive);
                        }
                    }
                }

                /***************Lấy thông tin dữ liệu công việc***************/
                DynamicParameters parmJob = new DynamicParameters();
                parmJob.Add("@Y", _filterHrVM.Year);
                parmJob.Add("@M", _filterHrVM.Month);
                parmJob.Add("@DivsID", _filterHrVM.DivisionID);
                await conn.ExecuteAsync("HR.Payroll_calcJobActiveMonth", parmJob, commandType: CommandType.StoredProcedure);

                /***************Cập nhật ngày công việc active cho những người thay đổi trong tháng***************/
                string sqlJob = "select Eserial from HR.JobActiveMonth where Period=" + _filterHrVM.Period + " and DivisionID='" + _filterHrVM.DivisionID + "' group by Eserial having count(Eserial) >1 ";

                var _eserialJobs = await conn.QueryAsync<string>(sqlJob);

                if (_eserialJobs.ToList().Count() > 0)
                {
                    for (int i = 0; i < _eserialJobs.ToList().Count; i++)
                    {
                        var _eserialJob = _eserialJobs.ToList()[i];

                        string sqlEserialJob = "select JobStartDate from HR.JobActiveMonth where Period=" + _filterHrVM.Period + " and Eserial='" + _eserialJob + "' order by JobStartDate";

                        var _jobStartDates = await conn.QueryAsync<string>(sqlEserialJob);

                        for (int j = 0; j < _jobStartDates.ToList().Count - 1; j++)
                        {
                            var _jobStartDate_fisrt = _jobStartDates.ToList()[j];

                            var _jobStartDate_last = _jobStartDates.ToList()[j + 1];

                            string sqlUpdateJobEndDateActive = "Update HR.JobActiveMonth set JobEndDateActive = DATEADD(day,-1,CONVERT(datetime,'" + _jobStartDate_last + "')) where Period=" + _filterHrVM.Period + " and Eserial='" + _eserialJob + "' and JobStartDate = '" + _jobStartDate_fisrt + "' ";

                            await conn.ExecuteAsync(sqlUpdateJobEndDateActive);
                        }
                    }
                }

                /***************Đẩy thông tin MonthlySalary***************/
                parmJob.Add("@Y", _filterHrVM.Year);
                parmJob.Add("@M", _filterHrVM.Month);
                parmJob.Add("@DivsID", _filterHrVM.DivisionID);
                await conn.ExecuteAsync("HR.Payroll_calcMonthlySalaryStaff", parmJob, commandType: CommandType.StoredProcedure);

                /***************Đẩy thông tin mức lương tính toán trong tháng***************/
                parmJob.Add("@Y", _filterHrVM.Year);
                parmJob.Add("@M", _filterHrVM.Month);
                parmJob.Add("@DivsID", _filterHrVM.DivisionID);
                await conn.ExecuteAsync("HR.Payroll_calcAvgSalaryActive", parmJob, commandType: CommandType.StoredProcedure);

                /***************Cập nhật ngày công***************/
                parmJob.Add("@Y", _filterHrVM.Year);
                parmJob.Add("@M", _filterHrVM.Month);
                parmJob.Add("@DivsID", _filterHrVM.DivisionID);
                await conn.ExecuteAsync("HR.Payroll_calcShiftTypeActive", parmJob, commandType: CommandType.StoredProcedure);

                var sqlShiftType = "select * from HR.ShiftType where coalesce(PercentIncome,0) > 0 order by PercentIncome desc";
                var _shiftTypes = await conn.QueryAsync<string>(sqlShiftType);
                if (_shiftTypes.ToList().Count() > 0)
                {
                    var sqlUpdateShiftType = string.Empty;
                    for (int i = 0; i < _shiftTypes.ToList().Count; i++)
                    {
                        var _shiftType = _shiftTypes.ToList()[i];

                        sqlUpdateShiftType += "Update HR.ShiftTypeActive set TotalShiftType = coalesce(ro.ST,0) from (select ro.Eserial,sum(case when coalesce(sr.ShiftTypeID,'') = '" + _shiftType + "' and coalesce(sr1.ShiftTypeID,'') = '' then 1 else ";
                        sqlUpdateShiftType += "case when coalesce(sr.ShiftTypeID,'') = '" + _shiftType + "' and coalesce(sr1.ShiftTypeID,'') <> '" + _shiftType + "' then 0.5 else ";
                        sqlUpdateShiftType += "case when coalesce(sr.ShiftTypeID,'') <> '" + _shiftType + "' and coalesce(sr1.ShiftTypeID,'') = '" + _shiftType + "' then 0.5 else ";
                        sqlUpdateShiftType += "case when coalesce(sr.ShiftTypeID,'') = '" + _shiftType + "' and coalesce(sr1.ShiftTypeID,'') = '" + _shiftType + "' then 1 else 0 end end end end) as ST, BeginSalaryDateActive,EndSalaryDateActive ";
                        sqlUpdateShiftType += "from HR.DutyRoster ro ";
                        sqlUpdateShiftType += "join (select * from HR.MonthlySalaryStaff where Period=" + _filterHrVM.Period + " and DivisionID='" + _filterHrVM.DivisionID + "') mss on mss.Eserial = ro.Eserial ";
                        sqlUpdateShiftType += "join (select * from HR.LockDutyRoster where Period=" + _filterHrVM.Period + " and DivisionID='" + _filterHrVM.DivisionID + "') rld on rld.DepartmentID = mss.DepartmentID and dDate >= LockFrom and dDate <=LockTo ";
                        sqlUpdateShiftType += "join HR.Shift sr on sr.ShiftID = ro.FirstShiftID left join HR.Shift sr1 on sr1.ShiftID = ro.SecondShiftID join (select * from HR.ShiftTypeActive where Period=" + _filterHrVM.Period + ") sta on sta.Eserial = ro.Eserial and sta.ShiftTypeID = '" + _shiftType + "' ";
                        sqlUpdateShiftType += "where dDate >= BeginSalaryDateActive and dDate <= EndSalaryDateActive and ro.Eserial in (select distinct et.Eserial from HR.EmployeeTransaction et join HR.JobHistory jh on jh.Eserial = et.Eserial where CurrentJobID = 1 and jh.DivisionID='" + _filterHrVM.DivisionID + "') group by ro.Eserial,BeginSalaryDateActive,EndSalaryDateActive) ro where  ";
                        sqlUpdateShiftType += "ro.BeginSalaryDateActive = ShiftTypeActive.BeginSalaryDateActive and ro.EndSalaryDateActive = ShiftTypeActive.EndSalaryDateActive and ShiftTypeActive.ShiftTypeID = '" + _shiftType + "' and ro.Eserial = ShiftTypeActive.Eserial and ShiftTypeActive.Period = " + _filterHrVM.Period + " ";
                    }
                    await conn.ExecuteAsync(sqlUpdateShiftType);

                    string sqlSumTotalShift = string.Empty;
                    for (int i = 0; i < _shiftTypes.ToList().Count; i++)
                    {
                        var _shiftType = _shiftTypes.ToList()[i];

                        if (i == 0)
                        {
                            sqlSumTotalShift += "Update HR.ShiftTypeActive set TotalShiftTypeCalc = mo.SumTotalShiftType from (select Eserial,Period,BeginSalaryDateActive, sum(coalesce(TotalShiftType,0)) as SumTotalShiftType from HR.ShiftTypeActive where Period = " + _filterHrVM.Period + " and Eserial in (select distinct et.Eserial from HR.EmployeeTransaction et join HR.JobHistory jh on jh.Eserial = et.Eserial where CurrentJobID = 1 and jh.DivisionID='" + _filterHrVM.DivisionID + "') ";
                            sqlSumTotalShift += "Group by Eserial,Period,BeginSalaryDateActive) mo where mo.Eserial = ShiftTypeActive.Eserial and ShiftTypeActive.Period = " + _filterHrVM.Period + " and mo.BeginSalaryDateActive = ShiftTypeActive.BeginSalaryDateActive and ShiftTypeActive.ShiftTypeID = '" + _shiftType + "' and ShiftTypeActive.Eserial in (select distinct et.Eserial from HR.EmployeeTransaction et join HR.JobHistory jh on jh.Eserial = et.Eserial where CurrentJobID = 1 and jh.DivisionID='" + _filterHrVM.DivisionID + "') ";
                        }
                        sqlSumTotalShift += "Update HR.ShiftTypeActive set TotalShiftTypeCalc = coalesce(PaidDefault,0) where coalesce(TotalShiftTypeCalc,0) > coalesce(PaidDefault,0) and Period = " + _filterHrVM.Period + " and ShiftTypeID = '" + _shiftType + "' and ShiftTypeActive.Eserial in (select distinct et.Eserial from HR.EmployeeTransaction et join HR.JobHistory jh on jh.Eserial = et.Eserial where CurrentJobID = 1 and jh.DivisionID='" + _filterHrVM.DivisionID + "') ";
                        sqlSumTotalShift += "Update HR.ShiftTypeActive set TotalShiftTypeCalc = coalesce(TotalShiftTypeCalc,0) - coalesce(TotalShiftType,0) where Period = " + _filterHrVM.Period + " and ShiftTypeID = '" + _shiftType + "' and ShiftTypeActive.Eserial in (select distinct et.Eserial from HR.EmployeeTransaction et join HR.JobHistory jh on jh.Eserial = et.Eserial where CurrentJobID = 1 and jh.DivisionID='" + _filterHrVM.DivisionID + "') ";
                        sqlSumTotalShift += "Update HR.ShiftTypeActive set TotalShiftTypeActive = case when coalesce(TotalShiftTypeCalc,0) >= 0 then coalesce(TotalShiftType,0) else coalesce(TotalShiftTypeCalc,0) + coalesce(TotalShiftType,0) end where Period = " + _filterHrVM.Period + " and ShiftTypeID = '" + _shiftType + "' and ShiftTypeActive.Eserial in (select distinct et.Eserial from HR.EmployeeTransaction et join HR.JobHistory jh on jh.Eserial = et.Eserial where CurrentJobID = 1 and jh.DivisionID='" + _filterHrVM.DivisionID + "') ";
                        sqlSumTotalShift += "Update HR.ShiftTypeActive set TotalShiftTypeCalc = coalesce(mo.TotalShiftTypeCalc,0) from (select Eserial,Period,BeginSalaryDateActive, TotalShiftTypeCalc from HR.ShiftTypeActive where Period = " + _filterHrVM.Period + " and ShiftTypeID = '" + _shiftType + "' and ShiftTypeActive.Eserial in (select distinct et.Eserial from HR.EmployeeTransaction et join HR.JobHistory jh on jh.Eserial = et.Eserial where CurrentJobID = 1 and jh.DivisionID='" + _filterHrVM.DivisionID + "')) mo ";
                        sqlSumTotalShift += "where mo.Eserial = ShiftTypeActive.Eserial and ShiftTypeActive.Period = " + _filterHrVM.Period + " and mo.BeginSalaryDateActive = ShiftTypeActive.BeginSalaryDateActive and ShiftTypeActive.Eserial in (select distinct et.Eserial from HR.EmployeeTransaction et join HR.JobHistory jh on jh.Eserial = et.Eserial where CurrentJobID = 1 and jh.DivisionID='" + _filterHrVM.DivisionID + "') ";
                        sqlSumTotalShift += "Update HR.ShiftTypeActive set TotalShiftTypeActive = 0 where coalesce(TotalShiftTypeActive,0) < 0 and Period = " + _filterHrVM.Period + " and ShiftTypeActive.Eserial in (select distinct et.Eserial from HR.EmployeeTransaction et join HR.JobHistory jh on jh.Eserial = et.Eserial where CurrentJobID = 1 and jh.DivisionID='" + _filterHrVM.DivisionID + "')  ";
                    }
                    await conn.ExecuteAsync(sqlSumTotalShift);
                }

                //Clear MonthlyIncome truoc khi tinh toan
                string sqlClearMonthlyIncome = "Delete from HR.MonthlyIncome where Period = " + _filterHrVM.Period + " and Eserial in (select distinct et.Eserial from HR.EmployeeTransaction et join HR.JobHistory jh on jh.Eserial = et.Eserial where CurrentJobID = 1 and jh.DivisionID='" + _filterHrVM.DivisionID + "') ";
                await conn.ExecuteAsync(sqlClearMonthlyIncome);

                //*******************Tính thu nhập theo công theo lương Profile*********************/

                //Tinh luong theo thang
                //Tinh luong theo ngay cong cap nhat tài khoan giao dich 100
                string sqlSalaryTypeIsCalcByShift = "select SalaryType from HR.SalaryDef where coalesce(isCalcByShift,0) = 1 and TrnCode = 0 ";
                var _salaryTypeIsCalcByShifts = await conn.QueryAsync<string>(sqlSalaryTypeIsCalcByShift);
                if (_salaryTypeIsCalcByShifts.ToList().Count > 0)
                {
                    string sqlCalcByShift = string.Empty;
                    for (int i = 0; i < _salaryTypeIsCalcByShifts.ToList().Count; i++)
                    {
                        var _salaryTypeIsCalcByShift = _salaryTypeIsCalcByShifts.ToList()[i];

                        sqlCalcByShift += "Insert into HR.MonthlyIncome (Eserial, Period, TrnCode, TrnSubCode, Amount, IsPIT, RatePIT, isPaySlip) ";
                        sqlCalcByShift += "select mss.Eserial, mss.Period, stc.TrnCode, stc.TrnSubCode,ABS((coalesce(TotalShiftTypeActive,0) *coalesce(" + _salaryTypeIsCalcByShift + "Active,0)*coalesce(PercentIncome,0)/100)/sta.WDDefault)*stc.Rate, stc.isPIT, stc.RatePIT, stc.isPaySlip ";
                        sqlCalcByShift += "from HR.ShiftTypeActive sta ";
                        sqlCalcByShift += "join (select * from HR.SalaryTransactionCode) stc on sta.ShiftTypeID = stc.ShiftTypeID ";
                        sqlCalcByShift += "join HR.ShiftType st on st.ShiftTypeID = sta.ShiftTypeID ";
                        sqlCalcByShift += "join HR.EmployeeTransaction et on et.TrnCode = stc.TrnCode and et.TrnSubCode = stc.TrnSubCode and et.Eserial = sta.Eserial ";
                        sqlCalcByShift += "join (select * from HR.MonthlySalaryStaff where Period=" + _filterHrVM.Period + ") mss on mss.Eserial = sta.Eserial ";
                        sqlCalcByShift += "where sta.Period=" + _filterHrVM.Period + " and sta.Eserial in (select distinct et.Eserial from HR.EmployeeTransaction et join HR.JobHistory jh on jh.Eserial = et.Eserial where CurrentJobID = 1 and jh.DivisionID='" + _filterHrVM.DivisionID + "') and coalesce(sta.WDDefault,0) > 0 and coalesce(mss.IsPayByMonth,0) = 1 ";
                    }
                    await conn.ExecuteAsync(sqlCalcByShift);
                }

                //Tinh luong theo ngay cong cap nhat tai khoan giao dich khac 100
                string sqlSalaryTypeIsNotCalcByShift = "select SalaryType from HR.SalaryDef where coalesce(isCalcByShift,0) = 1 and TrnCode <> 0";
                var _salaryTypeIsNotCalcByShifts = await conn.QueryAsync<string>(sqlSalaryTypeIsNotCalcByShift);
                if (_salaryTypeIsNotCalcByShifts.ToList().Count > 0)
                {
                    var sqlNotCalcByShift = string.Empty;
                    for (int i = 0; i < _salaryTypeIsNotCalcByShifts.ToList().Count; i++)
                    {
                        var _salaryTypeIsNotCalcByShift = _salaryTypeIsNotCalcByShifts.ToList()[i];

                        sqlNotCalcByShift += "Insert into HR.MonthlyIncome (Eserial, Period, TrnCode, TrnSubCode, Amount, IsPIT, RatePIT, isPaySlip) ";
                        sqlNotCalcByShift += "select mss.Eserial, mss.Period, sd.TrnCode, sd.TrnSubCode,SUM(ABS((coalesce(TotalShiftTypeActive, 0) * coalesce(" + _salaryTypeIsNotCalcByShift + "Active, 0) * coalesce(PercentIncome, 0)/100)/sta.WDDefault)*stc.Rate), stc.isPIT, stc.RatePIT, stc.isPaySlip ";
                        sqlNotCalcByShift += "from HR.ShiftTypeActive sta ";
                        sqlNotCalcByShift += "join(select * from HR.SalaryDef where SalaryType = '" + _salaryTypeIsNotCalcByShift + "') sd on 1 = 1 ";
                        sqlNotCalcByShift += "join(select * from HR.SalaryTransactionCode) stc on stc.TrnCode = sd.TrnCode and stc.TrnSubCode = sd.TrnSubCode ";
                        sqlNotCalcByShift += "join HR.ShiftType st on st.ShiftTypeID = sta.ShiftTypeID ";
                        sqlNotCalcByShift += "join HR.EmployeeTransaction et on et.TrnCode = sd.TrnCode and et.TrnSubCode = sd.TrnSubCode and et.Eserial = sta.Eserial ";
                        sqlNotCalcByShift += "join(select * from HR.MonthlySalaryStaff where Period = " + _filterHrVM.Period + ") mss on mss.Eserial = sta.Eserial ";
                        sqlNotCalcByShift += "where sta.Period = " + _filterHrVM.Period + " and sta.Eserial in (select distinct et.Eserial from HR.EmployeeTransaction et join HR.JobHistory jh on jh.Eserial = et.Eserial where CurrentJobID = 1 and jh.DivisionID = '" + _filterHrVM.DivisionID + "') and coalesce(sta.WDDefault,0) > 0 and coalesce(mss.IsPayByMonth,0) = 1 ";
                        sqlNotCalcByShift += "group by mss.Eserial, mss.Period, sd.TrnCode, sd.TrnSubCode, stc.isPIT, stc.RatePIT, stc.isPaySlip ";
                    }
                    await conn.ExecuteAsync(sqlNotCalcByShift);
                }

                //Tinh bao hiem
                parmJob.Add("@Y", _filterHrVM.Year);
                parmJob.Add("@M", _filterHrVM.Month);
                parmJob.Add("@DivsID", _filterHrVM.DivisionID);
                await conn.ExecuteAsync("HR.Payroll_calcInsurrance", parmJob, commandType: CommandType.StoredProcedure);

                //Tinh cac khoan thu nhap khac
                parmJob.Add("@Y", _filterHrVM.Year);
                parmJob.Add("@M", _filterHrVM.Month);
                parmJob.Add("@DivsID", _filterHrVM.DivisionID);
                await conn.ExecuteAsync("HR.Payroll_calcMonthlyIncomeTrnOther", parmJob, commandType: CommandType.StoredProcedure);

                //Tinh giam tru gia canh/nguoi phu thuoc
                parmJob.Add("@Y", _filterHrVM.Year);
                parmJob.Add("@M", _filterHrVM.Month);
                parmJob.Add("@DivsID", _filterHrVM.DivisionID);
                await conn.ExecuteAsync("HR.Payroll_calcDependanDeduction", parmJob, commandType: CommandType.StoredProcedure);

                //Tinh thue
                parmJob.Add("@Y", _filterHrVM.Year);
                parmJob.Add("@M", _filterHrVM.Month);
                parmJob.Add("@DivsID", _filterHrVM.DivisionID);
                await conn.ExecuteAsync("HR.Payroll_calcPIT", parmJob, commandType: CommandType.StoredProcedure);

                var sqlEndLog = "Update HR.LockSalary set isSalCalc=1, EserialSalCalc = '" + _filterHrVM.UserID + "', TimeSalCalc = GETDATE(), StatusSalCalc=1 where Period=" + _filterHrVM.Period + " and DivisionID='" + _filterHrVM.DivisionID + "' "; ;
                await conn.ExecuteAsync(sqlEndLog);
            }
            return true;
        }

        [HttpPost("CancelCalcSalary")]
        public async Task<ActionResult<bool>> CancelCalcSalary(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "delete from HR.MonthlySalaryStaff where Period=@Period and DivisionID=@DivisionID ";
                sql += "Update HR.LockSalary set isSalCalc=0, EserialSalCalc = @UserID, TimeSalCalc = GETDATE(), StatusSalCalc=0 where Period=@Period and DivisionID=@DivisionID "; ;
                await conn.ExecuteAsync(sql, _filterHrVM);
            }
            return true;
        }

        [HttpPost("LockSalary")]
        public async Task<ActionResult<bool>> LockSalary(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "Update HR.LockSalary set isSalLock=1, EserialSalLock = @UserID, TimeSalLock = GETDATE() where Period=@Period and DivisionID=@DivisionID "; ;
                await conn.ExecuteAsync(sql, _filterHrVM);
            }
            return true;
        }

        [HttpPost("CancelLockSalary")]
        public async Task<ActionResult<bool>> CancelLockSalary(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "Update HR.LockSalary set isSalLock=0, EserialSalLock = @UserID, TimeSalLock = GETDATE() where Period=@Period and DivisionID=@DivisionID "; ;
                await conn.ExecuteAsync(sql, _filterHrVM);
            }
            return true;
        }

        [HttpPost("GetPayrollList")]
        public async Task<ActionResult<string>> GetPayrollList(FilterHrVM _filterHrVM)
        {
            return JsonConvert.SerializeObject(ExecuteStoredProcPrmsToDataTable("HR.Payroll_viewPayrollDetail",
                "@Y", _filterHrVM.Year,
                "@M", _filterHrVM.Month,
                "@DivsID", _filterHrVM.DivisionID,
                "@SectionID", _filterHrVM.SectionID,
                "@DeptID", _filterHrVM.DepartmentID,
                "@arrPos", _filterHrVM.PositionGroupID,
                "@Eserial", _filterHrVM.Eserial,
                "@PayByBank", 0,
                "@PayByCash", 0));
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

        [HttpPost("GetLockSalary")]
        public async Task<ActionResult<LockSalaryVM>> GetLockSalary(FilterHrVM _filterHrVM)
        {
            var sql = "IF NOT EXISTS (select * from HR.LockSalary where Period=@Period and DivisionID=@DivisionID) ";
            sql += "BEGIN ";
            sql += "Insert into HR.LockSalary (Period, DivisionID,isSalCalc,EserialSalCalc,StatusSalCalc,isSalLock,EserialSalLock) ";
            sql += "Values (@Period,@DivisionID,0,'',0,0,'') ";
            sql += "END ";

            sql += "select ls.Period, ls.DivisionID, ls.isSalCalc, ls.StatusSalCalc, ls.EserialSalCalc, pCalc.LastName +' '+pCalc.MiddleName +' '+pCalc.FirstName as FullNameSalCalc, ls.TimeSalCalc, ";
            sql += "ls.isSalLock, ls.EserialSalLock, pLock.LastName +' '+pLock.MiddleName +' '+pLock.FirstName as FullNameSalLock, ls.TimeSalLock ";
            sql += "from HR.LockSalary ls ";
            sql += "left join HR.Profile pCalc on pCalc.Eserial = ls.EserialSalCalc ";
            sql += "left join HR.Profile pLock on pLock.Eserial = ls.EserialSalLock ";
            sql += "where ls.Period=@Period and ls.DivisionID=@DivisionID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryFirstAsync<LockSalaryVM>(sql, _filterHrVM);
                return result;
            }
        }

        [HttpPost("IsOpenFunc")]
        public async Task<bool> IsOpenFunc(FilterHrVM _filterHrVM)
        {
            var sql = "SELECT CAST(CASE WHEN NOT EXISTS (SELECT 1 FROM HR.LockSalary where Period=@Period and DivisionID = @DivisionID and isSalCalc=1) THEN 1 ELSE 0 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, _filterHrVM);
            }
        }

        //SalaryDef
        [HttpGet("GetSalaryDefList")]
        public async Task<ActionResult<IEnumerable<SalaryDefVM>>> GetSalaryDefList()
        {
            var sql = "select *  from HR.SalaryDef sd left join HR.SalaryTransactionCode stc on stc.TrnCode = sd.TrnCode and stc.TrnSubCode = sd.TrnSubCode ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<SalaryDefVM>(sql);
                return Ok(result);
            }
        }

        [HttpPost("UpdateSalaryDef")]
        public async Task<ActionResult<bool>> UpdateSalaryDef(SalaryDefVM _salaryDefVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "Update HR.SalaryDef set SalaryTypeName = @SalaryTypeName, isSIns = @isSIns, isCalcByShift = @isCalcByShift, TrnCode = @TrnCode, TrnSubCode = @TrnSubCode ";
                sql += "where SalaryType=@SalaryType "; ;
                await conn.ExecuteAsync(sql, _salaryDefVM);
            }
            return true;
        }

        //SalTrnCode
        [HttpGet("GetSalTrnCodeList")]
        public async Task<ActionResult<IEnumerable<SalaryTransactionCodeVM>>> GetSalTrnCodeList()
        {
            var sql = "select *, case when isPIT=0 then 0 else case when isPIT=1 and RatePIT=1 then 1 else 2 end end as TypePIT ";
            sql += "from HR.SalaryTransactionCode stc join HR.SalaryTransactionGroup stg on stg.TrnGroupCode = stc.TrnGroupCode ";
            sql += "left join HR.ShiftType st on st.ShiftTypeID = stc.ShiftTypeID ";
            sql += "order by stc.TrnCode, stc.TrnSubCode ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<SalaryTransactionCodeVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("ContainsTrnCodeID/{_TrnCode}/{_TrnSubCode}")]
        public async Task<bool> ContainsTrnCodeID(int _TrnCode, int _TrnSubCode)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.SalaryTransactionCode where TrnCode = @TrnCode and TrnSubCode = @TrnSubCode) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { TrnCode = _TrnCode, TrnSubCode = _TrnSubCode });
            }
        }

        [HttpPost("UpdateSalTrnCode")]
        public async Task<ActionResult<int>> UpdateSalTrnCode(SalaryTransactionCodeVM _salaryTransactionCodeVM)
        {
            var sql = "BEGIN TRANSACTION ";
            if (_salaryTransactionCodeVM.IsTypeUpdate == 0)
            {
                sql += "Insert into HR.SalaryTransactionCode (TrnCode,TrnSubCode,TrnName,isPIT,RatePIT,Rate,TrnGroupCode,InsPercent) Values (@TrnGroupCode,@TrnSubCode,@TrnName,@isPIT,@RatePIT,@Rate,@TrnGroupCode,@InsPercent) ";
            }
            if (_salaryTransactionCodeVM.IsTypeUpdate == 1)
            {
                sql += "Update HR.SalaryTransactionCode set TrnName = @TrnName, isPIT = @isPIT, RatePIT = @RatePIT, Rate = @Rate, InsPercent = @InsPercent where SalTrnID = @SalTrnID ";
            }
            if (_salaryTransactionCodeVM.IsTypeUpdate == 2)
            {
                sql += "if not exists (select * from HR.MonthlyIncome where TrnCode=@TrnCode and TrnSubCode=@TrnSubCode) ";
                sql += "begin ";
                sql += "delete from HR.SalaryTransactionCode where SalTrnID=@SalTrnID ";
                sql += "end ";
            }
            sql += "IF @@ERROR>0 BEGIN IF (@@TRANCOUNT>0) ROLLBACK TRANSACTION END ELSE BEGIN COMMIT TRANSACTION; END ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();


                return await conn.ExecuteAsync(sql, _salaryTransactionCodeVM);
            }
        }

        //WDDefaut
        [HttpPost("GetWDDefautList")]
        public async Task<ActionResult<IEnumerable<WDDefaultVM>>> GetWDDefautList(FilterHrVM _filterHrVM)
        {
            var sql = "select * from HR.DODefault dd join HR.WorkType wt on wt.WorkTypeID = dd.WorkTypeID where Period=@Period ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<WDDefaultVM>(sql, _filterHrVM);
                return Ok(result);
            }
        }

        //Payslip
        [HttpPost("GetPayslipList")]
        public async Task<ActionResult<List<PayslipVM>>> GetPayslipList(FilterHrVM _filterHrVM)
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
                parm.Add("@isSearch", _filterHrVM.IsTypeSearch);

                var result = await conn.QueryAsync<PayslipVM>("HR.Payslip_viewMain", parm, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        [HttpPost("UpdateSalaryReply")]
        public async Task<ActionResult<bool>> UpdateSalaryReply(PayslipVM _payslipVM)
        {
            var sql = "Update HR.MonthlySalaryStaff set SalaryReply = @SalaryReply, EserialSalaryReply = @UserID,TimeSalaryReply = GETDATE() where Period = @Period and Eserial=@Eserial ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _payslipVM);
            }
            return true;
        }

        [HttpPost("UpdateSalaryQuestion")]
        public async Task<ActionResult<bool>> UpdateSalaryQuestion(PayslipVM _payslipVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "";

                if (_payslipVM.TypeUpdateSalaryQuestion == 0)
                {
                    sql += "Update HR.MonthlySalaryStaff set SalaryQuestion = @SalaryQuestion, TimeSalaryQuestion = GETDATE() ";
                    sql += "where Period = @Period and Eserial=@Eserial ";
                }
                else
                {
                    sql += "Update HR.MonthlySalaryStaff set SalaryQuestion = NULL, TimeSalaryQuestion = NULL ";
                    sql += "where Period = @Period and Eserial=@Eserial ";
                }
                await conn.ExecuteAsync(sql, _payslipVM);
            }
            return true;
        }

        [HttpGet("GetPayslipUser/{_UserID}")]
        public async Task<ActionResult<IEnumerable<PayslipVM>>> GetPayslipUser(string _UserID)
        {
            var sql = "select top 12 mss.ID, mss.Period, mss.Eserial, p.Eserial + ' - ' + p.LastName + ' ' + p.MiddleName + ' ' + p.FirstName as FullNameSalaryReply, p.UrlAvatar as UrlAvatarSalaryReply, case when coalesce(SalaryQuestion,'') = '' then 0 else 1 end as TypeUpdateSalaryQuestion, * from HR.MonthlySalaryStaff mss ";
            sql += "left join HR.LockSalary ls on ls.Period = mss.Period and ls.DivisionID = mss.DivisionID ";
            sql += "left join HR.Profile p on p.Eserial = mss.EserialSalaryReply where ls.isSalLock = 1 and mss.Eserial = @Eserial order by mss.Period desc ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<PayslipVM>(sql, new { Eserial = _UserID });
                return Ok(result);
            }
        }

    }
}
