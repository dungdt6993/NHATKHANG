using Data.Infrastructure;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;

namespace D69soft.Server.Controllers.FIN
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoneyController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public MoneyController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        [HttpGet("GetBankList")]
        public async Task<ActionResult<IEnumerable<BankVM>>> GetBankList()
        {
            var sql = "select * from FIN.Bank order by BankShortName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<BankVM>(sql);
                return Ok(result);
            }
        }

        [HttpPost("UpdateBank")]
        public async Task<ActionResult<bool>> UpdateBank(BankVM _bankVM)
        {
            var sql = "";
            if (_bankVM.IsTypeUpdate == 0)
            {
                sql = "Insert into FIN.Bank (SwiftCode,BankShortName,BankFullName,BankNameEng,BankLogo,BankActive) Values (@SwiftCode,@BankShortName,@BankFullName,@BankNameEng,@BankLogo,@BankActive)";
            }
            if (_bankVM.IsTypeUpdate == 1)
            {
                sql = "Update FIN.Stock set BankShortName = @BankShortName, BankFullName = @BankFullName, BankNameEng = @BankNameEng, BankLogo = @BankLogo, BankActive = @BankActive where SwiftCode = @SwiftCode";
            }
            if (_bankVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from FIN.BankAccount where SwiftCode=@SwiftCode) ";
                sql += "begin ";
                sql += "delete from FIN.Bank where SwiftCode = @SwiftCode ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _bankVM);
            }

            return true;
        }
    }
}
