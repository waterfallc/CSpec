﻿#region Licence
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
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using CSpec.Exceptions;
using System.Collections;
using CSpec.Resources;
using CSpec.Extensions.Tags;

namespace CSpec.Extensions
{
    /// <summary>
    /// Contains extension methods that are object scoped.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Extends evey object with Should methods that is a basic,
        /// validation method.
        /// 
        /// It uses special set of lambda metatags like:
        /// @be: 
        ///  It's used to indicate that a type or return type should be equal to something.
        ///  
        /// @have: 
        ///  It's used on booleans and object.
        ///  When it's called on objects properties it does null checks on those properties.
        ///  When called on booleans it check if some return type or value is true.
        /// 
        /// @not_be: 
        ///  negation of @be tag.
        /// 
        /// @contain:
        ///  It's used on collections and arrays to check for a specified item.
        ///  
        /// @rcontain:
        ///  It's used on collections and arrays to check for a specified item but using reflection matching
        ///  instead of reference.
        /// 
        /// @raise: [Class must implement Interface]
        ///  It's used on methods to instrut that a certain set of parameter data
        ///  should throw a specified exception.
        ///  
        /// @not_raise: [Class must implement Interface]
        ///  It's used on methods to instrut that a certain set of parameter data
        ///  should not throw a specified exception.
        ///  
        /// To enable strong typing use Tags namespace for strong typed methods.
        /// </summary>
        /// <example>
        /// 
        /// // Object Extension example
        /// 
        /// obj.Sum(1,1).Should(@be => 2);
        /// </example>
        /// <example>
        /// 
        /// // Interface Extension example
        /// // Note: obj should implement some interface
        /// // in order for this extension to work
        /// 
        /// obj.Sum(1,1).Should(@raise => typeof(MyException));
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="N"></typeparam>
        /// <param name="obj"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static T Should<T, N>(this T obj, Expression<Func<T, N>> operation)
        {
			//parse the parameter names and make operations.
            switch (operation.Parameters[0].Name)
            {
                case "have": return TagExtensions.Have(obj, operation.Compile()(obj));
                case "be": return TagExtensions.Be(obj, operation.Compile()(obj));
                case "not_be": return TagExtensions.NotBe(obj, operation.Compile()(obj));
                case "contain": return TagExtensions.Contain(obj, operation.Compile()(obj));
                case "rcontain": return TagExtensions.ReflectionContain(obj, operation.Compile()(obj));

                case "raise": return TagExtensions.Raise(obj, operation.Compile()(obj));
                case "not_raise": return TagExtensions.NotRaise(obj, operation.Compile()(obj));
            }

            throw new CSpecException("Not a keyword");
        }

        /// <summary>
        /// Extends evey object with Should methods that is a basic,
        /// validation method.
        /// 
        /// This is a two parameter version of should operator, that's used for metatags
        /// conaining two arguments:
        /// 
        /// @be_close: 
        ///  It's used on numbers and represents the logical rule actual == expected +/- delta
        ///  
        /// To enable strong typing use Tags namespace for strong typed methods.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="N"></typeparam>
        /// <param name="obj"></param>
        /// <param name="operation"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static T Should<T, N>(this T obj, Expression<Func<T, N>> operation, N operand)
        {
            //parse the parameter names and make operations.
            switch (operation.Parameters[0].Name)
            {
                case "be_close": return TagExtensions.BeClose(obj, operation.Compile()(obj), operand);
            }

            throw new CSpecException("Not a keyword");
        }
    }
}
