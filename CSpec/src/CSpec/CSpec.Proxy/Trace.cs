using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSpec.Proxy
{
    public class Trace
    {
        public List<string> MethodCalls { get; set; } 
        public object Target { get; set; }
        public Exception Ex { get; set; }

        public Trace()
        {
            MethodCalls = new List<string>();
        }
    }
}
