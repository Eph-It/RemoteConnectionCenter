using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMRDP
{
    public class RDPSettings
    {
        RegistryKey _appPrefs;
        RegistryKey _userAppPrefs;
        public RDPSettings()
        {
            try
            {
                _appPrefs = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\CMRDP", true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not open registry key in HKLM - Could be a permissions issue");
            }
            _userAppPrefs = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\CMRDP", true);
            if (_appPrefs != null || _userAppPrefs != null)
            {
                if (String.IsNullOrEmpty(AdminGroups) && String.IsNullOrEmpty(AdminUsers))
                {
                    Configured = false;
                }
                else
                {
                    Configured = true;
                }

            }
        }
        private string GetPref(string name)
        {
            if (_appPrefs != null)
            {
                try
                {
                    var result = _appPrefs.GetValue(name);
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error getting registry value from HKLM but key exists. Make sure the IIS app pool account has read permissions to this key if you want to use it. Falling back to HKCU.");
                }
            }
            if (_userAppPrefs != null)
            {
                var result = _userAppPrefs.GetValue(name);
                if (result != null)
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
            if (_appPrefs == null)
            {
                try
                {
                    RegistryKey newKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
                    _appPrefs = newKey.CreateSubKey("CMRDP");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error creating registry key in HKLM - falling back to HKCU");
                }
            }
            if (_appPrefs != null)
            {
                try
                {
                    _appPrefs.SetValue(name, value);
                    return;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error setting registry value in HKLM\\SOFTWARE but CMRDP key exists - Will fall back to HKCU to set the value.");
                }
            }
            if (_userAppPrefs == null)
            {
                try
                {
                    RegistryKey newKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                    _userAppPrefs = newKey.CreateSubKey("CMRDP");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error creating registry key in HKCU.");
                }
            }
            if (_userAppPrefs != null)
            {
                try
                {
                    _userAppPrefs.SetValue(name, value);
                    return;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error setting registry value in HKCU\\SOFTWARE but CMRDP key exists");
                }
            }
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

                foreach (var i in values)
                {
                    returnList.Add(i.Trim());
                }
            }
            return returnList.ToArray();
        }
        public bool Configured { get; set; }
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
        public string TextBoxUsers
        {
            get => GetPref("TextBoxUsers");
            set => SetPref("TextBoxUsers", value);
        }
        public string TextBoxGroups
        {
            get => GetPref("TextBoxGroups");
            set => SetPref("TextBoxGroups", value);
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
        public string[] TextBoxUsersArray => GetArrayValues(TextBoxUsers);
        public string[] TextBoxGroupsArray => GetArrayValues(TextBoxGroups);
    }

}
