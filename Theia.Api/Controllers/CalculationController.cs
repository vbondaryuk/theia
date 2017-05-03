using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Theia.Infrastructure.Calculation;
using Theia.Infrastructure.Models;

namespace Theia.Api.Controllers
{
    public class CalculationController : ApiController
    {
        private readonly ICalculationService _calculationServiceAdapter;

        public CalculationController(ICalculationService calculationServiceAdapter)
        {
            _calculationServiceAdapter = calculationServiceAdapter;
        }

        public IEnumerable<string> Get()
        {
            return new List<string> { "ASP.NET", "Docker", "Windows Containers" };
        }

        [Route("api/calculation/calculate/")]
        [HttpPost]
        public HttpResponseMessage Calculate(JsonCalculationRequestModel calculationRequest)
        {
            try
            {
                var calculationModelResponse = _calculationServiceAdapter.Calculate(calculationRequest);
                return Request.CreateResponse(HttpStatusCode.OK, calculationModelResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Error = e.Message});
            }
        }

        //[Route("api/calculation/calculate/")]
        //[HttpPost]
        //public CalculationResponse Calculate([FromBody]string json)
        //{
        //    var calculationRequest = JsonConvert.DeserializeObject<CalculationRequest>(json);
        //  var calculationModelResponse =_calculationServiceAdapter.Calculate(calculationRequest);
        //var json = JsonConvert.SerializeObject(calculationModelResponse, Formatting.None,
        //    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
        //    var q = Request.Content.ReadAsStringAsync().Result;
        //    return null;
        //}
    }
}