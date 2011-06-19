#region Licence
// Copyright (c) 2011 BAX Services Bartosz Adamczewski
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Reflection;
using System.Linq;

namespace CSpec.Proxy
{
	/// <summary>
	/// Generates a proxy for the Spec(ed) object that
    /// implements an InterfaceType.
	/// </summary>
	public class ProxyGenerator
	{
        /// <summary>
        /// Creates a proxy for a target type that implelents
        /// an interface type.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public Type CreateProxy(Type target, Type interfaceType)
        {
            ProxyGenerator proxyGen = new ProxyGenerator();
            DynamicType proxyType = new DynamicType();

            proxyType.DefineType(target, interfaceType);

            foreach (var method in target.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
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
