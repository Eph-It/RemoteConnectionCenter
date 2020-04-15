using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using CMRDP.Models;
using Serilog;

namespace CMRDP.Repository
{
    public class UserDevices
    {
        private string _user;
        private string _domain;
        public UserDevices(string userName)
        {
            _user = userName.Split('\\')[1];
            _domain = userName.Split('\\')[0];
            if(_user == "Ryan2")
            {
                _user = "Administrator";
                _domain = "Home";
            }
        }

        private string GetDeviceList = @"Select Name0 FROM v_R_System";

        private string GetResourceIds = @"Select ResourceId FROM v_R_System where Name0 = @CompName";

        private string GetUsersPrimaryDeviceQuery = @"
            SELECT sy.Name0 AS 'DeviceName'
              ,sy.Full_Domain_Name0 AS 'DeviceFullDomainName'
              ,sy.ResourceID AS 'DeviceResourceId'
              ,u.User_Name0 AS 'UserName'
	          ,u.Full_Domain_Name0 AS 'UserFullDomainName'
	          ,u.Windows_NT_Domain0 AS 'UserWindowsNTDomain'
              FROM [v_UsersPrimaryMachines]
                    upm
            JOIN v_R_System sy ON sy.ResourceID = upm.MachineID
            JOIN v_R_User u ON u.ResourceID = upm.UserResourceID
            WHERE u.User_Name0 = @username";

        private string GetUsersConsoleDeviceQuery = @"
            WITH CTE AS(
	            SELECT * FROM v_R_User u
	            WHERE u.User_Name0 = @username
            )

            SELECT sy.Name0 AS 'DeviceName'
              ,sy.Full_Domain_Name0 AS 'DeviceFullDomainName'
              ,sy.ResourceID AS 'DeviceResourceId'
              ,u.User_Name0 AS 'UserName'
	          ,u.Full_Domain_Name0 AS 'UserFullDomainName'
	          ,u.Windows_NT_Domain0 AS 'UserWindowsNTDomain'
            FROM [v_GS_SYSTEM_CONSOLE_USER] cu
              JOIN CTE u ON cu.SystemConsoleUser0 = u.Unique_User_Name0
              JOIN v_R_System sy ON sy.ResourceID = cu.ResourceID
            ORDER BY cu.LastConsoleUse0 DESC
        ";

        private string GetUsersLastUsedExplorerQuery = @"
            WITH CTE AS(
	            SELECT * FROM v_R_User u
	            WHERE u.User_Name0 = @username
            )

            SELECT sy.Name0 AS 'DeviceName'
              ,sy.Full_Domain_Name0 AS 'DeviceFullDomainName'
              ,sy.ResourceID AS 'DeviceResourceId'
              ,u.User_Name0 AS 'UserName'
	          ,u.Full_Domain_Name0 AS 'UserFullDomainName'
	          ,u.Windows_NT_Domain0 AS 'UserWindowsNTDomain'
            FROM v_GS_CCM_RECENTLY_USED_APPS cu
              JOIN CTE u ON cu.LastUserName0 = u.Unique_User_Name0
              JOIN v_R_System sy ON sy.ResourceID = cu.ResourceID
			WHERE cu.ExplorerFileName0 = 'explorer.exe'
			AND cu.LastUsedTime0 > DATEADD(Month, -3, GETUTCDATE())
            ORDER BY cu.LastUsedTime0 DESC
        ";

        private string GetScriptQuery = "SELECT * FROM [vSMS_Scripts] WHERE ScriptName like @ScriptName";

        private string GetOnlineStatus = "SELECT * FROM BGB_ResStatus WHERE ResourceID = @ResId";

        private string GetOnlinePeers = @"
            WITH CTE AS (
	            SELECT * FROM v_RA_System_IPSubnets WHERE ResourceID = @resourceId
            ),
            CTE2 AS(
            SELECT 
	            bgb.ResourceID
	            ,ips.IP_Subnets0
	            ,ROW_NUMBER() OVER (PARTITION BY ips.IP_Subnets0 ORDER BY LastOnlineTime DESC) AS rn
            FROM BGB_ResStatus bgb
            JOIN v_RA_System_IPSubnets ips ON ips.ResourceID = bgb.ResourceID
            WHERE ips.IP_Subnets0 IN ( SELECT IP_Subnets0 FROM CTE )
            AND bgb.OnlineStatus = 1
            AND bgb.ResourceID != @resourceId
            )
            SELECT * FROM CTE2 WHERE rn < 4
        ";

