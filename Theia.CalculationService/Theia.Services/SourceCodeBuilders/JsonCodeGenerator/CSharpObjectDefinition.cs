using System.Linq;
using System.Text;
using Newtonsoft.Json.Schema;
using Theia.Common.Exceptions;

namespace Theia.Services.SourceCodeBuilders.JsonCodeGenerator
{
    public class CSharpObjectDefinition : ObjectDefinition
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("public class ");
            sb.Append(Name);
            sb.AppendLine("{");
            sb.AppendLine("");

            foreach (var item in Properties)
            {
                sb.AppendLine($"    public {item.Value} {item.Key};");

                sb.AppendLine($"    public void set{item.Key}({item.Value} value)");
                sb.AppendLine("     {");
                sb.AppendLine($"        {item.Key} = value;");
                sb.AppendLine("     }");
                sb.AppendLine($"    public {item.Value} get{item.Key}()");
                sb.AppendLine("     {");
                sb.AppendLine($"        return {item.Key};");
                sb.AppendLine("     }");
            }

            sb.AppendLine("};");
            return sb.ToString();
        }

        public override string GetTypeFromSchema(JSchema parent, JSchema jsonSchema, string name = null)
        {

            switch (jsonSchema.Type)
            {
                case JSchemaType.Array:
                    if (!jsonSchema.Items.Any())
                        return "List<object>";
                    return $"List<{GetTypeFromSchema(jsonSchema, jsonSchema.Items.First(), name)}>";
                case JSchemaType.Boolean:
                    return "bool";

                case JSchemaType.Number:
                    return "double";

                case JSchemaType.Integer:
                    return "int";

                case JSchemaType.String:
                    return "string";

                case JSchemaType.Object:
                    throw new TheiaException("object type must be set in parent class");

                default:
                    return "object";
            }
        }
        public override string GetArrayTypeFromSchema(string name)
        {
            return $"List<{name}>";
        }
    }
}