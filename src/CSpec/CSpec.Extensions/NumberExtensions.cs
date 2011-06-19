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
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace CSpec.Extensions
{
    /// <summary>
    /// Contains extension methods that are number scoped.
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Invokes the dynamic operation X times where X is the number that is extended.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="operation">Action that does not take any arguments</param>
        /// <returns></returns>
        public static int Times(this int obj, Action operation)
        {
            for (int i = 0; i < obj; i++)
            {
                operation();
            }

            return obj;
        }
    }
}
