using Data.Infrastructure;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace Data.Repositories.CRUISES
{
    [Route("api/[controller]")]
    [ApiController]
    public class OccupancyController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public OccupancyController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        //SyncCRS
        [HttpGet("SyncDataCRS")]
        public async Task<ActionResult<bool>> SyncDataCRS()
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();

                await conn.ExecuteAsync("SYNC.BHAYASOFT_CRS", parm, commandType: CommandType.StoredProcedure);

                return true;
            }
        }
    }
}
