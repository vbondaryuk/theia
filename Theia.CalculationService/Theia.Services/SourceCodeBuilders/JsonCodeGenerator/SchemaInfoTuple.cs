using Newtonsoft.Json.Schema;

namespace Theia.Services.SourceCodeBuilders.JsonCodeGenerator
{
    public class SchemaInfoTuple
    {
        public JSchema JSchema { get; set; }
        public string RootClassName { get; set; }
        public SchemaInfoTuple(JSchema schema, string rootClassName)
        {
            JSchema = schema;
            RootClassName = rootClassName;
        }
    }
}