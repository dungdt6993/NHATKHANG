using Data.Infrastructure;
using Model.ViewModels.HR;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;

namespace Data.Repositories.HR
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgreementTextController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public AgreementTextController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        [HttpGet("GetAgreementTextTypeList")]
        public async Task<ActionResult<IEnumerable<AgreementTextTypeVM>>> GetAgreementTextTypeList()
        {
            var sql = "select RptID, RptName from SYSTEM.Rpt where RptID in (select distinct RptID from HR.AdjustProfileRpt) order by RptName";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<AgreementTextTypeVM>(sql);
                return result;
            }
        }

        [HttpPost("GetAgreementTextList")]
        public async Task<ActionResult<List<AgreementTextVM>>> GetAgreementTextList(FilterHrVM _filterHrVM)
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
                parm.Add("@AgreementText", _filterHrVM.RptID);

                var result = await conn.QueryAsync<AgreementTextVM>("HR.AgreementText_viewMain", parm, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        [HttpPost("PrintAgreementText/{_agreementTexts}/{_UserID}")]
        public async Task<ActionResult<IEnumerable<SysRptVM>>> PrintAgreementText(IEnumerable<AgreementTextVM> _agreementTexts, string _UserID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sqlInsertTmp = "delete from HR.tmpPrintLaborContract ";
                foreach(var _agreementText in _agreementTexts)
                {
                    sqlInsertTmp += "Insert into HR.tmpPrintLaborContract Values('"+ _agreementText.Seq+"') ";
                }
                await conn.ExecuteAsync(sqlInsertTmp);

                var sqlUpdateLog = " Update HR.LogPrintAgreementText set isPrint = 1, EserialPrint = @UserID, TimePrint = GETDATE() where Seq in (select SeqLog from HR.tmpPrintLaborContract) ";
                await conn.ExecuteAsync(sqlUpdateLog, new { UserID = _UserID });

                var sql = "select distinct r.RptID ,r.RptUrl from SYSTEM.Rpt r join HR.LogPrintAgreementText lpat on lpat.RptID = r.RptID where Seq in (select SeqLog from HR.tmpPrintLaborContract) ";

                var result = await conn.QueryAsync<SysRptVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("GetAdjustProfileList")]
        public async Task<ActionResult<IEnumerable<AdjustProfileVM>>> GetAdjustProfileList()
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "select AdjustProfileID, AdjustProfileName from HR.AdjustProfile ";
                sql += "order by AdjustProfileID ";

                var result = await conn.QueryAsync<AdjustProfileVM>(sql);
                return result;
            }
        }

        [HttpGet("GetAdjustProfileRptList")]
        public async Task<ActionResult<IEnumerable<AdjustProfileRptVM>>> GetAdjustProfileRptList()
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "select ap.AdjustProfileID, ap.AdjustProfileName, rpt.RptID, rpt.RptName from HR.AdjustProfileRpt apr ";
                sql += "join HR.AdjustProfile ap on ap.AdjustProfileID = apr.AdjustProfileID ";
                sql += "join SYSTEM.Rpt rpt on rpt.RptID = apr.RptID ";
                sql += "order by ap.AdjustProfileName ";

                var result = await conn.QueryAsync<AdjustProfileRptVM>(sql);
                return result;
            }
        }

        [HttpPost("UpdateAdjustProfile")]
        public async Task<ActionResult<bool>> UpdateAdjustProfile(AdjustProfileVM _adjustProfileVM)
        {
            var sql = "";
            sql += "Update HR.AdjustProfile set AdjustProfileName = @AdjustProfileName where AdjustProfileID = @AdjustProfileID ";

            if (_adjustProfileVM.strRpt != string.Empty)
            {
                _adjustProfileVM.strRpt = "," + _adjustProfileVM.strRpt + ",";
            }

            sql += "delete from HR.AdjustProfileRpt where AdjustProfileID = @AdjustProfileID ";
            sql += "Insert into HR.AdjustProfileRpt (AdjustProfileID, RptID) select @AdjustProfileID, RptID from SYSTEM.Rpt where CHARINDEX(',' +CONVERT(VARCHAR(MAX), RptID) + ',',@strRpt)>0 ";

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _adjustProfileVM);
                return true;
            }
        }
    }
}
