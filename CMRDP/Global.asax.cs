using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Serilog;

namespace CMRDP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var logFile =  Path.Combine(System.Environment.GetEnvironmentVariable("TEMP"), "CMRDP.log");
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFile, rollingInterval: RollingInterval.Day).CreateLogger();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
