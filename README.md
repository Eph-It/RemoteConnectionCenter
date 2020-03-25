# RemoteConnectionCenter

## Introduction

The Remote Connection Center is a site that integrates into Microsoft Configuration Manager to easily enable self-service remoting! With everything going on in the world, I wanted to make a simple site end users could go to and RDP into their recently used machines.

How does that help? If the user has a desktop and a laptop, this will enable them to bring the laptop home and log into their desktop and continue working! You can also set up a VPN connection and have users set up the VPN on their home machines so they can RDP right into their work machines.

In the future, I'll support editing the .RDP file that's generated, which will hopefully let admins set up RDP Gateways through this.

If you have any other ideas for how this can help enable remote work, please open an issue to let me know!

## How It Works

This works by giving a user a list of devices they recently logged into. This list is generated based on:

1) Primary Devices
2) Console Usage Data
3) Recently Used Apps (based on explorer.exe)

After the user selects a device, we look to see if the device is online by using the Fast Channel information. If it is online, the user is given the RDP file to connect. If it is not online, we look for 3 online peers in the same subnet. If at least 1 peer is found, a CM script is triggered to tell the online peers to send a wake up packet. Then the user is given a .rdp file to connect to the device and told to wait a little.

## Install

1) Download the latest release
2) Run the MSI installer following the prompts.
   1) This installer will create an IIS site and IIS app pool
   2) The Installer will install the site to HTTP port 5500 - Please change this to HTTPS with a trusted certificate in your organization.
3) Go to http://```<ServerName>```:```5500``` - On first run, you will be redirected to /Admin.
4) Fill out the Admin page, filling in your ConfigMgr site information. These settings, when saved, will be saved on the web server in HKLM\SOFTWARE\CMRDP
5) Copy the script on the admin page (at the bottom) to Configuration Manager and then put the name of the script in the portal.
6) Use the portal!

If you encounter any errors, logs are located at ```IISUserTempFolder```\CMRDP```<Date>```.log.

If you find an error you cannot solve, feel free to log an issue and I'll look right into it!

Thank you for using this program!
