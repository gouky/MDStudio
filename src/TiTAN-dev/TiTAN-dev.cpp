#include "stdafx.h"
#include "TiTAN-dev.h"

#include <iostream>

using namespace System::Runtime::InteropServices;

namespace TiTAN
{
	TiTANInterface::TiTANInterface()
	{
		
	}

	TiTANInterface::~TiTANInterface()
	{

	}

	bool TiTANInterface::Initialise()
	{
		return ttn_initialise() == TTN_RESULT::SUCCESS;
	}

	void TiTANInterface::Shutdown()
	{
		ttn_shutdown();
	}

	bool TiTANInterface::LoadBinary(String^ filename)
	{
		const char* str = (const char*)(Marshal::StringToHGlobalAnsi(filename)).ToPointer();
		return ttn_open(str) == TTN_RESULT::SUCCESS;
	}

	void TiTANInterface::Break()
	{
		ttn_break();
	}

	void TiTANInterface::Resume()
	{
		ttn_resume();
	}

	void TiTANInterface::Step()
	{
		ttn_step();
	}

	bool TiTANInterface::IsHalted()
	{
		return ttn_get_state() == TTN_STATE::BREAK;
	}

	bool TiTANInterface::SetBreakpoint(int address)
	{
 		TTN_RESULT result = ttn_set_breakpoint(address);

		if (result == TTN_RESULT::NO_FREE_BREAKPOINTS)
		{
			std::cout << "TiTANInterface::SetBreakpoint() - Unable to set breakpoint at " << address << ", no free breakpoints" << std::endl;
		}

		return result == TTN_RESULT::SUCCESS;
	}

	void TiTANInterface::ClearBreakpoint(int address)
	{
		TTN_RESULT result = ttn_clear_breakpoint(address);

		if (result == TTN_RESULT::INVALID_BREAKPOINT)
		{
			std::cout << "TiTANInterface::ClearBreakpoint() - Unable to clear breakpoint at " << address << ", breakpoint not set" << std::endl;
		}
	}

	void TiTANInterface::ClearAllBreakpoints()
	{
		ttn_clear_all_breakpoints();
	}

	int TiTANInterface::GetReg(TTN_REGS reg)
	{
		int value = 0;
		ttn_get_reg((::TTN_REGS)reg, value);
		return value;
	}

	void TiTANInterface::ReadMemory(unsigned int address, unsigned int size, unsigned char* memory)
	{
		ttn_get_memory(address, size, (char*)memory);
	}
}
