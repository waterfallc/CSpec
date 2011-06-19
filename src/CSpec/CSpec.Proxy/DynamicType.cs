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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using CSpec.Proxy;

namespace CSpec.Proxy
{
    /*
     * NOTE: Stolen from my other project InternalClasses
     * A lot of things here might not be used right now
     * but might be in the future.
     * 
     * This class is not reausable anymore
     */

    /// <summary>
    /// Allows the consumer to create a dynamic proxy type, using fluent
    /// interface.
    /// </summary>
    public class DynamicType : DynamicBase, IDynamicTypeBuilder
    {
        private TypeBuilder typeBuilder;
        private AssemblyBuilder assemblyBuilder;
        private Type interfaceType;
        private Type objectType;
        private Type resultType;
        private ILGenerator ilGenerator;
        private MethodInfo method;
        private FieldBuilder fieldBuilder;

        /// <summary>
        /// Defines the Type.
        /// </summary>
        /// <param name="sourceType">sourceType.</param>
        /// <param name="interfaceType">interfaceType.</param>
        /// <returns>InternalClasses.Dynamic.IDynamicTypeBuilder.</returns>
        public IDynamicTypeBuilder DefineType(Type sourceType, Type interfaceType)
        {
            this.objectType = sourceType;
            this.interfaceType = interfaceType;


            CreateTypeEntry();
            CreateField("trace", typeof(ProxyTrace));
            CreateConstructors();

            return this;
        }

        /// <summary>
        /// Creates the Type Entry.
        /// </summary>
        private void CreateTypeEntry()
        {
            AssemblyName assemblyName = new AssemblyName("DynamicTypeAssembly");
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            Type[] interfaces = new Type[] { interfaceType };

            if (interfaceType != null)
            {
                typeBuilder = moduleBuilder.DefineType("DynamicType", TypeAttributes.Public, objectType, interfaces);
                
            }
        }

        /// <summary>
        /// Creates the Constructors.
        /// </summary>
        private void CreateConstructors()
        {
            ConstructorBuilder constructorBuilder = null;

            foreach (var constructor in objectType.GetConstructors())
            {
                constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructor.GetParameters().Select(x => (x.ParameterType)).ToArray());

                ilGenerator = constructorBuilder.GetILGenerator();

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Newobj, typeof(ProxyTrace).GetConstructor(new Type[] {}));
                ilGenerator.Emit(OpCodes.Stfld, fieldBuilder);

                ilGenerator.Emit(OpCodes.Ldarg_0);
                
                ParameterInfo[] parametersInfo = constructor.GetParameters();

