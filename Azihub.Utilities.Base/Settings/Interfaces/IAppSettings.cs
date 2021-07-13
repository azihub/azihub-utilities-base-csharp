﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Azihub.Utilities.Base.Settings.Interfaces
{
    interface IAppSettings
    {
        IGlobalSettings Global { get; set; }
        IWorkerSettings Worker { get; set; }
    }
}
