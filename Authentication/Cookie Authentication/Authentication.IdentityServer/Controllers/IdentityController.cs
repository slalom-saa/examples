using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WebApplication4.Controllers
{
    [RoutePrefix("api/account")]
    public class IdentityController : ApiController
    {
        [Route("sign-in"), HttpGet]
        public dynamic SignIn()
        {
            return "asdfasf";
        }
    }
}