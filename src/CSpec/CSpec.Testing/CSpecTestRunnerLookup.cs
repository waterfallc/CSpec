using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSpec.Testing
{
    //TODO: This is a rly nasty workaround, find a better solution

    /// <summary>
    /// This is a lookup class that can be checked by extensions to read
    /// data from and do actions based on the collected data.
    /// </summary>
    public class CSpecTestRunnerLookup
    {
        /// <summary>
        /// Gets the current described object, that is
        /// an object that is beeing tested.
        /// </summary>
        public static object CurrentDescribedObject { get; protected set; }
    }
}
