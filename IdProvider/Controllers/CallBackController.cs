using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace IdProvider.Controllers
{
    [Route("/callback/[controller]")]

    public class CallBackController : ControllerBase
    {
        [HttpGet("/auth_code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult AuthCodeCallBack([FromQuery] string code)
        {
            Debug.WriteLine($"code: {code}");
            return Ok();
        }
    }
}
