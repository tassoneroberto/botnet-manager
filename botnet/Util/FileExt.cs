using System.Collections.Generic;

namespace Botnet
{
    class FileExt
    {
        // FILETYPE
        public static readonly int UNKNOWN_FILE = -1;
        public static readonly int SYSTEM_FILE = 0;
        public static readonly int VIDEO_FILE = 1;
        public static readonly int IMAGE_FILE = 2;
        public static readonly int AUDIO_FILE = 3;
        public static readonly int TEXT_FILE = 4;
        public static readonly int ARCHIVE_FILE = 5;
        public static readonly int APPLICATION_FILE = 6;
        public static readonly int DISK_FILE = 7;
        public static readonly int DATABASE_FILE = 8;
        public static readonly int FONT_FILE = 9;
        public static readonly int WEB_FILE = 10;
        public static readonly int PRESENTATION_FILE = 11;
        public static readonly int PROGRAMMING_FILE = 12;
        public static readonly int SPREADSHEET_FILE = 13;
        // EXTENSIONS LISTS
        private static List<string> video_ext;
        private static List<string> image_ext;
        private static List<string> audio_ext;
        private static List<string> text_ext;
        private static List<string> archive_ext;
        private static List<string> application_ext;
        private static List<string> disk_ext;
        private static List<string> database_ext;
        private static List<string> font_ext;
        private static List<string> web_ext;
        private static List<string> presentation_ext;
        private static List<string> programming_ext;
        private static List<string> spreadsheet_ext;
        private static List<string> system_ext;

        public static void InitializeExtensions()
        {
            video_ext = new List<string>
            {
                "3g2",
                "3gp",
                "avi",
                "flv",
                "h264",
                "m4v",
                "mkv",
                "mov",
                "mp4",
                "mpg",
                "mpeg",
                "rm",
                "swf",
                "vob",
                "wmv"
            };
            image_ext = new List<string>
            {
                "ai",
                "bmp",
                "gif",
                "ico",
                "jpeg",
                "jpg",
                "png",
                "ps",
                "psd",
                "svg",
                "tif",
                "tiff"
            };
            audio_ext = new List<string>
            {
                "aif",
                "cda",
                "mid",
                "midi",
                "mp3",
                "mpa",
                "ogg",
                "wav",
                "wma",
                "wpl"
            };
            text_ext = new List<string>
            {
                "doc",
                "docx",
                "odt",
                "pdf",
                "rtf",
                "tex",
                "txt",
                "wks",
                "wps",
                "wpd"
            };
            archive_ext = new List<string>
            {
                "7z",
                "arj",
                "deb",
                "pkg",
                "rar",
                "rpm",
                "tar",
                "gz",
                "z",
                "zip"
            };
            application_ext = new List<string>
            {
                "apk",
                "bat",
                "bin",
                "cgi",
                "pl",
                "com",
                "exe",
                "gadget",
                "jar",
                "py",
                "wsf"
            };
            disk_ext = new List<string>
            {
                "bin",
                "dmg",
                "iso",
                "toast",
                "vcd"
            };
            database_ext = new List<string>
            {
                "csv",
                "dat",
                "db",
                "dbf",
                "log",
                "mdb",
                "sav",
                "sql",
                "tar",
                "xml"
            };
            font_ext = new List<string>
            {
                "fnt",
                "fon",
                "otf",
                "ttf"
            };
            web_ext = new List<string>
            {
                "asp",
                "aspx",
                "cfm",
                "cgi",
                "pl",
                "css",
                "htm",
                "html",
                "js",
                "jsp",
                "part",
                "php",
                "py",
                "rss",
                "xhtml"
            };
            presentation_ext = new List<string>
            {
                "key",
                "odp",
                "pps",
                "ppt",
                "pptx"
            };
            programming_ext = new List<string>
            {
                "c",
                "class",
                "cpp",
                "cs",
                "h",
                "java",
                "sh",
                "swift",
                "vb"
            };
            spreadsheet_ext = new List<string>
            {
                "ods",
                "xlr",
                "xls",
                "xlsx"
            };
            system_ext = new List<string>
            {
                "bak",
                "cab",
                "cfg",
                "cpl",
                "cur",
                "dll",
                "dmp",
                "drv",
                "icns",
                "ico",
                "ini",
                "lnk",
                "msi",
                "sys",
                "tmp"
            };
        }
        public static List<int> GetFileTypeByExt(string extension)
        {
            List<int> fileTypes = new List<int>();
            if (video_ext.Contains(extension))
                fileTypes.Add(VIDEO_FILE);
            if (image_ext.Contains(extension))
                fileTypes.Add(IMAGE_FILE);
            if (audio_ext.Contains(extension))
                fileTypes.Add(AUDIO_FILE);
            if (text_ext.Contains(extension))
                fileTypes.Add(TEXT_FILE);
            if (archive_ext.Contains(extension))
                fileTypes.Add(ARCHIVE_FILE);
            if (application_ext.Contains(extension))
                fileTypes.Add(APPLICATION_FILE);
            if (disk_ext.Contains(extension))
                fileTypes.Add(DISK_FILE);
            if (database_ext.Contains(extension))
                fileTypes.Add(DATABASE_FILE);
            if (font_ext.Contains(extension))
                fileTypes.Add(FONT_FILE);
            if (web_ext.Contains(extension))
                fileTypes.Add(WEB_FILE);
            if (presentation_ext.Contains(extension))
                fileTypes.Add(PRESENTATION_FILE);
            if (spreadsheet_ext.Contains(extension))
                fileTypes.Add(SPREADSHEET_FILE);
            if (system_ext.Contains(extension))
                fileTypes.Add(SYSTEM_FILE);
            return fileTypes;
        }
    }
}
