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

namespace CSpec.Shell.Display
{
    /// <summary>
    /// Used to display formated test results, and console information.
    /// </summary>
    public class CSpecConsole
    {
        private ConsoleFormatter formatter = null;

        /// <summary>
        /// Initializes the CSpecConsole
        /// </summary>
        /// <param name="formatter">formater</param>
        public CSpecConsole(ConsoleFormatter formatter)
        {
            this.formatter = formatter;
        }

        /// <summary>
        /// Writes the operation name.
        /// </summary>
        /// <param name="testItem"></param>
        public void WriteTestName(Testing.CSpecTestItem testItem)
        {
            WriteLine(formatter.NameColor, testItem.Name, true);
        }

        /// <summary>
        /// Writes the operation description.
        /// </summary>
        /// <param name="testItem"></param>
        public void WriteTestDescription(Testing.CSpecTestItem testItem)
        {
            WriteLine(formatter.DescriptionColor, testItem.Description, true);
        }

        /// <summary>
        /// Writes the operation result.
        /// </summary>
        /// <param name="testItem"></param>
        public void WriteTestResult(Testing.CSpecTestItem testItem)
        {
            if (testItem.TestSucceed)
                WriteLine(formatter.SuccessResultColor, testItem.Results, true);
            else
                WriteLine(formatter.ErrorResultColor, testItem.Results, true);
        }

        /// <summary>
        /// Writes an information to the console.
        /// </summary>
        /// <param name="value"></param>
        public void WriteInfo(string value)
        {
            WriteLine(formatter.InfoColor, value, false);
        }

        /// <summary>
        /// Writes error information to the console.
        /// </summary>
        /// <param name="value"></param>
        public void WriteError(string value)
        {
            WriteLine(formatter.ErrorResultColor, value, false);
        }

        /// <summary>
        /// Writes a test line to the console using the specified ColorFormater
        /// </summary>
        /// <param name="colorFormatter"></param>
        /// <param name="value"></param>
        private void WriteLine(ColorFormatter colorFormatter, string value, bool useSeparator)
        {
            //Temp values.
            var background = Console.BackgroundColor;
            var foreground = Console.ForegroundColor;

            Console.BackgroundColor = colorFormatter.Background;
            Console.ForegroundColor = colorFormatter.Foreground;

            if (formatter.Separator != null && useSeparator)
                Console.WriteLine(formatter.Separator);
            Console.WriteLine(value);

            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
        }
    }
}
