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
using CSpec.Testing;
using CSpec.Extensions;
using CSpec.Shell.Execution;
using CSpec.Shell.Execution.Commands;

namespace Test.Spec
{
    /// <summary>
    /// Spec class that defines the behavior of TestRunner in Cspec.
    /// </summary>
    public class TestRunnerActionSpec : CSpecFacade<TestRunnerAction>
    {
        public TestRunnerActionSpec()
            : base(new TestRunnerAction())
        {
            Should(@have => ObjSpec.Commands);
            Should(@have => ObjSpec.Commands.Count != 0);
            //describe if command list contains a command.
            ObjSpec.Commands.Should(@rcontain => new RunnerAllCommand());
            ObjSpec.Commands.Count.Should(@be => 1);
        }

        /// <summary>
        /// Describes the execute operation, with null argument 
        /// </summary>
        DescribeAll describe_execute_null =
            (@it, @do) =>
            {
                @it("Calls the run command with null params, it should throw  exception, that's catched in the ConsoleRunner");
                try
                {
                    @do.Execute(null);
                }
                catch (Exception ex)
                {
                    //lets catch it ourselfs and print it.
                    @it(ex.Message);
                }
            };

        /// <summary>
        /// Describes the execute operation with empty array of strings.
        /// </summary>
        DescribeAll describe_execute_empty =
            (@it, @do) =>
            {
                @it("Calls the run command with no params, it should throw  exception, that's catched in the ConsoleRunner");
                try
                {
                    @do.Execute(new string[]{});
                }
                catch (Exception ex)
                {
                    @it(ex.Message);
                }
            };


    }
}
