using System;

namespace LCVersion
{
    //
    // Mach-O Reference
    //
    // https://en.wikipedia.org/wiki/Mach-O
    // https://opensource.apple.com/source/xnu/xnu-2050.18.24/EXTERNAL_HEADERS/mach-o/loader.h
    // https://opensource.apple.com/source/xnu/xnu-4570.41.2/osfmk/mach/machine.h
    //

    public static class MachO
    {
        // magic headers
        public const uint MH_MAGIC = 0xfeedface;                        // the mach magic number
        public const uint MH_CIGAM = 0xcefaedfe;                        // NXSwapInt(MH_MAGIC)
        public const uint MH_MAGIC_64 = 0xfeedfacf;                     // the 64-bit mach magic number
        public const uint MH_CIGAM_64 = 0xcffaedfe;                     // NXSwapInt(MH_MAGIC_64)

        // filetypes
        public const uint MH_OBJECT = 0x1;                              // relocatable object file
        public const uint MH_EXECUTE = 0x2;                             // demand paged executable file
        public const uint MH_FVMLIB = 0x3;                              // fixed VM shared library file
        public const uint MH_CORE = 0x4;                                // core file
        public const uint MH_PRELOAD = 0x5;                             // preloaded executable file
        public const uint MH_DYLIB = 0x6;                               // dynamically bound shared library
        public const uint MH_DYLINKER = 0x7;                            // dynamic link editor
        public const uint MH_BUNDLE = 0x8;                              // dynamically bound bundle file
        public const uint MH_DYLIB_STUB = 0x9;                          // shared library stub for static linking only, no section contents
        public const uint MH_DSYM = 0xa;                                // companion file with only debug sections
        public const uint MH_KEXT_BUNDLE = 0xb;                         // x86_64 kexts

        // load commands
        public const uint LC_REQ_DYLD = 0x80000000;
        public const uint LC_SEGMENT = 0x1;      	                    // segment of this file to be mapped
        public const uint LC_SYMTAB = 0x2;      	                    // link-edit stab symbol table info
        public const uint LC_SYMSEG = 0x3;      	                    // link-edit gdb symbol table info (obsolete)
        public const uint LC_THREAD = 0x4;      	                    // thread
        public const uint LC_UNIXTHREAD = 0x5;      	                // unix thread (includes a stack)
        public const uint LC_LOADFVMLIB = 0x6;      	                // load a specified fixed VM shared library
        public const uint LC_IDFVMLIB = 0x7;      	                    // fixed VM shared library identification
        public const uint LC_IDENT = 0x8;      	                        // object identification info (obsolete)
        public const uint LC_FVMFILE = 0x9;      	                    // fixed VM file inclusion (internal use)
        public const uint LC_PREPAGE = 0xa;                             // prepage command (internal use)
        public const uint LC_DYSYMTAB = 0xb;      	                    // dynamic link-edit symbol table info
        public const uint LC_LOAD_DYLIB = 0xc;      	                // load a dynamically linked shared library
        public const uint LC_ID_DYLIB = 0xd;      	                    // dynamically linked shared lib ident
        public const uint LC_LOAD_DYLINKER = 0xe;      	                // load a dynamic linker
        public const uint LC_ID_DYLINKER = 0xf;      	                // dynamic linker identification
        public const uint LC_PREBOUND_DYLIB = 0x10;     	            // modules prebound for a dynamically linked shared library
        public const uint LC_ROUTINES = 0x11;     	                    // image routines
        public const uint LC_SUB_FRAMEWORK = 0x12;     	                // sub framework
        public const uint LC_SUB_UMBRELLA = 0x13;     	                // sub umbrella
        public const uint LC_SUB_CLIENT = 0x14;     	                // sub client
        public const uint LC_SUB_LIBRARY = 0x15;        	            // sub library
        public const uint LC_TWOLEVEL_HINTS = 0x16;     	            // two-level namespace lookup hints
        public const uint LC_PREBIND_CKSUM = 0x17;        	            // prebind checksum
        public const uint LC_LOAD_WEAK_DYLIB = 0x18 | LC_REQ_DYLD;      // load a dynamically linked shared library that is allowed to be missing (all symbols are weak imported).
        public const uint LC_SEGMENT_64 = 0x19;     	                // 64-bit segment of this file to be mapped
        public const uint LC_ROUTINES_64 = 0x1a;     	                // 64-bit image routines
        public const uint LC_UUID = 0x1b;        	                    // the uuid
        public const uint LC_RPATH = 0x1c | LC_REQ_DYLD;                // runpath additions
        public const uint LC_CODE_SIGNATURE = 0x1d;     	            // local of code signature
        public const uint LC_SEGMENT_SPLIT_INFO = 0x1e;                 // local of info to split segments
        public const uint LC_REEXPORT_DYLIB = 0x1f | LC_REQ_DYLD;       // load and re-export dylib
        public const uint LC_LAZY_LOAD_DYLIB = 0x20;     	            // delay load of dylib until first use
        public const uint LC_ENCRYPTION_INFO = 0x21;     	            // encrypted segment information
        public const uint LC_DYLD_INFO = 0x22;        	                // compressed dyld information
        public const uint LC_DYLD_INFO_ONLY = 0x22 | LC_REQ_DYLD;       // compressed dyld information only
        public const uint LC_LOAD_UPWARD_DYLIB = 0x23 | LC_REQ_DYLD;    // load upward dylib
        public const uint LC_VERSION_MIN_MACOSX = 0x24;                 // build for MacOSX min OS version
        public const uint LC_VERSION_MIN_IPHONEOS = 0x25;               // build for iPhoneOS min OS version
        public const uint LC_FUNCTION_STARTS = 0x26;                    // compressed table of function start addresses
        public const uint LC_DYLD_ENVIRONMENT = 0x27;                   // string for dyld to treat like environment variable
        public const uint LC_MAIN = 0x28 | LC_REQ_DYLD;                 // replacement for LC_UNIXTHREAD
        public const uint LC_DATA_IN_CODE = 0x29;                       // table of non-instructions in __text
        public const uint LC_SOURCE_VERSION = 0x2A;                     // source version used to build binary
        public const uint LC_DYLIB_CODE_SIGN_DRS = 0x2B;                // Code signing DRs copied from linked dylibs


