#pragma once

#include <ttndev_lib.h>

using namespace System;

namespace TiTAN
{
	public ref class TiTANInterface
	{
	public:
		TiTANInterface();
		~TiTANInterface();

		enum class TTN_REGS
		{
			D0,
			D1,
			D2,
			D3,
			D4,
			D5,
			D6,
			D7,
			A0,
			A1,
			A2,
			A3,
			A4,
			A5,
			A6,
			SP,
			PC,
			SR,

			COUNT
		};

		bool Initialise();
		void Shutdown();

		bool LoadBinary(String^ filename);

		void Break();
		void Resume();
		void Step();
		bool IsHalted();

		bool SetBreakpoint(int address);
		void ClearBreakpoint(int address);
		void ClearAllBreakpoints();

		int GetReg(TTN_REGS reg);
		void ReadMemory(unsigned int address, unsigned int size, unsigned char* memory);
	};
}
