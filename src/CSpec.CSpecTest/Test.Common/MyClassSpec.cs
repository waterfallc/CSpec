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
using System.Text;
using CSpec.Testing;
using CSpec.Extensions;

namespace Test.Common
{
    public class MyClassSpec : CSpecFacade<MyClass>
    {
        public MyClassSpec()
            : base(new MyClass())
        {
            Should(@have => ObjSpec.Total == 0);
        }

        DescribeAll describe_sum =
            (@it, @do) =>
        	{
                @it("Sums two numbers, given 1 and 1 it should be 2");
                @do.Sum(1, 1).Should(@be => 2);
            };

        DescribeAll describe_sub =
            (@it, @do) =>
            {
                @it("Substracts two numbers, given 1 and 3 it should be -2");
                @do.Sub(1, 3).Should(@be => -2);
            };

        DescribeAll describe_total =
            (@it, @do) =>
            {
                @do = new MyClass();
                @it("Describes total ammount, given sum of 1,2 and 3 total should have apropriate value 6");
                @do.Sum(1, 2);
                @it("We summed 1 and 2 now we need to sum 3 and 0");
                @do.Sum(0, 3);
                @do.Total.Should(@be => 6);
            };
    }
}