        public class Header64
        {
            public uint magic;         // mach magic number identifier
            public int cputype;        // cpu specifier
            public int cpusubtype;     // machine specifier
            public uint filetype;      // type of file
            public uint cmds;          // number of load commands
            public uint sizeofcmds;    // the size of all the load commands
            public uint flags;         // flags
            public uint reserved;      // reserved


            public static string MagicName(uint magic)
            {
                switch (magic)
                {
                    case MachO.MH_MAGIC:                return "MH_MAGIC";
                    case MachO.MH_CIGAM:                return "MH_CIGAM";
                    case MachO.MH_MAGIC_64:             return "MH_MAGIC_64";
                    case MachO.MH_CIGAM_64:             return "MH_CIGAM_64";
                    default:                            return "UNKNOWN";
                };
            }

            public static string FileTypeName(uint filetype)
            {
                switch (filetype)
                {
                    case MachO.MH_OBJECT:               return "MH_OBJECT";
                    case MachO.MH_EXECUTE:              return "MH_EXECUTE";
                    case MachO.MH_FVMLIB:               return "MH_FVMLIB";
                    case MachO.MH_CORE:                 return "MH_CORE";
                    case MachO.MH_PRELOAD:              return "MH_PRELOAD";
                    case MachO.MH_DYLIB:                return "MH_DYLIB";
                    case MachO.MH_DYLINKER:             return "MH_DYLINKER";
                    case MachO.MH_BUNDLE:               return "MH_BUNDLE";
                    case MachO.MH_DYLIB_STUB:           return "MH_DYLIB_STUB";
                    case MachO.MH_DSYM:                 return "MH_DSYM";
                    case MachO.MH_KEXT_BUNDLE:          return "MH_KEXT_BUNDLE";
                    default:                            return "UNKNOWN";
                };
            }
        };


        public class LoadCommand
        {
            public uint cmd;            // type of load command
            public uint cmdsize;        // total size of command in bytes

            public const int basesize = 8;


