/*
 * Created by SharpDevelop.
 * User: Pierwszy
 * Date: 2011-04-28
 * Time: 11:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;

namespace CSpec.Proxy
{
	/// <summary>
	/// Description of IDynamicTypeBuilder.
	/// </summary>
	public interface IDynamicTypeBuilder
	{
		/// <summary>
        /// Defines the type.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns></returns>
        IDynamicTypeBuilder DefineType(Type sourceType, Type interfaceType);
        /// <summary>
        /// Opens the method for creation.
        /// </summary>
        /// <returns></returns>
        IDynamicTypeBuilder OpenMethodForCreation(MethodInfo method);
        /// <summary>
        /// Creates the method.
        /// </summary>
        /// <returns></returns>
        IDynamicTypeBuilder CreateMethod();
        /// <summary>
        /// Creates the type, that's need to be Activated in order to be used as an object.
        /// </summary>
        /// <returns></returns>
        Type CreateType();
	}
}
