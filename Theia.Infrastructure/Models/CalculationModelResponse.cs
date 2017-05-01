using System.Collections.Generic;

namespace Theia.Infrastructure.Models
{
    public class CalculationModelResponse
    {
        public int FiredRules { get; set; }
        public List<CalculationObjectModel<object>> CalculationObjects { get; set; }

        public CalculationModelResponse()
        {
            CalculationObjects = new List<CalculationObjectModel<object>>();
        }
    }
}