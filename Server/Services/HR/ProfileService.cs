using D69soft.Shared.Models.ViewModels.HR;
using Dapper;
using Data.Infrastructure;
using System.Data;
using System.Data.SqlClient;

namespace D69soft.Server.Services.HR
{
    public class ProfileService
    {
        private readonly SqlConnectionConfig _connConfig;

        public ProfileService(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        //Contact
        public async Task<List<ProfileManagamentVM>> GetContacts(string _UserID)
        {
            var sql = "select p.UrlAvatar, p.Eserial, p.LastName, p.MiddleName, p.FirstName, p.LastName + ' ' + p.MiddleName + ' ' + p.FirstName as FullName, p.Birthday, p.Mobile, s.EmailCompany, de.DepartmentName, po.PositionName from HR.Profile p ";
            sql += "join (select * from HR.Staff where coalesce(Terminated,0) = 0) s on s.Eserial = p.Eserial ";
            sql += "join (select * from HR.JobHistory where CurrentJobID=1 and DivisionID in (select DivisionID from HR.JobHistory where CurrentJobID=1 and Eserial=@UserID)) jh on jh.Eserial = s.Eserial ";
            sql += "join HR.Department de on de.DepartmentID = jh.DepartmentID ";
            sql += "join HR.Position po on po.PositionID = jh.PositionID ";
            sql += "order by de.DepartmentName, p.FirstName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ProfileManagamentVM>(sql, new { UserID = _UserID });
                return result.ToList();
            }
        }

    }
}