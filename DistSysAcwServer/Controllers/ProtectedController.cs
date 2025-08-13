using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DistSysAcwServer.Models;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DistSysAcwServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class ProtectedController : BaseController
    {
        public ProtectedController(UserContext dbContext, SharedError error)
            : base(dbContext, error)
        {
        }

        [HttpGet("hello")]
        public IActionResult Hello()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            return Ok($"Hello {username}");
        }

        [HttpGet("sha1")]
        public IActionResult SHA1HASH([FromQuery] string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Bad Request");
            }

            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(message);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
                return Ok(hashString);
            }

        }

        [HttpGet("sha256")]
        public IActionResult SHA256HASH([FromQuery] string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Bad Request");
            }
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(message);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
                return Ok(hashString);
            }

            Console.WriteLine("test");
        }
    }
}

