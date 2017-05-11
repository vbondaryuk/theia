using System.Reflection;

namespace Theia.Core.Services
{
    public interface IObjectBuilder
    {
        Assembly BuildAssembly(string sourceCode);
    }
}