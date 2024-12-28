using System;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;

public static class HardwareInfo
{
    static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    public static string BytesToHuman(Int64 value)
    {
        if (value < 0) { return "-" + BytesToHuman(-value); }
        if (value == 0) { return "0.0 bytes"; }

        int mag = (int)Math.Log(value, 1024);
        decimal adjustedSize = (decimal)value / (1L << (mag * 10));

        return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
    }

    public static string Graphics(bool complete)
    {
        ManagementObjectSearcher objvideo = new ManagementObjectSearcher("select * from Win32_VideoController");
        string info = string.Empty;
        foreach (ManagementObject obj in objvideo.Get())
        {
            if (complete)
            {
                foreach (PropertyData prop in obj.Properties)
                {
                    if (prop.Value != null)
                    {
                        info += prop.Name + ": ";
                        if (prop.IsArray)
                        {
                            Array oArr = prop.Value as Array;
                            info += "[";
                            foreach (var a in oArr)
                                info += a + ", ";
                            info = info.Substring(0, info.Length - 2);
                            info += "]\n";
                        }
                        else
                        {
                            info += prop.Value + "\n";
                        }
                    }
                }
            }
            else
            {
                info += obj["Name"].ToString();
            }
        }
        return info.Trim();
    }

    public static bool IsNvidiaCardInstalled()
    {
        string graphicsInfo = Graphics(false).ToLower();
        if (graphicsInfo.Contains("nvidia") || graphicsInfo.Contains("geforce") || graphicsInfo.Contains("gtx"))
            return true;
        return false;
    }

    public static bool IsAmdCardInstalled()
    {
        string graphicsInfo = Graphics(false).ToLower();
        if (graphicsInfo.Contains("amd") || graphicsInfo.Contains("ati") || graphicsInfo.Contains("radeon"))
            return true;
        return false;
    }



    public static string HardDrives(bool complete)
    {
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        string info = string.Empty;
        foreach (DriveInfo d in allDrives)
        {
            if (complete)
            {
                info += "Drive " + d.Name + "\n";
                info += "Drive type: " + d.DriveType + "\n";
                if (d.IsReady)
                {
                    info += "  Volume label: " + d.VolumeLabel + "\n";
                    info += "  File system: " + d.DriveFormat + "\n";
                    info += "  Available space to current user: " + BytesToHuman(d.AvailableFreeSpace) + "\n";

                    info += "  Total available space: " + BytesToHuman(d.TotalFreeSpace) + "\n";

                    info += "  Total size of drive: " + BytesToHuman(d.TotalSize) + "\n";
                    info += "  Root directory: " + d.RootDirectory + "\n";
                }
                info += "\n";
            }
            else
            {
                info += "[Drive " + d.Name + "][Type: " + d.DriveType + "]";
                if (d.IsReady)
                {
                    info += "[Total: " + BytesToHuman(d.TotalSize) + "]";
                    info += "[Available: " + BytesToHuman(d.AvailableFreeSpace) + "]\n";
                }
            }
        }
        if (complete)
        {
            ManagementObjectSearcher mosDisks = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            foreach (ManagementObject moDisk in mosDisks.Get())
            {
                foreach (PropertyData prop in moDisk.Properties)
                {
                    if (prop.Value != null)
                    {
                        info += prop.Name + ": ";
                        if (prop.IsArray)
                        {
                            Array oArr = prop.Value as Array;

                            info += "[";
                            foreach (var a in oArr)
                                info += a + ", ";
                            info = info.Substring(0, info.Length - 2);
                            info += "]\n";
                        }
                        else
                        {
                            info += prop.Value + "\n";
                        }
                    }
                }
                info += "\n";
            }
        }
        return info.Trim();
    }

    public static string CPU(bool complete)
    {
        ManagementObjectSearcher mpo = new ManagementObjectSearcher("select * from Win32_Processor");
        string info = string.Empty;
        foreach (ManagementObject mo in mpo.Get())
        {
            if (complete)
            {
                foreach (PropertyData prop in mo.Properties)
                {
                    if (prop.Value != null)
                    {
                        info += prop.Name + ": ";
                        if (prop.IsArray)
                        {
                            Array oArr = prop.Value as Array;

                            info += "[";
                            foreach (var a in oArr)
                                info += a + ", ";
                            info = info.Substring(0, info.Length - 2);
                            info += "]\n";
                        }
                        else
                        {
                            info += prop.Value + "\n";
                        }
                    }
                }
            }
            else
            {
                info += mo["Name"].ToString();
            }
        }
        return info.Trim();

    }

