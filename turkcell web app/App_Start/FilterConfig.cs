using System.Web;
using System.Web.Mvc;

namespace turkcell_web_app.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // GlobalFilters.Filters.Add(new AuthorizeAttribute()); NOT WORKING
        }
    }
}
