using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Schema;

namespace Theia.Services.SourceCodeBuilders.JsonCodeGenerator
{
    public class SourceCodeGenerator<T> where T : ObjectDefinition, new()
    {
        public string RootClassName { get; }
        public List<T> ObjectDefinitions { get; }

        public SourceCodeGenerator(JSchema schema, string rootClassName)
        {
            RootClassName = rootClassName;
            ObjectDefinitions = new List<T>();
            ConvertJsonSchemaToModel(schema);
        }

        private void ConvertJsonSchemaToModel(JSchema schema)
        {
            if (schema.Type == null)
                throw new Exception("Schema does not specify a type.");

            switch (schema.Type)
            {
                case JSchemaType.Object:
                    CreateTypeFromSchema(schema, RootClassName);
                    break;

                case JSchemaType.Array:
                    foreach (var item in schema.Items.Where(x => x.Type.HasValue && x.Type == JSchemaType.Object))
                    {
                        CreateTypeFromSchema(item, RootClassName);
                    }
                    break;
            }
        }

        private T CreateTypeFromSchema(JSchema schema, string name = null)
        {
            var def = new T
            {
                Name = name ?? schema.Title,
                Properties =
                    schema.Properties.ToDictionary(item => item.Key.Trim(),
                        item => GetTypeFromSchema(schema, item.Value, item.Key))
            };

            ObjectDefinitions.Add(def);
            return def;
        }

        private string GetTypeFromSchema(JSchema parent, JSchema jsonSchema, string name = null)
        {
            if (jsonSchema.Type != JSchemaType.Object)
                return new T().GetTypeFromSchema(parent, jsonSchema, name);
            var def = CreateTypeFromSchema(jsonSchema, name);
            return def.Name;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var objectDefinition in ObjectDefinitions)
            {
                sb.AppendLine(objectDefinition.ToString());
            }
            return sb.ToString();
        }
    }
}