    public static string OS(bool complete)
    {
        ManagementObjectSearcher mpo = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
        string info = string.Empty;
        foreach (ManagementObject mo in mpo.Get())
        {
            if (complete)
            {
                foreach (PropertyData prop in mo.Properties)
                {
                    if (prop.Value != null)
                    {
                        info += prop.Name + ": ";
                        if (prop.IsArray)
                        {
                            Array oArr = prop.Value as Array;

                            info += "[";
                            foreach (var a in oArr)
                                info += a + ", ";
                            info = info.Substring(0, info.Length - 2);
                            info += "]\n";
                        }
                        else
                        {
                            info += prop.Value + "\n";
                        }
                    }
                }
            }
            else
            {
                info += mo["Caption"].ToString();
            }
        }
        return info.Trim();

    }

    public static string Network(bool complete)
    {
        IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        string info = string.Empty;
        if (nics == null || nics.Length < 1)
        {
            info += "  No network interfaces found.\n";
            return info.Trim();
        }


        foreach (NetworkInterface adapter in nics)
        {
            IPInterfaceProperties properties = adapter.GetIPProperties();
            info += adapter.Description + "\n";
            info += string.Empty.PadLeft(adapter.Description.Length, '=') + "\n";
            info += "  Interface type: " + adapter.NetworkInterfaceType + "\n";
            info += "  Physical Address: " + adapter.GetPhysicalAddress().ToString() + "\n";
            info += "  Operational status: " + adapter.OperationalStatus + "\n";
            if (complete)
            {
                string versions = "";

                // Create a display string for the supported IP versions.
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    versions = "IPv4";
                }
                if (adapter.Supports(NetworkInterfaceComponent.IPv6))
                {
                    if (versions.Length > 0)
                    {
                        versions += " ";
                    }
                    versions += "IPv6";
                }
                info += "  IP version: " + versions + "\n";
                //ShowIPAddresses(properties);

                // The following information is not useful for loopback adapters.
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }
                info += "  DNS suffix: " + properties.DnsSuffix + "\n";
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
                    info += "  MTU: " + ipv4.Mtu + "\n";
                    if (ipv4.UsesWins)
                    {

                        IPAddressCollection winsServers = properties.WinsServersAddresses;
                        if (winsServers.Count > 0)
                        {
                            info += "  WINS Servers ............................ :";
                            //ShowIPAddresses(label, winsServers);
                        }
                    }
                }

