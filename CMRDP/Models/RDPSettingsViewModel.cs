using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMRDP.Models
{
    public class RDPSettingsViewModel
    {
        [Display(Name = "Database Server")]
        [Required]
        public string DBServer { get; set; }
        [Display(Name = "Database Name")]
        [Required]
        public string DBName { get; set; }
        [Display(Name = "Site Code")]
        [Required]
        public string SiteCode { get; set; }
        [Display(Name = "WMI Server")]
        [Required]
        public string WMIServer { get; set; }
        [Display(Name = "Admin Users (comma separated list)")]
        public string AdminUsers { get; set; }
        [Display(Name = "Admin Groups (comma separated list)")]
        public string AdminGroups { get; set; }
        [Display(Name = "RDP Groups (comma separated list)")]
        public string AllowedGroups { get; set; }
        [Display(Name = "RDP Users (comma separated list)")]
        public string AllowedUsers { get; set; }
        [Display(Name = "ConfigMgr WOL Script Name")]
        [Required]
        public string CMScriptName { get; set; }
        [Display(Name = "Text Box Computer Name Users (comma separated list)")]
        public string UsersByComputerName { get; set; }
        [Display(Name = "Text Box Computer Name Groups (comma separated list)")]
        public string GroupsByComputerName { get; set; }
    }
}