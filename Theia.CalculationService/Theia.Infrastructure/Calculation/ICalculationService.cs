using Theia.Infrastructure.Models;

namespace Theia.Infrastructure.Calculation
{
    public interface ICalculationService
    {
        CalculationModelResponse Calculate<T>(CalculationRequestModel<T> calculationRequestModel);
    }
}