using Data.Infrastructure;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using D69soft.Shared.Models.ViewModels.FIN;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.OP;
using D69soft.Shared.Models.ViewModels.SYSTEM;

namespace D69soft.Server.Controllers.OP
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public RequestController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        [HttpGet("GetCarts/{_UserID}")]
        public async Task<ActionResult<IEnumerable<CartVM>>> GetCarts(string _UserID)
        {
            var sql = "select c.SeqCart, i.ICode, i.IName, iu.IUnitName, c.Qty, c.EserialAddToCart, c.Note from EA.Cart c ";
            sql += "join FIN.Items i on c.ICode = i.ICode ";
            sql += "left join FIN.ItemsUnit iu on iu.IUnitCode = i.IUnitCode where c.EserialAddToCart = @UserID ";
            sql += "order by c.TimeUpdate desc ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<CartVM>(sql, new { UserID = _UserID });
                return Ok(result);
            }
        }

        [HttpPost("UpdateItemsCart/{_UserID}")]
        public async Task<ActionResult<bool>> UpdateItemsCart(ItemsVM _itemsVM, string _UserID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@ICode", _itemsVM.ICode);
                parm.Add("@UserID", _UserID);

                await conn.ExecuteAsync("EA.Cart_UpdateItemsCart", parm, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        [HttpPost("DelItemsCart")]
        public async Task<ActionResult<bool>> DelItemsCart(CartVM _cartVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var sql = "delete from EA.Cart where SeqCart=@SeqCart ";
                await conn.ExecuteAsync(sql, _cartVM);

                return true;
            }
        }

        [HttpPost("UpdateQtyItemsCart")]
        public async Task<ActionResult<bool>> UpdateQtyItemsCart(CartVM _cartVM)
        {
            var sql = "Update EA.Cart set Qty=@Qty where SeqCart=@SeqCart ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _cartVM);
            }

            return true;
        }

        [HttpPost("UpdateNoteItemsCart")]
        public async Task<ActionResult<bool>> UpdateNoteItemsCart(CartVM _cartVM)
        {
            var sql = "Update EA.Cart set Note=@Note where SeqCart=@SeqCart ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _cartVM);
            }

            return true;
        }

        [HttpPost("SendRequest/{_UserID}")]
        public async Task<ActionResult<bool>> SendRequest(RequestVM _request, string _UserID)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@ReasonOfRequest", _request.ReasonOfRequest);
                parm.Add("@DeptRequest", _request.DeptRequest);
                parm.Add("@UserID", _UserID);

                await conn.ExecuteAsync("EA.Cart_sendRequest", parm, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        [HttpPost("GetRequests")]
        public async Task<ActionResult<List<RequestVM>>> GetRequests(FilterVM _filterVM)
        {
            var sql = "select * from EA.RequestDetail rdtl ";

            sql += "join (select ";
            if (_filterVM.ShowEntity != 0)
            {
                sql += "top " + _filterVM.ShowEntity + " ";
            }
            sql += "* from EA.Request where (DeptRequest=@DepartmentID or coalesce(@DepartmentID,'') = '') and format(DateOfRequest,'MM/dd/yyyy')>=format(@StartDate,'MM/dd/yyyy') and format(DateOfRequest,'MM/dd/yyyy')<=format(@EndDate,'MM/dd/yyyy') ";
            if (_filterVM.RequestStatus == "pending")
            {
                sql += "and coalesce(isSendApprove,0) = 0 ";
            }
            if (_filterVM.RequestStatus == "approved")
            {
                sql += "and coalesce(isSendApprove,0) = 1 ";
            }
            if (_filterVM.RequestStatus == "nothandover")
            {
                sql += "and coalesce(isSendApprove,0) = 1 ";
            }
            if (!_filterVM.isHandover)
            {
                sql += "and (EserialRequest = @UserID or DirectManager_Eserial = @UserID or ControlDept_Eserial = @UserID or Approve_Eserial = @UserID) ";
            }
            if (_filterVM.ShowEntity != 0)
            {
                sql += "order by DateOfRequest desc ";
            }

            sql += ") r on r.RequestCode = rdtl.RequestCode ";


            sql += "join FIN.Items i on i.ICode = rdtl.ICode ";
            sql += "join FIN.ItemsUnit iu on iu.IUnitCode = i.IUnitCode ";

            sql += "left join (select VReference, VActive from FIN.Voucher) v on v.VReference = r.RequestCode ";

            sql += "join (select Eserial, LastName + ' ' + MiddleName + ' ' + FirstName as Request_FullName from HR.Profile) pRequest on pRequest.Eserial = r.EserialRequest ";
            sql += "left join (select DivisionID, DepartmentID, DepartmentName as Request_DepartmentName from HR.Department) de on de.DepartmentID = r.DeptRequest ";

            sql += "left join (select Eserial, LastName + ' ' + MiddleName + ' ' + FirstName as DirectManager_FullName from HR.Profile) pDirectManager on pDirectManager.Eserial = r.DirectManager_Eserial ";
            sql += "left join (select Eserial, LastName + ' ' + MiddleName + ' ' + FirstName as ControlDept_FullName from HR.Profile) pControlDept on pControlDept.Eserial = r.ControlDept_Eserial ";
            sql += "left join (select Eserial, LastName + ' ' + MiddleName + ' ' + FirstName as Approve_FullName from HR.Profile) pApprove on pApprove.Eserial = r.Approve_Eserial ";

            if (_filterVM.RequestStatus == "nothandover")
            {
                sql += "where sv.VActive = 0 ";
            }

            sql += "order by r.DateOfRequest desc ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<RequestVM>(sql, _filterVM);
                return result.ToList();
            }
        }

        [HttpPost("SendApprove/{_type}")]
        public async Task<ActionResult<bool>> SendApprove(RequestVM _requestVM, string _type)
        {
            var sql = "";

            if (_type == "SendDirectManager")
            {
                sql = "Update EA.Request set isSendDirectManager=@isSendDirectManager, TimeSendDirectManager=@TimeSendDirectManager where RequestCode=@RequestCode ";
            }

            if (_type == "SendControlDept")
            {
                sql = "Update EA.Request set isSendControlDept=@isSendControlDept, TimeSendControlDept=@TimeSendControlDept where RequestCode=@RequestCode ";
            }

            if (_type == "SendApprove")
            {
                sql = "Update EA.Request set isSendApprove=@isSendApprove, TimeSendApprove=@TimeSendApprove where RequestCode=@RequestCode ";
                sql += "Update EA.RequestDetail set QtyHandover = QtyApproved where RequestCode=@RequestCode ";

                if (!_requestVM.isSendApprove)
                {
                    sql += "delete FIN.StockVoucherDetail where VNumber in (select VNumber from FIN.StockVoucher where Reference_VNumber=@RequestCode and coalesce(VActive,0)=0) ";
                    sql += "delete from FIN.StockVoucher where Reference_VNumber=@RequestCode and coalesce(VActive,0)=0 ";
                }

            }

            if (_type == "SendDelRequest")
            {
                sql = "delete from EA.Request where RequestCode=@RequestCode ";
                sql += "delete from EA.RequestDetail where RequestCode=@RequestCode ";
            }

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _requestVM);
            }

            return true;
        }

        [HttpPost("UpdateQtyApproved")]
        public async Task<ActionResult<bool>> UpdateQtyApproved(RequestVM _requestVM)
        {
            var sql = "Update EA.RequestDetail set QtyApproved=@QtyApproved where SeqRequestDetail=@SeqRequestDetail ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _requestVM);
            }

            return true;
        }

        [HttpPost("UpdateRDNote")]
        public async Task<ActionResult<bool>> UpdateRDNote(RequestVM _requestVM)
        {
            var sql = "Update EA.RequestDetail set RDNote=@RDNote where SeqRequestDetail=@SeqRequestDetail ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _requestVM);
            }

            return true;
        }

        //Get RequestDetail to Stock
        [HttpGet("GetRequestDetailToStockVoucherDetail/{_RequestCode}")]
        public async Task<ActionResult<List<VoucherDetailVM>>> GetRequestDetailToStockVoucherDetail(string _RequestCode)
        {
            var sql = "select rd.ICode, QtyApproved as Qty, INote + case when coalesce(RDNote,'') <> '' then ', ' + RDNote else '' end as VDNote, i.StockDefault as FromStockCode, 1 as IsReference from EA.RequestDetail rd ";
            sql += "join FIN.Items i on i.ICode = rd.ICode ";
            sql += "where RequestCode=@RequestCode and QtyApproved>0 ";

            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<VoucherDetailVM>(sql, new { RequestCode = _RequestCode });
                return result.ToList();
            }
        }
    }
}
