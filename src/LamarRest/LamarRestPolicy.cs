using System;
using System.Linq;
using Baseline.Reflection;
using Lamar;
using Lamar.IoC.Instances;
using LamarCompiler;
using LamarRest.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace LamarRest
{
    public class LamarRestPolicy : Lamar.IFamilyPolicy
    {
        public ServiceFamily Build(Type type, ServiceGraph serviceGraph)
        {
            if (type.GetMethods().Any(x => x.HasAttribute<PathAttribute>()))
            {
                var generatedAssembly = new GeneratedAssembly(new GenerationRules("LamarRest"));
                var generatedType = new GeneratedServiceType(generatedAssembly, type);

                var container = (IContainer)serviceGraph.RootScope;
                
                container.CompileWithInlineServices(generatedAssembly);
                
                return new ServiceFamily(type, new IDecoratorPolicy[0], new ConstructorInstance(type, generatedType.CompiledType, ServiceLifetime.Singleton));
            }

            return null;
        }
    }
}