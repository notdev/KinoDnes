using Hangfire.Dashboard;

namespace KinoDnesApi
{
    public class HangfireAuthorization : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // TODO
            return true;
        }
    }
}