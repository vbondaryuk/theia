using System;
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
    }
}