                for (int i = 0; i < parametersInfo.Length; i++)
                {
                    if (parametersInfo[i].GetType().IsByRef)
                    {
                        ilGenerator.Emit(OpCodes.Ldarga_S, i + 1);
                    }
                    else
                        ilGenerator.Emit(OpCodes.Ldarg_S, i + 1);
                }
            }
        }

        /// <summary>
        /// Opens the Method For Creation.
        /// </summary>
        /// <param name="method">method.</param>
        /// <returns>InternalClasses.Dynamic.IDynamicTypeBuilder.</returns>
        public IDynamicTypeBuilder OpenMethodForCreation(MethodInfo method)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(method.Name, method.Attributes, method.CallingConvention | CallingConventions.HasThis, method.ReturnType, method.GetParameters().Select(p => p.ParameterType).ToArray());
            ilGenerator = builder.GetILGenerator();
            this.method = method;
            return this;
        }

        /// <summary>
        /// Creates an public field of the ProxyType
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private void CreateField(string name, Type type)
        {
            fieldBuilder = typeBuilder.DefineField(name, type, FieldAttributes.Public);
        }

        /// <summary>
        /// Creates the Method.
        /// </summary>
        /// <returns>InternalClasses.Dynamic.IDynamicTypeBuilder.</returns>
        public IDynamicTypeBuilder CreateMethod()
        {
            ParameterInfo[] parameters = method.GetParameters();

            foreach (var param in parameters)
            {
                if (param.ParameterType.IsByRef)
                {
                    ilGenerator.DeclareLocal(param.ParameterType.GetElementType());
                }
                else
                {
                    ilGenerator.DeclareLocal(param.ParameterType);
                }
            }

            //TODO: Refactor!
            //This method needs to be changes as it's hard to read
            //the solution is to split it up into few method that represent chunks
            //of the solution.
            //right now we have anonymous namespaces to the rescue!
            //I havent settled on a final look of the ProxyTrace class thus
            //it's gointg to stay as is for a while

            ilGenerator.BeginExceptionBlock();

            //Load the field in the proxy.
            //and set it
            {
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.EmitCall(OpCodes.Callvirt, typeof(ProxyTrace).GetProperty("Target").GetSetMethod(), null);
            }

            //Track the called method
            //it boils down to MethodBase.GetCurrentMethod
            //then add that to a list in the ProxyTrace
            {
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                ilGenerator.EmitCall(OpCodes.Callvirt, typeof(ProxyTrace).GetProperty("MethodCalls").GetGetMethod(), null);

                LocalBuilder proxyMethodLocal = ilGenerator.DeclareLocal(typeof(ProxyMethod));

                //Call a default constructor on ProxyMethod
                ilGenerator.Emit(OpCodes.Newobj, typeof(ProxyMethod).GetConstructors()[0]);
                //
                ilGenerator.Emit(OpCodes.Stloc, proxyMethodLocal.LocalIndex);
                ilGenerator.Emit(OpCodes.Ldloc, proxyMethodLocal.LocalIndex);
                ilGenerator.Emit(OpCodes.Ldnull);
                ilGenerator.Emit(OpCodes.Callvirt, typeof(ProxyMethod).GetProperty("Ex").GetSetMethod());
                ilGenerator.Emit(OpCodes.Ldloc, proxyMethodLocal.LocalIndex);

                ilGenerator.EmitCall(OpCodes.Call, typeof(System.Reflection.MethodBase).GetMethod("GetCurrentMethod"), null);
                ilGenerator.EmitCall(OpCodes.Callvirt, typeof(System.Reflection.MemberInfo).GetProperty("Name").GetGetMethod(), null);
                ilGenerator.EmitCall(OpCodes.Callvirt, typeof(ProxyMethod).GetProperty("MethodName").GetSetMethod(), null);
                ilGenerator.Emit(OpCodes.Ldloc, proxyMethodLocal.LocalIndex);
                ilGenerator.EmitCall(OpCodes.Callvirt, typeof(List<ProxyMethod>).GetMethod("Add"), null);
            }

            //Load *this
            ilGenerator.Emit(OpCodes.Ldarg_0);

            for (int i = 0; i < parameters.Length; i++)
                ilGenerator.Emit(OpCodes.Ldarg, i + 1);

            //Call the oryginal method
            ilGenerator.EmitCall(OpCodes.Call, method, null);
            LocalBuilder local = null;

            LocalBuilder exLocal = ilGenerator.DeclareLocal(typeof(Exception));

            Label label = new Label();
            label = ilGenerator.DefineLabel();

            if (method.ReturnType != typeof(void))
            {
                local = ilGenerator.DeclareLocal(method.ReturnType);
                ilGenerator.Emit(OpCodes.Stloc, local.LocalIndex);
                ilGenerator.Emit(OpCodes.Ldloc, local.LocalIndex);
            }

            //Leave the try block

            ilGenerator.Emit(OpCodes.Leave, label);

            ilGenerator.BeginCatchBlock(typeof(Exception));

            //If the method call, blows up then don't throw exception
            //but insted assign that to the MethodCalls list in ProxyTrace
            {
                ilGenerator.Emit(OpCodes.Stloc, exLocal.LocalIndex);

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                ilGenerator.EmitCall(OpCodes.Callvirt, typeof(ProxyTrace).GetProperty("MethodCalls").GetGetMethod(), null);

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);

                ilGenerator.EmitCall(OpCodes.Callvirt, typeof(ProxyTrace).GetProperty("MethodCalls").GetGetMethod(), null);
                ilGenerator.EmitCall(OpCodes.Callvirt, typeof(List<ProxyMethod>).GetProperty("Count").GetGetMethod(), null);
                ilGenerator.Emit(OpCodes.Ldc_I4_1);
                ilGenerator.Emit(OpCodes.Sub);
                ilGenerator.EmitCall(OpCodes.Callvirt, typeof(List<ProxyMethod>).GetProperty("Item").GetGetMethod(), null);
                ilGenerator.Emit(OpCodes.Ldloc, exLocal.LocalIndex);
                ilGenerator.Emit(OpCodes.Callvirt, typeof(ProxyMethod).GetProperty("Ex").GetSetMethod());
            }

            ilGenerator.EndExceptionBlock();

            ilGenerator.MarkLabel(label);

            if (method.ReturnType != typeof(void))
            {
                ilGenerator.Emit(OpCodes.Ldloc, local.LocalIndex);
            }

            ilGenerator.Emit(OpCodes.Ret);

            return this;
        }

        /// <summary>
        /// Creates the ProxyType.
        /// </summary>
        /// <returns>System.Type.</returns>
        public Type CreateType()
        {       
            resultType = typeBuilder.CreateType();
            
            return resultType;
        }
    }
}
