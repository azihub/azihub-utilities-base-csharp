using Azihub.Utilities.Base.Tools;
using System.IO;
using Xunit.Abstractions;

namespace Azihub.Utilities.Base.Tests
{
    public abstract class TestBase
    {
        protected static string EnvPath => ".." + Path.DirectorySeparatorChar + ".env";
        protected ITestOutputHelper Output { get; }

        public TestBase(ITestOutputHelper outputHelper)
        {
            Output = outputHelper;
        }

        protected void LoadEnv()
        {
            DotEnv.Load(EnvPath);
        }
    }
}
