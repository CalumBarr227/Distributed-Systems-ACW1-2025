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
        private static readonly RSACryptoServiceProvider _rsa;

        // Static constructor runs once when app starts
        static ProtectedController()
        {
            _rsa = new RSACryptoServiceProvider(2048);
        }
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

           
        }

        [HttpGet("getpublickey")]
        public IActionResult GetPublicKey()
        {
            var apiKey = Request.Headers["ApiKey"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(apiKey) || !DbContext.Users.Any(u => u.ApiKey == apiKey))
            {
                return Unauthorized("Invalid API Key");
            }

            string publicKeyXml = _rsa.ToXmlString(false);
            return Ok(publicKeyXml);
        }

        [HttpGet("sign")]
        public IActionResult Sign([FromQuery] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return BadRequest("Message is required");

            var apiKey = Request.Headers["ApiKey"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(apiKey) || !DbContext.Users.Any(u => u.ApiKey == apiKey))
            {
                return Unauthorized("Invalid API Key");
            }

            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            byte[] signedBytes = _rsa.SignData(messageBytes, new SHA1CryptoServiceProvider());
            string hexWithDashes = BitConverter.ToString(signedBytes);
            return Ok(hexWithDashes);
        }



    }
}

