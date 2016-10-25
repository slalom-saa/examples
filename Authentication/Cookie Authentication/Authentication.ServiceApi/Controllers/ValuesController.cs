using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace Authentication.ServiceApi.Controllers
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
            return ((ClaimsPrincipal)this.User).Claims.Select(e =>
                                                              new
                                                              {
                                                                  e.Type,
                                                                  e.Value
                                                              });
        }
    }
}