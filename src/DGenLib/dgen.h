#pragma once

#include <Windows.h>

#define		strcasecmp	_stricmp
#define		strncasecmp	_strnicmp
#define		snprintf(buf,len, format,...) _snprintf_s(buf, len,len, format, __VA_ARGS__)
#define		__func__	__FUNCTION__

#define eInputUp		0
#define eInputDown		1
#define eInputLeft		2
#define eInputRight		3
#define eInputB			4
#define eInputC			5
#define eInputA			6
#define eInputStart		7
#define eInputZ			8
#define eInputY			9
#define eInputX			10
#define eInputMode		11
#define eInput_COUNT	12

extern int		InitDGen(int windowWidth, int windowHeight, HWND parent);
extern void		SetDGenWindowPosition(int x, int y);
extern int		GetDGenWindowXPosition();
extern int		GetDGenWindowYPosition();
extern void		BringToFront();
extern int		LoadRom(const char* path);
extern int		Reset();
extern int		Shutdown();

extern void		ShowSDLWindow();
extern void		HideSDLWindow();

extern int		AddBreakpoint(int addr);
extern void		ClearBreakpoints();
extern int		AddWatchPoint(int fromAddr, int toAddr);
extern void		ClearWatchpoints();

extern int		KeyPressed(int vkCode, int keyDown);

extern int		StepInto();
extern int		Resume();
extern int		Break();
extern int		IsDebugging();
extern unsigned int* GetProfilerResults(int* instructionCount);
extern unsigned int GetInstructionCycleCount(unsigned int address);

extern int		UpdateDGen();

extern int		GetDReg(int index);
extern int		GetAReg(int index);
extern int		GetSR();
extern int		GetCurrentPC();
extern void		ReadMemory(unsigned int address, unsigned int size, BYTE* memory);

extern void		SetInputMapping(int input, int mapping);
extern int		GetInputMapping(int input);

extern int		GetPaletteEntry(int index);

extern unsigned char GetVDPRegisterValue(int index);