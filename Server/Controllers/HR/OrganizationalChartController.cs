using Data.Infrastructure;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using D69soft.Shared.Models.ViewModels.HR;

namespace D69soft.Server.Controllers.HR
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationalChartController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        public OrganizationalChartController(SqlConnectionConfig connConfig)
        {
            _connConfig = connConfig;
        }

        //Division
        [HttpPost("GetDivisionList")]
        public async Task<ActionResult<IEnumerable<DivisionVM>>> GetDivisionList(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryAsync<DivisionVM>("HR.Profile_viewDivisionMain", parm, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpGet("CheckContainsDivisionID/{id}")]
        public async Task<bool> CheckContainsDivisionID(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.Division where DivisionID = @DivisionID) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { DivisionID = id });
            }
        }

        [HttpPost("UpdateDivision")]
        public async Task<ActionResult<int>> UpdateDivision(DivisionVM _divisionVM)
        {
            var sql = "";
            if (_divisionVM.IsTypeUpdate == 0)
            {
                sql += "Insert into HR.Division (DivisionID,DivisionName,CodeDivs,isAutoEserial,is2625,INOUTNumber,isActive) Values (@DivisionID,@DivisionName,@CodeDivs,@isAutoEserial,@is2625,@INOUTNumber,@isActive)";
            }
            if (_divisionVM.IsTypeUpdate == 1)
            {
                sql += "Update HR.Division set DivisionName = @DivisionName, CodeDivs = @CodeDivs, isAutoEserial = @isAutoEserial, is2625 = @is2625, INOUTNumber = @INOUTNumber, isActive = @isActive where DivisionID = @DivisionID";
            }
            if (_divisionVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from HR.JobHistory where DivisionID=@DivisionID) ";
                sql += "begin ";
                sql += "delete from HR.Division where DivisionID = @DivisionID ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteAsync(sql, _divisionVM);
            }
        }

        //Department
        [HttpPost("GetDepartmentList")]
        public async Task<ActionResult<IEnumerable<DepartmentVM>>> GetDepartmentList(FilterHrVM _filterHrVM)
        {
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@DivsID", _filterHrVM.DivisionID);
                parm.Add("@UserID", _filterHrVM.UserID);

                var result = await conn.QueryAsync<DepartmentVM>("HR.Profile_viewDepartmentMain", parm, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpGet("CheckContainsDepartmentID/{id}")]
        public async Task<bool> CheckContainsDepartmentID(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.Department where DepartmentID = @DepartmentID) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { DepartmentID = id });
            }
        }

        [HttpPost("UpdateDepartment")]
        public async Task<ActionResult<int>> UpdateDepartment(DepartmentVM _departmentVM)
        {
            var sql = "";
            if (_departmentVM.IsTypeUpdate == 0)
            {
                sql += "Insert into HR.Department (DepartmentID,DepartmentName,DepartmentGroupID,DivisionID,isActive) Values (@DepartmentID,@DepartmentName,@DepartmentGroupID,@DivisionID,@isActive) ";
            }
            if (_departmentVM.IsTypeUpdate == 1)
            {
                sql += "Update HR.Department set DepartmentName = @DepartmentName,DepartmentGroupID = @DepartmentGroupID, isActive = @isActive where DepartmentID = @DepartmentID ";
            }
            if (_departmentVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from HR.JobHistory where DepartmentID=@DepartmentID) ";
                sql += "begin ";
                sql += "delete from HR.Department where DepartmentID = @DepartmentID ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteAsync(sql, _departmentVM);
            }
        }

        //DepartmentGroup
        [HttpGet("GetDepartmentGroupList")]
        public async Task<ActionResult<IEnumerable<DepartmentGroupVM>>> GetDepartmentGroupList()
        {
            var sql = "select * from HR.DepartmentGroup order by DepartmentGroupName ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<DepartmentGroupVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("CheckContainsDepartmentGroupID/{id}")]
        public async Task<bool> CheckContainsDepartmentGroupID(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.DepartmentGroup where DepartmentGroupID = @DepartmentGroupID) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { DepartmentGroupID = id });
            }
        }

        [HttpPost("UpdateDepartmentGroup")]
        public async Task<ActionResult<int>> UpdateDepartmentGroup(DepartmentGroupVM _departmentGroupVM)
        {
            var sql = "";
            if (_departmentGroupVM.IsTypeUpdate == 0)
            {
                sql = "Insert into HR.DepartmentGroup (DepartmentGroupID, DepartmentGroupName) Values (@DepartmentGroupID,@DepartmentGroupName) ";
            }
            if (_departmentGroupVM.IsTypeUpdate == 1)
            {
                sql = "Update HR.DepartmentGroup set DepartmentGroupName = @DepartmentGroupName where DepartmentGroupID = @DepartmentGroupID ";
            }
            if (_departmentGroupVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from HR.Department where DepartmentGroupID=@DepartmentGroupID) ";
                sql += "begin ";
                sql += "delete from HR.DepartmentGroup where DepartmentGroupID = @DepartmentGroupID ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteAsync(sql, _departmentGroupVM);
            }
        }

        //Section
        [HttpGet("CheckContainsSectionID/{id}")]
        public async Task<bool> CheckContainsSectionID(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.Section where SectionID = @SectionID) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { SectionID = id });
            }
        }

        [HttpGet("GetSectionList")]
        public async Task<ActionResult<IEnumerable<SectionVM>>> GetSectionList()
        {
            var sql = "select * from HR.Section order by SectionName";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<SectionVM>(sql);
                return Ok(result);
            }
        }

        [HttpPost("UpdateSection")]
        public async Task<ActionResult<int>> UpdateSection(SectionVM _sectionVM)
        {
            var sql = "";
            if (_sectionVM.IsTypeUpdate == 0)
            {
                sql = "Insert into HR.Section (SectionID,SectionName,isActive) Values (@SectionID,@SectionName,@isActive)";
            }
            if (_sectionVM.IsTypeUpdate == 1)
            {
                sql = "Update HR.Section set SectionName = @SectionName, isActive = @isActive where SectionID = @SectionID";
            }
            if (_sectionVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from HR.JobHistory where SectionID=@SectionID) ";
                sql += "begin ";
                sql += "delete from HR.Section where SectionID = @SectionID ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteAsync(sql, _sectionVM);
            }
        }

        //Position
        [HttpGet("GetPositionList")]
        public async Task<ActionResult<List<PositionVM>>> GetPositionList()
        {
            var sql = "select * from HR.Position po join HR.PositionGroup pogrp on pogrp.PositionGroupID = po.PositionGroupID order by PositionGroupNo, PositionName";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<PositionVM>(sql);
                return result.ToList();
            }
        }

        [HttpGet("CheckContainsPositionID/{id}")]
        public async Task<bool> CheckContainsPositionID(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.Position where PositionID = @PositionID) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { PositionID = id });
            }
        }

        [HttpPost("UpdatePosition")]
        public async Task<ActionResult<int>> UpdatePosition(PositionVM _positionVM)
        {
            var sql = "";
            if (_positionVM.IsTypeUpdate == 0)
            {
                sql += "Insert into HR.Position (PositionID,PositionName,PositionGroupID,JobDesc,isActive) Values (@PositionID,@PositionName,@PositionGroupID,@JobDesc,@isActive) ";
            }
            if (_positionVM.IsTypeUpdate == 1)
            {
                sql += "Update HR.Position set PositionName = @PositionName, PositionGroupID=@PositionGroupID, JobDesc=@JobDesc, isActive = @isActive where PositionID = @PositionID ";
            }
            if (_positionVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from HR.JobHistory where PositionID=@PositionID) ";
                sql += "begin ";
                sql += "delete from HR.Position where PositionID = @PositionID ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteAsync(sql, _positionVM);
            }
        }

        //PositionGroup
        [HttpGet("GetPositionGroupList")]
        public async Task<ActionResult<IEnumerable<PositionGroupVM>>> GetPositionGroupList()
        {
            var sql = "select * from HR.PositionGroup order by PositionGroupNo ";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var result = await conn.QueryAsync<PositionGroupVM>(sql);
                return Ok(result);
            }
        }

        [HttpGet("CheckContainsPositionGroupID/{id}")]
        public async Task<bool> CheckContainsPositionGroupID(string id)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.PositionGroup where PositionGroupID = @PositionGroupID) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { PositionGroupID = id });
            }
        }

        [HttpPost("UpdatePositionGroup")]
        public async Task<ActionResult<int>> UpdatePositionGroup(PositionGroupVM _positionGroupVM)
        {
            var sql = "";
            if (_positionGroupVM.IsTypeUpdate == 0)
            {
                sql = "Insert into HR.PositionGroup (PositionGroupID, PositionGroupName, PositionGroupNo) Values (@PositionGroupID,@PositionGroupName, @PositionGroupNo) ";
            }
            if (_positionGroupVM.IsTypeUpdate == 1)
            {
                sql = "Update HR.PositionGroup set PositionGroupName = @PositionGroupName, PositionGroupNo = @PositionGroupNo where PositionGroupID = @PositionGroupID ";
            }
            if (_positionGroupVM.IsTypeUpdate == 2)
            {
                sql = "if not exists (select * from HR.Position where PositionGroupID=@PositionGroupID) ";
                sql += "begin ";
                sql += "delete from HR.PositionGroup where PositionGroupID = @PositionGroupID ";
                sql += "end ";
            }
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteAsync(sql, _positionGroupVM);
            }
        }
    }
}
