using System.Collections.Generic;

namespace Theia.Infrastructure.Models
{
    public class CalculationRequestModel<T> : ICalculationRequest<T>
    {
        public List<RuleModel> Rules { get; set; }
        public List<CalculationObjectModel<T>> CalculationObjects { get; set; }
    }
}