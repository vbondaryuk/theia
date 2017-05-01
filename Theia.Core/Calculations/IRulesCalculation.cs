using System;
using System.Collections.Generic;
using System.Reflection;
using Theia.Core.Models;

namespace Theia.Core.Calculations
{
    public interface IRulesCalculation
    {
        int Calculate<T>(Assembly assembly, List<Type> types, List<T> objects, List<IRule> rules);
    }
}