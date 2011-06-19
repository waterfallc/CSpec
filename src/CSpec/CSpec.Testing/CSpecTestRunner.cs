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
using System.Reflection;
using CSpec.Proxy;
using System.IO;

namespace CSpec.Testing
{

    //NOTE: Eliminate corner casses, and do assembly and type validation
    //NOTE: Provide rich information about errors.

    /// <summary>
    /// Validates objects, that are described by CSpec.
    /// </summary>
    public class CSpecTestRunner : ITestRunner
    {
        /// <summary>
        /// Gets operations passed
        /// </summary>
		public int Passed {get; private set;}
        /// <summary>
        /// Gets operations failed
        /// </summary>
		public int Failed {get; private set;}

        /// <summary>
        /// Event that's invoked before operation
        /// CSpecTestItem will be containing a name in this state.
        /// </summary>
        public event Action<CSpecTestItem> BeforeOperation;
        /// <summary>
        /// Event that's invoked with operation
        /// CSpecTestItem will be containing a name and description in this state.
        /// </summary>
        public event Action<CSpecTestItem> Operation;
        /// <summary>
        /// Event that's invoked after operation
        /// CSpecTestItem will be containing a name, description and result in this state.
        /// </summary>
        public event Action<CSpecTestItem> AfterOperation;


        /// <summary>
        /// Runs tests on an assembly, it is advides that all 
        /// descriptor types will be kept in seperate assembly.
        /// </summary>
        /// <param name="asm"></param>
        public void RunTestOnAssembly(Assembly asm)
        {
            foreach (var cspecType in asm.GetTypes())
            {
                RunTestOnType(cspecType);
            }
        }

        /// <summary>
        /// Runs a single batch of tests on the designated object.
        /// </summary>
        /// <param name="objType">Type of the object to be tested</param>
        public void RunTestOnType(Type objType)
        {
            //Get the object and call the constructor.
            object obj = null;
            CSpecTestItem testItem = null;

            if (objType.IsInterface)
                return;

            try
            {
                obj = Activator.CreateInstance(objType);
                testItem = new CSpecTestItem();
            }
            catch (Exception ex)
            {
                HandleRunnerException(ex, testItem, null);
            }

            Type facadeType = objType.BaseType;

            FieldInfo[] fields = objType.GetFields(System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance);

            //hook a dynamic delegate to do @it description writes
            Action<string> description = x => { testItem.Description = x; if(Operation != null) Operation(testItem); };

            //check if we have the CSpecFacade
            if (facadeType.Name == typeof(CSpecFacade<>).Name)
            {
                //Get ObjSpec property from the facade.
                var baseObject = facadeType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)[0];
                var describedObj = baseObject.GetValue(obj, null);

                RunOperation(testItem, fields, description, obj, describedObj);
            }
            else if (facadeType.Name == typeof(CSpecFacadeStatic).Name)
            {
                RunOperation(testItem, fields, description, obj, null);
            }
        }

        /// <summary>
        /// Runs the operations for a type, that inherit one of the Facade classes.
        /// </summary>
        /// <param name="testItem">CspecTestItem that describes the status of the running test</param>
        /// <param name="fields">class fields</param>
        /// <param name="description">the @it operation</param>
        /// <param name="obj">object that represents the spec</param>
        /// <param name="describedObject">the object that's beeing described</param>
        private void RunOperation(CSpecTestItem testItem, FieldInfo[] fields, Action<string> description, object obj, object describedObject)
        {
            MethodInfo beforeOp = null;
            MethodInfo afterOp = null;
            ProxyTrace trace = null;
            Type objType = obj.GetType();

            //Now call the operation methods.
            foreach (var field in fields.Where(x => x.FieldType.Name == "DescribeAll" 
                || x.FieldType.Name == "Describe"
                ))
            {
                Delegate testRunner = (Delegate)field.GetValue(obj);
                testItem.Name = field.Name;

                if (BeforeOperation != null) 
                    BeforeOperation(testItem);

                beforeOp = objType.GetMethod("BeforeOperation", BindingFlags.NonPublic | BindingFlags.Instance);
                afterOp = objType.GetMethod("AfterOperation", BindingFlags.NonPublic | BindingFlags.Instance);

                if (beforeOp != null)
                    beforeOp.Invoke(obj, null);

                try
                {
                    if (describedObject != null && describedObject.GetType().GetInterfaces().Length != 0)
                    {
                        trace = (ProxyTrace)describedObject.GetType().GetField("trace").GetValue(describedObject);
                        trace = new ProxyTrace();
                        describedObject.GetType().GetField("trace").SetValue(describedObject, trace);

                        //put to lookup
                        //NOTE: when doing multithreaded runners the entire section of this code
                        //should be locked by monitor
                        //NOTE: Moved to CSpecFacade constructor, it has much more sence there
                        //CurrentDescribedObject = describedObject;
                    }

                    if (field.FieldType.Name == "DescribeAll")
                    {
                        if (describedObject != null)
                            testRunner.DynamicInvoke(description, describedObject);
                        else
                            testRunner.DynamicInvoke(description);
                    }
                    else
                    {
                        ItAttribute it = (ItAttribute)field.GetCustomAttributes(typeof(ItAttribute), false)[0];
                        Console.WriteLine(it.Description);
                        testRunner.DynamicInvoke(null);
                    }

                    testItem.Results = "Test Passed!";
                    testItem.TestSucceed = true;

                    if (AfterOperation != null) 
                        AfterOperation(testItem);

                    Passed++;
                }
                catch (System.Exception ex)
                {
                    HandleRunnerException(ex, testItem, trace);
                }

                if (afterOp != null)
                    afterOp.Invoke(obj, null);
            }
        }

        /// <summary>
        /// Handles the exceptions thrown by the testing method.
        /// </summary>
        private void HandleRunnerException(Exception ex, CSpecTestItem testItem, ProxyTrace trace)
        {
            Failed++;

            if (ex.InnerException != null)
                testItem.Results = "Test Failed: " + ex.InnerException.Message;
            else
                testItem.Results = "CSpec Exception: " + ex.Message;

            if (trace != null)
            {
                StringBuilder traceErrorBuilder = new StringBuilder();
                traceErrorBuilder.AppendLine("\n Trace: ");

                //foreach (var item in trace.MethodCalls)
                //    traceErrorBuilder.AppendLine(item);

                testItem.Results += traceErrorBuilder.ToString();
            }

            testItem.TestSucceed = false;

            if(AfterOperation != null)
                AfterOperation(testItem);
        }
    }
}
