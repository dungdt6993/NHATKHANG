using Data.Infrastructure;
using Dapper;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.FIN;

namespace D69soft.Server.Controllers.FIN
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

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                if (_customerVM.IsTypeUpdate == 0)
                {
                    sql += "Create table #tmpAuto_Code_ID (Code_ID varchar(50)) ";
                    sql += "Insert #tmpAuto_Code_ID ";
                    sql += "exec SYSTEM.AUTO_CODE_ID 'CRM.Customer','CustomerCode','KH','0000' ";
                    sql += "Insert into CRM.Customer (CustomerCode,CustomerName,CustomerTaxCode,CustomerBirthday,CustomerTel,CustomerAddress) ";
                    sql += "select Code_ID, @CustomerName, @CustomerTaxCode, @CustomerBirthday, @CustomerTel, @CustomerAddress from #tmpAuto_Code_ID ";
                    sql += "select Code_ID from #tmpAuto_Code_ID";

                    _customerVM.CustomerCode = await conn.ExecuteScalarAsync<string>(sql, _customerVM);
                }

                if (_customerVM.IsTypeUpdate == 1)
                {
                    sql += "Update CRM.Customer set CustomerName = @CustomerName, CustomerTaxCode = @CustomerTaxCode, CustomerBirthday = @CustomerBirthday, CustomerTel = @CustomerTel, CustomerAddress = @CustomerAddress  ";
                    sql += "where CustomerCode = @CustomerCode ";
                    await conn.ExecuteAsync(sql, _customerVM);
                }

                if (_customerVM.IsTypeUpdate == 2)
                {
                    sql = "delete from CRM.Customer where CustomerCode=@CustomerCode ";
                    await conn.ExecuteAsync(sql, _customerVM);
                }

                return _customerVM.CustomerCode;
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

        [HttpGet("ContainsCustomerTel/{_CustomerTel}")]
        public async Task<ActionResult<bool>> ContainsCustomerTel(string _CustomerTel)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM CRM.Customer where CustomerTel = @CustomerTel) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { CustomerTel = _CustomerTel });
            }
        }
    }
}
