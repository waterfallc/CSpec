using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using CSpec.Shell.Display;
using CSpec.Shell.Execution;

namespace CSpec.Shell
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine("CSpec Test Runner: \n");

            //my stupid tests!
			//args = new string[3];
			//args[0] = "runner";
            //args[1] = @"C:\Users\BaxServices\Desktop\CSpecSVN\CSpec\bin\Debug\Test\CSpec.TestsSpec.dll"; //path to Spec
			//args[2] = "-a";
                          
            ConsoleRunner runner = new ConsoleRunner();
            runner.Run(args);
		
		}
	}
}

