using System.Collections.Generic;
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
        public CalculationModelResponse Calculate(JsonCalculationRequestModel calculationRequest)
        {
            var calculationModelResponse =_calculationServiceAdapter.Calculate(calculationRequest);
            return calculationModelResponse;
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