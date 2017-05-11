using System.Collections.Generic;
using Theia.Core.Models;
using Theia.Infrastructure.Models;

namespace Theia.Infrastructure.Rules
{
    public interface IRuleMaper
    {
        List<IRule> Map(List<RuleModel> ruleModels);
    }
}