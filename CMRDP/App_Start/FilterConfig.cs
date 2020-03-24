using System.Web;
using System.Web.Mvc;
using CMRDP.Models;

namespace CMRDP
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            GlobalFilters.Filters.Add(new RDPAuthorization());
        }
    }
}
