// DGenInterface.h

#pragma once

using namespace System;

namespace DGenInterface {

	public ref class DGen
	{
		// TODO: Add your methods for this class here.
	public:

		enum class SDLInputs
		{
			eInputUp,
			eInputDown,
			eInputLeft,
			eInputRight,
			eInputB,
			eInputC,
			eInputA,
			eInputStart,
			eInputZ,
			eInputY,
			eInputX,
			eInputMode,

			eInput_COUNT
		};

		DGen();
		~DGen();

		int		Init(int windowWidth, int windowHeight, IntPtr hwnd, bool pal, char region);
		void	SetWindowPosition(int x, int y);
		int		GetWindowXPosition();
		int		GetWindowYPosition();
		void	BringToFront();
		int		Reset();
		int		LoadRom(String^ path);
		int		Update();

		int		AddBreakpoint(int addr);
		void	ClearBreakpoints();
		int		AddWatchPoint(int fromAddr, int toAddr);
		void	ClearWatchpoints();
		int		StepInto();
		int		Resume();
		int		Break();
		bool	IsDebugging();
		unsigned int* GetProfilerResults(int* instructionCount);
		unsigned int GetInstructionCycleCount(unsigned int address);

		void	Show();
		void	Hide();

		void	KeyPressed(int vkCode, int keyDown);

		///<	Accessors
		int		GetDReg(int index);
		int		GetAReg(int index);
		int		GetSR();
		int		GetCurrentPC();
		int		GetRegisters();
		unsigned char	ReadByte(unsigned int address);
		unsigned short	ReadWord(unsigned int address);
		unsigned int	ReadLong(unsigned int address);
		void    ReadMemory(unsigned int address, unsigned int size, BYTE* memory);

		void	SetInputMapping(SDLInputs input, int mapping);
		int		GetInputMapping(SDLInputs input);

		int		GetColor(int index);

		unsigned char	GetVDPRegisterValue(int index);
	};
}
