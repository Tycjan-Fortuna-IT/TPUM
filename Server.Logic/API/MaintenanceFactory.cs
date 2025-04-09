using Server.Logic.Implementation;

namespace Server.Logic.API
{
    public abstract class MaintenanceFactory
    {
        public static IMaintenanceTracker CreateTracker()
        {
            return new MaintenanceTracker();
        }

        public static IMaintenanceReporter CreateReporter()
        {
            return new MaintenanceReporter();
        }
    }
}
