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
using Shell.Resources;

namespace CSpec.Shell.Execution.Commands
{
    /// <summary>
    /// Displays detailed information about the operations.
    /// </summary>
    public class RunnerAllCommand : ICommand
    {
        public string Key
        {
            get { return "-a"; }
        }

        public string Description
        {
            get { return CommandMessages.RunnerAll; }
        }

        /// <summary>
        /// Executes the command.
        /// Hooks all runner events to the console to display all possible information
        /// from the runner.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="actionParams"></param>
        public void Execute(CSpecConsole console, params object[] actionParams)
        {
            if (actionParams[0] is ITestRunner)
            {
                ITestRunner runner = (ITestRunner)actionParams[0];

                runner.BeforeOperation += x => console.WriteTestName(x);
                runner.Operation += x => console.WriteTestDescription(x);
                runner.AfterOperation += x => console.WriteTestResult(x);
            }       
        }
    }
}
