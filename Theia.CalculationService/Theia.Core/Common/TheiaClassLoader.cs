using System.Reflection;
using java.lang;

namespace Theia.Core.Common
{
    public class TheiaClassLoader : ClassLoader
    {
        public TheiaClassLoader(Assembly assembly)
             : base(new ikvm.runtime.AssemblyClassLoader(assembly))
        {
            
        }
    }
}