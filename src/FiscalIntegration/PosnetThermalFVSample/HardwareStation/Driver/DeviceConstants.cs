namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.Driver
    {
        /// <summary>
        /// The device constants.
        /// </summary>
        internal static class DeviceConstants
        {
            // device type
            public const ulong POSNET_INTERFACE_ETH = 0x0003;
            public const ulong POSNET_INTERFACE_RS232 = 0x0001;
            public const ulong POSNET_INTERFACE_USB = 0x0002;

            // device parameters identifiers
            public const ulong POSNET_DEV_PARAM_COMSETTINGS = 0x00020001;
            public const ulong POSNET_DEV_PARAM_OUTQUEUELENGTH = 0x00020009;
            public const ulong POSNET_DEV_PARAM_SENDTIMEOUT = 0x00020004;
            public const ulong POSNET_DEV_PARAM_STATUSPOLLINGINTERVAL = 0x0002000A;
            public const ulong POSNET_DEV_PARAM_FILEHANDLE = 0x0002000E;
            public const ulong POSNET_DEV_PARAM_IP = 0x00020005;
            public const ulong POSNET_DEV_PARAM_IPPORT = 0x00020006;
            public const ulong POSNET_DEV_PARAM_MAXSENT = 0x0002000F;

            // device statuses
            public const ulong POSNET_STATUS_OK = 0x00000000;
            public const ulong POSNET_STATUS_OUTOFMEMORY = 0x00000001;
            public const ulong POSNET_STATUS_FRAMETOOSHORT = 0x00000002;
            public const ulong POSNET_STATUS_FRAMINGERROR = 0x00000003;
            public const ulong POSNET_STATUS_COULDNOTOPEN = 0x00000005;
            public const ulong POSNET_STATUS_CRCERROR = 0x00000006;
            public const ulong POSNET_STATUS_IPCERROR = 0x00000007;
            public const ulong POSNET_STATUS_COMMERROR = 0x00000008;
            public const ulong POSNET_STATUS_USBERROR = 0x00000009;
            public const ulong POSNET_STATUS_FTLIBIMPORTFAIL = 0x0000000A;
            public const ulong POSNET_STATUS_COULDNOTSETUPPORT = 0x0000000B;
            public const ulong POSNET_STATUS_COULDNOTOPEN_ACCESSDENIED = 0x0000000C;
            public const ulong POSNET_STATUS_COULDNOTOPEN_FILENOTFOUND = 0x0000000D;
            public const ulong POSNET_STATUS_SETUP_INVALIDBAUD = 0x0000000E;
            public const ulong POSNET_STATUS_SETUP_INVALIDDATA = 0x0000000F;
            public const ulong POSNET_STATUS_SETUP_INVALIDPARITY = 0x00000010;
            public const ulong POSNET_STATUS_SETUP_INVALIDSTOP = 0x00000011;
            public const ulong POSNET_STATUS_SETUP_INVALIDHANDSHAKE = 0x00000012;
            public const ulong POSNET_STATUS_INVALIDSTATE = 0x00000013;
            public const ulong POSNET_STATUS_DEVICE_BUSY = 0x00000014;
            public const ulong POSNET_STATUS_BUSY = 0x00000020;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAEINTR = 0x00000030;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAEACCES = 0x00000031;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAEINPROGRESS = 0x00000032;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAEDESTADDRREQ = 0x00000033;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAEADDRINUSE = 0x00000034;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAEADDRNOTAVAIL = 0x00000035;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAENETDOWN = 0x00000036;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAENETUNREACH = 0x00000037;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAENETRESET = 0x00000038;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAECONNABORTED = 0x00000039;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAECONNRESET = 0x0000003A;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAETIMEDOUT = 0x0000003B;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAECONNREFUSED = 0x0000003c;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAEHOSTDOWN = 0x0000003d;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAEHOSTUNREACH = 0x0000003e;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSAHOSTNOTFOUND = 0x0000003f;
            public const ulong POSNET_STATUS_COULDNOTOPEN_WSATRYAGAIN = 0x00000040;
            public const ulong POSNET_STATUS_ADVBMP_READERROR = 0x00000070;
            public const ulong POSNET_STATUS_ADVBMP_BADSIZE = 0x00000071;
            public const ulong POSNET_STATUS_ALREADY_COMPLETED = 0x00010000;
            public const ulong POSNET_STATUS_EMPTY = 0x00010001;
            public const ulong POSNET_STATUS_INVALIDVALUE = 0x00010002;
            public const ulong POSNET_STATUS_TIMEOUT = 0x00010003;
            public const ulong POSNET_STATUS_PENDING = 0x00010004;
            public const ulong POSNET_STATUS_INVALIDCOMMAND = 0x00010005;
            public const ulong POSNET_STATUS_INVALIDHANDLE = 0x00010006;
            public const ulong POSNET_STATUS_BUFFERTOOSHORT = 0x00010007;
            public const ulong POSNET_STATUS_OUTOFRANGE = 0x00010008;
            public const ulong POSNET_STATUS_INVALIDSPOOLMODE = 0x00010009;
            public const ulong POSNET_STATUS_CANCELLED = 0x0001000A;
            public const ulong POSNET_STATUS_INVALID_PARAM1 = 0x00010101;
            public const ulong POSNET_STATUS_INVALID_PARAM2 = 0x00010102;
            public const ulong POSNET_STATUS_INVALID_PARAM3 = 0x00010103;
            public const ulong POSNET_STATUS_INVALID_PARAM4 = 0x00010104;
            public const ulong POSNET_STATUS_INVALID_PARAM5 = 0x00010105;
            public const ulong POSNET_STATUS_INVALID_PARAM6 = 0x00010106;
            public const ulong POSNET_STATUS_CASHREGBASE = 0x00430000;
            public const ulong POSNET_STATUS_CASHREGCOMMBASE = 0x00440000;

            // debug levels
            public const ulong POSNET_DEBUG_NONE = 0x00000000;
            public const ulong POSNET_DEBUG_ALL = 0xFFFFFFFF;

            // request send modes
            public const byte POSNET_REQMODE_SPOOL = 0x00;

            // request states
            public const ulong POSNET_RSTATE_NEW = 0x00019000;
            public const ulong POSNET_RSTATE_PENDING = 0x00019001;
            public const ulong POSNET_RSTATE_SENT = 0x00019002;
            public const ulong POSNET_RSTATE_COMPLETED = 0x00019004;
            public const ulong POSNET_RSTATE_ERRCOMPLETED = 0x00019005;
        }
    }
}
