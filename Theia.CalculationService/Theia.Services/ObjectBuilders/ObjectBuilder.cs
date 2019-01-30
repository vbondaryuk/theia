using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using Theia.Common.Exceptions;
using Theia.Common.Utils;
using Theia.Core.Services;

namespace Theia.Services.ObjectBuilders
{
    public class ObjectBuilder : IObjectBuilder
    {
        public Assembly BuildAssembly(string sourceCode)
        {
            var libraryName = $"{StaticVariables.DynamicAssemblyPath()}\\{Guid.NewGuid():D}.dll";
            CodeDomProvider compiler = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateExecutable = false,
                OutputAssembly = libraryName
            };

            var results = compiler.CompileAssemblyFromSource(parameters, sourceCode);
            if (results.Errors.HasErrors)
            {
                var errors = new StringBuilder();
                foreach (CompilerError error in results.Errors)
                {
                    errors.AppendLine($"{error.ErrorNumber}   {error.ErrorText}");
                }
                
                throw new TheiaException($"Assembly was not compiled: {Environment.NewLine} {errors}");
            }

            var assembly = Assembly.LoadFrom(libraryName);

            return assembly;
        }
    }
}