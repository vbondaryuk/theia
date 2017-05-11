using System;
using System.Configuration;
using System.IO;

namespace Theia.Common.Utilits
{
    public static class StaticVariables
    {
        public static string GetDynamicAssemblyPath()
        {
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DynamicAssembly");
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string TheiaHost => ConfigurationManager.AppSettings["Theia.Host"];
        public static string TheiaPort => ConfigurationManager.AppSettings["Theia.Port"];
        public static string SchemaGeneratorHost => ConfigurationManager.AppSettings["SchemaGenerator.Host"];
        public static string SchemaGeneratorPort => ConfigurationManager.AppSettings["SchemaGenerator.Port"];
    }
}