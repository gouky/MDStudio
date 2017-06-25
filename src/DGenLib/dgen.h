#pragma once

#define		strcasecmp	_stricmp
#define		strncasecmp	_strnicmp
#define		snprintf(buf,len, format,...) _snprintf_s(buf, len,len, format, __VA_ARGS__)
#define		__func__	__FUNCTION__

extern int		InitDGen(int windowWidth, int windowHeight);
extern int		LoadRom(const char* path);
extern int		Reset();
extern int		Shutdown();

extern void		ShowSDLWindow();
extern void		HideSDLWindow();

extern int		AddBreakpoint(int addr);
extern void		ClearBreakpoints();

extern int		StepInto();
extern int		Resume();
extern int		Break();
extern int		IsDebugging();

extern int		UpdateDGen();

extern int		GetDReg(int index);
extern int		GetAReg(int index);
extern int		GetSR();
extern int		GetCurrentPC();

