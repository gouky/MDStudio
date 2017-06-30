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

		int		Init(int windowWidth, int windowHeight, IntPtr hwnd);
		void	SetWindowPosition(int x, int y);
		int		GetWindowXPosition();
		int		GetWindowYPosition();
		void	BringToFront();
		int		Reset();
		int		LoadRom(String^ path);
		int		Update();

		int		AddBreakpoint(int addr);
		void	ClearBreakpoints();
		int		StepInto();
		int		Resume();
		int		Break();
		bool	IsDebugging();

		void	Show();
		void	Hide();

		///<	Accessors
		int		GetDReg(int index);
		int		GetAReg(int index);
		int		GetSR();
		int		GetCurrentPC();
		int		GetRegisters();

		void	SetInputMapping(SDLInputs input, int mapping);
		int		GetInputMapping(SDLInputs input);
	};
}
