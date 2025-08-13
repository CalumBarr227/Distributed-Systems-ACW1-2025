using DistSysAcwServer.Models;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DistSysAcwServer.Controllers
{
    [ApiController]
    [Route("api/testauth")]
    public class AuthController : BaseController
    {

        public AuthController(UserContext dbcontext, SharedError error)
            : base(dbcontext, error)
        {
        }


        [HttpGet("user")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult TestUserAccess()
        {
            return Ok("Valid API key or Admin");
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult TestAdminAccess()
        {
            return Ok("You are Admin");
        }
    }
}
