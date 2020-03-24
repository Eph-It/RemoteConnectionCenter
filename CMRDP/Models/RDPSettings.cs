using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Win32;

namespace CMRDP.Models
{
    public class RDPSettings
    {
        RegistryKey _appPrefs;
        public RDPSettings()
        {
            RegistryKey appPrefs = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\CMRDP", true);
            if(appPrefs == null)
            {
                appPrefs = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\CMRDP", true);
            }
            _appPrefs = appPrefs;
        }
        private string GetPref(string name)
        {
            if(_appPrefs != null)
            {
                var result = _appPrefs.GetValue(name);
                if(result != null)
                {
                    return result.ToString();
                }
                
            }
            return null;
        }
        private string[] GetPrefs(string name)
        {
            if (_appPrefs != null)
            {
                return (string[])_appPrefs.GetValue(name);
            }
            return null;
        }
        private void SetPref(string name, string value)
        {
            if (String.IsNullOrEmpty(value)) { return; }
            if(_appPrefs == null)
            {
                RegistryKey newKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                _appPrefs = newKey.CreateSubKey("CMRDP");
            }
            _appPrefs.SetValue(name, value);
        }
        private void SetPref(string name, string[] value)
        {
            if (_appPrefs == null)
            {
                RegistryKey newKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                _appPrefs = newKey.CreateSubKey("CMRDP");
            }
            _appPrefs.SetValue(name, value, RegistryValueKind.MultiString);
        }
        private string[] GetArrayValues(string value)
        {
            List<string> returnList = new List<string>();
            if (!String.IsNullOrEmpty(value))
            {
                var values = value.Split(',');
                
                foreach(var i in values)
                {
                    returnList.Add(i.Trim());
                }
            }
            return returnList.ToArray();
        }
        public bool Configured
        {
            get => _appPrefs != null;
        }
        [Display(Name = "Database Server")]
        public string DBServer
        {
            get => GetPref("DBServer");
            set => SetPref("DBServer", value);
        }
        [Display(Name = "Database Name")]
        public string DBName
        {
            get => GetPref("DBName");
            set => SetPref("DBName", value);
        }
        [Display(Name = "Site Code")]
        public string SiteCode
        {
            get => GetPref("SiteCode");
            set => SetPref("SiteCode", value);
        }
        [Display(Name = "WMI Server")]
        public string WMIServer
        {
            get => GetPref("WMIServer");
            set => SetPref("WMIServer", value);
        }
        [Display(Name = "Admin Users (comma separated list)")]
        public string AdminUsers
        {
            get => GetPref("AdminUsers");
            set => SetPref("AdminUsers", value);
        }
        [Display(Name = "Admin Groups (comma separated list)")]
        public string AdminGroups
        {
            get => GetPref("AdminGroups");
            set => SetPref("AdminGroups", value);
        }
        [Display(Name = "RDP Groups (comma separated list)")]
        public string AllowedGroups
        {
            get => GetPref("AllowedGroups");
            set => SetPref("AllowedGroups", value);
        }
        [Display(Name = "RDP Users (comma separated list)")]
        public string AllowedUsers
        {
            get => GetPref("AllowedUsers");
            set => SetPref("AllowedUsers", value);
        }
        public string CMScriptName
        {
            get => GetPref("CMScriptName");
            set => SetPref("CMScriptName", value);
        }
        public string[] AllowedUsersArray => GetArrayValues(AllowedUsers);
        public string[] AllowedGroupsArray => GetArrayValues(AllowedGroups);
        public string[] AdminGroupsArray => GetArrayValues(AdminGroups);
        public string[] AdminUsersArray => GetArrayValues(AdminUsers);
    }
}