using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMRDP.Models;
using System.Management;

namespace CMRDP.Repository
{
    public class WMIUtility
    {
        private RDPSettings _settings;
        private ManagementScope _scope;
        public WMIUtility()
        {
            _settings = new RDPSettings();
            _scope = new ManagementScope($@"\\{_settings.WMIServer}\root\sms\site_{_settings.SiteCode}");
        }

        private ManagementScope GetScope
        {
            get
            {
                if (!_scope.IsConnected)
                {
                    _scope.Connect();
                }
                return _scope;
            }
        }

        private ManagementClass GetMC(string className)
        {
            return new ManagementClass(GetScope.Path.Path, className, null);
        }

        public ManagementBaseObject InvokeClassMethod(string className, string methodName, Dictionary<string, object> Params = null)
        {
            var cls = GetMC(className);
            ManagementBaseObject inParams = cls.GetMethodParameters(methodName);
            foreach(var key in Params.Keys)
            {
                inParams[key] = Params[key];
            }
            return cls.InvokeMethod(methodName, inParams, null);
        }

    }
}