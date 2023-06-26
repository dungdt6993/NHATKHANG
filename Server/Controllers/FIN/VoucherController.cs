using Data.Infrastructure;
using Dapper;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.FIN;
using Newtonsoft.Json;
using System.Collections;
using System.Data;

namespace D69soft.Server.Controllers.FIN
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public VoucherController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        [HttpGet("GetVTypeVMs/{_FuncID}")]
        public async Task<ActionResult<IEnumerable<VTypeVM>>> GetVTypeVMs(string _FuncID)
        {
            var sql = "select * from FIN.VType where FuncID=@FuncID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<VTypeVM>(sql, new { FuncID = _FuncID });
                return Ok(result);
            }
        }

        [HttpGet("GetVSubTypeVMs/{_FuncID}")]
        public async Task<ActionResult<IEnumerable<VSubTypeVM>>> GetVSubTypeVMs(string _FuncID)
        {
            var sql = "select * from FIN.VSubType where VTypeID in (select VTypeID from FIN.VType where FuncID=@FuncID) ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<VSubTypeVM>(sql, new { FuncID = _FuncID });
                return Ok(result);
            }
        }

        [HttpPost("GetStockVouchers")]
        public async Task<ActionResult<List<StockVoucherVM>>> GetStockVouchers(FilterFinVM _filterFinVM)
        {
            var sql = "select sv.VNumber, sv.VDate, sv.VDesc, sv.VTypeID, sv.VSubTypeID, vType.VTypeDesc, vsType.VSubTypeDesc, ";
            sql += "pEU.Eserial + ' - ' + pEU.LastName + ' ' + pEU.MiddleName + ' ' + pEU.FirstName as FullNameEU, pEC.Eserial + ' - ' + pEC.LastName + ' ' + pEC.MiddleName + ' ' + pEC.FirstName as FullNameEC, sv.TimeUpdate, sv.TimeCreated, ";
            sql += "sv.VendorCode, sv.CustomerCode, sv.VActive, sv.IsMultipleInvoice, sv.Reference_VNumber, ref_vtype.FuncURL, ref_vtype.Reference_VSubTypeName, sv.Reference_StockCode, s.StockCode, s.StockName from FIN.StockVoucher sv ";
            sql += "left join FIN.Stock s on s.StockCode = sv.StockCode ";
            sql += "left join HR.Profile pEC on pEC.Eserial = sv.EserialCreated ";
            sql += "left join HR.Profile pEU on pEU.Eserial = sv.EserialUpdate ";
            sql += "join FIN.VType vType on vType.VTypeID = sv.VTypeID ";
            sql += "join FIN.VSubType vsType on vsType.VSubTypeID = sv.VSubTypeID ";

            sql += "left join ( ";
            sql += "select f.FuncURL, vs.VSubTypeID as Reference_VSubTypeID, vs.VSubTypeDesc as Reference_VSubTypeName from FIN.VType v ";
            sql += "join FIN.VSubType vs on vs.VTypeID = v.VTypeID ";
            sql += "join SYSTEM.Func f on f.FuncID = v.FuncID ";
            sql += ") ref_vtype on ref_vtype.Reference_VSubTypeID = sv.Reference_VSubTypeID ";

            sql += "where vType.FuncID=@FuncID ";
            sql += "and format(sv.VDate,'MM/dd/yyyy')>=format(@StartDate,'MM/dd/yyyy') and format(sv.VDate,'MM/dd/yyyy')<=format(@EndDate,'MM/dd/yyyy') ";
            sql += "and (sv.DivisionID=@DivisionID or coalesce(@DivisionID,'')='') ";
            sql += "and (sv.VSubTypeID=@VSubTypeID or coalesce(@VSubTypeID,'')='') ";
            sql += "and (sv.VActive=@searchActive or @searchActive=2) ";
            sql += "and (sv.VNumber LIKE CONCAT('%',@VNumber,'%') or coalesce(@VNumber,'')='') ";
            sql += "order by sv.TimeCreated desc, sv.VDate ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<StockVoucherVM>(sql, _filterFinVM);
                return result.ToList();
            }
        }

        [HttpGet("GetStockVoucherDetails/{_VNumber}")]
        public async Task<ActionResult<List<StockVoucherDetailVM>>> GetStockVoucherDetails(string _VNumber)
        {
            var sql = "select SeqVD, i.ICode, i.IName, i.IPrice, i.VendorDefault, i.StockDefault, svd.Qty, iu.IUnitName, svd.Price, svd.ToStockCode, toStock.StockName as ToStockName, svd.FromStockCode, fromStock.StockName as FromStockName, svd.VDNote, ";
            sql += "svd.VendorCode, v.VendorName, svd.InventoryCheck_StockCode, inventorycheckStock.StockName as InventoryCheck_StockName, svd.InventoryCheck_Qty, svd.InventoryCheck_ActualQty, svd.Request_ICode, svd.IsReference, ";
            sql += "vat.VATCode, coalesce(vat.VATRate,0) as VATRate, vat.VATName ";
            sql += "from FIN.StockVoucherDetail svd ";
            sql += "join FIN.Items i on i.ICode = svd.ICode ";
            sql += "join FIN.ItemsUnit iu on iu.IUnitCode = i.IUnitCode ";
            sql += "left join FIN.VATDef vat on svd.VATCode = vat.VATCode ";
            sql += "left join FIN.Stock fromStock on fromStock.StockCode = svd.FromStockCode ";
            sql += "left join FIN.Stock toStock on toStock.StockCode = svd.ToStockCode ";
            sql += "left join FIN.Vendor v on v.VendorCode = svd.VendorCode ";
            sql += "left join FIN.Stock inventorycheckStock on inventorycheckStock.StockCode = svd.InventoryCheck_StockCode ";
            sql += "where svd.VNumber = @VNumber order by SeqVD ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<StockVoucherDetailVM>(sql, new { VNumber = _VNumber });
                return result.ToList();
            }
        }

        [HttpPost("GetSearchItems")]
        public async Task<ActionResult<IEnumerable<StockVoucherDetailVM>>> GetSearchItems(FilterFinVM _filterFinVM)
        {
            var sql = "select top 5 i.ICode, i.IName, iu.IUnitName, i.IPrice as Price, ";
            sql += "s.StockCode as ToStockCode, s.StockName as ToStockName, ";
            sql += "s.StockCode as FromStockCode, s.StockName as FromStockName, v.VendorCode, v.VendorName from FIN.Items i ";
            sql += "join FIN.ItemsType it on it.ITypeCode = i.ITypeCode ";
            sql += "join FIN.ItemsUnit iu on iu.IUnitCode = i.IUnitCode ";
            sql += "left join FIN.Stock s on s.StockCode = i.StockDefault ";
            sql += "left join FIN.Vendor v on v.VendorCode = i.VendorDefault ";
            sql += "where i.IActive=1 and it.IsInventory=1 and (i.ICode LIKE CONCAT('%',@searchText,'%') or i.IName LIKE CONCAT('%',@searchText,'%')) ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<StockVoucherDetailVM>(sql, _filterFinVM);
                return Ok(result);
            }
        }

        [HttpPost("UpdateVoucher")]
        public async Task<ActionResult<string>> UpdateVoucher(ArrayList _arrayList)
        {
            StockVoucherVM _stockVoucherVM = JsonConvert.DeserializeObject<StockVoucherVM>(_arrayList[0].ToString());

            IEnumerable<StockVoucherDetailVM> _stockVoucherDetailVMs = JsonConvert.DeserializeObject<IEnumerable<StockVoucherDetailVM>>(_arrayList[1].ToString());

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var sqlStockVoucherVM = String.Empty;
                var sqlStockVoucherDetailVM = String.Empty;

                var VNumber = _stockVoucherVM.VNumber;

                if (_stockVoucherVM.IsTypeUpdate == 0)
                {
                    //StockVoucherVM
                    sqlStockVoucherVM = "Declare @VCode varchar(50) ";
                    sqlStockVoucherVM += "select @VCode = VCode from FIN.VSubType where VSubTypeID=@VSubTypeID ";
                    sqlStockVoucherVM += "Create table #tmpAuto_Code_ID (Code_ID varchar(50)) ";
                    sqlStockVoucherVM += "Insert #tmpAuto_Code_ID ";
                    sqlStockVoucherVM += "exec SYSTEM.AUTO_CODE_ID 'FIN.StockVoucher','VNumber',@VCode,'0000' ";
                    sqlStockVoucherVM += "Insert into FIN.StockVoucher (DivisionID,VNumber,VDesc,VDate,VendorCode,CustomerCode,VTypeID,VSubTypeID,IsMultipleInvoice,VActive,EserialCreated,TimeCreated,Reference_VNumber,Reference_StockCode,Reference_VSubTypeID,StockCode) ";
                    sqlStockVoucherVM += "select @DivisionID,CODE_ID,@VDesc,@VDate,@VendorCode,@CustomerCode,@VTypeID,@VSubTypeID,@IsMultipleInvoice,@VActive,@UserID,GETDATE(),@Reference_VNumber,@Reference_StockCode,@Reference_VSubTypeID,@StockCode from #tmpAuto_Code_ID ";
                    sqlStockVoucherVM += "select CODE_ID from #tmpAuto_Code_ID ";

                    VNumber = await conn.ExecuteScalarAsync<string>(sqlStockVoucherVM, _stockVoucherVM);

                    //StockVoucherDetailVM
                    foreach (var _stockVoucherDetailVM in _stockVoucherDetailVMs)
                    {
                        sqlStockVoucherDetailVM = "Insert into FIN.StockVoucherDetail(VNumber,ICode,Qty,Price,VATCode,FromStockCode,ToStockCode,VendorCode,CustomerCode,VDNote,InventoryCheck_StockCode,InventoryCheck_Qty,InventoryCheck_ActualQty,Request_ICode,IsReference) ";
                        sqlStockVoucherDetailVM += "Values('" + VNumber + "',@ICode,@Qty,@Price,@VATCode,@FromStockCode,@ToStockCode,@VendorCode,@CustomerCode,@VDNote,@InventoryCheck_StockCode,@InventoryCheck_Qty,@InventoryCheck_ActualQty,@ICode,@IsReference) ";
                        await conn.ExecuteAsync(sqlStockVoucherDetailVM, _stockVoucherDetailVM);
                    }
                }

                if (_stockVoucherVM.IsTypeUpdate == 1)
                {
                    //StockVoucherVM
                    sqlStockVoucherVM = "Update FIN.StockVoucher set VTypeID=@VTypeID,VSubTypeID=@VSubTypeID,VDesc=@VDesc,VDate=@VDate,VendorCode=@VendorCode,CustomerCode=@CustomerCode,IsMultipleInvoice=@IsMultipleInvoice,StockCode=@StockCode,EserialUpdate=@UserID,TimeUpdate=GETDATE() ";
                    sqlStockVoucherVM += "where VNumber=@VNumber ";
                    sqlStockVoucherVM += "Delete from FIN.StockVoucherDetail where VNumber=@VNumber ";
                    await conn.ExecuteAsync(sqlStockVoucherVM, _stockVoucherVM);

                    //StockVoucherDetailVM
                    foreach (var _stockVoucherDetailVM in _stockVoucherDetailVMs)
                    {
                        sqlStockVoucherDetailVM = "Insert into FIN.StockVoucherDetail(VNumber,ICode,Qty,Price,VATCode,FromStockCode,ToStockCode,VendorCode,CustomerCode,VDNote,InventoryCheck_StockCode,InventoryCheck_Qty,InventoryCheck_ActualQty,Request_ICode,IsReference) ";
                        sqlStockVoucherDetailVM += "Values('" + _stockVoucherVM.VNumber + "',@ICode,@Qty,@Price,@VATCode,@FromStockCode,@ToStockCode,@VendorCode,@CustomerCode,@VDNote,@InventoryCheck_StockCode,@InventoryCheck_Qty,@InventoryCheck_ActualQty,@Request_ICode,@IsReference) ";
                        await conn.ExecuteAsync(sqlStockVoucherDetailVM, _stockVoucherDetailVM);
                    }
                }

                if (_stockVoucherVM.IsTypeUpdate == 2)
                {
                    sqlStockVoucherVM = "delete from FIN.StockVoucher where VNumber=@VNumber ";
                    sqlStockVoucherVM += "delete from FIN.StockVoucherDetail where VNumber=@VNumber ";
                    await conn.ExecuteAsync(sqlStockVoucherVM, _stockVoucherVM);
                }

                if (_stockVoucherVM.IsTypeUpdate == 4)
                {
                    sqlStockVoucherVM = "Update FIN.StockVoucher set VActive=@VActive,EserialUpdate=@UserID,TimeUpdate=GETDATE() where VNumber=@VNumber ";
                    await conn.ExecuteAsync(sqlStockVoucherVM, _stockVoucherVM);
                }

                return VNumber;
            }
        }

        //VAT
        [HttpGet("GetVATDefs")]
        public async Task<ActionResult<IEnumerable<VATDefVM>>> GetVATDefs()
        {
            var sql = "select * from FIN.VATDef order by VATRate, VATName desc ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<VATDefVM>(sql);
                return Ok(result);
            }
        }

    }
}