        private string GetDeviceMacs = "SELECT [MAC_Addresses0] FROM [v_RA_System_MACAddresses] WHERE ResourceID = @resourceId";

        //private string ScriptParams = @"<ScriptContent ScriptGuid='{0}'><ScriptVersion>{1}</ScriptVersion><ScriptType>{2}</ScriptType><ScriptHash ScriptHashAlg='{3}'>{4}</ScriptHash><ScriptParameters><ScriptParameter ParameterGroupGuid="""" ParameterGroupName=""PG_"" ParameterName=""Mac"" ParameterDataType=""System.String"" ParameterVisibility=""0"" ParameterType=""1"" ParameterValue=""{5}""/></ScriptParameters><ParameterGroupHash ParameterHashAlg='SHA256'>86c21158bac3025fb8069be567dfc1e5405aefa95a075c8954c161eb769aa18f</ParameterGroupHash></ScriptContent>";

        private RDPDevice NewDevice(Dictionary<string, object> result, bool isPrimary = false)
        {
            string deviceDisplayName = result["DeviceName"].ToString();
            if (isPrimary)
            {
                deviceDisplayName = deviceDisplayName + " (Primary Device)";
            }
            var tempDevice = new RDPDevice()
            {
                DeviceDisplayName = deviceDisplayName,
                DeviceName = result["DeviceName"].ToString(),
                DeviceResourceId = (int)result["DeviceResourceId"],
                FQDN = $"{result["DeviceName"].ToString()}.{result["DeviceFullDomainName"].ToString()}"
            };
            return tempDevice;
        }

        public int GetResourceId(string computerName)
        {
            Dictionary<string, object> SqlParams = new Dictionary<string, object>()
            {
                { "@CompName", computerName }
            };
            using (var _sql = new SQL())
            {
                var resourceIds = _sql.Invoke(GetResourceIds, SqlParams);
                foreach (var result in resourceIds)
                {
                    return (int)result["ResourceId"];
                }
            }
            return 0;
        }

        public List<string> GetDevices()
        {
            var returnList = new List<string>();

            using(var _sql = new SQL())
            {
                var deviceList = _sql.Invoke(GetDeviceList);
                foreach(var result in deviceList)
                {
                    returnList.Add(result["Name0"].ToString());
                }
            }

            return returnList;
        }

        public List<RDPDevice> GetUserDevices()
        {
            var returnList = new List<RDPDevice>();

            Dictionary<string, object> SqlParams = new Dictionary<string, object>()
            {
                { "@username", _user }
            };
            using(var _sql = new SQL())
            {
                var primaryDevices = _sql.Invoke(GetUsersPrimaryDeviceQuery, SqlParams);
                foreach (var result in primaryDevices)
                {
                    returnList.Add(NewDevice(result, true));
                }
            }

            using (var _sql = new SQL())
            {
                var ConsoleDevices = _sql.Invoke(GetUsersConsoleDeviceQuery, SqlParams);
                foreach (var result in ConsoleDevices)
                {
                    int resourceId = (int)result["DeviceResourceId"];
                    if (returnList.Where(p => p.DeviceResourceId.Equals(resourceId)).FirstOrDefault() == null)
                    {
                        returnList.Add(NewDevice(result));
                    }
                }
            }

            using (var _sql = new SQL())
            {
                var ConsoleDevices = _sql.Invoke(GetUsersLastUsedExplorerQuery, SqlParams);
                foreach (var result in ConsoleDevices)
                {
                    int resourceId = (int)result["DeviceResourceId"];
                    if (returnList.Where(p => p.DeviceResourceId.Equals(resourceId)).FirstOrDefault() == null)
                    {
                        returnList.Add(NewDevice(result));
                    }
                }
            }

            return returnList;
        }

        private bool DeviceOnline(int resourceId)
        {
            bool returnBool = false;
            Dictionary<string, object> SqlParams = new Dictionary<string, object>()
            {
                { "@ResId", resourceId }
            };
            using (var _sql = new SQL())
            {
                var results = _sql.Invoke(GetOnlineStatus, SqlParams);
                foreach (var result in results)
                {
                    if (result["OnlineStatus"].ToString() == "1")
                    {
                        returnBool = true;
                    }
                }
            }

            return returnBool;
        }

        private List<int> GetDevicePeers(int resourceId)
        {
            var returnList = new List<int>();
            Dictionary<string, object> SqlParams = new Dictionary<string, object>()
            {
                { "@resourceId", resourceId }
            };
            using (var _sql = new SQL())
            {
                var results = _sql.Invoke(GetOnlinePeers, SqlParams);
                foreach (var result in results)
                {
                    returnList.Add((int)result["ResourceID"]);
                }
            }
            return returnList;
        }

