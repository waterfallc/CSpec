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

namespace CSpec.Proxy
{

    /*
     * NOTE: Stolen from my other project InternalClasses
     * A lot of things here might not be used right now
     * but might be in the future.
     */

	/// <summary>
	/// Defines a basic dynamic operations on op codes.
	/// </summary>
	public class DynamicBase
	{
		/// <summary>
        /// Creates the Method Call.
        /// </summary>
        /// <param name="methodInfo">methodInfo.</param>
        /// <param name="ilGenerator">ilGenerator.</param>
        /// <param name="removeRet">removeRet.</param>
        protected static void CreateMethodCall(MethodInfo methodInfo, ILGenerator ilGenerator, bool removeRet)
        {
            ParameterInfo[] parametersInfo = methodInfo.GetParameters();
            Type[] paramTypes = new Type[parametersInfo.Length];

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (parametersInfo[i].ParameterType.IsByRef)
                {
                    paramTypes[i] = parametersInfo[i].ParameterType.GetElementType();
                }
                else
                {
                    paramTypes[i] = parametersInfo[i].ParameterType;
                }
            }

            if (!methodInfo.IsStatic)
            {
                ilGenerator.Emit(OpCodes.Ldarg_0);
            }

            for (int i = 0; i < paramTypes.Length; i++)
            {
                ilGenerator.Emit(OpCodes.Ldarg_1);
                EmitInt(ilGenerator, i);
                ilGenerator.Emit(OpCodes.Ldelem_Ref);
                EmitCast(ilGenerator, paramTypes[i]);
            }

            if (methodInfo.IsStatic)
            {
                ilGenerator.EmitCall(OpCodes.Call, methodInfo, null);
            }
            else
            {
                ilGenerator.EmitCall(OpCodes.Call, methodInfo, null);
            }
            if (methodInfo.ReturnType == typeof(void))
            {
                if (!removeRet)
                {
                    ilGenerator.Emit(OpCodes.Ldnull);
                }
            }
            else
            {
                LocalBuilder local = ilGenerator.DeclareLocal(typeof(object));
                EmitBoxing(ilGenerator, methodInfo.ReturnType);
                ilGenerator.Emit(OpCodes.Stloc_S, local.LocalIndex);
                ilGenerator.Emit(OpCodes.Ldloc_S, local.LocalIndex);
            }

            if (!removeRet)
            {
                ilGenerator.Emit(OpCodes.Ret);
            }
        }

        /// <summary>
        /// Calls the injected method.
        /// </summary>
        /// <param name="ilGenerator">The il generator.</param>
        /// <param name="method">The method.</param>
        /// <param name="hasExternalParameters"></param>
        protected static void CallInjectedMethod(ILGenerator ilGenerator, MethodInfo method, bool hasExternalParameters)
        {
            if (!method.IsStatic)
            {
                ilGenerator.Emit(OpCodes.Ldarg_0);
            }

            ilGenerator.Emit(OpCodes.Ldarg_1);

            if (hasExternalParameters)
            {
                ilGenerator.Emit(OpCodes.Ldloc_0);
            }

            ilGenerator.EmitCall(OpCodes.Call, method, null);
        }

        /// <summary>
        /// Creates the array, with parameters.
        /// </summary>
        /// <remarks>
        /// Parameters must only be base types,
        /// from Il there is no way to preserve objects, state.
        /// </remarks>
        /// <param name="ilGenerator">The il generator.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="localIndex">Index of the local.</param>
        protected static void CreateArray(ILGenerator ilGenerator, object[] parameters, int localIndex)
        {
            ilGenerator.DeclareLocal(typeof(object[]));
            ilGenerator.Emit(OpCodes.Ldc_I4_S, parameters.Length);
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));
            ilGenerator.Emit(OpCodes.Stloc_S, localIndex);

            for (int i = 0; i < parameters.Length; i++)
            {
                ilGenerator.Emit(OpCodes.Ldloc_S, localIndex);
                EmitInt(ilGenerator, i);

                if (parameters[i] is string)
                {
                    ilGenerator.Emit(OpCodes.Ldstr, (string)parameters[i]);
                }
                else if (parameters[i] is int)
                {
                    ilGenerator.Emit(OpCodes.Ldc_I4_S, (int)parameters[i]);
                    ilGenerator.Emit(OpCodes.Box, typeof(int));
                }
                else if (parameters[i] is bool)
                {
                    bool value = (bool)parameters[i];

                    if (!value)
                        ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    else
                        ilGenerator.Emit(OpCodes.Ldc_I4_1);

                    
                    ilGenerator.Emit(OpCodes.Box, typeof(bool));
                }

                ilGenerator.Emit(OpCodes.Stelem_Ref);
            }
        }

        /// <summary>
        /// Gets the parameter types.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        protected static Type[] GetParameterTypes(MethodInfo method)
        {
            List<Type> types = new List<Type>();

            foreach (var parameter in method.GetParameters())
            {
                types.Add(parameter.ParameterType);
            }

            return types.ToArray();
        }

        /// <summary>
        /// Emits the Cast.
        /// </summary>
        /// <param name="ilGenerator">the il Generator.</param>
        /// <param name="type">the type.</param>
        protected static void EmitCast(ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>
        /// Emits the Boxing.
        /// </summary>
        /// <param name="ilGenerator">the il Generator.</param>
        /// <param name="type">the type.</param>
        protected static void EmitBoxing(ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, type);
            }
        }

        /// <summary>
        /// Emits the Int.
        /// </summary>
        /// <param name="ilGenerator">the il Generator.</param>
        /// <param name="value">the value.</param>
        protected static void EmitInt(ILGenerator ilGenerator, int value)
        {
            if (value > -129 && value < 128)
            {
                ilGenerator.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Ldc_I4, value);
            }
        }
	}
}
