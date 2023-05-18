using Data.Infrastructure;
using Dapper;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.CRM;

namespace Data.Repositories.CRM
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public CustomerController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        [HttpPost("UpdateCustomer")]
        public async Task<ActionResult<string>> UpdateCustomer(CustomerVM _customerVM)
        {
            var sql = "";
            if(_customerVM.IsTypeUpdate==0)
            {
                if (_customerVM.CustomerID != null)
                {
                    sql += "Insert into CRM.Customer (CustomerID,CustomerName,Tel,Address) Values (@CustomerID,@CustomerName,@Tel,@Address)";
                    sql += "select @CustomerID";
                }
                else
                {
                    sql += "Create table #tmpAuto_Code_ID (Code_ID varchar(50)) ";
                    sql += "Insert #tmpAuto_Code_ID ";
                    sql += "exec SYSTEM.AUTO_CODE_ID 'CRM.Customer','CustomerID','KH','00' ";
                    sql += "Insert into CRM.Customer (CustomerID,CustomerName,Tel,Address) ";
                    sql += "select Code_ID, @CustomerName, @Tel, @Address from #tmpAuto_Code_ID ";
                    sql += "select Code_ID from #tmpAuto_Code_ID";
                }
            }
            else
            {
                sql += "Update CRM.Customer set CustomerName = @CustomerName, ";
                sql += "Tel = @Tel, Address = @Address ";
                sql += "where CustomerID = @CustomerID ";
                sql += "select @CustomerID";
            }
  
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<string>(sql, _customerVM);
            }
        }

        [HttpGet("GetCustomerByID/{_CustomerID}")]
        public async Task<ActionResult<CustomerVM>> GetCustomerByID(string _CustomerID)
        {
            var sql = "select * from CRM.Customer where CustomerID = @CustomerID";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryFirstAsync<CustomerVM>(sql, new { CustomerID = _CustomerID });
                return result;
            }
        }

        [HttpGet("GetCustomers")]
        public async Task<ActionResult<IEnumerable<CustomerVM>>> GetCustomers()
        {
            var sql = "select * from CRM.Customer ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<CustomerVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("SearchCustomers/{_searchText}")]
        public async Task<ActionResult<IEnumerable<CustomerVM>>> SearchCustomers(string _searchText)
        {
            var sql = "select * from CRM.Customer where Tel LIKE CONCAT('%',@searchText,'%') order by CustomerName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<CustomerVM>(sql, new { searchText = _searchText });
                return Ok(result);
            }
        }

        [HttpGet("CheckContains_Customer/{_CustomerID}")]
        public async Task<ActionResult<bool>> CheckContains_Customer(string _CustomerID)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM CRM.Customer where CustomerID = @CustomerID) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { CustomerID = _CustomerID });
            }
        }

        [HttpGet("CheckContains_Tel/{_Tel}")]
        public async Task<ActionResult<bool>> CheckContains_Tel(string _Tel)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM CRM.Customer where Tel = @Tel) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { Tel = _Tel });
            }
        }
    }
}
