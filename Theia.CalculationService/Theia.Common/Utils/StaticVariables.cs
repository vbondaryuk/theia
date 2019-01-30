using System;
using System.Configuration;
using System.IO;

namespace Theia.Common.Utils
{
    public static class StaticVariables
    {
	    private static string _dynamicAssemblyDirectory;

        public static string DynamicAssemblyPath()
        {
	        if (_dynamicAssemblyDirectory != null) return _dynamicAssemblyDirectory;
	        var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DynamicAssembly");
	        Directory.CreateDirectory(directory);
	        _dynamicAssemblyDirectory = directory;
	        return _dynamicAssemblyDirectory;
        }

        public static string TheiaHost => ConfigurationManager.AppSettings["Theia.Host"];
        public static string TheiaPort => ConfigurationManager.AppSettings["Theia.Port"];
        public static string SchemaGeneratorHost => ConfigurationManager.AppSettings["SchemaGenerator.Host"];
        public static string SchemaGeneratorPort => ConfigurationManager.AppSettings["SchemaGenerator.Port"];
    }
}