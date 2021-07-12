using System;
using System.Collections.Generic;
using System.Text;

namespace Azihub.Utilities.Base.Tools
{
    using Azihub.Utilities.Base.Extensions.String;
    using Azihub.Utilities.Base.Tools.DotEnvExceptions;
    using System;
    using System.IO;
    using System.Reflection;

    public static class DotEnv
    {

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            IDictionary<string, string> variable = DotEnvFile.LoadFile(filePath, true);
            DotEnvFile.InjectIntoEnvironment(EnvironmentVariableTarget.Process, variable);
        }

        public static T Load<T>() where T : new()
        {
            
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(T).GetProperties();

            T container = Activator.CreateInstance<T>();

            var variables = Environment.GetEnvironmentVariables();

            foreach (PropertyInfo property  in propertyInfos)
            {
                string key = property.Name.AddSpaceToPascalCase().ToConstantCase();
                if (variables.Contains(key))
                {
                    var type = property.PropertyType;
                    //property.SetValue(container, (property.PropertyType) variables[key]);
                    property.SetValue(container, Convert.ChangeType(variables[key], property.PropertyType));
                }
                else
                    throw new EnvironmentVariableIsNotSetException(key);
            }

            return container;
        }
    }
}
