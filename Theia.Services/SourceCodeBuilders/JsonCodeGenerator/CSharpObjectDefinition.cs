using System;
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

                sb.AppendLine($"    public void Set{item.Key}({item.Value} value)");
                sb.AppendLine("     {");
                sb.AppendLine($"        {item.Key} = value;");
                sb.AppendLine("     }");
                sb.AppendLine($"    public {item.Value} Get{item.Key}()");
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
                    if (jsonSchema.Items.Count == 0)
                        return "List<object>";
                    if (jsonSchema.Items.Count == 1)
                        return $"List<{GetTypeFromSchema(jsonSchema, jsonSchema.Items.First())}>";
                    throw new Exception("Not sure what type this will be.");

                case JSchemaType.Boolean:
                    return "bool";

                case JSchemaType.Number:
                    return "double";

                case JSchemaType.Integer:
                    return "int";

                case JSchemaType.String:
                    return "string";

                case JSchemaType.Object:
                    throw new TheiaException("Object type should be mnage by the base type");

                default:
                    return "object";
            }
        }
    }
}