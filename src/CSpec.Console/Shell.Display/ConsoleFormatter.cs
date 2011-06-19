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

namespace CSpec.Shell.Display
{
    /// <summary>
    /// Formats colors and style of displaying test information.
    /// </summary>
    public class ConsoleFormatter
    {
        /// <summary>
        /// Default Constructior when invoked uses default formatting
        /// </summary>
        public ConsoleFormatter()
        {
            DescriptionColor = new ColorFormatter();
            DescriptionColor.Background = ConsoleColor.Black;
            DescriptionColor.Foreground = ConsoleColor.White;

            NameColor = new ColorFormatter();
            NameColor.Background = ConsoleColor.DarkGreen;
            NameColor.Foreground = ConsoleColor.White;

            SuccessResultColor = new ColorFormatter();
            SuccessResultColor.Background = ConsoleColor.Black;
            SuccessResultColor.Foreground = ConsoleColor.Green;

            ErrorResultColor = new ColorFormatter();
            ErrorResultColor.Background = ConsoleColor.Black;
            ErrorResultColor.Foreground = ConsoleColor.Red;

            InfoColor = new ColorFormatter();
            InfoColor.Background = ConsoleColor.Black;
            InfoColor.Foreground = ConsoleColor.Gray;

            Separator = "----------------------";
        }

        /// <summary>
        /// Gets/Sets Colors for standard writeLine operation
        /// </summary>
        public ColorFormatter InfoColor { get; set; }
        /// <summary>
        /// Gets/Sets Colors for operation description
        /// </summary>
        public ColorFormatter DescriptionColor { get; set; }
        /// <summary>
        /// Gets/Sets Colors for operation name
        /// </summary>
        public ColorFormatter NameColor { get; set; }
        /// <summary>
        /// Gets/Sets Colors for result of the successful operation
        /// </summary>
        public ColorFormatter SuccessResultColor { get; set; }
        /// <summary>
        /// Gets/Sets Colors for result of the error operation
        /// </summary>
        public ColorFormatter ErrorResultColor { get; set; }
        /// <summary>
        /// Gets/Sets The separator string.
        /// </summary>
        public string Separator { get; set; }
    }
}
