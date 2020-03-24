using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMRDP.Models
{
    public class RDPDevice
    {
        public string DeviceDisplayName { get; set; }
        public string DeviceName { get; set; }
        public int DeviceResourceId { get; set; }
        public string Ipv4Address { get; set; }
        public string FQDN { get; set; }
        public string MACAddress { get; set; }
    }
}