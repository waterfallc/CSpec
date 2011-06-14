using System;
using System.Reflection;
using System.Linq;

namespace CSpec.Proxy
{
	/// <summary>
	/// Description of ProxyGenerator.
	/// </summary>
	public class ProxyGenerator
	{
		public Type CreateProxy(Type target, Type interfaceType)
		{	
			ProxyGenerator proxyGen = new ProxyGenerator();
			
			DynamicType proxyType = new DynamicType();
			proxyType.DefineType(target,interfaceType);
			
			foreach(var method in target.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
			        {
                        if (interfaceType.GetMethod(method.Name, method.GetParameters().Select(x => x.ParameterType).ToArray()) != null)
                        {
                            proxyType.OpenMethodForCreation(method);
                            proxyType.CreateMethod();
                        }
			        }
			        
			 Type proxy = proxyType.CreateType();
			 return proxy;
		}
	}
}
