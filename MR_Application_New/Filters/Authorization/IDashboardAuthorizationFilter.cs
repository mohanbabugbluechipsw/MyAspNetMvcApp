using Hangfire.Dashboard;


namespace MR_Application_New.Filters.Authorization
{


    public class HangfireAuthorization : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true; // allow all requests (local or remote)
        }
    }

}
