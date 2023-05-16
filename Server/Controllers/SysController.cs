using D69soft.Server.Services;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Mvc;

namespace D69soft.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SysController : ControllerBase
    {
        private readonly SysService _sysService;

        public SysController(SysService sysService)
        {
            _sysService = sysService;
        }

        //Insert ErrorLog
        [HttpPost("ErrorLog")]
        public async Task<ActionResult> ErrorLog(ErrorLogVM _errorLogVM)
        {
            await _sysService.ErrorLog(_errorLogVM);
            return Ok();
        }

        //Info User
        [HttpGet("GetInfoUser/{_UserID}")]
        public async Task<ActionResult> GetInfoUser(string _UserID)
        {
            return Ok(await _sysService.GetInfoUser(_UserID));
        }

        //Menu Func
        [HttpGet("GetModuleMenu/{_UserID}")]
        public async Task<ActionResult> GetModuleMenu(string _UserID)
        {
            return Ok(await _sysService.GetModuleMenu(_UserID));
        }

        [HttpGet("GetFuncMenuGroup/{_UserID}")]
        public async Task<ActionResult> GetFuncMenuGroup(string _UserID)
        {
            return Ok(await _sysService.GetFuncMenuGroup(_UserID));
        }

        [HttpGet("GetFuncMenu/{_UserID}")]
        public async Task<ActionResult> GetFuncMenu(string _UserID)
        {
            return Ok(await _sysService.GetFuncMenu(_UserID));
        }

        [HttpGet("CheckViewFuncMenuRpt/{_UserID}")]
        public async Task<ActionResult> CheckViewFuncMenuRpt(string _UserID)
        {
            return Ok(await _sysService.CheckViewFuncMenuRpt(_UserID));
        }

    }
}
