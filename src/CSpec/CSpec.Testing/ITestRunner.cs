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
using System.Reflection;

namespace CSpec.Testing
{
    /// <summary>
    /// Test runner interface that can be used to create 
    /// new better test runners.
    /// </summary>
    public interface ITestRunner
    {
        /// <summary>
        /// Gets operations passed
        /// </summary>
        int Passed { get;}
        /// <summary>
        /// Gets operations failed
        /// </summary>
        int Failed { get; }

        /// <summary>
        /// Runs tests on an assembly, it is advides that all 
        /// descriptor types will be kept in seperate assembly.
        /// </summary>
        /// <param name="asm"></param>
        void RunTestOnAssembly(Assembly asm);

        /// <summary>
        /// Runs a single batch of tests on the designated object.
        /// </summary>
        /// <param name="objType">Type of the object to be tested</param>
        void RunTestOnType(Type objType);

        /// <summary>
        /// Event that's invoked before operation
        /// CSpecTestItem will be containing a name in this state.
        /// </summary>
        event Action<CSpecTestItem> BeforeOperation;
        /// <summary>
        /// Event that's invoked with operation
        /// CSpecTestItem will be containing a name and description in this state.
        /// </summary>
        event Action<CSpecTestItem> Operation;
        /// <summary>
        /// Event that's invoked after operation
        /// CSpecTestItem will be containing a name, description and result in this state.
        /// </summary>
        event Action<CSpecTestItem> AfterOperation;
    }
}
