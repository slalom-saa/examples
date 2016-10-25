using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Authentication.IdentityServer.Api
{
    [RoutePrefix("api")]
    public class ValuesController : ApiController
    {
        [HttpGet, Route("")]
        public dynamic Get()
        {
            return "aa";
        }

        [HttpGet, Route("secure"), Authorize]
        public dynamic GetSecure()
        {
            return "aa";
        }
    }
}
