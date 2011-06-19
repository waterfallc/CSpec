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
using CSpec.Extensions;
using CSpec.Exceptions;
using CSpec.Testing;
using CSpec;
using Test.Common;
using System.Reflection;
using System.IO;

namespace Test.Spec
{
    /// <summary>
    /// Spec class that tests the Runner, of CSpec.
    /// </summary>
	public class CSpecTestRunnerSpec : CSpecFacade<ITestRunner>
	{
        public CSpecTestRunnerSpec()
            : base(new CSpecTestRunner())
        {
            Should(@have => ObjSpec.Failed == 0);
            Should(@have => ObjSpec.Passed == 0);

            CreateOperations();
        }

        //MS aparently has a bug in the code.
        //without this nothing will work and the DESCRIBE ALL delegate will throw exception.
        //In mono it works perfect.
        private Action<string> dummy;
        private MyClassSpec myClassSpec;
        private DescribeAll run_on_type;

        /// <summary>
        /// Create a new instance of our test class befor each operation.
        /// </summary>
		protected override void BeforeOperation()
		{
            myClassSpec = new MyClassSpec();
		}

        /// <summary>
        /// Creates operations, that describe behaviour of this class.
        /// </summary>
        /// <remarks>
        /// This method is used for cases when we want o put external private objects
        /// in the operation itself, having operations initialized on the field level
        /// passing external types would be impossible.
        /// </remarks>
        private void CreateOperations()
        {
            run_on_type =
               (@it, @do) =>
               {
                   @it("Runs all of the operations contained in a type");
                   @do.RunTestOnType(myClassSpec.GetType());
                   @do.Passed.Should(@be => 3);
                   @do.Failed.Should(@be => 0);

               };
        }

	}
}
