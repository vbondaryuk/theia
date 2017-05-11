using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Theia.Core.Models;
using Theia.Infrastructure.Models;

namespace Theia.Infrastructure.Rules
{
    public class RuleMaper : IRuleMaper
    {
        public List<IRule> Map(List<RuleModel> ruleModels)
        {
            return
                ruleModels.Select(ruleModel => new Rule {Source = ConvertSource(ruleModel.Source)})
                    .Cast<IRule>()
                    .ToList();
        }

        private string ConvertSource(string source)
        {
            var ruleStringBuilder = new StringBuilder();
            var lines =
                source.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
            var containsPackage = false;
            foreach (var line in lines)
            {
                if (line.StartsWith("package"))
                {
                    ruleStringBuilder.AppendLine("package cli.Theia.*;");
                    ruleStringBuilder.AppendLine("import cli.Theia.*;");
                    containsPackage = true;
                } else if (line.StartsWith("import") && !(line.Contains(" cli.") || line.Contains(" java.")))
                {
                }
                else
                {
                    ruleStringBuilder.AppendLine(line);
                }
            }
            if (!containsPackage)
            {
                ruleStringBuilder.Insert(0, "import cli.Theia.*;\n");
                //ruleStringBuilder.Insert(0,$"package cli.Theia.*;{Environment.NewLine}");
            }
            return ruleStringBuilder.ToString();
        }
    }
}