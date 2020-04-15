using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMRDP.Models
{
    public class IndexViewModel
    {
        public SelectList ComputerList { get; set; }
        public bool GetsTextBox
        {
            get
            {
                var rdpSettings = new RDPSettings();
                foreach (string g in rdpSettings.AdminGroupsArray)
                {
                    if (HttpContext.Current.User.IsInRole(g))
                    {
                        return true;
                    }
                }
                foreach (string use in rdpSettings.AdminUsersArray)
                {
                    if (HttpContext.Current.User.Identity.Name.Equals(use, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                foreach (string g in rdpSettings.TextBoxGroupsArray)
                {
                    if (HttpContext.Current.User.IsInRole(g))
                    {
                        return true;
                    }
                }
                foreach (string use in rdpSettings.TextBoxUsersArray)
                {
                    if (HttpContext.Current.User.Identity.Name.Equals(use, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public bool GetsDropDown
        {
            get
            {
                var rdpSettings = new RDPSettings();
                foreach (string g in rdpSettings.AdminGroupsArray)
                {
                    if (HttpContext.Current.User.IsInRole(g))
                    {
                        return true;
                    }
                }
                foreach (string use in rdpSettings.AdminUsersArray)
                {
                    if (HttpContext.Current.User.Identity.Name.Equals(use, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                foreach (string g in rdpSettings.AllowedGroupsArray)
                {
                    if (HttpContext.Current.User.IsInRole(g))
                    {
                        return true;
                    }
                }
                foreach (string use in rdpSettings.AllowedUsersArray)
                {
                    if (HttpContext.Current.User.Identity.Name.Equals(use, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}