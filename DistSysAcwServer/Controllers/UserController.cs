using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DistSysAcwServer.Models;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Authorization;

namespace DistSysAcwServer.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly UserRepo _repo;

        public UserController(UserContext dbcontext, SharedError error) : base(dbcontext, error)
        {
            _repo = new UserRepo(dbcontext);
        }

        [HttpGet("new")]
        public IActionResult GetUserStatus([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                Error.StatusCode = 200;
                return Ok("False - User Does Not Exist! Did you mean to do a POST to create a new user?");
            }

            bool exists = _repo.UserExists(username);
            Error.StatusCode = 200;

            if (exists)
            {
                return Ok("True - User Does Exist! Did you mean to do a POST to create a new user?");
            }
            else
            {
                return Ok("False - User Does Not Exist! Did you mean to do a POST to create a new user?");
            }
        }

        [HttpPost("new")]
        public IActionResult CreateUser([FromBody] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                Error.StatusCode = 400;
                return BadRequest("Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json");
            }

            if (_repo.UserExists(username))
            {
                Error.StatusCode = 403;
                return StatusCode(403, "Oops. This username is already in use. Please try again with a new username.");
            }

            string role = _repo.GetUserCount() == 0 ? "Admin" : "User";

            var newUser = _repo.CreateUser(username, role);
            Error.StatusCode = 200;
            return Ok(newUser.ApiKey);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpDelete("removeuser")]
        public IActionResult RemoveUser([FromQuery] string username)
        {
            var apiKey = HttpContext.Request.Headers["ApiKey"].ToString();

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(username))
            {
                Error.StatusCode = 200;
                return Ok(false);
            }

            var user = _repo.GetUserByApiKey(apiKey);
            if (user == null || user.UserName != username)
            {
                Error.StatusCode = 200;
                return Ok(false);
            }

            bool deleted = _repo.DeleteUserByApiKey(apiKey);
            Error.StatusCode = 200;
            return Ok(deleted);

        }

        public class ChangeRoleRequest
        {
            public string Username { get; set; }
            public string Role { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("changerole")]
        public IActionResult ChangeUser([FromBody] ChangeRoleRequest data)
        {
            try
            {
                string username = data.Username;
                string role = data.Role;

                if (!_repo.UserExists(username))
                {
                    Error.StatusCode = 400;
                    return BadRequest("NOT DONE: Username does not exist");
                }

                if (role != "User" && role != "Admin")
                {
                    Error.StatusCode = 400;
                    return BadRequest("NOT DONE: Role does not exist");
                }

                var user = DbContext.Users.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                {
                    Error.StatusCode = 400;
                    return BadRequest("NOT DONE: An error occured");
                }

                user.Role = role;
                DbContext.SaveChanges();

                Error.StatusCode = 200;
                return Ok("DONE");
            }
            catch (Exception ex)
            {
                Error.StatusCode = 400;
                return BadRequest("NOT DONE: An error occured");
            }
        }

    }
}
