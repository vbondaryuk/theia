using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Schema;
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
            foreach (var keyValue in _jsonSchemaDictionary)
            {
                var jsonSchema = JSchema.Parse(keyValue.Value);
                var schemaGenerator = new SourceCodeGenerator<CSharpObjectDefinition>(jsonSchema, keyValue.Key);
                objectsCodeStringBuilder.AppendLine(schemaGenerator.ToString());
            }
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
            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = client.PostAsync("http://127.0.0.1:8081/jsonSchema", content).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}