using System;
using System.Collections.Generic;
using System.Reflection;
using Theia.Infrastructure.Models;

namespace Theia.Infrastructure.Calculation
{
    public interface ICalculationService
    {
        CalculationModelResponse Calculate<T>(CalculationRequestModel<T> calculationRequestModel);
        int Calculate<T>(Assembly assembly, List<Type> types, List<RuleModel> ruleModels, List<T> objects);
    }
}