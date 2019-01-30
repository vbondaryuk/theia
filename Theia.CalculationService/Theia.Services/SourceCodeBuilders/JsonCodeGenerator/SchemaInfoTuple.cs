using Newtonsoft.Json.Schema;

namespace Theia.Services.SourceCodeBuilders.JsonCodeGenerator
{
    public class SchemaInfoTuple
    {
        public JSchema JSchema { get; }
        public string RootClassName { get; }

        public SchemaInfoTuple(JSchema schema, string rootClassName)
        {
            JSchema = schema;
            RootClassName = rootClassName;
        }
    }
}