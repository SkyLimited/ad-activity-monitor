using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Configuration.Install;
using System.Reflection;

namespace LogViewerService
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main(string[] args)
        {
            if (System.Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {

                    case "--install":
                        try
                        {
                            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        }
                        catch (Exception) { }
                        break;
                    case "--uninstall":
                        try
                        {
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        }
                        catch (Exception) { }
                        break;
                }
            }
            else
            {
                ServiceBase.Run(new LogViewerService());
            }
        }
    }
}
