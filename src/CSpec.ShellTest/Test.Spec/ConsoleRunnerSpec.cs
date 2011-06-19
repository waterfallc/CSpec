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
using CSpec.Shell.Display;
using CSpec.Shell.Execution;
using CSpec.Extensions;
using System.Reflection;
using System.IO;

namespace Test.Spec
{
    /// <summary>
    /// Space class that defines the behavior of console runner.
    /// </summary>
    public class ConsoleRunnerSpec : CSpecFacade<ConsoleRunner>
    {
        public ConsoleRunnerSpec()
            : base(new ConsoleRunner())
        {
        }

        //MS aparently has a bug in the code.
        //without this nothing will work and the DESCRIBE ALL delegate will throw exception.
        //In mono it works perfect.
        private Action<string> dummy;

        /*
         * As we cannot test this, we just do pragmatic testing, and check for unexpected uncatched exceptions.
         */

        /// <summary>
        /// Describes the run operation.
        /// </summary>
        DescribeAll describe_run =
            (@it, @do) =>
            {
                @it("Runs the console with arguments, for null args it should write help");
                @do.Run(null);
            };

        /// <summary>
        /// Describes the run operation but with 'help' argument.
        /// </summary>
        DescribeAll describe_run_help =
            (@it, @do) =>
            {
                @it("Runs the console with arguments, for help it should write help");
                @do.Run(new string[] { "help" });
            };

        /// <summary>
        /// Describes the assembly run operation,
        /// that will run all tests on a given assembly.
        /// </summary>
        DescribeAll describe_run_assembly_all =
            (@it, @do) =>
            {
                @it("Runs the console with runner arguments, it should write out full verbose test results for -a");
                @do.Run(new string[] { "runner", Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + "\\CSpec.CSpecTest.dll", "-a" });
            };

        /// <summary>
        /// Describes the assembly bad operation
        /// that will fail due to a wrong path given.
        /// </summary>
        DescribeAll describe_run_assembly_bad_path =
            (@it, @do) =>
            {
                @it("Runs the console with runner arguments, it should fail for non abosolute path, but with a proper message");
                @do.Run(new string[] { "runner", "CSpec.CSpecTest.dll", "-a" });
                Console.ForegroundColor.Should(@be => ConsoleColor.Gray);
            };
    }
}
