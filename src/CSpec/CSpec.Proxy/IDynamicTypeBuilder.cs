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
