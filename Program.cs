/*

    LCVersion - macOS Mach-O Load Command LC_VERSION_MIN_MACOSX version and sdk editor C# console application

    LCVersion reads/writes the Mach-O (https://en.wikipedia.org/wiki/Mach-O) LC_VERSION_MIN_MACOSX load command in macOS applications, dylibs and some other binaries.
    It can be used to edit applications or libraries that do not contain useful version or sdk information which prevents notarisation (https://developer.apple.com/documentation/xcode/notarizing_macos_software_before_distribution) by Apple. Notarisation currently requires the sdk to be 10.9 or newer.

    Unity 2017.4 LTS and some versions of Unity 2018 ship with Mono libraries that are set to sdk 10.6 which prevents notarisation with the message: "The binary uses an SDK older than the 10.9 SDK."
    - YourUnity.app/Contents/Frameworks/Mono/MonoEmbedRuntime/osx/libMonoPosixHelper.dylib
    - YourUnity.app/Contents/Frameworks/Mono/MonoEmbedRuntime/osx/libmono.0.dylib


    Usage (Mono/macOS):
        > mono lcversion.exe pathToAppOrDylib [version sdk]

    Print current LC_VERSION_MIN_MACOSX:
        > mono lcversion.exe pathToAppOrDylib

        > mono lcversion.exe YourUnity.app/Contents/Frameworks/Mono/MonoEmbedRuntime/osx/libMonoPosixHelper.dylib
        Found LC_VERSION_MIN_MACOSX at offset 1568 (0x620)
        Current version 10.6.0 sdk 10.6.0

    Set new LC_VERSION_MIN_MACOSX:
        > mono lcversion.exe pathToAppOrDylib 10.9.0 10.12.0

    Check with otool:
        > otool -l pathToAppOrDylib | fgrep -A 3 LC_VERSION_MIN_MACOSX

    Verify changes with  Hex Fiend (https://ridiculousfish.com/hexfiend/).


    Reference:
        https://en.wikipedia.org/wiki/Mach-O
        Mach-O headers/mach-o/loader.h https://opensource.apple.com/source/xnu/xnu-2050.18.24/EXTERNAL_HEADERS/mach-o/loader.h
        http://www.discretecosine.com/hacking-framework-macos-versions/


    Author:
        Greg Harding greg@flightless.co.nz
        www.flightless.co.nz
    
    Copyright 2020 Flightless Ltd.


    The MIT License (MIT)
    
    Copyright (c) 2020 Flightless Ltd
    
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

*/

using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text.RegularExpressions;


namespace LCVersion
{
    class Program
    {
        private static string usage = @"Usage: lcversion file [version sdk]";

        public enum Mode
        {
            Read,
            Write
        }


        public static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 3)
            {
                Console.WriteLine(usage);
                return;
            }

            var mode = (args.Length < 3) ? Mode.Read : Mode.Write ;

            var fileName = args[0];
            var versionString = (mode == Mode.Write) ? args[1] : null;
            var sdkString = (mode == Mode.Write) ? args[2] : null;

