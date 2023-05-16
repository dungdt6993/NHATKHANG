using D69soft.Server.Services;
using D69soft.Server.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Mvc;

namespace D69soft.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("LoginRequest")]
        public async Task<ActionResult<int>> LoginRequest(UserVM _UserVM)
        {
            return await _authService.LoginRequest(_UserVM);
        }

        [HttpGet("CheckChangePassDefault/{_UserID}")]
        public async Task<ActionResult> CheckChangePassDefault(string _UserID)
        {
            return Ok(await _authService.CheckChangePassDefault(_UserID));
        }

        [HttpPost("ChangePass")]
        public async Task<ActionResult<bool>> ChangePass(ChangePassVM _ChangePassVM)
        {
            return await _authService.ChangePass(_ChangePassVM);
        }
    }
}
