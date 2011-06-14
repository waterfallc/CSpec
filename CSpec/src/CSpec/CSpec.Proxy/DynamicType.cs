using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using CSpec.Proxy;

namespace CSpec.Proxy
{
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
            CreateField();
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

        private void CreateField()
        {
            fieldBuilder = typeBuilder.DefineField("trace", typeof(Trace), FieldAttributes.Public);
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
                ilGenerator.Emit(OpCodes.Newobj, typeof(Trace).GetConstructor(new Type[] {}));
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

 
        #region IDynamicTypeBuilder Members

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

            ilGenerator.BeginExceptionBlock();

            ilGenerator.Emit(OpCodes.Ldarg_0);

            for (int i = 0; i < parameters.Length; i++)
                ilGenerator.Emit(OpCodes.Ldarg, i + 1);

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

            //TODO: Refactor!
           
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.EmitCall(OpCodes.Callvirt, typeof(Trace).GetProperty("Target").GetSetMethod(), null);

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            ilGenerator.EmitCall(OpCodes.Callvirt, typeof(Trace).GetProperty("MethodCalls").GetGetMethod(), null);
            ilGenerator.EmitCall(OpCodes.Call, typeof(System.Reflection.MethodBase).GetMethod("GetCurrentMethod"), null);
            ilGenerator.EmitCall(OpCodes.Callvirt, typeof(System.Reflection.MemberInfo).GetProperty("Name").GetGetMethod(), null);
            ilGenerator.EmitCall(OpCodes.Callvirt, typeof(List<string>).GetMethod("Add"), null);

            ilGenerator.Emit(OpCodes.Leave, label);

            ilGenerator.BeginCatchBlock(typeof(Exception));

            ilGenerator.Emit(OpCodes.Stloc, exLocal.LocalIndex);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            ilGenerator.Emit(OpCodes.Ldloc, exLocal.LocalIndex);
            ilGenerator.EmitCall(OpCodes.Callvirt, typeof(Trace).GetProperty("Ex").GetSetMethod(), null);


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
        /// Creates the Type.
        /// </summary>
        /// <returns>System.Type.</returns>
        public Type CreateType()
        {
            
            resultType = typeBuilder.CreateType();
            
            return resultType;
        }

        #endregion
    }
}
