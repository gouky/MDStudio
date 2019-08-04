MDStudio
========

MDStudio consist of an IDE and source level debugger to write Sega Megadrive assembly code. I've started it as I wanted a tool to source level debug my first step in the wonderful world of the Megadrive. Once the tool was more or less usable I've contacted Matt from Big Evil Corporation as he was developing the excellent game TangleWood and proposed it to give it a try. Matt liked it and joined the development effort to improve and debug it. MDStudio wouldn't be in this current state without his un-valuable help.

It uses DGen as emulator back-end, however new back-end are being in the work to support UMDK or TiTan cartdev for used on real hardware to run and debug step by step.

MDStudio tried to match as much as possible the same short-cut as Visual Studio (F7 to assemble, F5 to run, Shift-F5 to end, F9 to put a breakpoint, etc...).

A bunch of samples are provided courtesy of Matt from Big Evil Corporation and you can found more here: https://github.com/BigEvilCorporation/megadrive_samples as well as his awesome tutorials here: https://blog.bigevilcorporation.co.uk/2012/02/28/sega-megadrive-1-getting-started/

This tool is provided as is without any guarantee, it is still under heavy development so make sure to save regularly your work! 

In the configuration panel you can set:
	- Path to asm68k.exe
	- Extra arguments passed to asm68k
	- Emulator resolution
	- Open last project on launch
	- Refresh rate
	- Region
	- ASM Include
	- Emulator input setup
	
Pre-requisites
=============

MDStudio can be built using Visual Studio 2017 and use SDL2 for the emulator rendering part. It use DGen as Megadrive emulator and the text editor control from DigitalRune.

To assemble code you will need asm68k.exe and set the path to the executable in the options panel.

License
=======

MDStudio is released under GPL V3 license term.

Credits
=======

Nicolas 'Gouky' Hamel - mdstudio@gouky.com
Matt Phillips

3rd party library:

DGen - http://dgen.sourceforge.net/
DigitalRune .NET TextEditor Control

History
=======

Version 0.1:
	- Initial release

Website: https://github.com/gouky/MDStudio