            ProcessFile(fileName, versionString, sdkString);
        }

        private static void ProcessFile(string fileName, string versionString = null, string sdkString = null)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"Error: File '{fileName}' not found!");
                return;
            }

            // mode read/write
            var mode = (!string.IsNullOrEmpty(versionString) && !string.IsNullOrEmpty(sdkString)) ? Mode.Write : Mode.Read ;

            ushort versionX = 0;
            byte versionY = 0, versionZ = 0;
            ushort sdkX = 0;
            byte sdkY = 0, sdkZ = 0;

            if (mode == Mode.Write)
            {
                // convert version and sdk
                if (!ParseVersion(versionString, out versionX, out versionY, out versionZ))
                {
                    Console.WriteLine($"New version {versionString} is invalid!");
                    return;
                }

                if (!ParseVersion(sdkString, out sdkX, out sdkY, out sdkZ))
                {
                    Console.WriteLine($"New sdk {sdkString} is invalid!");
                    return;
                }

                //Console.WriteLine($"New LC_VERSION_MIN_MACOSX version {versionX}.{versionY}.{versionZ} sdk {sdkX}.{sdkY}.{sdkZ}");
            }

            // open memory mapped source file
            using (var mmf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open))
            {
                var readStream = mmf.CreateViewStream();

                using (var br = new BinaryReader(readStream))
                {
                    // read mach_header_64
                    var header64 = ReadHeader64(br);
                    if (header64 == null)
                        return;

                    // find LC_VERSION_MIN_MACOSX
                    var lcPosition = FindLoadCommand(br, header64, MachO.LC_VERSION_MIN_MACOSX);
                    if (lcPosition < 0)
                    {
                        Console.WriteLine($"Could not find LC_VERSION_MIN_MACOSX!");
                        return;
                    }

                    Console.WriteLine($"Found LC_VERSION_MIN_MACOSX at offset {lcPosition} (0x{lcPosition:x})");

                    // read LC_VERSION_MIN_MACOSX
                    var lcVersionMinCommand = ReadVersionMinCommand(br, lcPosition);
                    if (lcVersionMinCommand == null)
                    {
                        Console.WriteLine($"Could not read LC_VERSION_MIN_MACOSX!");
                        return;
                    }

                    var version = lcVersionMinCommand.Version;
                    var sdk = lcVersionMinCommand.SDK;

                    Console.WriteLine($"Current version {version.x}.{version.y}.{version.z} sdk {sdk.x}.{sdk.y}.{sdk.z}");

                    // read only?
                    if (mode == Mode.Read)
                        return;

                    // write new versions
                    lcVersionMinCommand.Version = (versionX, versionY, versionZ);
                    lcVersionMinCommand.SDK = (sdkX, sdkY, sdkZ);

                    var writeStream = mmf.CreateViewStream();

                    using (var bw = new BinaryWriter(writeStream))
                    {
                        // offset to version_min_command payload
                        bw.Seek((int)lcPosition + MachO.LoadCommand.basesize, SeekOrigin.Begin);

                        bw.Write(lcVersionMinCommand.version);
                        bw.Write(lcVersionMinCommand.sdk);
                    }

                    // verify written versions
                    br.BaseStream.Position = lcPosition;

                    // read LC_VERSION_MIN_MACOSX
                    lcVersionMinCommand = ReadVersionMinCommand(br, lcPosition);
                    if (lcVersionMinCommand == null)
                    {
                        Console.WriteLine($"Could not read new LC_VERSION_MIN_MACOSX!");
                        return;
                    }

                    version = lcVersionMinCommand.Version;
                    sdk = lcVersionMinCommand.SDK;

                    Console.WriteLine($"    New version {version.x}.{version.y}.{version.z} sdk {sdk.x}.{sdk.y}.{sdk.z}");
                }
            }
        }

        private static MachO.Header64 ReadHeader64(BinaryReader br)
        {
            try
            {
                var magic = br.ReadUInt32();

                // only support the matching magic header, endian swapping currently not supported
                if (magic != MachO.MH_MAGIC_64)
                    throw new NotSupportedException($"Unsupported magic header 0x{magic:x}.");

                var header64 = new MachO.Header64()
                {
                    magic       = magic,
                    cputype     = br.ReadInt32(),
                    cpusubtype  = br.ReadInt32(),
                    filetype    = br.ReadUInt32(),
                    cmds        = br.ReadUInt32(),
                    sizeofcmds  = br.ReadUInt32(),
                    flags       = br.ReadUInt32(),
                    reserved    = br.ReadUInt32()
                };

                return header64;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: could not read mach_header_64! {e}");
                return null;
            }
        }

        private static bool ReadLoadCommands(BinaryReader br, MachO.Header64 mach_header_64)
        {
            try
            {
                Console.WriteLine("Load Commands;");

                for (int c = 0; c < mach_header_64.cmds; c++)
                {
                    var lcPosition = br.BaseStream.Position;

                    var loadCommand = new MachO.LoadCommand()
                    {
                        cmd     = br.ReadUInt32(),
                        cmdsize = br.ReadUInt32()
                    };

                    Console.WriteLine($"{MachO.LoadCommand.LoadCommandName(loadCommand.cmd)} pos {lcPosition} (0x{lcPosition:x})");

                    // skip over load command payload
                    br.BaseStream.Position += loadCommand.cmdsize - MachO.LoadCommand.basesize;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: Could not read load commands! {e}");
                return false;
            }

            return true;
        }

        private static long FindLoadCommand(BinaryReader br, MachO.Header64 header64, uint cmd)
        {
            try
            {
                var loadCommand = new MachO.LoadCommand();

                for (int c = 0; c < header64.cmds; c++)
                {
                    var lcPosition = br.BaseStream.Position;

                    loadCommand.cmd     = br.ReadUInt32();
                    loadCommand.cmdsize = br.ReadUInt32();

                    // found command?
                    if (loadCommand.cmd == cmd)
                        return lcPosition;

                    // skip over any remaining load command structure
                    br.BaseStream.Position += loadCommand.cmdsize - 8;
                }

                return -1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: Could not find {MachO.LoadCommand.LoadCommandName(cmd)}! {e}");
                return -1;
            }
        }

        private static MachO.LCVersionMinCommand ReadVersionMinCommand(BinaryReader br, long lcPosition)
        {
            try
            {
                br.BaseStream.Position = lcPosition;

                var versionMinCommand = new MachO.LCVersionMinCommand()
                {
                    cmd     = br.ReadUInt32(),
                    cmdsize = br.ReadUInt32(),
                    version = br.ReadUInt32(),
                    sdk     = br.ReadUInt32()
                };

                return versionMinCommand;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: Could not read LC_VERSION_MIN_MACOSX! {e}");
                return null;
            }
        }

        private static bool ParseVersion(string versionString, out ushort x, out byte y, out byte z)
        {
            x = y = z = 0;

            var pattern = @"^(\d+)\.(\d+)\.(\d+)$"; // 10.9.0
            var match = Regex.Match(versionString, pattern, RegexOptions.Singleline);

            if (match.Success)
            {
                int _x = Int32.Parse(match.Groups[1].Value);
                int _y = Int32.Parse(match.Groups[2].Value);
                int _z = Int32.Parse(match.Groups[3].Value);

                // 10.6.0 - 10.20.255 (much future)
                if (_x == 10 && (_y >= 6 && _y <= 20) && (_z >= 0 && _z <= 255))
                {
                    x = (ushort)_x;
                    y = (byte)_y;
                    z = (byte)_z;
                    return true;
                }
            }

            return false;
        }
    }    
}