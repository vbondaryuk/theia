using System.Reflection;
using java.lang;

namespace Theia.Core.ClassLoaders
{
    internal class TheiaClassLoader : ClassLoader
    {
        internal TheiaClassLoader(Assembly assembly)
             : base(new ikvm.runtime.AssemblyClassLoader(assembly))
        {
            
        }
    }
}