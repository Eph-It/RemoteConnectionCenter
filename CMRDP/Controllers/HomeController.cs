using CMRDP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMRDP.Repository;
using System.Text;
using System.IO;
using Serilog;

namespace CMRDP.Controllers
{
    public class HomeController : Controller
    {
        RDPSettings _settings;
        public HomeController()
        {
            _settings = new RDPSettings();
        }
        public ActionResult Index()
        {
            var userDevices = new UserDevices(User.Identity.Name);
            if (!_settings.Configured)
            {
                return Redirect("Admin");
            }
            var vm = new IndexViewModel();
            vm.ComputerList = new SelectList(
                        userDevices.GetUserDevices()
                        .Select(p => p.DeviceDisplayName)
                        .ToArray()
                    );
            return View(vm);
        }
        [HttpPost]
        public string WakeDevice(string computerName)
        {
            Log.Information($"User {User.Identity.Name} is requesting to RDP to {computerName}");
            var userDevices = new UserDevices(User.Identity.Name);
            foreach(var d in userDevices.GetUserDevices())
            {
                if (d.DeviceDisplayName.Equals(computerName))
                {
                    try
                    {
                        Log.Information("Found device - checking if it needs to wake up");
                        return userDevices.WOL(d.DeviceResourceId);
                    }
                    catch(Exception ex)
                    {
                        Log.Error(ex, "Error waking up device");
                        return "Error trying to turn on device. You can still run the Default.rdp file to try and connect.";
                    }
                }
            }
            Log.Information("Device not found! Returning");
            return string.Empty;
        }
        public ActionResult GetRDPFile(string computerName)
        {
            var userDevices = new UserDevices(User.Identity.Name);
            string fullComputerName = "";
            foreach (var d in userDevices.GetUserDevices())
            {
                if (d.DeviceDisplayName.Equals(computerName))
                {
                    fullComputerName = d.DeviceName;
                }
            }

            string filePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".rdp";

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            using (FileStream fs = System.IO.File.Create(filePath))
            {
                Byte[] title = new UTF8Encoding(true).GetBytes("full address:s:" + fullComputerName + "\nprompt for credentials:i:1");
                fs.Write(title, 0, title.Length);
            }
            string contentType = MimeMapping.GetMimeMapping(filePath);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            //dataStream = new MemoryStream(fileBytes);
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "Default.rdp",
                Inline = false,
            };
            System.IO.File.Delete(filePath);
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(fileBytes, contentType);
        }
    }
}