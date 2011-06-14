using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSpec.Proxy
{
    [Serializable]
    public class DynamicException : Exception
    {
        public DynamicException() { }
        public DynamicException(string message) : base(message) { }
        public DynamicException(string message, Exception inner) : base(message, inner) { }
        protected DynamicException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
