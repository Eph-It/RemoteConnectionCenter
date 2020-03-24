using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMRDP.Models
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RDPAuthorization : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var rdpSettings = new RDPSettings();
            if(!rdpSettings.Configured)
            {
                return true;
            }

            // First check admins as they are authorized everywhere
            foreach (string g in rdpSettings.AdminGroupsArray)
            {
                if (httpContext.User.IsInRole(g))
                {
                    return true;
                }
            }
            foreach(string use in rdpSettings.AdminUsersArray)
            {
                if(httpContext.User.Identity.Name.Equals(use, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            string controller = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            if (!controller.Equals("Admin"))
            {
                foreach (string g in rdpSettings.AllowedGroupsArray)
                {
                    if (httpContext.User.IsInRole(g))
                    {
                        return true;
                    }
                }
                foreach (string use in rdpSettings.AllowedUsersArray)
                {
                    if (httpContext.User.Identity.Name.Equals(use, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}