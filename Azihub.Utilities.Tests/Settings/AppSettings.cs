using Azihub.Utilities.Base.Settings;
using Azihub.Utilities.Base.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azihub.Utilities.Tests.Settings
{
    public class AppSettings : AppSettingsBase
    {
        public AppSettings(IGlobalSettings global, IWorkerSettings worker) : base(global, worker)
        {
        }
    }
}