        private List<string>GetDeviceMACs(int resourceId)
        {
            var returnList = new List<string>();

            Dictionary<string, object> SqlParams = new Dictionary<string, object>()
            {
                { "@resourceId", resourceId }
            };
            using (var _sql = new SQL())
            {
                var results = _sql.Invoke(GetDeviceMacs, SqlParams);
                foreach (var result in results)
                {
                    returnList.Add(result["MAC_Addresses0"].ToString());
                }
            }

            return returnList;
        }

        public string WOL(int resourceId, bool alwaysWake = false)
        {
            string returnMessage = string.Empty;
            var settings = new RDPSettings();
            bool deviceIsOnline = DeviceOnline(resourceId);
            if (deviceIsOnline && alwaysWake == false)
            {
                Log.Information("Device found to be online. Will not wake up.");
                return "Device is online and ready for RDP. Please run the downloaded file called Default.rdp";
            }

            var peerList = GetDevicePeers(resourceId);

            if(peerList.Count == 0)
            {
                Log.Information("Device not online and no peers found.");
                return "Device does not show online and no peers found. You can try running the Default.rdp file but the device probably can't be turned on.";
            }

            var MACList = GetDeviceMACs(resourceId);

            if(MACList.Count == 0)
            {
                Log.Information("MAC Address for device not found. This is required for WOL");
                return "Device does not show online and no MAC address was found. You can try running the Default.rdp file but the device probably can't be turned on.";
            }


            string macs = String.Join(",", MACList.ToArray());

            Dictionary<string, object> scriptInformation = null;

            var SqlParams = new Dictionary<string, object>()
            {
                { "@ScriptName", settings.CMScriptName }
            };

            using (var _sql = new SQL())
            {
                var results = _sql.Invoke(GetScriptQuery, SqlParams);
                foreach (var result in results)
                {
                    scriptInformation = result;
                }
            }

            if(scriptInformation == null)
            {
                Log.Information("Could not find WOL script");
                return "Device does not show online and no script was found to turn it on. You can try running the Default.rdp file but the device probably can't be turned on.";
            }
            string scriptParameters = $"<ScriptParameters><ScriptParameter ParameterGroupGuid=\"\" ParameterGroupName=\"PG_\" ParameterName=\"Macs\" ParameterDataType=\"System.String\" ParameterVisibility=\"0\" ParameterType=\"0\" ParameterValue=\"{macs}\"/></ScriptParameters>";
            string shaHash = "";
            byte[] bytes = Encoding.Unicode.GetBytes(scriptParameters);
            using (SHA256 sHA = new SHA256Cng())
            {
                byte[] a = sHA.ComputeHash(bytes);
                StringBuilder stringBuilder = new StringBuilder();
                byte[] a2 = a;
                foreach (byte b in a2)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }
                shaHash = stringBuilder.ToString();
            }
            string scriptXML = string.Format(CultureInfo.InvariantCulture, "<ScriptContent ScriptGuid='{0}'><ScriptVersion>{1}</ScriptVersion><ScriptType>{2}</ScriptType><ScriptHash ScriptHashAlg='SHA256'>{3}</ScriptHash>{4}<ParameterGroupHash ParameterHashAlg='SHA256'>{5}</ParameterGroupHash></ScriptContent>", scriptInformation["ScriptGuid"].ToString(), scriptInformation["ScriptVersion"].ToString(), scriptInformation["ScriptType"].ToString(), scriptInformation["ScriptHash"].ToString(), scriptParameters, shaHash);
            var scriptXMLBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(scriptXML));
            var wmiUtility = new WMIUtility();
            var wmiDictionary = new Dictionary<string, object>();
            wmiDictionary.Add("Param", scriptXMLBase64);
            uint scriptType = 135;
            wmiDictionary.Add("Type", scriptType);
            List<uint> uintResourceIds = new List<uint>();
            foreach(var r in peerList)
            {
                uintResourceIds.Add(Convert.ToUInt32(r));
            }
            wmiDictionary.Add("TargetResourceIDs", uintResourceIds.ToArray());
            Log.Information("Attempting to run script on these resourceIds {uintResourceIds}", uintResourceIds);
            var i = wmiUtility.InvokeClassMethod("SMS_ClientOperation", "InitiateClientOperationEx", wmiDictionary);
            Log.Information("Successfully sent request to start script");
            return "Device does not show online so we are attempting to turn it on. You can run the Default.rdp file that was downloaded in 2-3 minutes to log into your computer. Thank you.";
            
        }
    }
}