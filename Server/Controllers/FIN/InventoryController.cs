using Data.Infrastructure;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Newtonsoft.Json;
using System.Collections;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Utilities;

namespace D69soft.Server.Controllers.FIN
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;
        private readonly IWebHostEnvironment _env;

        public InventoryController(SqlConnectionConfig connConfig, IWebHostEnvironment env)
        {
            _connConfig = connConfig;
            _env = env;
        }

        [HttpGet("GetItemsTypes")]
        public async Task<ActionResult<IEnumerable<ItemsTypeVM>>> GetItemsTypes()
        {
            var sql = "select * from FIN.ItemsType order by ITypeName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ItemsTypeVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("GetItemsClassList")]
        public async Task<ActionResult<IEnumerable<ItemsClassVM>>> GetItemsClassList()
        {
            var sql = "select * from FIN.ItemsClass order by IClsName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ItemsClassVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("GetItemsGroupList")]
        public async Task<ActionResult<IEnumerable<ItemsGroupVM>>> GetItemsGroupList()
        {
            var sql = "select * from FIN.ItemsGroup order by IGrpName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ItemsGroupVM>(sql);
                return Ok(result);
            }
        }

        [HttpPost("UpdateItemsGroup")]
        public async Task<ActionResult<int>> UpdateItemsGroup(ItemsGroupVM _itemsGroupVM)
        {
            var sql = "";
            if (_itemsGroupVM.IsTypeUpdate == 0)
            {
                sql = "Insert into FIN.ItemsGroup (IGrpCode,IGrpName,IClsCode,IGrpActive) Values (@IGrpCode,@IGrpName,@IClsCode,1) ";
            }
            if (_itemsGroupVM.IsTypeUpdate == 1)
            {
                sql = "Update FIN.ItemsGroup set IGrpName = @IGrpName, IClsCode = @IClsCode where IGrpCode = @IGrpCode ";
            }
            if (_itemsGroupVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from FIN.Items where IGrpCode=@IGrpCode) ";
                sql += "begin ";
                sql += "delete from FIN.ItemsGroup where IGrpCode=@IGrpCode ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteAsync(sql, _itemsGroupVM); ;
            }
        }

        [HttpGet("ContainsIGrpCode/{id}")]
        public async Task<ActionResult<bool>> ContainsIGrpCode(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM FIN.ItemsGroup where IGrpCode = @IGrpCode) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { IGrpCode = id });
            }
        }

        [HttpGet("GetItemsUnitList")]
        public async Task<ActionResult<IEnumerable<ItemsUnitVM>>> GetItemsUnitList()
        {
            var sql = "select * from FIN.ItemsUnit order by IUnitName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ItemsUnitVM>(sql);
                return Ok(result);
            }
        }

        [HttpPost("UpdateItemsUnit")]
        public async Task<ActionResult<int>> UpdateItemsUnit(ItemsUnitVM _itemsUnitVM)
        {
            var sql = "";
            if (_itemsUnitVM.IsTypeUpdate == 0)
            {
                sql = "Insert into FIN.ItemsUnit (IUnitCode,IUnitName) Values (@IUnitCode,@IUnitName) ";
            }
            if (_itemsUnitVM.IsTypeUpdate == 1)
            {
                sql = "Update FIN.ItemsUnit set IUnitName = @IUnitName where IUnitCode = @IUnitCode ";
            }
            if (_itemsUnitVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from FIN.Items where IUnitCode=@IUnitCode) ";
                sql += "begin ";
                sql += "delete from FIN.ItemsUnit where IUnitCode=@IUnitCode ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteAsync(sql, _itemsUnitVM);
            }
        }

        [HttpGet("ContainsIUnitCode/{id}")]
        public async Task<ActionResult<bool>> ContainsIUnitCode(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM FIN.ItemsUnit where IUnitCode = @IUnitCode) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { IUnitCode = id });
            }
        }

        [HttpPost("GetItemsList")]
        public async Task<ActionResult<List<ItemsVM>>> GetItemsList(FilterFinVM _filterFinVM)
        {
            var sql = "select * from FIN.Items i ";
            sql += "join FIN.ItemsUnit iu on iu.IUnitCode = i.IUnitCode ";
            sql += "join FIN.ItemsType it on it.ITypeCode = i.ITypeCode ";
            sql += "left join FIN.ItemsClass ic on ic.IClsCode = i.IClsCode ";
            sql += "left join FIN.ItemsGroup ig on ig.IGrpCode = i.IGrpCode ";
            sql += "where IActive=@IActive ";
            sql += "and (i.IClsCode=@IClsCode or coalesce(@IClsCode,'') = '') ";
            sql += "and (i.IGrpCode=@IGrpCode or coalesce(@IGrpCode,'') = '') ";
            sql += "and (i.ICode LIKE CONCAT('%',@searchText,'%') or i.IName LIKE CONCAT('%',@searchText,'%')) ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ItemsVM>(sql, _filterFinVM);
                return result.ToList();
            }
        }

        [HttpGet("GetItemsForQuantitative/{_ItemSearch}/{_ICode}")]
        public async Task<ActionResult<IEnumerable<ItemsVM>>> GetItemsForQuantitative(string _ItemSearch, string _ICode)
        {
            var sql = "select * from FIN.Items i ";
            sql += "join FIN.ItemsType it on it.ITypeCode = i.ITypeCode ";
            sql += "where i.IActive=1 and it.IsInventory=1 and i.ICode <> @ICode ";
            sql += "and (i.ICode LIKE CONCAT('%',@ItemSearch,'%') or i.IName LIKE CONCAT('%',@ItemSearch,'%')) ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<ItemsVM>(sql, new { ItemSearch = _ItemSearch, ICode = _ICode });
                return Ok(result);
            }
        }

        [HttpGet("GetQuantitativeItems/{_ICode}")]
        public async Task<ActionResult<List<QuantitativeItemsVM>>> GetQuantitativeItems(string _ICode)
        {
            var sql = "select qi.QI_ICode, i.IName as QI_IName, iu.IUnitName as QI_IUnitName, qi.QI_UnitRatio from FIN.QuantitativeItems qi ";
            sql += "join FIN.Items i on i.ICode = qi.QI_ICode join FIN.ItemsUnit iu on iu.IUnitCode = i.IUnitCode ";
            sql += "where qi.ICode=@ICode ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<QuantitativeItemsVM>(sql, new { ICode = _ICode });
                return result.ToList();
            }
        }

        [HttpPost("UpdateItems")]
        public async Task<ActionResult<string>> UpdateItems(ArrayList _arrayList)
        {
            ItemsVM _itemsVM = JsonConvert.DeserializeObject<ItemsVM>(_arrayList[0].ToString());
            IEnumerable<QuantitativeItemsVM> _quantitativeItemsVMs = JsonConvert.DeserializeObject<IEnumerable<QuantitativeItemsVM>>(_arrayList[1].ToString());

            var sql = String.Empty;
            if (_itemsVM.IsTypeUpdate == 0)
            {
                if (String.IsNullOrEmpty(_itemsVM.ICode))
                {
                    sql = "Create table #tmpAuto_Code_ID (Code_ID varchar(50)) ";
                    sql += "Insert #tmpAuto_Code_ID ";
                    sql += "exec SYSTEM.AUTO_CODE_ID 'FIN.Items','ICode','VT','0000' ";
                    sql += "Insert into FIN.Items (ICode,IName,IUnitCode,ITypeCode,IClsCode,IGrpCode,StockDefault,VendorDefault,IActive) ";
                    sql += "select Code_ID, @IName,@IUnitCode,@ITypeCode,@IClsCode,@IGrpCode,@StockDefault,@VendorDefault,@IActive from #tmpAuto_Code_ID ";

                    sql += "select Code_ID from #tmpAuto_Code_ID";
                }
                else
                {
                    sql = "Insert into FIN.Items (ICode,IName,IUnitCode,ITypeCode,IClsCode,IGrpCode,StockDefault,VendorDefault,IActive) ";
                    sql += "Values (@ICode,@IName,@IUnitCode,@ITypeCode,@IClsCode,@IGrpCode,@StockDefault,@VendorDefault,@IActive) ";

                    sql += "select @ICode";
                }
            }
            if (_itemsVM.IsTypeUpdate == 1)
            {
                sql = "Update FIN.Items set IName = @IName, IUnitCode = @IUnitCode,ITypeCode=@ITypeCode, IClsCode = @IClsCode, IGrpCode = @IGrpCode, ";
                sql += "StockDefault = @StockDefault, VendorDefault = @VendorDefault, IActive = @IActive ";
                sql += "where ICode = @ICode ";

                sql += "Delete from FIN.QuantitativeItems where ICode=@ICode ";
                foreach (var _quantitativeItemsVM in _quantitativeItemsVMs)
                {
                    sql += "Insert into FIN.QuantitativeItems(ICode,QI_ICode,QI_UnitRatio) ";
                    sql += $"Values(@ICode,{_quantitativeItemsVM.QI_ICode},{_quantitativeItemsVM.QI_UnitRatio}) ";
                }

                sql += "select @ICode";
            }
            if (_itemsVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from FIN.VoucherDetail where ICode=@ICode) ";
                sql += "begin ";
                sql += "delete from FIN.Items where ICode=@ICode ";
                sql += "end ";
                sql += "else ";
                sql += "begin ";
                sql += "select 'Err_NotDel' ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                _itemsVM.ICode = await conn.ExecuteScalarAsync<string>(sql, _itemsVM);

                //Update ImageItems
                if (_itemsVM.IsDelFileUpload)
                {
                    LibraryFunc.DelFileFrom(Path.Combine(_env.ContentRootPath, $"{UrlDirectory.Upload_FIN_Items_URL_Private}{_itemsVM.ICode}.png"));
                    _itemsVM.IURLPicture1 = UrlDirectory.Default_Items;
                }

                if (_itemsVM.FileContent != null)
                {
                    var filename = $"{_itemsVM.ICode}.png";
                    var path = Path.Combine(_env.ContentRootPath, $"{UrlDirectory.Upload_FIN_Items_URL_Private}{filename}");
                    var fs = System.IO.File.Create(path);
                    fs.Write(_itemsVM.FileContent, 0, _itemsVM.FileContent.Length);
                    fs.Close();

                    _itemsVM.IURLPicture1 = $"{UrlDirectory.Upload_FIN_Items_URL_Public}{filename}";
                }

                var sqlImageItems = "Update FIN.Items set IURLPicture1=@IURLPicture1 where ICode=@ICode";
                await conn.ExecuteAsync(sqlImageItems, _itemsVM);

                return _itemsVM.ICode;
            }
        }

        [HttpGet("ContainsICode/{id}")]
        public async Task<ActionResult<bool>> ContainsICode(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM FIN.Items where ICode = @ICode) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { ICode = id });
            }
        }

        [HttpGet("GetStockList")]
        public async Task<ActionResult<IEnumerable<StockVM>>> GetStockList()
        {
            var sql = "select * from FIN.Stock order by StockName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<StockVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("ContainsStockCode/{id}")]
        public async Task<ActionResult<bool>> ContainsStockCode(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM FIN.Stock where StockCode = @StockCode) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { StockCode = id });
            }
        }

        [HttpPost("UpdateStock")]
        public async Task<ActionResult<int>> UpdateStock(StockVM _stockVM)
        {
            var sql = "";
            if (_stockVM.IsTypeUpdate == 0)
            {
                sql = "Insert into FIN.Stock (StockCode,StockName,StockAddress,DivisionID,StockActive) Values (@StockCode,@StockName,@StockAddress,@DivisionID,@StockActive)";
            }
            if (_stockVM.IsTypeUpdate == 1)
            {
                sql = "Update FIN.Stock set StockName = @StockName, StockAddress = @StockAddress, StockActive = @StockActive where StockCode = @StockCode";
            }
            if (_stockVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from FIN.VoucherDetail where FromStockCode=@StockCode or ToStockCode=@StockCode) ";
                sql += "begin ";
                sql += "delete from FIN.Stock where StockCode = @StockCode ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteAsync(sql, _stockVM);
            }
        }

        [HttpPost("GetInventorys")]
        public async Task<ActionResult<List<InventoryVM>>> GetInventorys(FilterFinVM _filterFinVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@DivisionID", _filterFinVM.DivisionID);
                parm.Add("@StartDate", _filterFinVM.StartDate);
                parm.Add("@EndDate", _filterFinVM.EndDate);
                parm.Add("@StockCode", _filterFinVM.StockCode);
                parm.Add("@ICode", _filterFinVM.ICode);

                var result = await conn.QueryAsync<InventoryVM>("FIN.FIN_Inventory_view", parm, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        [HttpPost("GetInventoryBookDetails")]
        public async Task<ActionResult<List<InventoryBookDetailVM>>> GetInventoryBookDetails(ArrayList _arrayList)
        {
            FilterFinVM _filterFinVM = JsonConvert.DeserializeObject<FilterFinVM>(_arrayList[0].ToString());
            InventoryVM _inventoryVM = JsonConvert.DeserializeObject<InventoryVM>(_arrayList[1].ToString());

            var sql = "select v.VDate, v.VNumber, v.VDesc, ";
            sql += "case when vd.ToStockCode = @StockCode then vd.VDQty else 0 end as QtyInput, ";
            sql += "case when vd.FromStockCode = @StockCode then vd.VDQty else 0 end as QtyOutput ";
            sql += "from (select * from FIN.Voucher where VActive=1) v ";
            sql += "join FIN.VoucherDetail vd on vd.VNumber = v.VNumber ";
            sql += "where format(v.VDate,'MM/dd/yyyy')>='" + _filterFinVM.StartDate.ToString("MM/dd/yyyy") + "' and format(v.VDate,'MM/dd/yyyy')<='" + _filterFinVM.EndDate.ToString("MM/dd/yyyy") + "' and (vd.ToStockCode = @StockCode or vd.FromStockCode = @StockCode) and vd.ICode=@ICode ";
            sql += "order by v.VDate ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<InventoryBookDetailVM>(sql, _inventoryVM);
                return result.ToList();
            }
        }

        [HttpPost("GetInventoryCheck_Qty/{_VDate}")]
        public async Task<ActionResult<float>> GetInventoryCheck_Qty(DateTimeOffset _VDate, VoucherDetailVM _voucherDetailVM)
        {
            var sql = String.Empty;
            sql += "select sum(case when coalesce(ToStockCode,'') <> '' then VDQty else 0 end) - sum(case when coalesce(FromStockCode,'') <> '' then VDQty else 0 end) as InventoryCheck_Qty ";
            sql += "from FIN.VoucherDetail vd ";
            sql += "join (select * from FIN.Voucher where VActive=1) v on v.VNumber = vd.VNumber ";
            sql += "where v.VDate <= CAST('"+_VDate.ToString("MM/dd/yyyy")+ "' as datetime) and (ToStockCode = @InventoryCheck_StockCode or FromStockCode = @InventoryCheck_StockCode) and ICode=@ICode ";
            sql += "group by ICode ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.ExecuteScalarAsync<float>(sql, _voucherDetailVM);
                return result;
            }
        }

    }
}
