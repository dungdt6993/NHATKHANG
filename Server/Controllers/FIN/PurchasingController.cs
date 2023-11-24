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
            var sql = "select * from FIN.Vendor order by VendorName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<VendorVM>(sql);
                return Ok(result);
            }
        }

        [HttpPost("UpdateVendor")]
        public async Task<ActionResult<string>> UpdateVendor(VendorVM _vendorVM)
        {
            var sql = "";

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                if (_vendorVM.IsTypeUpdate == 0)
                {
                    sql = "Create table #tmpAuto_Code_ID (Code_ID varchar(50)) ";
                    sql += "Insert #tmpAuto_Code_ID ";
                    sql += "exec SYSTEM.AUTO_CODE_ID 'FIN.Vendor','VendorCode','NCC','0000' ";
                    sql += "Insert into FIN.Vendor (VendorCode,VendorName,VendorTaxCode,VendorAddress,VendorTel,VendorContractFile,VendorContractStartDate,VendorContractEndDate) ";
                    sql += "select Code_ID,@VendorName,@VendorTaxCode,@VendorAddress,@VendorTel,@VendorContractFile,@VendorContractStartDate,@VendorContractEndDate from #tmpAuto_Code_ID ";
                    sql += "select Code_ID from #tmpAuto_Code_ID";
                }

                if (_vendorVM.IsTypeUpdate == 1)
                {
                    sql += "Update FIN.Vendor set VendorName = @VendorName, VendorTaxCode = @VendorTaxCode, VendorAddress = @VendorAddress, VendorTel = @VendorTel, VendorContractFile = @VendorContractFile, VendorContractStartDate = @VendorContractStartDate, VendorContractEndDate = @VendorContractEndDate where VendorCode = @VendorCode ";
                    await conn.ExecuteAsync(sql, _vendorVM);
                }

                if (_vendorVM.IsTypeUpdate == 2)
                {
                    sql = "if not exists (select * from FIN.Voucher where VendorCode=@VendorCode) ";
                    sql += "begin ";
                    sql += "delete from FIN.Vendor where VendorCode=@VendorCode ";
                    sql += "end ";
                    sql += "else ";
                    sql += "begin ";
                    sql += "select 'Err_NotDel' ";
                    sql += "end ";
                }

                _vendorVM.VendorCode = await conn.ExecuteScalarAsync<string>(sql, _vendorVM);

                return _vendorVM.VendorCode;
            }


        }

    }
}
