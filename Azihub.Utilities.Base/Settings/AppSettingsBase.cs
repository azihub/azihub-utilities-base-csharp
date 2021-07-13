using Azihub.Utilities.Base.Settings.Interfaces;

namespace Azihub.Utilities.Base.Settings
{
    public abstract class AppSettingsBase : IAppSettings
    {
        public AppSettingsBase(IGlobalSettings global, IWorkerSettings worker)
        {
            Global = global;
            Worker = worker;
        }
        public IGlobalSettings Global { get; set; }
        public IWorkerSettings Worker { get; set; }
    }
}
