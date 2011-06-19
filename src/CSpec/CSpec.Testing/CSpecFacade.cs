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
using System.Linq.Expressions;
using CSpec.Extensions;

namespace CSpec.Testing
{
    /// <summary>
    /// CSpec Testing Facade.
    /// This a base testing class every (non static) object that want's to be tested
    /// should inherit from this class.
    /// 
    /// "Should" methods are for testing the initial state of newly created
    /// object, for other methods a descibe deletate should be used with
    /// extension methods.
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    public class CSpecFacade<TClass> : CSpecFacadeBase
    {


        /// <summary>
        /// Gets the described object.
        /// </summary>
        protected TClass ObjSpec { get; private set; }

		/// <summary>
		/// base constructor, you need to call it to initialize
		/// the facade. 
		/// </summary>
		/// <param name="objSpec">
        /// Class that is being speced (MyClass)
		///  <see cref="TClass"/>
		/// </param>
        protected CSpecFacade(TClass objSpec)
        {
            var inter = objSpec.GetType().GetInterfaces().FirstOrDefault();
            if (inter != null)
            {
                Proxy.ProxyGenerator gen = new CSpec.Proxy.ProxyGenerator();
                var obj = Activator.CreateInstance(gen.CreateProxy(objSpec.GetType(), inter));
                ObjSpec = (TClass)obj;
                CSpec.Testing.CSpecTestRunnerLookup.CurrentDescribedObject = ObjSpec; 

            }
            else
                InitializeFacade(objSpec);
        }

        /// <summary>
        /// Default constructor, use it in case you want to 
        /// call InitializeFacade in your facade.
        /// 
        /// To see what InitializeFacade is for check the comments for the method.
        /// </summary>
        protected CSpecFacade()
        { }

        /// <summary>
        /// Initializes a facade
        /// The reason why children can acces this class is that sometimes
        /// building a constructor of objSpec can be problematic and very very complex
        /// and require hundreds of objects to finaly get the proper params and their states
        /// so this method is available for such case.
        /// Why not make setter of ObjSpec public?
        /// because this would not be noticible that this is thing that we need to set in order to
        /// initialize facade and besides by doing it in a method we give it gravity so this is an 
        /// indicator that we do it if we really must.
        /// </summary>
        /// <param name="objSpec"></param>
        protected void InitializeFacade(TClass objSpec)
        {
            this.ObjSpec = objSpec;
        }

        /// <summary>
        /// Used to describe the initial state of the object after it's been created.
        /// </summary>
        /// <typeparam name="N"></typeparam>
        /// <param name="operation"></param>
        /// <returns></returns>
        protected TClass Should<N>(Expression<Func<TClass,N>> operation)
        {  
            return ObjectExtensions.Should(ObjSpec, operation);
        }
		
        /// <summary>
        /// Describes the behavior of the object (it's methods) using the attribte pattern.
        /// </summary>
        /// <param name="objSpec"></param>
        public delegate void Describe(TClass objSpec);
        /// <summary>
        /// Describes the behavior of the object (it's methods) using the fluent pattern.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="objSpec"></param>
        public delegate void DescribeAll(Action<string> description, TClass objSpec);
    }
}
