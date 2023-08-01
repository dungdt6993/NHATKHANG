﻿using Data.Infrastructure;
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

        [HttpGet("GetVSubTypeVMs/{_VTypeID}")]
        public async Task<ActionResult<IEnumerable<VSubTypeVM>>> GetVSubTypeVMs(string _VTypeID)
        {
            var sql = "select * from FIN.VSubType where VTypeID=@VTypeID ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<VSubTypeVM>(sql, new { VTypeID = _VTypeID });
                return Ok(result);
            }
        }

        [HttpPost("GetVouchers")]
        public async Task<ActionResult<List<VoucherVM>>> GetVouchers(FilterFinVM _filterFinVM)
        {
            var sql = "select v.VNumber, v.VDate, v.VDesc, vType.VTypeID, vType.VTypeDesc, vSubType.VSubTypeID, ";
            sql += "v.VendorCode, v.CustomerCode, v.StockCode, v.VContact, v.ITypeCode, v.VActive, v.IsPayment, v.PaymentTypeCode, v.IsInventory, v.IsInvoice, v.InvoiceNumber, v.InvoiceDate ";
            sql += "from FIN.Voucher v ";
            sql += "join FIN.VType vType on vType.VTypeID = v.VTypeID ";
            sql += "left join FIN.VSubType vSubType on vSubType.VSubTypeID = v.VSubTypeID ";
            sql += "where vType.FuncID=@FuncID ";
            sql += "and format(v.VDate,'MM/dd/yyyy')>=format(@StartDate,'MM/dd/yyyy') and format(v.VDate,'MM/dd/yyyy')<=format(@EndDate,'MM/dd/yyyy') ";
            sql += "and (v.DivisionID=@DivisionID or coalesce(@DivisionID,'')='') ";
            sql += "and (v.VTypeID=@VTypeID or coalesce(@VTypeID,'')='') ";
            sql += "and (v.VActive=@searchActive or @searchActive=2) ";
            sql += "and (v.VNumber LIKE CONCAT('%',@VNumber,'%') or coalesce(@VNumber,'')='') ";
            sql += "order by v.TimeCreated desc, v.VDate ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<VoucherVM>(sql, _filterFinVM);
                return result.ToList();
            }
        }

        [HttpGet("GetVoucherDetails/{_VNumber}")]
        public async Task<ActionResult<List<VoucherDetailVM>>> GetVoucherDetails(string _VNumber)
        {
            var sql = "select SeqVD, i.ICode, i.IName, i.IPrice, i.VendorDefault, i.StockDefault, vd.VDQty, iu.IUnitName, vd.VDPrice, vd.ToStockCode, toStock.StockName as ToStockName, vd.FromStockCode, fromStock.StockName as FromStockName, vd.VDNote, ";
            sql += "vd.InventoryCheck_StockCode, inventorycheckStock.StockName as InventoryCheck_StockName, vd.InventoryCheck_Qty, vd.InventoryCheck_ActualQty, vat.VATCode, coalesce(vat.VATRate,0) as VATRate, vat.VATName ";
            sql += "from FIN.VoucherDetail vd ";
            sql += "left join FIN.Items i on i.ICode = vd.ICode ";
            sql += "left join FIN.ItemsUnit iu on iu.IUnitCode = i.IUnitCode ";
            sql += "left join FIN.VATDef vat on vd.VATCode = vat.VATCode ";
            sql += "left join FIN.Stock fromStock on fromStock.StockCode = vd.FromStockCode ";
            sql += "left join FIN.Stock toStock on toStock.StockCode = vd.ToStockCode ";
            sql += "left join FIN.Stock inventorycheckStock on inventorycheckStock.StockCode = vd.InventoryCheck_StockCode ";
            sql += "where vd.VNumber = @VNumber order by SeqVD ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<VoucherDetailVM>(sql, new { VNumber = _VNumber });
                return result.ToList();
            }
        }

        [HttpPost("GetSearchItems")]
        public async Task<ActionResult<IEnumerable<VoucherDetailVM>>> GetSearchItems(VoucherVM _voucherVM)
        {
            var sql = "select top 5 i.ICode, i.IName, iu.IUnitName, i.IPrice as VDPrice, ";
            sql += "s.StockCode as ToStockCode, s.StockName as ToStockName, ";
            sql += "s.StockCode as FromStockCode, s.StockName as FromStockName, s.StockCode as InventoryCheck_StockCode, v.VendorCode, v.VendorName from FIN.Items i ";
            sql += "join FIN.ItemsType it on it.ITypeCode = i.ITypeCode ";
            sql += "join FIN.ItemsUnit iu on iu.IUnitCode = i.IUnitCode ";
            sql += "left join FIN.Stock s on s.StockCode = i.StockDefault ";
            sql += "left join FIN.Vendor v on v.VendorCode = i.VendorDefault ";
            sql += "where i.IActive=1 and i.ITypeCode=@ITypeCode and (i.ICode LIKE CONCAT('%',@valueSearchItems,'%') or i.IName LIKE CONCAT('%',@valueSearchItems,'%')) ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<VoucherDetailVM>(sql, _voucherVM);
                return Ok(result);
            }
        }

        [HttpPost("UpdateVoucher")]
        public async Task<ActionResult<string>> UpdateVoucher(ArrayList _arrayList)
        {
            VoucherVM _voucherVM = JsonConvert.DeserializeObject<VoucherVM>(_arrayList[0].ToString());

            IEnumerable<VoucherDetailVM> _voucherDetailVMs = JsonConvert.DeserializeObject<IEnumerable<VoucherDetailVM>>(_arrayList[1].ToString());

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var sqlVoucherVM = String.Empty;
                var sqlVoucherDetailVM = String.Empty;

                var VNumber = _voucherVM.VNumber;

                if (_voucherVM.IsTypeUpdate == 0)
                {
                    //VoucherVM
                    sqlVoucherVM += "Create table #tmpAuto_Code_ID (Code_ID varchar(50)) ";
                    sqlVoucherVM += "Insert #tmpAuto_Code_ID ";
                    sqlVoucherVM += "exec SYSTEM.AUTO_CODE_ID 'FIN.Voucher','VNumber',@VCode,'0000' ";
                    sqlVoucherVM += "Insert into FIN.Voucher (DivisionID,VNumber,VDesc,VDate,VendorCode,CustomerCode,StockCode,VContact,ITypeCode,VActive,IsPayment,PaymentTypeCode,IsInventory,IsInvoice,InvoiceNumber,InvoiceDate,VTypeID,VSubTypeID,TimeCreated) ";
                    sqlVoucherVM += "select @DivisionID,CODE_ID,@VDesc,@VDate,@VendorCode,@CustomerCode,@StockCode,@VContact,@ITypeCode,@VActive,@IsPayment,@PaymentTypeCode,@IsInventory,@IsInvoice,@InvoiceNumber,@InvoiceDate,@VTypeID,@VSubTypeID,GETDATE() from #tmpAuto_Code_ID ";
                    sqlVoucherVM += "select CODE_ID from #tmpAuto_Code_ID ";

                    VNumber = await conn.ExecuteScalarAsync<string>(sqlVoucherVM, _voucherVM);

                    //VoucherDetailVM
                    foreach (var _voucherDetailVM in _voucherDetailVMs)
                    {
                        sqlVoucherDetailVM = "Insert into FIN.VoucherDetail(VNumber,ICode,VDQty,VDPrice,VATCode,FromStockCode,ToStockCode,VDNote,InventoryCheck_StockCode,InventoryCheck_Qty,InventoryCheck_ActualQty) ";
                        sqlVoucherDetailVM += "Values('" + VNumber + "',@ICode,@VDQty,@VDPrice,@VATCode,@FromStockCode,@ToStockCode,@VDNote,@InventoryCheck_StockCode,@InventoryCheck_Qty,@InventoryCheck_ActualQty) ";
                        await conn.ExecuteAsync(sqlVoucherDetailVM, _voucherDetailVM);
                    }
                }

                if (_voucherVM.IsTypeUpdate == 1)
                {
                    //VoucherVM
                    sqlVoucherVM = "Update FIN.Voucher set VDesc=@VDesc,VDate=@VDate,VendorCode=@VendorCode,CustomerCode=@CustomerCode,StockCode=@StockCode,VContact=@VContact,ITypeCode=@ITypeCode,IsPayment=@IsPayment,PaymentTypeCode=@PaymentTypeCode,IsInventory=@IsInventory,IsInvoice=@IsInvoice,InvoiceNumber=@InvoiceNumber,InvoiceDate=@InvoiceDate, VSubTypeID=@VSubTypeID ";
                    sqlVoucherVM += "where VNumber=@VNumber ";
                    sqlVoucherVM += "Delete from FIN.VoucherDetail where VNumber=@VNumber ";
                    await conn.ExecuteAsync(sqlVoucherVM, _voucherVM);

                    //VoucherDetailVM
                    foreach (var _voucherDetailVM in _voucherDetailVMs)
                    {
                        sqlVoucherDetailVM = "Insert into FIN.VoucherDetail(VNumber,ICode,VDQty,VDPrice,VATCode,FromStockCode,ToStockCode,VDNote,InventoryCheck_StockCode,InventoryCheck_Qty,InventoryCheck_ActualQty) ";
                        sqlVoucherDetailVM += "Values('" + _voucherVM.VNumber + "',@ICode,@VDQty,@VDPrice,@VATCode,@FromStockCode,@ToStockCode,@VDNote,@InventoryCheck_StockCode,@InventoryCheck_Qty,@InventoryCheck_ActualQty) ";

                        await conn.ExecuteAsync(sqlVoucherDetailVM, _voucherDetailVM);
                    }
                }

                if (_voucherVM.IsTypeUpdate == 2)
                {
                    sqlVoucherVM = "delete from FIN.Voucher where VNumber=@VNumber ";
                    sqlVoucherVM += "delete from FIN.VoucherDetail where VNumber=@VNumber ";
                    await conn.ExecuteAsync(sqlVoucherVM, _voucherVM);
                }

                if (_voucherVM.IsTypeUpdate == 4)
                {
                    sqlVoucherVM = "Update FIN.Voucher set VActive=@VActive where VNumber=@VNumber ";
                    await conn.ExecuteAsync(sqlVoucherVM, _voucherVM);
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
