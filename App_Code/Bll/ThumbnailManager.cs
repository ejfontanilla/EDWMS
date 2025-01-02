using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Dwms.Bll
{

    #region  Get OS Information Helper Class
    /// <summary>
    /// Provides detailed information about the host operating system.
    /// </summary>
    public static class OSInfo
    {
        #region BITS
        /// <summary>
        /// Determines if the current application is 32 or 64-bit.
        /// </summary>
        public static int Bits
        {
            get
            {
                return IntPtr.Size * 8;
            }
        }
        #endregion BITS

        #region EDITION
        private static string s_Edition;
        /// <summary>
        /// Gets the edition of the operating system running on this computer.
        /// </summary>
        public static string Edition
        {
            get
            {
                if (s_Edition != null)
                    return s_Edition;  //***** RETURN *****//

                string edition = String.Empty;

                OperatingSystem osVersion = Environment.OSVersion;
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                if (GetVersionEx(ref osVersionInfo))
                {
                    int majorVersion = osVersion.Version.Major;
                    int minorVersion = osVersion.Version.Minor;
                    byte productType = osVersionInfo.wProductType;
                    short suiteMask = osVersionInfo.wSuiteMask;

                    #region VERSION 4
                    if (majorVersion == 4)
                    {
                        if (productType == VER_NT_WORKSTATION)
                        {
                            // Windows NT 4.0 Workstation
                            edition = "Workstation";
                        }
                        else if (productType == VER_NT_SERVER)
                        {
                            if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                            {
                                // Windows NT 4.0 Server Enterprise
                                edition = "Enterprise Server";
                            }
                            else
                            {
                                // Windows NT 4.0 Server
                                edition = "Standard Server";
                            }
                        }
                    }
                    #endregion VERSION 4

                    #region VERSION 5
                    else if (majorVersion == 5)
                    {
                        if (productType == VER_NT_WORKSTATION)
                        {
                            if ((suiteMask & VER_SUITE_PERSONAL) != 0)
                            {
                                // Windows XP Home Edition
                                edition = "Home";
                            }
                            else
                            {
                                // Windows XP / Windows 2000 Professional
                                edition = "Professional";
                            }
                        }
                        else if (productType == VER_NT_SERVER)
                        {
                            if (minorVersion == 0)
                            {
                                if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                                {
                                    // Windows 2000 Datacenter Server
                                    edition = "Datacenter Server";
                                }
                                else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                                {
                                    // Windows 2000 Advanced Server
                                    edition = "Advanced Server";
                                }
                                else
                                {
                                    // Windows 2000 Server
                                    edition = "Server";
                                }
                            }
                            else
                            {
                                if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                                {
                                    // Windows Server 2003 Datacenter Edition
                                    edition = "Datacenter";
                                }
                                else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                                {
                                    // Windows Server 2003 Enterprise Edition
                                    edition = "Enterprise";
                                }
                                else if ((suiteMask & VER_SUITE_BLADE) != 0)
                                {
                                    // Windows Server 2003 Web Edition
                                    edition = "Web Edition";
                                }
                                else
                                {
                                    // Windows Server 2003 Standard Edition
                                    edition = "Standard";
                                }
                            }
                        }
                    }
                    #endregion VERSION 5

                    #region VERSION 6
                    else if (majorVersion == 6)
                    {
                        int ed;
                        if (GetProductInfo(majorVersion, minorVersion,
                            osVersionInfo.wServicePackMajor, osVersionInfo.wServicePackMinor,
                            out ed))
                        {
                            switch (ed)
                            {
                                case PRODUCT_BUSINESS:
                                    edition = "Business";
                                    break;
                                case PRODUCT_BUSINESS_N:
                                    edition = "Business N";
                                    break;
                                case PRODUCT_CLUSTER_SERVER:
                                    edition = "HPC Edition";
                                    break;
                                case PRODUCT_DATACENTER_SERVER:
                                    edition = "Datacenter Server";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_CORE:
                                    edition = "Datacenter Server (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE:
                                    edition = "Enterprise";
                                    break;
                                case PRODUCT_ENTERPRISE_N:
                                    edition = "Enterprise N";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER:
                                    edition = "Enterprise Server";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_CORE:
                                    edition = "Enterprise Server (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_CORE_V:
                                    edition = "Enterprise Server without Hyper-V (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_IA64:
                                    edition = "Enterprise Server for Itanium-based Systems";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_V:
                                    edition = "Enterprise Server without Hyper-V";
                                    break;
                                case PRODUCT_HOME_BASIC:
                                    edition = "Home Basic";
                                    break;
                                case PRODUCT_HOME_BASIC_N:
                                    edition = "Home Basic N";
                                    break;
                                case PRODUCT_HOME_PREMIUM:
                                    edition = "Home Premium";
                                    break;
                                case PRODUCT_HOME_PREMIUM_N:
                                    edition = "Home Premium N";
                                    break;
                                case PRODUCT_HYPERV:
                                    edition = "Microsoft Hyper-V Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT:
                                    edition = "Windows Essential Business Management Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING:
                                    edition = "Windows Essential Business Messaging Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY:
                                    edition = "Windows Essential Business Security Server";
                                    break;
                                case PRODUCT_SERVER_FOR_SMALLBUSINESS:
                                    edition = "Windows Essential Server Solutions";
                                    break;
                                case PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
                                    edition = "Windows Essential Server Solutions without Hyper-V";
                                    break;
                                case PRODUCT_SMALLBUSINESS_SERVER:
                                    edition = "Windows Small Business Server";
                                    break;
                                case PRODUCT_STANDARD_SERVER:
                                    edition = "Standard Server";
                                    break;
                                case PRODUCT_STANDARD_SERVER_CORE:
                                    edition = "Standard Server (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER_CORE_V:
                                    edition = "Standard Server without Hyper-V (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER_V:
                                    edition = "Standard Server without Hyper-V";
                                    break;
                                case PRODUCT_STARTER:
                                    edition = "Starter";
                                    break;
                                case PRODUCT_STORAGE_ENTERPRISE_SERVER:
                                    edition = "Enterprise Storage Server";
                                    break;
                                case PRODUCT_STORAGE_EXPRESS_SERVER:
                                    edition = "Express Storage Server";
                                    break;
                                case PRODUCT_STORAGE_STANDARD_SERVER:
                                    edition = "Standard Storage Server";
                                    break;
                                case PRODUCT_STORAGE_WORKGROUP_SERVER:
                                    edition = "Workgroup Storage Server";
                                    break;
                                case PRODUCT_UNDEFINED:
                                    edition = "Unknown product";
                                    break;
                                case PRODUCT_ULTIMATE:
                                    edition = "Ultimate";
                                    break;
                                case PRODUCT_ULTIMATE_N:
                                    edition = "Ultimate N";
                                    break;
                                case PRODUCT_WEB_SERVER:
                                    edition = "Web Server";
                                    break;
                                case PRODUCT_WEB_SERVER_CORE:
                                    edition = "Web Server (core installation)";
                                    break;
                            }
                        }
                    }
                    #endregion VERSION 6
                }

                s_Edition = edition;
                return edition;
            }
        }
        #endregion EDITION

        #region NAME
        private static string s_Name;
        /// <summary>
        /// Gets the name of the operating system running on this computer.
        /// </summary>
        public static string Name
        {
            get
            {
                if (s_Name != null)
                    return s_Name;  //***** RETURN *****//

                string name = "unknown";

                OperatingSystem osVersion = Environment.OSVersion;
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                if (GetVersionEx(ref osVersionInfo))
                {
                    int majorVersion = osVersion.Version.Major;
                    int minorVersion = osVersion.Version.Minor;

                    switch (osVersion.Platform)
                    {
                        case PlatformID.Win32Windows:
                            {
                                if (majorVersion == 4)
                                {
                                    string csdVersion = osVersionInfo.szCSDVersion;
                                    switch (minorVersion)
                                    {
                                        case 0:
                                            if (csdVersion == "B" || csdVersion == "C")
                                                name = "Windows 95 OSR2";
                                            else
                                                name = "Windows 95";
                                            break;
                                        case 10:
                                            if (csdVersion == "A")
                                                name = "Windows 98 Second Edition";
                                            else
                                                name = "Windows 98";
                                            break;
                                        case 90:
                                            name = "Windows Me";
                                            break;
                                    }
                                }
                                break;
                            }

                        case PlatformID.Win32NT:
                            {
                                byte productType = osVersionInfo.wProductType;

                                switch (majorVersion)
                                {
                                    case 3:
                                        name = "Windows NT 3.51";
                                        break;
                                    case 4:
                                        switch (productType)
                                        {
                                            case 1:
                                                name = "Windows NT 4.0";
                                                break;
                                            case 3:
                                                name = "Windows NT 4.0 Server";
                                                break;
                                        }
                                        break;
                                    case 5:
                                        switch (minorVersion)
                                        {
                                            case 0:
                                                name = "Windows 2000";
                                                break;
                                            case 1:
                                                name = "Windows XP";
                                                break;
                                            case 2:
                                                name = "Windows Server 2003";
                                                break;
                                        }
                                        break;
                                    case 6:
                                        switch (productType)
                                        {
                                            case 1:
                                                name = "Windows Vista";
                                                break;
                                            case 3:
                                                name = "Windows Server 2008";
                                                break;
                                        }
                                        break;
                                }
                                break;
                            }
                    }
                }

                s_Name = name;
                return name;
            }
        }
        #endregion NAME

        #region PINVOKE
        #region GET
        #region PRODUCT INFO
        [DllImport("Kernel32.dll")]
        internal static extern bool GetProductInfo(
            int osMajorVersion,
            int osMinorVersion,
            int spMajorVersion,
            int spMinorVersion,
            out int edition);
        #endregion PRODUCT INFO

        #region VERSION
        [DllImport("kernel32.dll")]
        private static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);
        #endregion VERSION
        #endregion GET

        #region OSVERSIONINFOEX
        [StructLayout(LayoutKind.Sequential)]
        private struct OSVERSIONINFOEX
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public short wServicePackMajor;
            public short wServicePackMinor;
            public short wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }
        #endregion OSVERSIONINFOEX

        #region PRODUCT
        private const int PRODUCT_UNDEFINED = 0x00000000;
        private const int PRODUCT_ULTIMATE = 0x00000001;
        private const int PRODUCT_HOME_BASIC = 0x00000002;
        private const int PRODUCT_HOME_PREMIUM = 0x00000003;
        private const int PRODUCT_ENTERPRISE = 0x00000004;
        private const int PRODUCT_HOME_BASIC_N = 0x00000005;
        private const int PRODUCT_BUSINESS = 0x00000006;
        private const int PRODUCT_STANDARD_SERVER = 0x00000007;
        private const int PRODUCT_DATACENTER_SERVER = 0x00000008;
        private const int PRODUCT_SMALLBUSINESS_SERVER = 0x00000009;
        private const int PRODUCT_ENTERPRISE_SERVER = 0x0000000A;
        private const int PRODUCT_STARTER = 0x0000000B;
        private const int PRODUCT_DATACENTER_SERVER_CORE = 0x0000000C;
        private const int PRODUCT_STANDARD_SERVER_CORE = 0x0000000D;
        private const int PRODUCT_ENTERPRISE_SERVER_CORE = 0x0000000E;
        private const int PRODUCT_ENTERPRISE_SERVER_IA64 = 0x0000000F;
        private const int PRODUCT_BUSINESS_N = 0x00000010;
        private const int PRODUCT_WEB_SERVER = 0x00000011;
        private const int PRODUCT_CLUSTER_SERVER = 0x00000012;
        private const int PRODUCT_HOME_SERVER = 0x00000013;
        private const int PRODUCT_STORAGE_EXPRESS_SERVER = 0x00000014;
        private const int PRODUCT_STORAGE_STANDARD_SERVER = 0x00000015;
        private const int PRODUCT_STORAGE_WORKGROUP_SERVER = 0x00000016;
        private const int PRODUCT_STORAGE_ENTERPRISE_SERVER = 0x00000017;
        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS = 0x00000018;
        private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM = 0x00000019;
        private const int PRODUCT_HOME_PREMIUM_N = 0x0000001A;
        private const int PRODUCT_ENTERPRISE_N = 0x0000001B;
        private const int PRODUCT_ULTIMATE_N = 0x0000001C;
        private const int PRODUCT_WEB_SERVER_CORE = 0x0000001D;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT = 0x0000001E;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY = 0x0000001F;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING = 0x00000020;
        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS_V = 0x00000023;
        private const int PRODUCT_STANDARD_SERVER_V = 0x00000024;
        private const int PRODUCT_ENTERPRISE_SERVER_V = 0x00000026;
        private const int PRODUCT_STANDARD_SERVER_CORE_V = 0x00000028;
        private const int PRODUCT_ENTERPRISE_SERVER_CORE_V = 0x00000029;
        private const int PRODUCT_HYPERV = 0x0000002A;
        #endregion PRODUCT

        #region VERSIONS
        private const int VER_NT_WORKSTATION = 1;
        private const int VER_NT_DOMAIN_CONTROLLER = 2;
        private const int VER_NT_SERVER = 3;
        private const int VER_SUITE_SMALLBUSINESS = 1;
        private const int VER_SUITE_ENTERPRISE = 2;
        private const int VER_SUITE_TERMINAL = 16;
        private const int VER_SUITE_DATACENTER = 128;
        private const int VER_SUITE_SINGLEUSERTS = 256;
        private const int VER_SUITE_PERSONAL = 512;
        private const int VER_SUITE_BLADE = 1024;
        #endregion VERSIONS
        #endregion PINVOKE

        #region SERVICE PACK
        /// <summary>
        /// Gets the service pack information of the operating system running on this computer.
        /// </summary>
        public static string ServicePack
        {
            get
            {
                string servicePack = String.Empty;
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();

                osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                if (GetVersionEx(ref osVersionInfo))
                {
                    servicePack = osVersionInfo.szCSDVersion;
                }

                return servicePack;
            }
        }
        #endregion SERVICE PACK

        #region VERSION
        #region BUILD
        /// <summary>
        /// Gets the build version number of the operating system running on this computer.
        /// </summary>
        public static int BuildVersion
        {
            get
            {
                return Environment.OSVersion.Version.Build;
            }
        }
        #endregion BUILD

        #region FULL
        #region STRING
        /// <summary>
        /// Gets the full version string of the operating system running on this computer.
        /// </summary>
        public static string VersionString
        {
            get
            {
                return Environment.OSVersion.Version.ToString();
            }
        }
        #endregion STRING

        #region VERSION
        /// <summary>
        /// Gets the full version of the operating system running on this computer.
        /// </summary>
        public static Version Version
        {
            get
            {
                return Environment.OSVersion.Version;
            }
        }
        #endregion VERSION
        #endregion FULL

        #region MAJOR
        /// <summary>
        /// Gets the major version number of the operating system running on this computer.
        /// </summary>
        public static int MajorVersion
        {
            get
            {
                return Environment.OSVersion.Version.Major;
            }
        }
        #endregion MAJOR

        #region MINOR
        /// <summary>
        /// Gets the minor version number of the operating system running on this computer.
        /// </summary>
        public static int MinorVersion
        {
            get
            {
                return Environment.OSVersion.Version.Minor;
            }
        }
        #endregion MINOR

        #region REVISION
        /// <summary>
        /// Gets the revision version number of the operating system running on this computer.
        /// </summary>
        public static int RevisionVersion
        {
            get
            {
                return Environment.OSVersion.Version.Revision;
            }
        }
        #endregion REVISION
        #endregion VERSION
    }
    #endregion


    public class ThumbnailManager : IDisposable
    {
        #region  Get OS Information
        public string GetOSVersionName()
        {
            string strVersion = "Unknown";
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32S:
                    return "Windows 3.1";
                case PlatformID.Win32Windows:
                    switch (Environment.OSVersion.Version.Minor)
                    {
                        case 0:
                            return "Windows 95";
                        case 10:
                            if (Environment.OSVersion.Version.Revision.ToString() == "2222A")
                            {
                                return "Windows 98 Second Edition";
                            }
                            else
                                return "Windows 98";
                        case 90:
                            return "Windows ME";
                    }
                    break;
                case PlatformID.Win32NT:
                    switch (Environment.OSVersion.Version.Major)
                    {
                        case 3:
                            return "Windows NT 3.51";
                        case 4:
                            return "Windows NT 4.0";
                        case 5:
                            switch (Environment.OSVersion.Version.Minor)
                            {
                                case 0:
                                    return "Windows 2000";
                                case 1:
                                    return "Windows XP";
                                case 2:
                                    return "Windows 2003";
                            }
                            break;
                        case 6:
                            switch (Environment.OSVersion.Version.Minor)
                            {
                                case 0:
                                    return "Windows Vista";
                                case 1:
                                    return "Windows 2008";
                                case 2:
                                    return "Windows 7";
                            }
                            break;
                    }
                    break;
                case PlatformID.WinCE:
                    return "Windows CE";
                case PlatformID.Unix:
                    return "Unix";
            }
            return strVersion;
        }
        #endregion




        #region ShellFolder Enumerations
        [Flags]
        private enum ESTRRET : int
        {
            STRRET_WSTR = 0x0000,
            STRRET_OFFSET = 0x0001,
            STRRET_CSTR = 0x0002
        }
        [Flags]
        private enum ESHCONTF : int
        {
            SHCONTF_FOLDERS = 32,
            SHCONTF_NONFOLDERS = 64,
            SHCONTF_INCLUDEHIDDEN = 128
        }

        [Flags]
        private enum ESHGDN : int
        {
            SHGDN_NORMAL = 0,
            SHGDN_INFOLDER = 1,
            SHGDN_FORADDRESSBAR = 16384,
            SHGDN_FORPARSING = 32768
        }
        [Flags]
        private enum ESFGAO : int
        {
            SFGAO_CANCOPY = 1,
            SFGAO_CANMOVE = 2,
            SFGAO_CANLINK = 4,
            SFGAO_CANRENAME = 16,
            SFGAO_CANDELETE = 32,
            SFGAO_HASPROPSHEET = 64,
            SFGAO_DROPTARGET = 256,
            SFGAO_CAPABILITYMASK = 375,
            SFGAO_LINK = 65536,
            SFGAO_SHARE = 131072,
            SFGAO_READONLY = 262144,
            SFGAO_GHOSTED = 524288,
            SFGAO_DISPLAYATTRMASK = 983040,
            SFGAO_FILESYSANCESTOR = 268435456,
            SFGAO_FOLDER = 536870912,
            SFGAO_FILESYSTEM = 1073741824,
            SFGAO_HASSUBFOLDER = -2147483648,
            SFGAO_CONTENTSMASK = -2147483648,
            SFGAO_VALIDATE = 16777216,
            SFGAO_REMOVABLE = 33554432,
            SFGAO_COMPRESSED = 67108864
        }
        #endregion

        #region IExtractImage Enumerations
        private enum EIEIFLAG
        {
            IEIFLAG_ASYNC = 0x0001,
            IEIFLAG_CACHE = 0x0002,
            IEIFLAG_ASPECT = 0x0004,
            IEIFLAG_OFFLINE = 0x0008,
            IEIFLAG_GLEAM = 0x0010,
            IEIFLAG_SCREEN = 0x0020,
            IEIFLAG_ORIGSIZE = 0x0040,
            IEIFLAG_NOSTAMP = 0x0080,
            IEIFLAG_NOBORDER = 0x0100,
            IEIFLAG_QUALITY = 0x0200
        }
        #endregion

        #region ShellFolder Structures
        [StructLayoutAttribute(LayoutKind.Sequential, Pack = 4, Size = 0, CharSet = CharSet.Auto)]
        private struct STRRET_CSTR
        {
            public ESTRRET uType;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 520)]
            public byte[] cStr;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
        private struct STRRET_ANY
        {
            [FieldOffset(0)]
            public ESTRRET uType;
            [FieldOffset(4)]
            public IntPtr pOLEString;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        private struct SIZE
        {
            public int cx;
            public int cy;
        }
        #endregion

        #region Com Interop for IUnknown
        [ComImport, Guid("00000000-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IUnknown
        {
            [PreserveSig]
            IntPtr QueryInterface(ref Guid riid, out IntPtr pVoid);

            [PreserveSig]
            IntPtr AddRef();

            [PreserveSig]
            IntPtr Release();
        }
        #endregion

        #region COM Interop for IEnumIDList
        [ComImportAttribute()]
        [GuidAttribute("000214F2-0000-0000-C000-000000000046")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IEnumIDList
        {
            [PreserveSig]
            int Next(
             int celt,
             ref IntPtr rgelt,
             out int pceltFetched);

            void Skip(
             int celt);

            void Reset();

            void Clone(
             ref IEnumIDList ppenum);
        };
        #endregion

        #region COM Interop for IShellFolder
        [ComImportAttribute()]
        [GuidAttribute("000214E6-0000-0000-C000-000000000046")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellFolder
        {
            void ParseDisplayName(
             IntPtr hwndOwner,
             IntPtr pbcReserved,
             [MarshalAs(UnmanagedType.LPWStr)] string lpszDisplayName,
             out int pchEaten,
             out IntPtr ppidl,
             out int pdwAttributes
             );

            void EnumObjects(
             IntPtr hwndOwner,
             [MarshalAs(UnmanagedType.U4)] ESHCONTF grfFlags,
             ref IEnumIDList ppenumIDList
             );

            void BindToObject(
             IntPtr pidl,
             IntPtr pbcReserved,
             ref Guid riid,
             ref IShellFolder ppvOut
             );

            void BindToStorage(
             IntPtr pidl,
             IntPtr pbcReserved,
             ref Guid riid,
             IntPtr ppvObj
             );

            [PreserveSig]
            int CompareIDs(
             IntPtr lParam,
             IntPtr pidl1,
             IntPtr pidl2
             );

            void CreateViewObject(
             IntPtr hwndOwner,
             ref Guid riid,
             IntPtr ppvOut
             );

            void GetAttributesOf(
             int cidl,
             IntPtr apidl,
             [MarshalAs(UnmanagedType.U4)] ref ESFGAO rgfInOut
             );

            void GetUIObjectOf(
             IntPtr hwndOwner,
             int cidl,
             ref IntPtr apidl,
             ref Guid riid,
             out int prgfInOut,
             ref IUnknown ppvOut
             );

            void GetDisplayNameOf(
             IntPtr pidl,
             [MarshalAs(UnmanagedType.U4)] ESHGDN uFlags,
             ref STRRET_CSTR lpName
             );

            void SetNameOf(
             IntPtr hwndOwner,
             IntPtr pidl,
             [MarshalAs(UnmanagedType.LPWStr)] string lpszName,
             [MarshalAs(UnmanagedType.U4)] ESHCONTF uFlags,
             ref IntPtr ppidlOut
             );

        };

        #endregion

        #region COM Interop for IExtractImage
        [ComImportAttribute()]
        [GuidAttribute("BB2E617C-0920-11d1-9A0B-00C04FC2D6C1")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IExtractImage
        {
            void GetLocation(
             [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPathBuffer,
             int cch,
             ref int pdwPriority,
             ref SIZE prgSize,
             int dwRecClrDepth,
             ref int pdwFlags
             );

            void Extract(
             out IntPtr phBmpThumbnail
             );
        }
        #endregion

        #region UnmanagedMethods for IShellFolder
        private class UnmanagedMethods
        {
            [DllImport("ole32", CharSet = CharSet.Auto)]
            internal extern static void CoTaskMemFree(IntPtr ptr);

            [DllImport("shell32", CharSet = CharSet.Auto)]
            internal extern static int SHGetDesktopFolder(out IShellFolder ppshf);

            [DllImport("shell32", CharSet = CharSet.Auto)]
            internal extern static int SHGetPathFromIDList(IntPtr pidl, StringBuilder pszPath);

            [DllImport("gdi32", CharSet = CharSet.Auto)]
            internal extern static int DeleteObject(IntPtr hObject);
        }
        #endregion

        #region Member Variables
        private bool disposed = false;
        private System.Drawing.Bitmap thumbNail = null;
        #endregion

        #region Implementation
        public System.Drawing.Bitmap ThumbNail
        {
            get
            {
                return thumbNail;
            }
        }

        public System.Drawing.Bitmap GetThumbnail(string file, int width, int height)
        {
            if ((!File.Exists(file)) && (!Directory.Exists(file)))
            {
                throw new FileNotFoundException(
                 String.Format("The file '{0}' does not exist", file),
                 file);
            }

            if (thumbNail != null)
            {
                thumbNail.Dispose();
                thumbNail = null;
            }

            IShellFolder folder = null;
            try
            {
                folder = GetDesktopFolder;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (folder != null)
            {
                IntPtr pidlMain = IntPtr.Zero;
                try
                {
                    int cParsed = 0;
                    int pdwAttrib = 0;
                    string filePath = Path.GetDirectoryName(file);
                    pidlMain = IntPtr.Zero;
                    folder.ParseDisplayName(
                     IntPtr.Zero,
                     IntPtr.Zero,
                     filePath,
                     out cParsed,
                     out pidlMain,
                     out pdwAttrib);
                }
                catch (Exception ex)
                {
                    Marshal.ReleaseComObject(folder);
                    throw ex;
                }

                if (pidlMain != IntPtr.Zero)
                {
                    // IShellFolder:
                    Guid iidShellFolder = new Guid("000214E6-0000-0000-C000-000000000046");
                    IShellFolder item = null;

                    try
                    {
                        folder.BindToObject(pidlMain, IntPtr.Zero, ref
       iidShellFolder, ref item);
                    }
                    catch (Exception ex)
                    {
                        Marshal.ReleaseComObject(folder);
                        UnmanagedMethods.CoTaskMemFree(pidlMain);
                        throw ex;
                    }

                    if (item != null)
                    {
                        IEnumIDList idEnum = null;
                        try
                        {
                            item.EnumObjects(
                             IntPtr.Zero,
                             (ESHCONTF.SHCONTF_FOLDERS |
                             ESHCONTF.SHCONTF_NONFOLDERS),
                             ref idEnum);
                        }
                        catch (Exception ex)
                        {
                            Marshal.ReleaseComObject(folder);
                            UnmanagedMethods.CoTaskMemFree(pidlMain);
                            throw ex;
                        }

                        if (idEnum != null)
                        {
                            int hRes = 0;
                            IntPtr pidl = IntPtr.Zero;
                            int fetched = 0;
                            bool complete = false;
                            while (!complete)
                            {
                                hRes = idEnum.Next(1, ref pidl, out fetched);
                                if (hRes != 0)
                                {
                                    pidl = IntPtr.Zero;
                                    complete = true;
                                }
                                else
                                {
                                    if (GetThumbnail(file, pidl, item, width, height))
                                    {
                                        complete = true;
                                    }
                                }
                                if (pidl != IntPtr.Zero)
                                {
                                    UnmanagedMethods.CoTaskMemFree(pidl);
                                }
                            }

                            Marshal.ReleaseComObject(idEnum);
                        }


                        Marshal.ReleaseComObject(item);
                    }

                    UnmanagedMethods.CoTaskMemFree(pidlMain);
                }

                Marshal.ReleaseComObject(folder);
            }
            return thumbNail;
        }

        private bool GetThumbnail(string file, IntPtr pidl, IShellFolder item, int width, int height)
        {
            IntPtr hBmp = IntPtr.Zero;
            IExtractImage extractImage = null;

            try
            {
                string pidlPath = PathFromPidl(pidl);
                if (Path.GetFileName(pidlPath).ToUpper().Equals(Path.GetFileName(file).ToUpper()))
                {
                    IUnknown iunk = null;
                    int prgf = 0;
                    Guid iidExtractImage = new Guid("BB2E617C-0920-11d1-9A0B-00C04FC2D6C1");
                    item.GetUIObjectOf(IntPtr.Zero, 1, ref pidl, ref iidExtractImage, out prgf, ref iunk);
                    extractImage = (IExtractImage)iunk;

                    if (extractImage != null)
                    {
                        SIZE sz = new SIZE();
                        sz.cx = width;
                        sz.cy = height;
                        StringBuilder location = new StringBuilder(260, 260);
                        int priority = 0;
                        int requestedColourDepth = 32;
                        EIEIFLAG flags = EIEIFLAG.IEIFLAG_ASPECT | EIEIFLAG.IEIFLAG_SCREEN;
                        int uFlags = (int)flags;

                        /* 2012-10-09, try catch added to handle the Error
                         * The data necessary to complete this operation is not yet available. (Exception from HRESULT: 0x8000000A)
                         */
                        try
                        {
                            extractImage.GetLocation(location, location.Capacity, ref priority, ref sz, requestedColourDepth, ref uFlags);
                        }
                        catch { }
                        extractImage.Extract(out hBmp);
                        if (hBmp != IntPtr.Zero)
                        {
                            thumbNail = System.Drawing.Bitmap.FromHbitmap(hBmp);
                        }

                        Marshal.ReleaseComObject(extractImage);
                        extractImage = null;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (hBmp != IntPtr.Zero)
                {
                    UnmanagedMethods.DeleteObject(hBmp);
                }
                if (extractImage != null)
                {
                    Marshal.ReleaseComObject(extractImage);
                }
                throw ex;
            }
        }


        private string PathFromPidl(IntPtr pidl)
        {
            StringBuilder path = new StringBuilder(260, 260);
            int result = UnmanagedMethods.SHGetPathFromIDList(pidl, path);
            if (result == 0)
            {
                return string.Empty;
            }
            else
            {
                return path.ToString();
            }
        }

        private IShellFolder GetDesktopFolder
        {
            get
            {
                IShellFolder ppshf;
                int r = UnmanagedMethods.SHGetDesktopFolder(out ppshf);
                return ppshf;
            }
        }
        #endregion

        #region Constructor, Destructor, Dispose
        public ThumbnailManager()
        {
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (thumbNail != null)
                {
                    thumbNail.Dispose();
                }
                disposed = true;
            }
        }

        ~ThumbnailManager()
        {
            Dispose();
        }
        #endregion
    }
}