using System;
using System.Collections.Generic;
using System.Text;

namespace Azihub.Utilities.Base.Tools.DotEnvExceptions
{
    /// <summary>
    /// When a matching CONSTANTCASE of Property is not found within Environment variables.
    /// </summary>
    public class EnvironmentVariableIsNotSetException : Exception
    {
        public EnvironmentVariableIsNotSetException(string property) : 
            base($"Property of {property} is not set in Environment variables") { }
    }
}
