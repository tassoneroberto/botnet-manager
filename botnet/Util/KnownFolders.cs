﻿using System;
using System.Runtime.InteropServices;

public static class KnownFolders
{
    private static readonly string[] _knownFolderGuids = new string[]
    {
        "{56784854-C6CB-462B-8169-88E350ACB882}", // Contacts
        "", // Cookies
        "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", // Desktop
        "{FDD39AD0-238F-46AF-ADB4-6C85480369C7}", // Documents
        "{374DE290-123F-4565-9164-39C4925E467B}", // Downloads
        "{1777F761-68AD-4D8A-87BD-30B759FA33DD}", // Favorites
        "{BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968}", // Links
        "{4BD8D571-6D19-48D3-BE97-422220080E43}", // Music
        "{33E28130-4E1E-4676-835A-98395C3BC3BB}", // Pictures
        "", // Recent
        "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}", // SavedGames
        "{7D1D3A04-DEBB-4115-95CF-2F29DA2920DA}", // SavedSearches
        "", // User
        "{18989B1D-99B5-455B-841C-AB7C74E4DDFC}", // Videos
    };

    public static string GetPath(KnownFolder knownFolder)
    {
        if (knownFolder.Equals(KnownFolder.User))
            return Environment.GetEnvironmentVariable("USERPROFILE");
        if (knownFolder.Equals(KnownFolder.Recent))
            return Environment.GetFolderPath(Environment.SpecialFolder.Recent);
        if (knownFolder.Equals(KnownFolder.Cookies))
            return Environment.GetFolderPath(Environment.SpecialFolder.Cookies);
        return GetPath(knownFolder, false);
    }

    public static string GetPath(KnownFolder knownFolder, bool defaultUser)
    {
        return GetPath(knownFolder, KnownFolderFlags.DontVerify, defaultUser);
    }

    private static string GetPath(KnownFolder knownFolder, KnownFolderFlags flags,
        bool defaultUser)
    {
        int result = SHGetKnownFolderPath(new Guid(_knownFolderGuids[(int)knownFolder]),
            (uint)flags, new IntPtr(defaultUser ? -1 : 0), out IntPtr outPath);
        if (result >= 0)
        {
            string path = Marshal.PtrToStringUni(outPath);
            Marshal.FreeCoTaskMem(outPath);
            return path;
        }
        else
        {
            throw new ExternalException("Unable to retrieve the known folder path. It may not "
                + "be available on this system.", result);
        }
    }

    [DllImport("Shell32.dll")]
    private static extern int SHGetKnownFolderPath(
        [MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken,
        out IntPtr ppszPath);

    [Flags]
    private enum KnownFolderFlags : uint
    {
        SimpleIDList = 0x00000100,
        NotParentRelative = 0x00000200,
        DefaultPath = 0x00000400,
        Init = 0x00000800,
        NoAlias = 0x00001000,
        DontUnexpand = 0x00002000,
        DontVerify = 0x00004000,
        Create = 0x00008000,
        NoAppcontainerRedirection = 0x00010000,
        AliasOnly = 0x80000000
    }
}

public enum KnownFolder
{
    Contacts,
    Cookies,
    Desktop,
    Documents,
    Downloads,
    Favorites,
    Links,
    Music,
    Pictures,
    Recent,
    SavedGames,
    Searches,
    User,
    Videos
}