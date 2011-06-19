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
using CSpec.Shell.Display;
using System.Reflection;

namespace CSpec.Shell.Execution
{
    /// <summary>
    /// Console runner class, used as a primary application
    /// start point.
    /// </summary>
    /// <remarks>
    /// The console is defined as a set of actions and commands
    /// each command (like for e.g RunTests) can have some external commands
    /// like -a: that defines chatty output to a console.
    /// 
    /// so each action can reuse existing commands, or redefine the existing commands
    /// in it's own context.
    /// </remarks>
    public class ConsoleRunner
    {
        private ConsoleFormatter formatter;
        private CSpecConsole console;
        private List<IAction> actions;

        /// <summary>
        /// Initializes the Console runner, and sets it's formaters
        /// to default values.
        /// </summary>
        public ConsoleRunner()
        {
            formatter = new ConsoleFormatter();
            console = new CSpecConsole(formatter);

            InitializeActions();
        }

        //TODO: add action parsing code.

        /// <summary>
        /// Runs specified Action on the console.
        /// </summary>
        /// <param name="args">console arguments</param>
        public void Run(string[] args)
        {
            if (args == null || args.Length == 0)
                DisplayHelp();
            else
            {
                try
                {
                    string actionKey = args[0];

                    if (actionKey.ToUpper() == "help".ToUpper())
                        DisplayHelp();

                    foreach (var action in actions)
                    {
                        //Match actions
                        if (action.Key.ToUpper() == actionKey.ToUpper())
                        {
                            action.Execute(args);
                        }
                    }
                }
                catch (Exception ex)
                {
                    console.WriteError(ex.Message);
                }
            }
        }

        /// <summary>
        /// Displays the help contents on the console.
        /// </summary>
        private void DisplayHelp()
        {
            foreach (var action in actions)
            {
                action.DisplayInterface();
            }
        }

        /// <summary>
        /// Adds actions provided by the console app.
        /// </summary>
        private void InitializeActions()
        {
            actions = new List<IAction>();
            actions.Add(new TestRunnerAction());
        }
    }
}
