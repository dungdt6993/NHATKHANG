using D69soft.Server.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace D69soft.Server.Controllers.HR
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _profileService;

        public ProfileController(ProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("GetContacts/{_UserID}")]
        public async Task<ActionResult<List<ProfileManagamentVM>>> GetContacts(string _UserID)
        {
            return Ok(await _profileService.GetContacts(_UserID));
        }
    }
}
