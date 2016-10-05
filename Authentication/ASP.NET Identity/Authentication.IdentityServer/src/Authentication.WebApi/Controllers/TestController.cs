using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.WebApi.Controllers
{
    public class TestController : Controller
    {
        [HttpGet, Route("identity")]
        public dynamic Get()
        {
            return new JsonResult(User.Claims.Select(e => new
            {
                e.Type,
                e.Value
            }));
        }
    }
}
