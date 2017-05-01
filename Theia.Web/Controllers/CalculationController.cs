using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace Theia.Web.Controllers
{
    public class CalculationController : ApiController
    {
        [Route("api/calculation/calculate/")]
        [HttpPost]
        public string Calculate([FromBody]string value)
        {
            return string.Empty;
        } 
    }
}
