using D69soft.Shared.Models.ViewModels.SYSTEM;
using Dapper;
using Data.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using D69soft.Shared.Utilities;
using DevExpress.Utils.About;
using DevExpress.XtraRichEdit.Import.Html;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using D69soft.Client.Pages.Auth;

namespace D69soft.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SqlConnectionConfig _connConfig;

        private readonly IConfiguration _configuration;

        public AuthController(SqlConnectionConfig connConfig, IConfiguration configuration)
        {
            _connConfig = connConfig;

            _configuration = configuration;
        }

        [HttpGet("CheckChangePassDefault/{_UserID}")]
        public async Task<ActionResult<bool>> CheckChangePassDefault(string _UserID)
        {
            var sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM HR.Profile where Eserial = @UserID and coalesce(User_isChangePass,0)=0) THEN 0 ELSE 1 END as BIT)";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                return await conn.ExecuteScalarAsync<bool>(sql, new { UserID = _UserID });
            }
        }

        [HttpPost("ChangePass")]
        public async Task<ActionResult<bool>> ChangePass(ChangePassVM _changePassVM)
        {
            _changePassVM.NewPass = LibraryFunc.GennerateToMD5(_changePassVM.NewPass);

            var sql = "Update HR.Profile set User_Password = @NewPass, User_isChangePass = 1 where Eserial = @Eserial";
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                await conn.ExecuteAsync(sql, _changePassVM);
            }

            return true;
        }

        //JWT
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseVM>> Login([FromBody] UserVM _userVM)
        {
            int loginResponse = 0;

            //checking if the user exists in the database
            using (var conn = new SqlConnection(_connConfig.Value))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                DynamicParameters parm = new DynamicParameters();
                parm.Add("@txtUser", _userVM.Eserial);
                parm.Add("@txtPass", LibraryFunc.GennerateToMD5(_userVM.User_Password));

                loginResponse = await conn.ExecuteScalarAsync<int>("SYSTEM.Authentication_login", parm, commandType: CommandType.StoredProcedure);
            }

            if (loginResponse != 1) return BadRequest(new LoginResponseVM { Successful = false, Error = "Tài khoản hoặc mật khẩu không đúng." });

            var claims = new[]
                {
                    new Claim("UserID", _userVM.Eserial)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

                var token = new JwtSecurityToken(
                    _configuration["JwtIssuer"],
                    _configuration["JwtAudience"],
                    claims,
                    expires: expiry,
                    signingCredentials: creds
                );
            return Ok(new LoginResponseVM { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
