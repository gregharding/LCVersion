# LCVersion

**macOS Mach-O Load Command LC_VERSION_MIN_MACOSX version and sdk editor C# console application.**

LCVersion reads/writes the [Mach-O](https://en.wikipedia.org/wiki/Mach-O) LC_VERSION_MIN_MACOSX load command in macOS applications, dylibs and some other binaries.
It can be used to edit applications or libraries that do not contain useful version or sdk information which prevents [notarisation](https://developer.apple.com/documentation/xcode/notarizing_macos_software_before_distribution) by Apple. Notarisation currently requires the sdk to be 10.9 or newer.

Unity 2017.4 LTS and some versions of Unity 2018 ship with Mono libraries that are set to sdk 10.6 which prevents notarisation with the message: _"The binary uses an SDK older than the 10.9 SDK."_

- YourUnity.app/Contents/Frameworks/Mono/MonoEmbedRuntime/osx/libMonoPosixHelper.dylib
- YourUnity.app/Contents/Frameworks/Mono/MonoEmbedRuntime/osx/libmono.0.dylib

**Usage**

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

Verify changes with [Hex Fiend](https://ridiculousfish.com/hexfiend/).

**Reference**

- [https://en.wikipedia.org/wiki/Mach-O](https://en.wikipedia.org/wiki/Mach-O)
- [Mach-O headers/mach-o/loader.h](https://opensource.apple.com/source/xnu/xnu-2050.18.24/EXTERNAL_HEADERS/mach-o/loader.h)
- [http://www.discretecosine.com/hacking-framework-macos-versions/](http://www.discretecosine.com/hacking-framework-macos-versions/)

**Author**

Greg Harding [http://www.flightless.co.nz](http://www.flightless.co.nz)

Copyright 2020 Flightless Ltd

**License**

> The MIT License (MIT)
> 
> Copyright (c) 2020 Flightless Ltd
> 
> Permission is hereby granted, free of charge, to any person obtaining
> a copy of this software and associated documentation files (the
> "Software"), to deal in the Software without restriction, including
> without limitation the rights to use, copy, modify, merge, publish,
> distribute, sublicense, and/or sell copies of the Software, and to
> permit persons to whom the Software is furnished to do so, subject to
> the following conditions:
> 
> The above copyright notice and this permission notice shall be
> included in all copies or substantial portions of the Software.
> 
> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
> EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
> MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
> NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
> BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
> ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
> CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
> SOFTWARE.