            public static string LoadCommandName(uint cmd)
            {
                switch (cmd)
                {
                    case MachO.LC_SEGMENT:              return "LC_SEGMENT";
                    case MachO.LC_SYMTAB:               return "LC_SYMTAB";
                    case MachO.LC_SYMSEG:               return "LC_SYMSEG";
                    case MachO.LC_THREAD:               return "LC_THREAD";
                    case MachO.LC_UNIXTHREAD:           return "LC_UNIXTHREAD";
                    case MachO.LC_LOADFVMLIB:           return "LC_LOADFVMLIB";
                    case MachO.LC_IDFVMLIB:             return "LC_IDFVMLIB";
                    case MachO.LC_IDENT:                return "LC_IDENT";
                    case MachO.LC_FVMFILE:              return "LC_FVMFILE";
                    case MachO.LC_PREPAGE:              return "LC_PREPAGE";
                    case MachO.LC_DYSYMTAB:             return "LC_DYSYMTAB";
                    case MachO.LC_LOAD_DYLIB:           return "LC_LOAD_DYLIB";
                    case MachO.LC_ID_DYLIB:             return "LC_ID_DYLIB";
                    case MachO.LC_LOAD_DYLINKER:        return "LC_LOAD_DYLINKER";
                    case MachO.LC_ID_DYLINKER:          return "LC_ID_DYLINKER";
                    case MachO.LC_PREBOUND_DYLIB:       return "LC_PREBOUND_DYLIB";
                    case MachO.LC_ROUTINES:             return "LC_ROUTINES";
                    case MachO.LC_SUB_FRAMEWORK:        return "LC_SUB_FRAMEWORK";
                    case MachO.LC_SUB_UMBRELLA:         return "LC_SUB_UMBRELLA";
                    case MachO.LC_SUB_CLIENT:           return "LC_SUB_CLIENT";
                    case MachO.LC_SUB_LIBRARY:          return "LC_SUB_LIBRARY";
                    case MachO.LC_TWOLEVEL_HINTS:       return "LC_TWOLEVEL_HINTS";
                    case MachO.LC_PREBIND_CKSUM:        return "LC_PREBIND_CKSUM";
                    case MachO.LC_LOAD_WEAK_DYLIB:      return "LC_LOAD_WEAK_DYLIB";
                    case MachO.LC_SEGMENT_64:           return "LC_SEGMENT_64";
                    case MachO.LC_ROUTINES_64:          return "LC_ROUTINES_64";
                    case MachO.LC_UUID:                 return "LC_UUID";
                    case MachO.LC_RPATH:                return "LC_RPATH";
                    case MachO.LC_CODE_SIGNATURE:       return "LC_CODE_SIGNATURE";
                    case MachO.LC_SEGMENT_SPLIT_INFO:   return "LC_SEGMENT_SPLIT_INFO";
                    case MachO.LC_REEXPORT_DYLIB:       return "LC_REEXPORT_DYLIB";
                    case MachO.LC_LAZY_LOAD_DYLIB:      return "LC_LAZY_LOAD_DYLIB";
                    case MachO.LC_ENCRYPTION_INFO:      return "LC_ENCRYPTION_INFO";
                    case MachO.LC_DYLD_INFO:            return "LC_DYLD_INFO";
                    case MachO.LC_DYLD_INFO_ONLY:       return "LC_DYLD_INFO_ONLY";
                    case MachO.LC_LOAD_UPWARD_DYLIB:    return "LC_LOAD_UPWARD_DYLIB";
                    case MachO.LC_VERSION_MIN_MACOSX:   return "LC_VERSION_MIN_MACOSX";
                    case MachO.LC_VERSION_MIN_IPHONEOS: return "LC_VERSION_MIN_IPHONEOS";
                    case MachO.LC_FUNCTION_STARTS:      return "LC_FUNCTION_STARTS";
                    case MachO.LC_DYLD_ENVIRONMENT:     return "LC_DYLD_ENVIRONMENT";
                    case MachO.LC_MAIN:                 return "LC_MAIN";
                    case MachO.LC_DATA_IN_CODE:         return "LC_DATA_IN_CODE";
                    case MachO.LC_SOURCE_VERSION:       return "LC_SOURCE_VERSION";
                    case MachO.LC_DYLIB_CODE_SIGN_DRS:  return "LC_DYLIB_CODE_SIGN_DRS";
                    default:                            return "UNKNOWN";
                };
            }
        }


        public class LCVersionMinCommand : LoadCommand
        {

            //public uint cmd;		    // LC_VERSION_MIN_MACOSX or LC_VERSION_MIN_IPHONEOS
            //public uint cmdsize;	    // sizeof(struct min_version_command)
            public uint version;	    // X.Y.Z is encoded in nibbles xxxx.yy.zz
            public uint sdk;		    // X.Y.Z is encoded in nibbles xxxx.yy.zz


            public (ushort x, byte y, byte z) Version
            {
                get => Helpers.GetVersionXYZ(version);
                set => version = (uint) ((value.x << 16) + (value.y << 8) + value.z);
            }

            public (ushort x, byte y, byte z) SDK
            {
                get => Helpers.GetVersionXYZ(sdk);
                set => sdk = (uint) ((value.x << 16) + (value.y << 8) + value.z);
            }
        }


        public static class Helpers
        {
            public static (ushort xxxx, byte yy, byte zz) GetVersionXYZ(uint packedVersion)
            {
                var xxxx = (ushort) ((packedVersion & 0xFFFF0000) >> 16);
                var yy = (byte) ((packedVersion & 0x0000FF00) >> 8);
                var zz = (byte) ((packedVersion & 0x000000FF));
                return (xxxx, yy, zz);
            }
        }
    }
}