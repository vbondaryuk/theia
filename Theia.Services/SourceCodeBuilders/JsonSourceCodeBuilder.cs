using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Schema;
using Theia.Common.Utilits;
using Theia.Core.Services;
using Theia.Services.SourceCodeBuilders.JsonCodeGenerator;

namespace Theia.Services.SourceCodeBuilders
{
    public class JsonSourceCodeBuilder : ISourceCodeBuilder
    {
        private readonly Dictionary<string, string> _jsonSchemaDictionary;
        public JsonSourceCodeBuilder()
        {
            _jsonSchemaDictionary = new Dictionary<string, string>();
        }

        public JsonSourceCodeBuilder AddJsonData(string className, string dataJson)
        {
            var jsonSchema = CreateJsonSchema(dataJson);
            _jsonSchemaDictionary.Add(className, jsonSchema);
            return this;
        }
        public JsonSourceCodeBuilder AddJsonSchema(string className, string jsonSchema)
        {
            _jsonSchemaDictionary.Add(className, jsonSchema);
            return this;
        }

        public string Build()
        {
            var objectsCodeStringBuilder = new StringBuilder();
            var schemaTuples = (from keyValue in _jsonSchemaDictionary
                let jsonSchema = JSchema.Parse(keyValue.Value)
                select new SchemaInfoTuple(jsonSchema, keyValue.Key)).ToList();

            var schemaGenerator = new SourceCodeGenerator<CSharpObjectDefinition>(schemaTuples);
            objectsCodeStringBuilder.AppendLine(schemaGenerator.Generate());
            var sourceCode = $@"
using System;
using System.Collections.Generic;
using System.Text;
namespace Theia
{{
{objectsCodeStringBuilder}
}}
";
            return sourceCode;
        }

        private string CreateJsonSchema(string jsonData)
        {
            var address = $"{StaticVariables.SchemaGeneratorHost}:{StaticVariables.SchemaGeneratorPort}/jsonSchema";

            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = client.PostAsync(address, content).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}