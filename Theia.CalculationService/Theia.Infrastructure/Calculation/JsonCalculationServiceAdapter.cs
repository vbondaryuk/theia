using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Theia.Common.Extensions;
using Theia.Core.Calculations;
using Theia.Core.Services;
using Theia.Infrastructure.Models;
using Theia.Infrastructure.Rules;
using Theia.Services.SourceCodeBuilders;

namespace Theia.Infrastructure.Calculation
{
    public class JsonCalculationServiceAdapter : CalculationService
    {
        private readonly JsonSourceCodeBuilder _sourceCodeBuilder;
        private readonly IObjectBuilder _objectBuilder;

        public JsonCalculationServiceAdapter(
            IRulesCalculation rulesCalculation, 
            IRuleMaper ruleMaper,
            ISourceCodeBuilder sourceCodeBuilder, 
            IObjectBuilder objectBuilder)
            : base(rulesCalculation, ruleMaper)
        {
            _sourceCodeBuilder = (JsonSourceCodeBuilder) sourceCodeBuilder;
            _objectBuilder = objectBuilder;
        }

        public override CalculationModelResponse Calculate<T>(CalculationRequestModel<T> calculationRequestModel)
        {
            return Calculate(calculationRequestModel as JsonCalculationRequestModel);
        }

        public CalculationModelResponse Calculate(JsonCalculationRequestModel calculationRequestModel)
        {
            var assembly = BuildAssembly(calculationRequestModel);

            var objectInfos = new List<CalculationObjectInfo>();
            List<object> calculationObjects = new List<object>();
            foreach (var calculationObject in calculationRequestModel.CalculationObjects)
            {
                var objectType = assembly.GetType($"Theia.{calculationObject.RootClassName}");
                if (calculationObject.Data is JArray)
                {
                    Type genericListType = typeof(List<>).MakeGenericType(objectType);
                    var listType = Activator.CreateInstance(genericListType).GetType();
                    calculationObjects.AddRange(((IList)calculationObject.Data.ToObject(listType)).Cast<object>());
                }
                else if (calculationObject.Data is JObject)
                {
                    calculationObjects.Add(calculationObject.Data.ToObject(objectType));
                }
                objectInfos.Add(new CalculationObjectInfo(calculationObject.RootClassName, objectType));
            }
            var types = objectInfos.Select(x => x.Type).ToList();
            var countExequtedRules = Calculate(assembly, types, calculationRequestModel.Rules, calculationObjects);

            var response = new CalculationModelResponse {FiredRules = countExequtedRules};
            foreach (var objectInfo in objectInfos)
            {
                var data = calculationObjects.OfType(objectInfo.Type);
                response.CalculationObjects.Add(new CalculationObjectModel<object> {Data = data, RootClassName = objectInfo.RootClassName});
            }

            return response;
        }

        private Assembly BuildAssembly(JsonCalculationRequestModel calculationRequestModel)
        {
            var sourceCode = GetAssemblySourceCode(calculationRequestModel);
            var assembly = _objectBuilder.BuildAssembly(sourceCode);
            return assembly;
        }

        private string GetAssemblySourceCode(JsonCalculationRequestModel calculationRequestModel)
        {
            foreach (var calculationObject in calculationRequestModel.CalculationObjects)
            {
                var schema = calculationObject.Schema;
                if (string.IsNullOrWhiteSpace(schema))
                {
                    var dataJson = calculationObject.Data.ToString(Formatting.None);
                    _sourceCodeBuilder.AddJsonData(calculationObject.RootClassName, dataJson);
                }
                else
                {
                    _sourceCodeBuilder.AddJsonSchema(calculationObject.RootClassName, calculationObject.Schema);
                }
            }

            var sourceCode = _sourceCodeBuilder.Build();
            return sourceCode;
        }

        private class CalculationObjectInfo
        {
            public string RootClassName { get; }
            public Type Type { get; }

            public CalculationObjectInfo(string rootClassName, Type type)
            {
                RootClassName = rootClassName;
                Type = type;
            }
        }
    }
}