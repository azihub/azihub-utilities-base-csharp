using System;
using System.Collections.Generic;
using System.Text;

namespace Azihub.Utilities.Tests
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<WorkerSettings>();
        }
    }
}
