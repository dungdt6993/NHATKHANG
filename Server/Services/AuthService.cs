using Dapper;
using Data.Infrastructure;
using Utilities;
using System.Data;
using System.Data.SqlClient;
using D69soft.Shared.Models.ViewModels.SYSTEM;

namespace D69soft.Server.Services
{
    public class AuthService
    {
        private readonly SqlConnectionConfig _connConfig;

        public AuthService(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        public async Task<int> LoginRequest(UserVM _userVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@txtUser", _userVM.Eserial);
                parm.Add("@txtPass", LibraryFunc.GennerateToMD5(_userVM.User_Password));

                var affectedRows = await conn.ExecuteAsync("SYSTEM.Authentication_login", parm, commandType: CommandType.StoredProcedure);

                return affectedRows;
            }
        }

        public async Task<bool> ChangePass(ChangePassVM _changePassVM)
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

        public async Task<bool> CheckChangePassDefault(string _UserID)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.Profile where Eserial = @UserID and coalesce(User_isChangePass,0)=0) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { UserID = _UserID });
            }
        }

    }
}