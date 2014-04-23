using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;
using System.ServiceProcess;

namespace LogViewerService
{
    [RunInstaller(true)]
    class LogViewerServiceInstaller : Installer
    {

        public LogViewerServiceInstaller()
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            //# Service Account Information
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;
            
            //# Service Information
            serviceInstaller.DisplayName = "SkyLimited Log Viewer Service";
            serviceInstaller.Description = "Сервис анализа логов (C) SkyLimited LLC 2013";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            // This must be identical to the WindowsService.ServiceBase name
            // set in the constructor of WindowsService.cs
            serviceInstaller.ServiceName = "SkyLimited Log Viewer Service";

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }

        }

    }

