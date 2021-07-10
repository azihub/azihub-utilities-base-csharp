using System;
using System.Collections.Generic;
using System.Text;

namespace Azihub.Utilities.Base.Tools
{
    using System;
    using System.IO;

    public static class DotEnv
    {
        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            IDictionary<string, string> variable = DotEnvFile.LoadFile(filePath, true);
            DotEnvFile.InjectIntoEnvironment(EnvironmentVariableTarget.Process, variable);

        }
    }
}
