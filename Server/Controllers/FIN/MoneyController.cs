using Data.Infrastructure;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Client.Pages.FIN;

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


        //Bank
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

        [HttpGet("CheckContainsSwiftCode/{id}")]
        public async Task<bool> CheckContainsSwiftCode(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM FIN.Bank where SwiftCode = @id) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { DivisionID = id });
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
                sql = "Update FIN.Bank set BankShortName = @BankShortName, BankFullName = @BankFullName, BankNameEng = @BankNameEng, BankLogo = @BankLogo, BankActive = @BankActive where SwiftCode = @SwiftCode";
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

        //BankAccount
        [HttpGet("GetBankAccountList")]
        public async Task<ActionResult<IEnumerable<BankAccountVM>>> GetBankAccountList()
        {
            var sql = "select * from FIN.BankAccount ba join FIN.Bank b on b.SwiftCode = ba.SwiftCode order by BankAccount ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<BankAccountVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("CheckContainsBankAccount/{id}")]
        public async Task<bool> CheckContainsBankAccount(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM FIN.BankAccount where BankAccount +''+ SwiftCode = @id) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { DivisionID = id });
            }
        }

        [HttpPost("UpdateBankAccount")]
        public async Task<ActionResult<int>> UpdateBankAccount(BankAccountVM _bankAccountVM)
        {
            var sql = "";
            if (_bankAccountVM.IsTypeUpdate == 0)
            {
                sql = "Insert into FIN.BankAccount (BankAccount,SwiftCode,AccountHolder) Values (@BankAccount,@SwiftCode,@AccountHolder)";
            }
            if (_bankAccountVM.IsTypeUpdate == 1)
            {
                sql = "Update FIN.BankAccount set BankAccount = @BankAccount, SwiftCode = @SwiftCode, AccountHolder = @AccountHolder where BankAccountID = @BankAccountID";
            }
            if (_bankAccountVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from FIN.Voucher where BankAccountID=@BankAccountID) ";
                sql += "begin ";
                sql += "delete from FIN.BankAccount where BankAccountID = @BankAccountID ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteAsync(sql, _bankAccountVM);
            }
        }
    }
}
