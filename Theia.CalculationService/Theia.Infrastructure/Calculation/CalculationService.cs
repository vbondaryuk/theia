using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Theia.Core.Calculations;
using Theia.Infrastructure.Models;
using Theia.Infrastructure.Rules;

namespace Theia.Infrastructure.Calculation
{
    public abstract  class CalculationService : ICalculationService
    {
        private readonly IRulesCalculation _rulesCalculation;
        private readonly IRuleMapper _ruleMapper;

        protected CalculationService(IRulesCalculation rulesCalculation, IRuleMapper ruleMapper)
        {
            _rulesCalculation = rulesCalculation;
            _ruleMapper = ruleMapper;
        }

        public abstract CalculationModelResponse Calculate<T>(CalculationRequestModel<T> calculationRequestModel);

        protected int Calculate<T>(Assembly assembly, List<Type> types, List<RuleModel> ruleModels, List<T> objects)
        {
            var groupedRules = ruleModels.GroupBy(x => x.Priority,
                (key, val) => new {Priority = key, Rules = val.ToList()}).OrderByDescending(x => x.Priority);

            return groupedRules.Sum(groupedRule => _rulesCalculation.Calculate(assembly, types, objects, _ruleMapper.Map(groupedRule.Rules)));
        }
    }
}