using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Cassia.Impl
{
    /// <summary>
    /// The relevant sections from winsta.h:
    /// 
    /// #define WINSTATIONNAME_LENGTH     32
    /// typedef WCHAR WINSTATIONNAME[ WINSTATIONNAME_LENGTH + 1 ];
    /// 
    /// typedef struct _PROTOCOLCOUNTERS { (size = 460)
    ///     ULONG WdBytes;             
    ///     ULONG WdFrames;            
    ///     ULONG WaitForOutBuf;       
    ///     ULONG Frames;              
    ///     ULONG Bytes;               
    ///     ULONG CompressedBytes;     
    ///     ULONG CompressFlushes;     
    ///     ULONG Errors;              
    ///     ULONG Timeouts;            
    ///     ULONG AsyncFramingError;   
    ///     ULONG AsyncOverrunError;   
    ///     ULONG AsyncOverflowError;  
    ///     ULONG AsyncParityError;    
    ///     ULONG TdErrors;            
    ///     USHORT ProtocolType;       
    ///     USHORT Length;             
    ///     union {
    ///         TSHARE_COUNTERS TShareCounters;
    ///         ULONG           Reserved[100];
    ///     } Specific;
    /// } PROTOCOLCOUNTERS, * PPROTOCOLCOUNTERS;
    /// 
    /// typedef struct CACHE_STATISTICS { (size = 84)
    ///     USHORT ProtocolType;    
    ///     USHORT Length;          
    ///     union {
    ///         RESERVED_CACHE    ReservedCacheStats;
    ///         TSHARE_CACHE TShareCacheStats;
    ///         ULONG        Reserved[20];
    ///     } Specific;
    /// } CACHE_STATISTICS, * PCACHE_STATISTICS;
    /// 
    /// typedef struct _PROTOCOLSTATUS { (size = 1012)
    ///     PROTOCOLCOUNTERS Output;
    ///     PROTOCOLCOUNTERS Input;
    ///     CACHE_STATISTICS Cache;
    ///     ULONG AsyncSignal;     
    ///     ULONG AsyncSignalMask; 
    /// } PROTOCOLSTATUS, * PPROTOCOLSTATUS;
    /// 
    /// #define DOMAIN_LENGTH             17
    /// #define USERNAME_LENGTH           20
    /// 
    /// typedef struct _WINSTATIONINFORMATION {
    ///     WINSTATIONSTATECLASS ConnectState;
    ///     WINSTATIONNAME WinStationName;
    ///     ULONG LogonId;
    ///     LARGE_INTEGER ConnectTime; // There seems to be an extra int just before this field
    ///     LARGE_INTEGER DisconnectTime;
    ///     LARGE_INTEGER LastInputTime;
    ///     LARGE_INTEGER LogonTime;
    ///     PROTOCOLSTATUS Status;
    ///     WCHAR Domain[DOMAIN_LENGTH + 1]; // This is incorrect; it should be USERNAME_LENGTH + 1
    ///     WCHAR UserName[USERNAME_LENGTH + 1];
    ///     LARGE_INTEGER CurrentTime;
    /// } WINSTATIONINFORMATION, * PWINSTATIONINFORMATION;
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WINSTATIONINFORMATIONW
    {
        public ConnectionState State;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string WinStationName;

        public int SessionId;
        public int Unknown;
        public FILETIME ConnectTime;
        public FILETIME DisconnectTime;
        public FILETIME LastInputTime;
        public FILETIME LoginTime;
        public PROTOCOLSTATUS ProtocolStatus;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
        public string Domain;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string UserName;

        public FILETIME CurrentTime;
    }
}