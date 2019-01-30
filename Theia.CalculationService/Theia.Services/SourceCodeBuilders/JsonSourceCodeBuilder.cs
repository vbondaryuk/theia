using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using Theia.Common.Utils;
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
            var jsonSchema = CreateJsonSchema(dataJson).GetAwaiter().GetResult();
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
            var schemaTuples = new List<SchemaInfoTuple>();
            foreach (var keyValue in _jsonSchemaDictionary)
            {
	            JSchema jsonSchema = JSchema.Parse(keyValue.Value);
	            schemaTuples.Add(new SchemaInfoTuple(jsonSchema, keyValue.Key));
            }

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

        private async Task<string> CreateJsonSchema(string jsonData)
        {
            var address = $"{StaticVariables.SchemaGeneratorHost}:{StaticVariables.SchemaGeneratorPort}/jsonSchema";

            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(address, content);
                return  await response.Content.ReadAsStringAsync();
            }
        }
    }
}