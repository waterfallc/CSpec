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
using Test.Common;

namespace Test.Spec
{
    /// <summary>
    /// Spec class that tests operations that
    /// use static methods 
    /// </summary>
    public class CSpecExtensionsSpec : CSpecFacadeStatic
    {
        /*
         * Testing static classes.
         * As static classes have no instance we
         * need to skip the @do keyword
         */
        
        /// <summary>
        /// Describes the ObjectTag Should.
        /// </summary>
        DescribeAll describe_should =
            (@it) =>
            {
                MyClass cls = new MyClass();

                @it("Describes the should operation");
                ObjectExtensions.Should(cls, @have => cls.Total);
                ObjectExtensions.Should(cls.Total, @be => 0);
                ObjectExtensions.Should(cls.Sum(1, 1), @be => 2);
                ObjectExtensions.Should(cls.Sub(1, 1), @be => 0);

                ObjectExtensions.Should(cls.Sum(1, 1), @be_close => 2, 0.5);
            };

        /// <summary>
        /// Describes classes that inherit some interfaceType.
        /// </summary>
        DescribeAll describe_should_interface =
            (@it) =>
            {
                //This is just for testing nomally we would create a SpecClass and this would
                //have been created for us.
                CSpec.Proxy.ProxyGenerator generator = new CSpec.Proxy.ProxyGenerator();
                Type proxyType = generator.CreateProxy(typeof(MyPublicClass), typeof(MyPublicInterface));

                MyPublicInterface cls = (MyPublicClass)Activator.CreateInstance(proxyType);
                CSpec.Testing.CSpecTestRunnerLookup.CurrentDescribedObject = cls;
            
                @it("Describes the should operation with public interface ");
                ObjectExtensions.Should(cls.DoDiv(1, 0), @raise => typeof(DivideByZeroException));
                ObjectExtensions.Should(cls.DoDiv(1, 1), @not_raise => typeof(DivideByZeroException));
                ObjectExtensions.Should(cls.DoDiv(2, 0), @raise => typeof(DivideByZeroException));
                ObjectExtensions.Should(cls.DoDiv(3, 0), @raise => typeof(DivideByZeroException));
            };

        /// <summary>
        /// Describes the NumberTag times.
        /// </summary>
        DescribeAll describe_times =
            (@it) =>
            {
                MyClass cls = new MyClass();

                @it("Describes the times operation");
                NumberExtensions.Times(10, () => { cls.Sum(1, 1); });
            };
    }
}
