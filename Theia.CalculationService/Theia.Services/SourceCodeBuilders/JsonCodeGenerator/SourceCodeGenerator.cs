using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Schema;

namespace Theia.Services.SourceCodeBuilders.JsonCodeGenerator
{
    public sealed class SourceCodeGenerator<T> where T : ObjectDefinition, new()
    {
        private readonly List<SchemaInfoTuple> _schemaInfoTuples;
        private readonly List<T> _objectDefinitions;

        public SourceCodeGenerator(List<SchemaInfoTuple> schemaInfoTuples)
        {
            _schemaInfoTuples = schemaInfoTuples;
            _objectDefinitions = new List<T>();
        }

        public SourceCodeGenerator(JSchema schema, string rootClassName)
            : this(new List<SchemaInfoTuple> { new SchemaInfoTuple(schema, rootClassName) })
        {
        }

        public string Generate()
        {
            foreach (var schemaInfoTuple in _schemaInfoTuples)
            {
                ConvertJsonSchemaToModel(schemaInfoTuple.JSchema, schemaInfoTuple.RootClassName);
            }

            return ToString();
        }

        private void ConvertJsonSchemaToModel(JSchema schema, string rootClassName)
        {
            if (schema.Type == null)
                throw new Exception("Schema does not specify a type.");

            switch (schema.Type)
            {
                case JSchemaType.Object:
                    CreateTypeFromSchema(schema, rootClassName);
                    break;

                case JSchemaType.Array:
                    foreach (var item in schema.Items.Where(x => x.Type.HasValue && x.Type == JSchemaType.Object))
                    {
                        CreateTypeFromSchema(item, rootClassName);
                    }
                    break;
            }
        }

        private T CreateTypeFromSchema(JSchema schema, string name = null)
        {
            var objectName = name ?? schema.Title;
            var properties = schema.Properties.ToDictionary(item => item.Key.Trim(),
                item => GetTypeFromSchema(schema, item.Value, item.Key));
            var def = _objectDefinitions.FirstOrDefault(x => x.Name == objectName);
            if (def == null)
            {
                def = new T { Name = objectName };
                _objectDefinitions.Add(def);
            }
            else
            {
                foreach (var defProperty in def.Properties)
                {
                    if (!properties.ContainsKey(defProperty.Key))
                    {
                        properties.Add(defProperty.Key, defProperty.Value);
                    }
                }
            }
            def.Properties = properties;
            return def;
        }

        private string GetTypeFromSchema(JSchema parent, JSchema jsonSchema, string name = null)
        {
            switch (jsonSchema.Type)
            {
                case JSchemaType.Object:
                    var def = CreateTypeFromSchema(jsonSchema, name);
                    return def.Name;
                case JSchemaType.Array:
                    var item = jsonSchema.Items.First();

                    if (item.Type == JSchemaType.Object)
                    {
                        CreateTypeFromSchema(item, name);
                        return new T().GetArrayTypeFromSchema(name);
                    }
                    return new T().GetTypeFromSchema(parent, jsonSchema, name);
                default:
                    return new T().GetTypeFromSchema(parent, jsonSchema, name);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var objectDefinition in _objectDefinitions)
            {
                sb.AppendLine(objectDefinition.ToString());
            }
            return sb.ToString();
        }
    }
}