using System.Collections.Generic;
using DistSysAcwServer.Middleware;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DistSysAcwServer.Controllers
{
    [Route("api/talkback")]
    [ApiController]
    public class TalkbackController : BaseController
    {


        /// <summary>
        /// Constructs a TalkBack controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public TalkbackController(Models.UserContext dbcontext, SharedError error) : base(dbcontext, error) { }


        #region TASK1
        //    TODO: add api/talkback/hello response
        [HttpGet("hello")]
        public IActionResult Hello()
        {
            //Error.StatusCode = 501;
            //Error.Message = "Not Implemented";
            //return new EmptyResult();
            Error.StatusCode = 200;
            return Ok("Hello World");
        }
        #endregion

        #region TASK1
        //    TODO:
        //       add a parameter to get integers from the URI query
        //       sort the integers into ascending order
        //       send the integers back as the api/talkback/sort response
        //       conform to the error handling requirements in the spec

        [HttpGet("sort")]
        public IActionResult Sort([FromQuery] List<string> _integers)
        {
            var result = new List<int>();

            foreach (var value in _integers)
            {
                if (int.TryParse(value, out int parsed))
                {
                    result.Add(parsed);
                }
                else
                {
                    Error.StatusCode = 400;
                    Error.Message = "One or more inputs are not valid integers";
                    return BadRequest("Invalid input");
                }
            }

            result.Sort();
            Error.StatusCode = 200;
            return Ok(result);
        }
        #endregion
    }
}