                info += "  DNS enabled: " + properties.IsDnsEnabled + "\n";
                info += "  Dynamically configured DNS: " + properties.IsDynamicDnsEnabled + "\n";
                info += "  Receive Only: " + adapter.IsReceiveOnly + "\n";
                info += "  Multicast: " + adapter.SupportsMulticast + "\n";
            }
            info += "\n";
        }
        return info.Trim();

    }


    public static string SoundCard(bool complete)
    {
        ManagementObjectSearcher mpo = new ManagementObjectSearcher("select * from Win32_SoundDevice");
        string info = string.Empty;
        foreach (ManagementObject mo in mpo.Get())
        {
            if (complete)
            {
                foreach (PropertyData prop in mo.Properties)
                {
                    if (prop.Value != null)
                    {
                        info += prop.Name + ": ";
                        if (prop.IsArray)
                        {
                            Array oArr = prop.Value as Array;

                            info += "[";
                            foreach (var a in oArr)
                                info += a + ", ";
                            info = info.Substring(0, info.Length - 2);
                            info += "]\n";
                        }
                        else
                        {
                            info += prop.Value + "\n";
                        }
                    }
                }
            }
            else
            {
                info += mo["Name"].ToString();
            }
            info += "\n";
        }
        return info.Trim();

    }

    public static string Motherboard(bool complete)
    {
        ManagementObjectSearcher mpo = new ManagementObjectSearcher("select * from Win32_BaseBoard");
        string info = string.Empty;
        foreach (ManagementObject mo in mpo.Get())
        {
            if (complete)
            {
                foreach (PropertyData prop in mo.Properties)
                {
                    if (prop.Value != null)
                    {
                        info += prop.Name + ": ";
                        if (prop.IsArray)
                        {
                            Array oArr = prop.Value as Array;

                            info += "[";
                            foreach (var a in oArr)
                                info += a + ", ";
                            info = info.Substring(0, info.Length - 2);
                            info += "]\n";
                        }
                        else
                        {
                            info += prop.Value + "\n";
                        }
                    }
                }
            }
            else
            {
                info += mo["Product"].ToString();
            }
            info += "\n";
        }
        return info.Trim();

    }

    public static string Bios(bool complete)
    {
        ManagementObjectSearcher mpo = new ManagementObjectSearcher("select * from Win32_BIOS");
        string info = string.Empty;
        foreach (ManagementObject mo in mpo.Get())
        {
            if (complete)
            {
                foreach (PropertyData prop in mo.Properties)
                {
                    if (prop.Value != null)
                    {
                        info += prop.Name + ": ";
                        if (prop.IsArray)
                        {
                            Array oArr = prop.Value as Array;

                            info += "[";
                            foreach (var a in oArr)
                                info += a + ", ";
                            info = info.Substring(0, info.Length - 2);
                            info += "]\n";
                        }
                        else
                        {
                            info += prop.Value + "\n";
                        }
                    }
                }
            }
            else
            {
                info += mo["Manufacturer"].ToString() + " " + mo["Description"].ToString();
            }
            info += "\n";
        }
        return info.Trim();

    }

    public static string Memory(bool complete)
    {

        long totalMemory = 0;
        ManagementObjectSearcher mpo = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
        string info = string.Empty;
        foreach (ManagementObject mo in mpo.Get())
        {
            if (complete)
            {
                foreach (PropertyData prop in mo.Properties)
                {
                    if (prop.Value != null)
                    {
                        info += prop.Name + ": ";
                        if (prop.IsArray)
                        {
                            Array oArr = prop.Value as Array;

                            info += "[";
                            foreach (var a in oArr)
                                info += a + ", ";
                            info = info.Substring(0, info.Length - 2);
                            info += "]\n";
                        }
                        else
                        {
                            info += prop.Value + "\n";
                        }
                    }
                }
            }
            else
            {
                totalMemory += Convert.ToInt64(mo["Capacity"]);
            }
            info += "\n";
        }
        if (!complete)
        {
            return BytesToHuman(totalMemory);
        }
        return info.Trim();

    }

    public static string CDROMDrive(bool complete)
    {
        ManagementObjectSearcher mpo = new ManagementObjectSearcher("select * from Win32_CDROMDrive");
        string info = string.Empty;
        foreach (ManagementObject mo in mpo.Get())
        {
            if (complete)
            {
                foreach (PropertyData prop in mo.Properties)
                {
                    if (prop.Value != null)
                    {
                        info += prop.Name + ": ";
                        if (prop.IsArray)
                        {
                            Array oArr = prop.Value as Array;

                            info += "[";
                            foreach (var a in oArr)
                                info += a + ", ";
                            info = info.Substring(0, info.Length - 2);
                            info += "]\n";
                        }
                        else
                        {
                            info += prop.Value + "\n";
                        }
                    }
                }
            }
            else
            {
                info += mo["Name"].ToString();
            }
            info += "\n";
        }
        if (info.Equals(string.Empty))
            return "None";
        return info.Trim();
    }

    public static string Account(bool complete)
    {
        ManagementObjectSearcher mpo = new ManagementObjectSearcher("select * from Win32_Account");
        string info = string.Empty;
        foreach (ManagementObject mo in mpo.Get())
        {
            if (complete)
            {
                foreach (PropertyData prop in mo.Properties)
                {
                    if (prop.Value != null)
                    {
                        info += prop.Name + ": ";
                        if (prop.IsArray)
                        {
                            Array oArr = prop.Value as Array;

                            info += "[";
                            foreach (var a in oArr)
                                info += a + ", ";
                            info = info.Substring(0, info.Length - 2);
                            info += "]\n";
                        }
                        else
                        {
                            info += prop.Value + "\n";
                        }
                    }
                }
            }
            else
            {
                foreach (PropertyData prop in mo.Properties)
                {
                    if (prop.Name == "FullName")
                    {
                        info += prop.Value.ToString();
                    }
                }
            }
            info += "\n";
        }
        return info.Trim();
    }

    // TODO: test me
    public static string CamDevice()
    {
        ManagementObjectSearcher search = default(ManagementObjectSearcher);
        string deviceName = string.Empty;
        search = new ManagementObjectSearcher("SELECT * From Win32_PnPEntity");
        foreach (ManagementObject info in search.Get())
        {
            try
            {
                string temp = info.Properties["Caption"].Value.ToString();
                if (temp.Contains("cam"))
                    deviceName += info.Properties["Caption"].Value.ToString() + "\n";
            }
            catch
            {

            }
        }
        return deviceName;
    }

    public static string Language()
    {
        return CultureInfo.InstalledUICulture.Name;
    }

}
