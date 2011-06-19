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
using System.Reflection;
using CSpec.Shell.Display;
using Shell.Resources;

namespace CSpec.Shell.Execution
{
    /// <summary>
    /// Action that runs operations conatined in an asembly
    /// Additional Attributes (Commands):
    ///  -a AllCommand - displays detailed information about operation curently performed.
    /// </summary>
    public class TestRunnerAction : IAction
    {
        private ITestRunner runner;
        private ConsoleFormatter formatter;
        private CSpecConsole console; 

        /// <summary>
        /// Default constructor that initializes the console and it's formatters, as
        /// well as a list of commands used.
        /// </summary>
        public TestRunnerAction()
        {
            runner = new CSpecTestRunner();
            formatter = new ConsoleFormatter();
            console = new CSpecConsole(formatter);

            Commands = new List<ICommand>() { new Commands.RunnerAllCommand() };
        }

        /// <summary>
        /// Gets List of commands used by this action
        /// </summary>
        public List<ICommand> Commands { get; private set; }

        /// <summary>
        /// Executes the action with specified console parameters
        /// </summary>
        /// <param name="args">application args</param>
        public void Execute(string[] args)
        {
            if (args[1] == null || args[1] == string.Empty)
            {
                console.WriteError("No arguments");
                DisplayInterface();
            }
            else
            {
                //skip the first two params, and go to the external attributes
                for (int i = 2; i < args.Length; i++)
                {
                    var command = Commands.Where(x => x.Key.ToUpper() == args[i].ToUpper()).FirstOrDefault();

                    if (command != null)
                    {
                        command.Execute(console, runner);
                    }
                }

                //Load the assembly and start testing baby!
                Assembly asm = Assembly.LoadFrom(args[1]);
                runner.RunTestOnAssembly(asm);

                console.WriteInfo("\n Tests passed: " + runner.Passed + "/" + (runner.Failed + runner.Passed));
            }
        }

        /// <summary>
        /// Displays the help interface to the console.
        /// </summary>
        public void DisplayInterface()
        {
            console.WriteInfo(this.Key + " : " + this.Description);
            foreach (var cmd in Commands)
                console.WriteInfo(cmd.Key + " : " + cmd.Description);
        }

        public string Key
        {
            get { return "runner"; }
        }

        public string Description
        {
            get { return ActionMessages.TestRunner; }
        }
    }
}
