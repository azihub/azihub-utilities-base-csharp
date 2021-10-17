using Azihub.Utilities.Tests.DotEnvTests.Samples;
using Microsoft.Extensions.DependencyInjection;

namespace Azihub.Utilities.Tests
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            // experimenting, not properly used yet.
            services.AddSingleton<WorkerSettings>();
        }
    }
}
