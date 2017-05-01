using System;
using System.Collections.Generic;
using System.Reflection;
using java.lang;
using org.drools;
using org.drools.builder;
using org.drools.io;
using org.drools.runtime;
using Theia.Common.Exceptions;
using Theia.Core.Common;
using Theia.Core.Models;


namespace Theia.Core.Calculations
{
    public class RulesCalculation : IRulesCalculation
    {
        public int Calculate<T>(Assembly assembly, List<Type>types, List<T> objects, List<IRule> rules)
        {
            var classLoader = new TheiaClassLoader(assembly);
            var kbuilder = CreateKnowledgeBuilder(rules, classLoader);

            var kbaseConfig = KnowledgeBaseFactory.newKnowledgeBaseConfiguration(null, classLoader);
            var kbase = KnowledgeBaseFactory.newKnowledgeBase(kbaseConfig);
            kbase.addKnowledgePackages(kbuilder.getKnowledgePackages());

            var ksession = kbase.newStatefulKnowledgeSession();
            foreach (var obj in objects)
            {
                ksession.insert(obj);
            }

            var countExequtedRules = ksession.fireAllRules();
            objects.AddRange(GetObjects(classLoader, ksession, types, objects));
            ksession.dispose();
            return countExequtedRules;
        }

        private List<T> GetObjects<T>(TheiaClassLoader classLoader, StatefulKnowledgeSession ksession, List<Type> types, List<T> objects)
        {
            var hash = new HashSet<T>(objects);
            List<T> list = new List<T>();
            foreach (var type in types)
            {
                Class javaClass = Class.forName($"cli.Theia.{type.Name}", false, classLoader);
                foreach (var newObject in ksession.getObjects(new ClassObjectFilter(javaClass)).toArray())
                {
                    if (!hash.Contains((T) newObject))
                        list.Add((T)newObject);
                }
            }
            return list;
        }

        private KnowledgeBuilder CreateKnowledgeBuilder(List<IRule> rules, TheiaClassLoader classLoader)
        {
            //Create configuration
            KnowledgeBuilderConfiguration kBuilderConfiguration = KnowledgeBuilderFactory.newKnowledgeBuilderConfiguration(null, classLoader);
            //Create knowledgeBuilder
            KnowledgeBuilder kbuilder = KnowledgeBuilderFactory.newKnowledgeBuilder(kBuilderConfiguration);
            foreach (var rule in rules)
            {
                //Create my resource
                Resource myResource = ResourceFactory.newReaderResource(new java.io.StringReader(rule.Source));

                //Add resource to KnowledgeBuilder
                kbuilder.add(myResource, ResourceType.DRL);
                var ruleErrors = kbuilder.getErrors();

                if (ruleErrors.size() > 0)
                {
                    throw new TheiaException(
                        $"Правила Содержат ошибку и не могут быть скомпилирована: {System.Environment.NewLine}{ruleErrors}");
                }
            }

            //Check resource errors
            
            return kbuilder;
        }
    }
}