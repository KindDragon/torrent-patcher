using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TorrentPatcher
{
    public enum IconSize : uint
    {
        Large = 0,
        Small = 1
    }

    public static class IconHandler
    {
        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x10;

        [StructLayout(LayoutKind.Sequential)]
        internal struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);
        [DllImport("Shell32", CharSet=CharSet.Auto)]
        internal static extern int ExtractIconEx([MarshalAs(UnmanagedType.LPTStr)] string lpszFile, int nIconIndex, IntPtr[] phIconLarge, IntPtr[] phIconSmall, int nIcons);
        public static Icon GetManagedIcon(ref Icon UnmanagedIcon)
        {
            Icon icon = (Icon) UnmanagedIcon.Clone();
            DestroyIcon(UnmanagedIcon.Handle);
            return icon;
        }

        public static Icon IconFromExtension(string Extension, IconSize Size)
        {
            try
            {
                if (Extension[0] != '.')
                {
                    Extension = '.' + Extension;
                }
                SHFILEINFO psfi = new SHFILEINFO();
                SHGetFileInfo(Extension, 0, ref psfi, (uint) Marshal.SizeOf(psfi), (uint) (((IconSize) 0x110) | Size));
                Icon icon = Icon.FromHandle(psfi.hIcon);
                return GetManagedIcon(ref icon);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Icon IconFromFile(string Filename, IconSize Size, int Index)
        {
            int num = ExtractIconEx(Filename, -1, null, null, 0);
            if ((num <= 0) || (Index >= num))
            {
                return null;
            }
            IntPtr[] phIconSmall = new IntPtr[1];
            if (Size == IconSize.Small)
            {
                ExtractIconEx(Filename, Index, null, phIconSmall, 1);
            }
            else
            {
                ExtractIconEx(Filename, Index, phIconSmall, null, 1);
            }
            Icon icon = Icon.FromHandle(phIconSmall[0]);
            return GetManagedIcon(ref icon);
        }

        public static Icon IconFromResource(string ResourceName)
        {
            return new Icon(Assembly.GetCallingAssembly().GetManifestResourceStream(ResourceName));
        }

        public static Icon[] IconsFromFile(string Filename, IconSize Size)
        {
            int nIcons = ExtractIconEx(Filename, -1, null, null, 0);
            IntPtr[] phIconSmall = new IntPtr[nIcons];
            if (Size == IconSize.Small)
            {
                ExtractIconEx(Filename, 0, null, phIconSmall, nIcons);
            }
            else
            {
                ExtractIconEx(Filename, 0, phIconSmall, null, nIcons);
            }
            Icon[] iconArray = new Icon[nIcons];
            for (int i = 0; i < nIcons; i++)
            {
                Icon unmanagedIcon = Icon.FromHandle(phIconSmall[i]);
                iconArray[i] = GetManagedIcon(ref unmanagedIcon);
            }
            return iconArray;
        }

        public static void SaveIcon(Icon SourceIcon, string IconFilename)
        {
            FileStream outputStream = new FileStream(IconFilename, FileMode.Create);
            SourceIcon.Save(outputStream);
            outputStream.Close();
        }

        public static void SaveIconFromImage(Image SourceImage, string IconFilename, IconSize DestenationIconSize)
        {
            Size newSize = (DestenationIconSize == IconSize.Large) ? new Size(0x20, 0x20) : new Size(0x10, 0x10);
            Icon icon = Icon.FromHandle(new Bitmap(SourceImage, newSize).GetHicon());
            FileStream outputStream = new FileStream(IconFilename, FileMode.Create);
            icon.Save(outputStream);
            outputStream.Close();
        }

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
    }
}

