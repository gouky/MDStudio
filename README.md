
# MDStudio

![](https://github.com/gouky/MDStudio/blob/master/MDStudio.png)

MDStudio consists of an IDE and source level debugger to write Sega Mega Drive assembly code. I started it as I wanted a tool to source level debug my first step in the wonderful world of the Mega Drive. Once the tool was more or less usable I contacted Matt from Big Evil Corporation as he was developing the excellent game TangleWood and proposed he give it a try. Matt liked it and joined the development effort to improve and debug it. MDStudio wouldn't be in this current state without his invaluable help.

It uses DGen as emulator back-end, however new back-end are being in the work to support UMDK or TiTan cartdev for used on real hardware to run and debug step by step.

MDStudio tried to match as much as possible the same short-cut as Visual Studio (F7 to assemble, F5 to run, Shift-F5 to end, F9 to put a breakpoint, etc...).

A bunch of samples are provided courtesy of Matt from Big Evil Corporation and you can find more here: https://github.com/BigEvilCorporation/megadrive_samples as well as his awesome tutorials here: https://blog.bigevilcorporation.co.uk/2012/02/28/sega-megadrive-1-getting-started/

This tool is provided as-is without any guarantee, it is still under heavy development so make sure to save your work regularly! 

In the configuration panel you can set:
- Path to asm68k.exe
- Extra arguments passed to asm68k
- Emulator resolution
- Open last project on launch
- Refresh rate
- Region
- ASM Include
- Emulator input setup

![](https://github.com/gouky/MDStudio/blob/master/mdstudio_screen.jpg)

# Features

- Source Level debugging
- Breakpoint
- Profiler
- CRam Viewer

In-progress:
- VDP Register viewer
- VDP Value helper

# Pre-requisites

MDStudio can be built using Visual Studio 2017 and use SDL2 for the emulator rendering part. It use DGen as Mega Drive emulator and the text editor control from DigitalRune.

To assemble code you will need asm68k.exe and set the path to the executable in the options panel.

# License

MDStudio is released under GPL V3 license term.

# Credits

Nicolas 'Gouky' Hamel

Matt Phillips

3rd party library:

- DGen - http://dgen.sourceforge.net/
- DigitalRune .NET TextEditor Control

# Contact

mdstudio@gouky.com

# History

Version 0.1:
- Initial release
