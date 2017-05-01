using System.Collections.Generic;

namespace Theia.Infrastructure.Models
{
    public interface ICalculationRequest<T>
    {
        List<RuleModel> Rules { get; set; }
        List<CalculationObjectModel<T>> CalculationObjects { get; set; }
    }
}