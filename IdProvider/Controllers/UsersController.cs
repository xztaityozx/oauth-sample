using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using IdProvider.Models;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Microsoft.IdentityModel.JsonWebTokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private static List<User> Users { get; } = new()
        {
            Models.User.GenerateUser(0, "foo", "password")
        };

        private static List<Service> Services { get; } = new()
        {
            new Service(0, "https://localhost/callback/auth_code", "sample")
        };

        private static List<AuthCode> AuthCodes { get; } = new();

        // GET api/<UsersController>/5
        [HttpGet("get/{id:int}")]
        public ActionResult<UserResponse> Get(int id, [FromQuery] string code)
        {
            try
            {
                var item = Users.First(u => u.Id == id);
                if (AuthCodes.Exists(ac => ac.UserId == id && ac.Code == code)) 
                    return new UserResponse(item.Id, item.Name);
                return Unauthorized();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST api/<UsersController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        public ActionResult Post([FromBody] UserCreateRequest request)
        {
            try
            {
                if (new[] { request.Name, request.Password }.Any(v => string.IsNullOrEmpty(v) || string.IsNullOrWhiteSpace(v)))
                {
                    return BadRequest();
                }

                var nextId = Users.Aggregate(0, (i, u) => Math.Max(i, u.Id));
                var user = Models.User.GenerateUser(nextId, request.Name, request.Password);

                Users.Add(user);
                return CreatedAtAction(nameof(Get), new { id = nextId }, request);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return StatusCode(500);
            }
        }

        // DELETE api/<UsersController>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Delete([FromBody] UserDeleteRequest request)
        {
            var (id, password) = request;
            if (!ExistsUser(id)) return BadRequest();
            if (!Login(id, password)) return Unauthorized();

            var target = Users.First(u => u.Id == id);
            Users.Remove(target);
            return Ok();
        }

        private static bool Login(int id, string password)
        {
            return Users.First(u => u.Id == id).Login(password);
        }

        private static bool ExistsUser(int id) => Users.Exists(u => u.Id == id);

        [HttpGet("code/auth/{id:int}/{serviceId:int}")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult GenerateAuthCode(int id, int serviceId)
        {
            if (!ExistsUser(id)) return BadRequest();
            if (!Services.Exists(s => s.Id == serviceId)) return BadRequest();

            var code = AuthCode.GenerateCode(AuthCodes.Aggregate(0, (i, ac) => Math.Max(i, ac.Id)), id);
            AuthCodes.Add(code);

            var (_, uri, _) = Services.First(s => s.Id == serviceId);
            var param = new Dictionary<string, string?>
            {
                ["code"] = code.Code
            };


            var location = new Uri(QueryHelpers.AddQueryString(uri, param)).ToString();
            Debug.WriteLine(location);

            return Redirect(location);
        }
    }
}
