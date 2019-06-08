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

		enum class Z80Regs
		{
			Z80_REG_FA,
			Z80_REG_CB,
			Z80_REG_ED,
			Z80_REG_LH,
			Z80_REG_FA_ALT,
			Z80_REG_CB_ALT,
			Z80_REG_ED_ALT,
			Z80_REG_LH_ALT,
			Z80_REG_IX,
			Z80_REG_IY,
			Z80_REG_SP,
			Z80_REG_PC
		};

		DGen();
		~DGen();

		int		Init(int windowWidth, int windowHeight, IntPtr hwnd, bool pal, char region);
		void	SetWindowPosition(int x, int y);
		int		GetWindowXPosition();
		int		GetWindowYPosition();
		void	BringToFront();
		int		Reset();
		void	SoftReset();
		int		LoadRom(String^ path);
		int		Update();

		int		AddBreakpoint(int addr);
		void	ClearBreakpoint(int addr);
		void	ClearBreakpoints();
		int		AddWatchpoint(int fromAddr, int toAddr);
		void	ClearWatchpoint(int fromAddr);
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

		int		GetZ80Reg(Z80Regs reg);
		unsigned char	ReadZ80Byte(unsigned int address);

		void	SetInputMapping(SDLInputs input, int mapping);
		int		GetInputMapping(SDLInputs input);

		int		GetColor(int index);

		unsigned char	GetVDPRegisterValue(int index);
	};
}
