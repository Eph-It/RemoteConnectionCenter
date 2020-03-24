﻿using CMRDP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMRDP.Controllers
{
    
    public class AdminController : Controller
    {
        RDPSettings _settings;
        public AdminController()
        {
            _settings = new RDPSettings();
        }
        // GET: Admin
        public ActionResult Index()
        {
            var vm = new RDPSettingsViewModel();
            vm.AdminGroups = _settings.AdminGroups;
            vm.AdminUsers = _settings.AdminUsers;
            vm.AllowedGroups = _settings.AllowedGroups;
            vm.AllowedUsers = _settings.AllowedUsers;
            vm.DBName = _settings.DBName;
            vm.DBServer = _settings.DBServer;
            vm.SiteCode = _settings.SiteCode;
            vm.WMIServer = _settings.WMIServer;
            vm.CMScriptName = _settings.CMScriptName;
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Index(RDPSettingsViewModel rdpSettings)
        {
            _settings.AdminGroups = rdpSettings.AdminGroups;
            _settings.AdminUsers = rdpSettings.AdminUsers;
            _settings.AllowedGroups = rdpSettings.AllowedGroups;
            _settings.AllowedUsers = rdpSettings.AllowedUsers;
            _settings.DBName = rdpSettings.DBName;
            _settings.DBServer = rdpSettings.DBServer;
            _settings.SiteCode = rdpSettings.SiteCode;
            _settings.WMIServer = rdpSettings.WMIServer;
            _settings.CMScriptName = rdpSettings.CMScriptName;

            return RedirectToAction("Index");
        }
    }
}