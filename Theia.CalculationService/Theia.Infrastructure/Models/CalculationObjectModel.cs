using Theia.Core.Models;

namespace Theia.Infrastructure.Models
{
    public class CalculationObjectModel<T> : ICalculationObject<T>
    {
        public string RootClassName { get; set; }
        public string Schema { get; set; }
        public T Data { get; set; }
        
    }
}