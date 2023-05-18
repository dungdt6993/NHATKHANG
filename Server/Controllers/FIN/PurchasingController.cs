using Data.Infrastructure;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.FIN;

namespace D69soft.Server.Controllers.FIN
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasingController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public PurchasingController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        [HttpGet("GetVendorList")]
        public async Task<ActionResult<IEnumerable<VendorVM>>> GetVendorList()
        {
            var sql = "select * from FIN.Vendor where VendorActive=1 order by VendorName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<VendorVM>(sql);
                return Ok(result);
            }
        }

    }
}
