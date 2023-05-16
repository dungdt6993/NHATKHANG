using D69soft.Shared.Models.ViewModels.SYSTEM;
using Dapper;
using Data.Infrastructure;
using System.Data;
using System.Data.SqlClient;

namespace D69soft.Server.Services
{
    public class SysService
    {
        private readonly SqlConnectionConfig _connConfig;

        public SysService(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        //Log Err
        public async Task ErrorLog(ErrorLogVM _errorLogVM)
        {
            var sql = "Insert into SYSTEM.ErrorLog (ErrType, ErrMessage, ErrTime, ErrNote) ";
            sql += "Values (@ErrType,@ErrMessage,@ErrTime,@ErrNote)";

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _errorLogVM);
            }
        }

        //Info User
        public async Task<UserVM> GetInfoUser(string _UserID)
        {
            var sql = "Select p.Eserial, LastName, MiddleName, FirstName, UrlAvatar, di.DivisionID, di.DivisionName, de.DepartmentName, po.PositionName, s.JoinDate from HR.Profile p ";
            sql += "left join HR.Staff s on s.Eserial = p.Eserial ";
            sql += "left join (select * from HR.JobHistory where CurrentJobID=1) jh on jh.Eserial = p.Eserial ";
            sql += "left join HR.Division di on di.DivisionID = jh.DivisionID ";
            sql += "left join HR.Department de on de.DepartmentID = jh.DepartmentID ";
            sql += "left join HR.Position po on po.PositionID = jh.PositionID ";
            sql += "where p.Eserial = @UserID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryFirstOrDefaultAsync<UserVM>(sql, new { UserID = _UserID });
                return result;
            }
        }

        //Menu Func
        public async Task<IEnumerable<FuncVM>> GetModuleMenu(string _UserID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@UserID", _UserID);
                parm.Add("@typeView", 1);

                var result = await conn.QueryAsync<FuncVM>("SYSTEM.viewFunc", parm, commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<IEnumerable<FuncVM>> GetFuncMenuGroup(string _UserID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@UserID", _UserID);
                parm.Add("@typeView", 2);

                var result = await conn.QueryAsync<FuncVM>("SYSTEM.viewFunc", parm, commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<IEnumerable<FuncVM>> GetFuncMenu(string _UserID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@UserID", _UserID);
                parm.Add("@typeView", 3);

                var result = await conn.QueryAsync<FuncVM>("SYSTEM.viewFunc", parm, commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<bool> CheckViewFuncMenuRpt(string _UserID)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (select 1 from SYSTEM.PermissionRpt where UserID=@UserID) THEN 1 ELSE 0 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { UserID = _UserID });
            }
        }

    }
}
