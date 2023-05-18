using D69soft.Server.Services;
using D69soft.Server.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Dapper;
using Data.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Utilities;

namespace D69soft.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public AuthController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        [HttpPost("LoginRequest")]
        public async Task<ActionResult<int>> LoginRequest(UserVM _UserVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@txtUser", _UserVM.Eserial);
                parm.Add("@txtPass", LibraryFunc.GennerateToMD5(_UserVM.User_Password));

                var affectedRows = await conn.ExecuteAsync("SYSTEM.Authentication_login", parm, commandType: CommandType.StoredProcedure);

                return affectedRows;
            }
        }

        [HttpGet("CheckChangePassDefault/{_UserID}")]
        public async Task<ActionResult<bool>> CheckChangePassDefault(string _UserID)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.Profile where Eserial = @UserID and coalesce(User_isChangePass,0)=0) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { UserID = _UserID });
            }
        }

        [HttpPost("ChangePass")]
        public async Task<ActionResult<bool>> ChangePass(ChangePassVM _changePassVM)
        {
            _changePassVM.NewPass = LibraryFunc.GennerateToMD5(_changePassVM.NewPass);

            var sql = "Update HR.Profile set User_Password = @NewPass, User_isChangePass = 1 where Eserial = @Eserial";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _changePassVM);
            }

            return true;
        }
    }
}
