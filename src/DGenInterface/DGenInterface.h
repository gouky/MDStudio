// DGenInterface.h

#pragma once

using namespace System;

namespace DGenInterface {

	public ref class DGen
	{
		// TODO: Add your methods for this class here.
	public:
		DGen();
		~DGen();

		int		Init(int windowWidth, int windowHeight);
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
	};
}
