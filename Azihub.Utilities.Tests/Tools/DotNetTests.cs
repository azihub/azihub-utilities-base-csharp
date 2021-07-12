using Azihub.Utilities.Base.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Azihub.Utilities.Tests.Tools
{
    public class DotNetTests
    {
        [Fact]
        public void TestLoadDotEndTests()
        {
            #region Preparing test data
            string varName1 = "TEST_VAR1";
            string varValue1 = "TEST_VALUE1";
            string varName2 = "TEST_VAR2";
            string varValue2 = "TEST_VALUE2";
            string testEnv = "# COMMENT TEST\n" +
                            $"{varName1}={varValue1}\n"+
                            $"{varName2}={varValue2}\n";

            string tempEnvFilePath = Path.GetTempPath() + ".env";
            File.WriteAllText(tempEnvFilePath, testEnv);
            #endregion

            // Conduct test
            DotEnv.Load(tempEnvFilePath);
            string testVar1 = Environment.GetEnvironmentVariable(varName1);
            string testVar2 = Environment.GetEnvironmentVariable(varName2);

            // evaluate result
            Assert.Equal(varValue1, testVar1);
            Assert.Equal(varValue2, testVar2);
        }

        #region Test Env Vars
        private const string STRING_VALUE   = "TEST";
        private const int    INT_VALUE      = -2147483648;
        private const uint   U_INT_VALUE    = 4294967295;
        private const long   LONG_VALUE     = 9223372036854775807;
        private const ulong  U_LONG_VALUE   = 18446744073709551615;
        #endregion
        [Fact]
        public void GetEnvModelType()
        {
            Environment.SetEnvironmentVariable(nameof(STRING_VALUE), STRING_VALUE);
            Environment.SetEnvironmentVariable(nameof(INT_VALUE)   , INT_VALUE.ToString() );
            Environment.SetEnvironmentVariable(nameof(U_INT_VALUE) , U_INT_VALUE.ToString() );
            Environment.SetEnvironmentVariable(nameof(LONG_VALUE)  , LONG_VALUE.ToString());
            Environment.SetEnvironmentVariable(nameof(U_LONG_VALUE), U_LONG_VALUE.ToString());

            WorkerSettings workerSettings = DotEnv.Load<WorkerSettings>();

            Assert.Equal(STRING_VALUE, workerSettings.StringValue);
            Assert.Equal(INT_VALUE, workerSettings.IntValue);
            Assert.Equal(U_INT_VALUE, workerSettings.UIntValue);
            Assert.Equal(LONG_VALUE, workerSettings.LongValue);
            Assert.Equal(U_LONG_VALUE, workerSettings.ULongValue);
        }
    }
}
