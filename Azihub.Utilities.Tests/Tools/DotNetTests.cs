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
    }
}
