using Data.Infrastructure;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.POS;
using D69soft.Shared.Models.ViewModels.FIN;

namespace D69soft.Server.Controllers.POS
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashierController :ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public CashierController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        [HttpGet("GetPointOfSale")]
        public async Task<ActionResult<IEnumerable<PointOfSaleVM>>> GetPointOfSale()
        {
            var sql = "select * from POS.PointOfSale pos join FIN.Stock s on s.StockCode = pos.StockCode order by POSName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<PointOfSaleVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("GetRoomTableArea/{_POSCode}")]
        public async Task<ActionResult<IEnumerable<RoomTableAreaVM>>> GetRoomTableArea(string _POSCode)
        {
            var sql = "select * from POS.RoomTableArea ";
            sql += "where POSCode=@POSCode order by RoomTableAreaName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<RoomTableAreaVM>(sql, new { POSCode = _POSCode });
                return Ok(result);
            }
        }

        [HttpPost("GetRoomTable")]
        public async Task<ActionResult<IEnumerable<RoomTableVM>>> GetRoomTable(FilterPosVM _filterPosVM)
        {
            var sql = "select distinct rt.RoomTableID, RoomTableName, coalesce(isOpen,0) as isOpen, pOpenBy.FirstName as OpenByName, OpenBy, OpenTime from POS.RoomTable rt  ";
            sql += "join POS.RoomTableArea rta on rta.RoomTableAreaID = rt.RoomTableAreaID  ";
            sql += "left join ( ";
            sql += "select POSCode, RoomTableID, OpenBy, MAX(OpenTime) as OpenTime, isOpen from POS.Invoice ";
            sql += "where isOpen=1 and POSCode=@POSCode group by POSCode, RoomTableID,OpenBy, isOpen ";
            sql += "having  MAX(CheckNo)>0 ";
            sql += ") inv on inv.RoomTableID = rt.RoomTableID ";
            sql += "left join HR.Profile pOpenBy on pOpenBy.Eserial = inv.OpenBy ";
            sql += "where rta.POSCode=@POSCode ";
            sql += " and (rt.RoomTableAreaID=@RoomTableAreaID or coalesce(@RoomTableAreaID,'')='') ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<RoomTableVM>(sql, _filterPosVM);
                return Ok(result);
            }
        }

        [HttpPost("GetItems")]
        public async Task<ActionResult<List<ItemsVM>>> GetItems(FilterPosVM _filterPosVM)
        {
            var sql = "select * from FIN.Items i ";
            sql += "join FIN.ItemsClass ic on ic.IClsCode = i.IClsCode ";
            sql += "join FIN.ItemsGroup ig on ig.IGrpCode = i.IGrpCode ";
            sql += "where IActive=1 and IsSale=1 ";
            sql += "and (i.IGrpCode=@IGrpCode or coalesce(@IGrpCode,'') = '') ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ItemsVM>(sql, _filterPosVM);
                return result.ToList();
            }
        }

        [HttpPost("OpenRoomTable/{_invoiceVM}")]
        public async Task<ActionResult<bool>> OpenRoomTable(FilterPosVM _filterPosVM, InvoiceVM _invoiceVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                if (_invoiceVM.IsClickChangeRoomTable != 1)
                {
                    DynamicParameters parm = new DynamicParameters();
                    parm.Add("@roomtableid", _filterPosVM.RoomTableID);
                    parm.Add("@isOpen", _filterPosVM.isOpen);
                    parm.Add("@poscode", _filterPosVM.POSCode);
                    parm.Add("@userid", _filterPosVM.UserID);

                    await conn.ExecuteAsync("POS.Cashier_openRoomTable", parm, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    var sql = "Update POS.Invoice set RoomTableID=@RoomTableID, IsChangeRoomTable = 1, TimeChangeRoomTable = GETDATE(), ChangeRoomTableBy=@ChangeRoomTableBy where CheckNo=@CheckNo ";
                    await conn.ExecuteAsync(sql, new { CheckNo = _invoiceVM.CheckNo, RoomTableID = _filterPosVM.RoomTableID, ChangeRoomTableBy = _filterPosVM.UserID });
                }

                return true;
            }
        }

        [HttpPost("OpenTakeOut")]
        public async Task<ActionResult<bool>> OpenTakeOut(FilterPosVM _filterPosVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@poscode", _filterPosVM.POSCode);
                parm.Add("@userid", _filterPosVM.UserID);

                await conn.ExecuteAsync("POS.Cashier_openTakeOut", parm, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        [HttpPost("ChooseItems")]
        public async Task<ActionResult<bool>> ChooseItems(FilterPosVM _filterPosVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@icode", _filterPosVM.ICode);
                parm.Add("@checkno", _filterPosVM.CheckNo);
                parm.Add("@userid", _filterPosVM.UserID);

                await conn.ExecuteAsync("POS.Cashier_chooseICode", parm, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        [HttpPost("UpdateInvoiceCustomer")]
        public async Task<ActionResult<bool>> UpdateInvoiceCustomer(InvoiceVM _invoiceVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "Update POS.Invoice set CustomerID=@CustomerID ";
                sql += "where CheckNo=@CheckNo";

                await conn.ExecuteAsync(sql, new
                {
                    CheckNo = _invoiceVM.CheckNo,
                    CustomerID = _invoiceVM.CustomerID,
                });

                return true;
            }
        }

        [HttpPost("UpdateInvoiceDetail")]
        public async Task<ActionResult<bool>> UpdateInvoiceDetail(InvoiceVM _invoiceDetail)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "Update POS.InvoiceDetail set Qty=@Qty, ";
                sql += "Price=@Price, ";
                sql += "INote=@INote, ";
                sql += "Items_DiscountPrice=@Items_DiscountPrice, ";
                sql += "Items_DiscountPercent=@Items_DiscountPercent ";
                sql += "where CheckNo=@CheckNo and Seq=@Seq ";

                sql += "Update POS.Invoice set Invoice_DiscountPrice=0, ";
                sql += "Invoice_DiscountPercent=0 ";
                sql += "where CheckNo=@CheckNo";

                await conn.ExecuteAsync(sql, new
                {
                    Seq = _invoiceDetail.Seq,
                    CheckNo = _invoiceDetail.CheckNo,
                    Qty = _invoiceDetail.Qty,
                    Price = _invoiceDetail.Price,                   
                    INote = _invoiceDetail.INote,
                    Items_DiscountPrice = _invoiceDetail.Items_DiscountPrice,
                    Items_DiscountPercent = _invoiceDetail.Items_DiscountPercent
                });

                return true;
            }
        }

        [HttpPost("UpdateInvoiceDiscount")]
        public async Task<ActionResult<bool>> UpdateInvoiceDiscount(InvoiceVM _invoiceDetail)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "Update POS.Invoice set Invoice_DiscountPrice=@Invoice_DiscountPrice, ";
                sql += "Invoice_DiscountPercent=@Invoice_DiscountPercent ";
                sql += "where CheckNo=@CheckNo";

                await conn.ExecuteAsync(sql, new
                {
                    CheckNo = _invoiceDetail.CheckNo,
                    Invoice_DiscountPrice = _invoiceDetail.Invoice_DiscountPrice,
                    Invoice_DiscountPercent = _invoiceDetail.Invoice_DiscountPercent
                });

                return true;
            }
        }

        [HttpPost("GetInfoInvoice")]
        public async Task<ActionResult<InvoiceVM>> GetInfoInvoice(FilterPosVM _filterPosVM)
        {
            var sql = "";
            if (_filterPosVM.RoomTableID == "TakeOut")
            {
                sql += "select inv.RoomTableID, N'Mang về' as RoomTableName, '' as RoomTableAreaName, pos.POSName, inv.CheckNo, inv.CustomerID from POS.Invoice inv ";
                sql += "join POS.PointOfSale pos on pos.POSCode = inv.POSCode ";
                sql += "where inv.isClose = 0 and inv.RoomTableID = 'TakeOut' and inv.POSCode=@POSCode and OpenBy=@UserID ";
                using (var conn = new SqlConnection(_connConfig.Value))
                {
                    var result = await conn.QueryFirstAsync<InvoiceVM>(sql, _filterPosVM);
                    return result;
                }
            }
            else
            {
                sql += "select rt.RoomTableID, rt.RoomTableName, rta.RoomTableAreaName, pos.POSName, inv.CheckNo, inv.CustomerID from POS.Invoice inv ";
                sql += "join POS.RoomTable rt on rt.RoomTableID = inv.RoomTableID ";
                sql += "join POS.RoomTableArea rta on rta.RoomTableAreaID = rt.RoomTableAreaID ";
                sql += "join POS.PointOfSale pos on pos.POSCode = rta.POSCode ";
                sql += "where inv.isClose = 0 and inv.RoomTableID = @RoomTableID ";
                using (var conn = new SqlConnection(_connConfig.Value))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var result = await conn.QueryFirstAsync<InvoiceVM>(sql, _filterPosVM);
                    return result;
                }
            }

        }

        [HttpGet("GetInvoiceItems/{_CheckNo}")]
        public async Task<ActionResult<IEnumerable<InvoiceVM>>> GetInvoiceItems(string _CheckNo)
        {
            var sql = "select invd.Seq, invd.CheckNo,i.ICode, i.IName, invd.Price, invd.Qty, (invd.Price - Items_DiscountPrice) * invd.Qty as IAmount, INote, Items_DiscountPrice, Items_DiscountPercent from POS.InvoiceDetail invd ";
            sql += "join FIN.Items i on i.ICode = invd.ICode where invd.CheckNo=@CheckNo order by invd.Seq desc ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<InvoiceVM>(sql, new { CheckNo = _CheckNo });
                return Ok(result);
            }
        }

        [HttpGet("GetInvoiceTotal/{_CheckNo}")]
        public async Task<ActionResult<InvoiceVM>> GetInvoiceTotal(string _CheckNo)
        {
            var sql = "select inv.CheckNo, sumQty, sumAmount, Invoice_DiscountPrice, Invoice_DiscountPercent, Invoice_TaxPercent, ";
            sql += "sumAmount - Invoice_DiscountPrice + sumAmount*Invoice_TaxPercent/100 as sumAmountPay from POS.Invoice inv ";
            sql += "join ( ";
            sql += "select CheckNo ,sum(Qty) as sumQty, sum((Price - Items_DiscountPrice) * Qty) as sumAmount from POS.InvoiceDetail where CheckNo=@CheckNo group by CheckNo ";
            sql += ") invd on invd.CheckNo = inv.CheckNo ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryFirstOrDefaultAsync<InvoiceVM>(sql, new { CheckNo = _CheckNo });
                return result;
            }
        }

        [HttpGet("DelInvoiceItems/{_CheckNo}/{_Seq}")]
        public async Task<ActionResult<bool>> DelInvoiceItems(string _CheckNo, int _Seq)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "delete from POS.InvoiceDetail where CheckNo=@CheckNo ";
                if (_Seq != 0)
                {
                    sql += "and Seq=@Seq ";
                }
                await conn.ExecuteAsync(sql, new { CheckNo = _CheckNo, Seq = _Seq });

                return true;
            }
        }

        [HttpGet("DelInvoice/{_CheckNo}")]
        public async Task<ActionResult<bool>> DelInvoice(string _CheckNo)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "delete from POS.Invoice where CheckNo=@CheckNo ";
                sql += "delete from POS.InvoiceDetail where CheckNo=@CheckNo ";
                sql += "delete from POS.Payment where CheckNo=@CheckNo ";
                sql += "select * from POS.OrderLog where CheckNo=@CheckNo ";
                await conn.ExecuteAsync(sql, new { CheckNo = _CheckNo });

                return true;
            }
        }

        [HttpGet("GetPaymentModeList")]
        public async Task<ActionResult<IEnumerable<PaymentModeVM>>> GetPaymentModeList()
        {
            var sql = "select PModeCode, PDesc from POS.PaymentMode order by PNo ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<PaymentModeVM>(sql);
                return Ok(result);
            }
        }

        [HttpPost("SavePayment/{_POSCode}/{_UserID}")]
        public async Task<ActionResult<bool>> SavePayment(PaymentVM _paymentVM, string _POSCode, string _UserID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@checkno", _paymentVM.CheckNo);
                parm.Add("@pmode", _paymentVM.PModeCode);
                parm.Add("@pnote", _paymentVM.PNote);
                parm.Add("@poscode", _POSCode);
                parm.Add("@userid", _UserID);

                await conn.ExecuteAsync("POS.Cashier_payment", parm, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        [HttpGet("GetCustomerAmountSuggest/{_AmountPay}")]
        public async Task<ActionResult<IEnumerable<PaymentVM>>> GetCustomerAmountSuggest(decimal _AmountPay)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@AmountPay", _AmountPay);

                var result = await conn.QueryAsync<PaymentVM>("POS.Cashier_CustomerAmountSuggest", parm, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        //Invoice
        [HttpPost("GetInvoices")]
        public async Task<ActionResult<List<InvoiceVM>>> GetInvoices(FilterPosVM _filterPosVM)
        {
            var sql = "select *, sv.Reference_VNumber, sv.VActive as Reference_VActive from POS.Invoice inv ";
            sql += "join POS.PointOfSale pos on pos.POSCode = inv.POSCode ";
            sql += "left join POS.Payment p on p.CheckNo = inv.CheckNo ";
            sql += "left join FIN.StockVoucher sv on sv.Reference_VNumber = inv.CheckNo ";
            sql += "where (inv.POSCode=@POSCode or coalesce(@POSCode,'')='') ";
            sql += "and (inv.INVActive=@searchActive or @searchActive=2) ";
            sql += "and inv.IDate>=format(@StartDate,'MM/dd/yyyy') and inv.IDate<=format(@EndDate,'MM/dd/yyyy') ";
            sql += "order by pos.POSName, p.PDate desc ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<InvoiceVM>(sql, _filterPosVM);
                return result.ToList();
            }
        }

        [HttpPost("ActiveInvoice")]
        public async Task<ActionResult<bool>> ActiveInvoice(InvoiceVM _invoiceVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "Update POS.Invoice set INVActive=@INVActive where CheckNo=@CheckNo ";

                sql += "delete FIN.StockVoucherDetail where VNumber in (select VNumber from FIN.StockVoucher where Reference_VNumber=@CheckNo and coalesce(VActive,0)=0) ";
                sql += "delete from FIN.StockVoucher where Reference_VNumber=@CheckNo and coalesce(VActive,0)=0 ";

                await conn.ExecuteAsync(sql, _invoiceVM);

                return true;
            }
        }

        [HttpGet("QI_StockVoucherDetails/{_CheckNo}")]
        public async Task<ActionResult<List<StockVoucherDetailVM>>> QI_StockVoucherDetails(string _CheckNo)
        {
            var sql = "select invoice.ICode, SUM(invoice.Qty) as Qty, invoice.VDNote, invoice.FromStockCode, 1 as IsReference from FIN.Items i  ";
            sql += "join (select * from FIN.ItemsType where IsInventory=1) it on it.ITypeCode = i.ITypeCode ";
            sql += "join ( ";
            sql += "select case when coalesce(qi.QI_ICode,'') = '' then invd.ICode else qi.QI_ICode end as ICode,  ";
            sql += "case when coalesce(qi.QI_ICode,'') = '' then invd.Qty else invd.Qty*qi.QI_UnitRatio end as Qty, pos.StockCode as FromStockCode, ";
            sql += "case when coalesce(qi.QI_ICode,'') <> '' then i.ICode + ' - ' + i.IName + ' - ' + iu.IUnitName + ' (SL:' + convert(varchar(50),invd.Qty)+')' else '' end as VDNote ";
            sql += "from POS.InvoiceDetail invd  ";
            sql += "join POS.Invoice inv on inv.CheckNo = invd.CheckNo ";
            sql += "join FIN.Items i on i.ICode = invd.ICode ";
            sql += "join FIN.ItemsUnit iu on iu.IUnitCode = i.IUnitCode ";
            sql += "join POS.PointOfSale pos on pos.POSCode = inv.POSCode ";
            sql += "left join FIN.QuantitativeItems qi on qi.ICode = invd.ICode ";
            sql += "where inv.CheckNo=@CheckNo ";
            sql += ") invoice on invoice.ICode = i.ICode ";
            sql += "group by invoice.ICode, invoice.VDNote, invoice.FromStockCode ";

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<StockVoucherDetailVM>(sql, new { CheckNo = _CheckNo });
                return result.ToList();
            }
        }

        //SyncSmile
        [HttpGet("SyncDataSmile")]
        public async Task<ActionResult<bool>> SyncDataSmile()
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();

                await conn.ExecuteAsync("SYNC.SMILE_POS", parm, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

    }
}
