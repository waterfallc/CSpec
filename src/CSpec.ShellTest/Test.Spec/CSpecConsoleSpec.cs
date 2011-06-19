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
using CSpec.Extensions;
using System.IO;

namespace Test.Spec
{
    /// <summary>
    /// Spec class that defines the behavior of Console of CSpec.
    /// </summary>
    public class CSpecConsoleSpec : CSpecFacade<CSpecConsole>
    {
        private ConsoleFormatter formatter;
        private const string dummyMessage = "Hello World";

        /// <summary>
        /// Initializes a default constructor.
        /// </summary>
        /// <remarks>
        /// In this context is hard to use the parmetrized contructor ( : base(....) )
        /// thus we use the second option,
        /// we initialize the constructor ourselfs and then we call 
        /// the method from base class InitializeFacade and pass our Spec(ed) class.
        /// </remarks>
        public CSpecConsoleSpec()
        {
            formatter = new ConsoleFormatter();
            CSpecConsole console = new CSpecConsole(formatter);
            InitializeFacade(console);

            CreateOperations();
        }

        /*
         * As we cannot test this, we just do pragmatic testing, and check for unexpected uncatched exceptions.
         * And test if the color is defaulted on the console after each write.
         */

        private DescribeAll describe_write_info_custom;

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
            /// <summary>
            /// Describes the write to console operation, with cutom color scheme.
            /// </summary>
            describe_write_info_custom =
            (@it, @do) =>
            {
                formatter.InfoColor.Foreground = ConsoleColor.Green;
                @it("Writes a info message to the console with custom color green");
                @it("The color should be then reset to its default state");
                @do.WriteInfo(dummyMessage);
                Console.ForegroundColor.Should(@be => ConsoleColor.Gray);
            };
        }

        /// <summary>
        /// Describes the write to console operation.
        /// </summary>
        DescribeAll describe_write_info =
            (@it, @do) =>
            {
                @it("Writes a info message to the console, that should be in a white (default) color");
                @it("The color should be then reset to its default state");
                @do.WriteInfo(dummyMessage);
                Console.ForegroundColor.Should(@be => ConsoleColor.Gray);
            };
    }
}
