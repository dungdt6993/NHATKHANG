using Data.Infrastructure;
using Dapper;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.FIN;
using Newtonsoft.Json;
using System.Collections;
using System.Data;
using D69soft.Client.Pages.FIN;

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
            var sql = "select v.VNumber, v.VReference, v.VDate, v.VDesc, vType.VTypeID, vType.VTypeDesc, vSubType.VSubTypeID, ";
            sql += "v.VendorCode, v.CustomerCode, v.StockCode, v.BankAccountID, v.VContact, v.ITypeCode, v.VActive, v.IsPayment, v.PaymentTypeCode, v.TotalAmount, v.PaymentAmount, v.IsInventory, v.IsInvoice, v.InvoiceNumber, v.InvoiceDate, v.EserialPerform ";
            sql += "from FIN.Voucher v ";
            sql += "join FIN.VType vType on vType.VTypeID = v.VTypeID ";
            sql += "left join FIN.VSubType vSubType on vSubType.VSubTypeID = v.VSubTypeID ";
            sql += "where vType.FuncID=@FuncID ";
            sql += "and format(v.VDate,'MM/dd/yyyy')>=format(@StartDate,'MM/dd/yyyy') and format(v.VDate,'MM/dd/yyyy')<=format(@EndDate,'MM/dd/yyyy') ";
            sql += "and (v.DivisionID=@DivisionID or coalesce(@DivisionID,'')='') ";
            sql += "and (v.VTypeID=@VTypeID or coalesce(@VTypeID,'')='') ";
            sql += "and (v.VNumber LIKE CONCAT('%',@VNumber,'%') or coalesce(@VNumber,'')='') ";
            sql += "order by v.VDate desc, v.VTimeActive desc ";
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
            var sql = "select SeqVD, vd.VDDesc, i.ICode, i.IName, i.IPrice, i.VendorDefault, i.StockDefault, vd.VDQty, iu.IUnitName, vd.VDPrice, vd.VDAmount, vd.VDDiscountPercent, vd.VDDiscountAmount,";
            sql += "vd.ToStockCode, toStock.StockName as ToStockName, vd.FromStockCode, fromStock.StockName as FromStockName, vd.VDNote, ";
            sql += "vd.InventoryCheck_StockCode, inventorycheckStock.StockName as InventoryCheck_StockName, vd.InventoryCheck_Qty, vd.InventoryCheck_ActualQty, vat.VATCode, coalesce(vat.VATRate,0) as VATRate, vat.VATName, vd.VATAmount, vd.TaxAccount ";
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
            var sql = "select top 5 i.ICode, i.IName, iu.IUnitName, case when @VtypeID='FIN_Purchasing' then i.ICost else case when @VtypeID='FIN_Sale' then i.IPrice else 0 end end as VDPrice, ";
            sql += "vat.VATCode, vat.VATName, s.StockCode as ToStockCode, s.StockName as ToStockName, ";
            sql += "s.StockCode as FromStockCode, s.StockName as FromStockName, s.StockCode as InventoryCheck_StockCode, s.StockName as InventoryCheck_StockName from FIN.Items i ";
            sql += "join FIN.ItemsType it on it.ITypeCode = i.ITypeCode ";
            sql += "join FIN.ItemsUnit iu on iu.IUnitCode = i.IUnitCode ";
            sql += "left join FIN.VATDef vat on vat.VATCode = i.VATDefault ";
            sql += "left join FIN.Stock s on s.StockCode = i.StockDefault ";
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
                    sqlVoucherVM += "Insert into FIN.Voucher (DivisionID,VNumber,VReference,VDesc,VDate,VendorCode,CustomerCode,StockCode,BankAccountID,VContact,ITypeCode,VActive,VTimeActive,IsPayment,PaymentTypeCode,TotalAmount,IsInventory,IsInvoice,InvoiceNumber,InvoiceDate,VTypeID,VSubTypeID,TimeCreated,EserialPerform) ";
                    sqlVoucherVM += "select @DivisionID,CODE_ID,@VReference,@VDesc,@VDate,@VendorCode,@CustomerCode,@StockCode,@BankAccountID,@VContact,@ITypeCode,@VActive,GETDATE(),@IsPayment,@PaymentTypeCode,@TotalAmount,@IsInventory,@IsInvoice,@InvoiceNumber,@InvoiceDate,@VTypeID,@VSubTypeID,GETDATE(),@EserialPerform from #tmpAuto_Code_ID ";
                    sqlVoucherVM += "select CODE_ID from #tmpAuto_Code_ID ";

                    VNumber = await conn.ExecuteScalarAsync<string>(sqlVoucherVM, _voucherVM);

                    //VoucherDetailVM
                    foreach (var _voucherDetailVM in _voucherDetailVMs)
                    {
                        sqlVoucherDetailVM = "Insert into FIN.VoucherDetail(VNumber,VDDesc,ICode,VDQty,VDPrice,VDAmount,VDDiscountPercent,VDDiscountAmount,VATCode,VATAmount,TaxAccount,FromStockCode,ToStockCode,VDNote,InventoryCheck_StockCode,InventoryCheck_Qty,InventoryCheck_ActualQty) ";
                        sqlVoucherDetailVM += "Values('" + VNumber + "',@VDDesc,@ICode,@VDQty,@VDPrice,@VDAmount,@VDDiscountPercent,@VDDiscountAmount,@VATCode,@VATAmount,@TaxAccount,@FromStockCode,@ToStockCode,@VDNote,@InventoryCheck_StockCode,@InventoryCheck_Qty,@InventoryCheck_ActualQty) ";
                        await conn.ExecuteAsync(sqlVoucherDetailVM, _voucherDetailVM);
                    }

                    if (_voucherVM.VTypeID == "FIN_Cash_Payment" || _voucherVM.VTypeID == "FIN_Cash_Receipt" || _voucherVM.VTypeID == "FIN_Deposit_Credit" || _voucherVM.VTypeID == "FIN_Deposit_Debit")
                    {
                        if (!String.IsNullOrEmpty(_voucherVM.VReference))
                        {
                            sqlVoucherVM = $"Update FIN.Voucher set PaymentAmount = coalesce(PaymentAmount,0) + {_voucherVM.TotalAmount} where VNumber=@VReference ";
                            await conn.ExecuteAsync(sqlVoucherVM, _voucherVM);
                        }
                    }
                }

                if (_voucherVM.IsTypeUpdate == 1)
                {
                    //VoucherVM
                    sqlVoucherVM = "Update FIN.Voucher set VDesc=@VDesc,VDate=@VDate,VendorCode=@VendorCode,CustomerCode=@CustomerCode,StockCode=@StockCode,BankAccountID=@BankAccountID,VContact=@VContact,ITypeCode=@ITypeCode,IsPayment=@IsPayment,PaymentTypeCode=@PaymentTypeCode,TotalAmount=@TotalAmount,PaymentAmount=@PaymentAmount, ";
                    sqlVoucherVM += "IsInventory=@IsInventory,IsInvoice=@IsInvoice,InvoiceNumber=@InvoiceNumber,InvoiceDate=@InvoiceDate,VSubTypeID=@VSubTypeID,EserialPerform=@EserialPerform ";
                    sqlVoucherVM += "where VNumber=@VNumber ";
                    sqlVoucherVM += "Delete from FIN.VoucherDetail where VNumber=@VNumber ";
                    await conn.ExecuteAsync(sqlVoucherVM, _voucherVM);

                    //VoucherDetailVM
                    foreach (var _voucherDetailVM in _voucherDetailVMs)
                    {
                        sqlVoucherDetailVM = "Insert into FIN.VoucherDetail(VNumber,VDDesc,ICode,VDQty,VDPrice,VDAmount,VDDiscountPercent,VDDiscountAmount,VATCode,VATAmount,TaxAccount,FromStockCode,ToStockCode,VDNote,InventoryCheck_StockCode,InventoryCheck_Qty,InventoryCheck_ActualQty) ";
                        sqlVoucherDetailVM += "Values('" + _voucherVM.VNumber + "',@VDDesc,@ICode,@VDQty,@VDPrice,@VDAmount,@VDDiscountPercent,@VDDiscountAmount,@VATCode,@VATAmount,@TaxAccount,@FromStockCode,@ToStockCode,@VDNote,@InventoryCheck_StockCode,@InventoryCheck_Qty,@InventoryCheck_ActualQty) ";

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
                    sqlVoucherVM = "Update FIN.Voucher set VActive=@VActive, VTimeActive=GETDATE() where VNumber=@VNumber ";

                    if (_voucherVM.VActive)
                    {
                        sqlVoucherVM += "Update FIN.Voucher set PaymentAmount = TotalAmount where VNumber=@VNumber and IsPayment=1 ";
                    }
                    else
                    {
                        sqlVoucherVM += "Update FIN.Voucher set PaymentAmount = 0 where VNumber=@VNumber ";

                        sqlVoucherVM += "delete FIN.VoucherDetail where VNumber in (select VNumber from FIN.Voucher where VReference=@VNumber) ";
                        sqlVoucherVM += "delete FIN.Voucher where VReference=@VNumber ";
                    }
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

        [HttpPost("GetInvoices")]
        public async Task<ActionResult<List<InvoiceVM>>> GetInvoices(FilterFinVM _filterFinVM)
        {
            var sql = "select vtype.VTypeDesc, v.InvoiceDate, v.InvoiceNumber, coalesce(vendor.VendorName,'') + coalesce(cus.CustomerName,'') as ObjectName, coalesce(vendor.VendorTaxCode,'') + coalesce(cus.CustomerTaxCode,'') as TaxCode, ";

            if (_filterFinVM.TypeView == 0)
            {
                sql += "sum(vd.VDAmount) as sumVDAmount, sum(vd.VDDiscountAmount) as sumVDDiscountAmount, sum(vd.VATAmount) as sumVATAmount, sum(vd.VDAmount-vd.VDDiscountAmount+vd.VATAmount) as sumTotalAmount ";
            }

            if (_filterFinVM.TypeView == 1)
            {
                sql += "i.Iname, vd.VDAmount, vd.VDDiscountAmount, vat.VATName, vd.VATAmount, vd.VDAmount - vd.VDDiscountAmount + vd.VATAmount as TotalAmount, vd.TaxAccount ";
            }

            sql += "from FIN.Voucher v join FIN.VoucherDetail vd on vd.VNumber = v.VNumber ";
            sql += "join FIN.Items i on i.ICode = vd.ICode ";
            sql += "join FIN.VATDef vat on vat.VATCode = vd.VATCode ";
            sql += "join FIN.VType vtype on vtype.VTypeID = v.VTypeID ";
            sql += "left join FIN.Vendor vendor on vendor.VendorCode = v.VendorCode ";
            sql += "left join CRM.Customer cus on cus.CustomerCode = v.CustomerCode ";
            sql += "where v.VActive=1 and coalesce(v.InvoiceNumber,0) <> 0 and coalesce(vd.VATCode,'') <> '' ";
            sql += "and (v.DivisionID=@DivisionID or coalesce(@DivisionID,'')='') ";
            sql += "and (v.VTypeID=@VTypeID or coalesce(@VTypeID,'')='') ";
            sql += "and format(v.InvoiceDate,'MM/dd/yyyy')>=format(@StartDate,'MM/dd/yyyy') and format(v.InvoiceDate,'MM/dd/yyyy')<=format(@EndDate,'MM/dd/yyyy') ";
            sql += "and (v.InvoiceNumber LIKE CONCAT('%',@InvoiceNumber,'%') or coalesce(@InvoiceNumber,'')='') ";
            if (_filterFinVM.TypeView == 0)
            {
                sql += "group by vtype.VTypeDesc, v.InvoiceDate, v.InvoiceNumber, coalesce(vendor.VendorName,'') + coalesce(cus.CustomerName,''), coalesce(vendor.VendorTaxCode,'') + coalesce(cus.CustomerTaxCode,'') ";
            }
            sql += "order by v.InvoiceDate desc ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<InvoiceVM>(sql, _filterFinVM);
                return result.ToList();
            }
        }

        //Account
        [HttpGet("GetAccounts")]
        public async Task<ActionResult<IEnumerable<AccountVM>>> GetAccounts()
        {
            var sql = "select * from FIN.Account where isGroup=0 ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<AccountVM>(sql);
                return Ok(result);
            }
        }

        //RPT
        [HttpPost("GetMoneyBooks")]
        public async Task<ActionResult<List<VoucherDetailVM>>> GetMoneyBooks(FilterFinVM _filterFinVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@DivisionID", _filterFinVM.DivisionID);
                parm.Add("@StartDate", _filterFinVM.StartDate);
                parm.Add("@EndDate", _filterFinVM.EndDate);
                parm.Add("@MoneyType", _filterFinVM.FuncID);

                var result = await conn.QueryAsync<VoucherDetailVM>("FIN.GetMoneyBooks", parm, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

    }
}
