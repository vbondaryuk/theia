using System;
using System.Collections.Generic;
using System.Reflection;
using java.lang;
using org.drools;
using org.drools.builder;
using org.drools.io;
using org.drools.runtime;
using Theia.Common.Exceptions;
using Theia.Core.ClassLoaders;
using Theia.Core.Models;


namespace Theia.Core.Calculations
{
    public class RulesCalculation : IRulesCalculation
    {
        public int Calculate<T>(Assembly assembly, List<Type>types, List<T> objects, List<IRule> rules)
        {
            var classLoader = new TheiaClassLoader(assembly);
            var kBuilder = CreateKnowledgeBuilder(rules, classLoader);

            var kBaseConfig = KnowledgeBaseFactory.newKnowledgeBaseConfiguration(null, classLoader);
            var kBase = KnowledgeBaseFactory.newKnowledgeBase(kBaseConfig);
            kBase.addKnowledgePackages(kBuilder.getKnowledgePackages());

            var kSession = kBase.newStatefulKnowledgeSession();
            foreach (var obj in objects)
            {
                kSession.insert(obj);
            }

            int firedRules = kSession.fireAllRules();
            objects.AddRange(GetObjects(classLoader, kSession, types, objects));
            kSession.dispose();

            return firedRules;
        }

        private List<T> GetObjects<T>(TheiaClassLoader classLoader, StatefulKnowledgeSession kSession, List<Type> types, List<T> objects)
        {
            var hash = new HashSet<T>(objects);
            List<T> list = new List<T>();
            foreach (var type in types)
            {
                Class javaClass = Class.forName($"cli.Theia.{type.Name}", false, classLoader);
                foreach (var newObject in kSession.getObjects(new ClassObjectFilter(javaClass)).toArray())
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
            KnowledgeBuilder kBuilder = KnowledgeBuilderFactory.newKnowledgeBuilder(kBuilderConfiguration);
            foreach (var rule in rules)
            {
                //Create my resource
                Resource myResource = ResourceFactory.newReaderResource(new java.io.StringReader(rule.Source));

                //Add resource to KnowledgeBuilder
                kBuilder.add(myResource, ResourceType.DRL);
                var ruleErrors = kBuilder.getErrors();

                if (ruleErrors.size() > 0)
                {
                    throw new TheiaException(
                        $"The rule contains an error and can not be compiled: {System.Environment.NewLine}{ruleErrors}");
                }
            }
            
            return kBuilder;
        }
    }
}