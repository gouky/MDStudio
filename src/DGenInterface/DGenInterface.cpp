// This is the main DLL file.

#include "stdafx.h"
#include <msclr\marshal.h>

#include "DGenInterface.h"
#include "../DGenLib/dgen.h"

#include <sdl.h>

using namespace System;
using namespace msclr::interop;

DGenInterface::DGen::DGen()
{
}

DGenInterface::DGen::~DGen()
{
	::Shutdown();
}

int DGenInterface::DGen::Init(int windowWidth, int windowHeight, IntPtr hwnd)
{
	return ::InitDGen(windowWidth, windowHeight, static_cast<HWND>(hwnd.ToPointer()));
}

void DGenInterface::DGen::SetWindowPosition(int x, int y)
{
	::SetDGenWindowPosition(x, y);
}

int	DGenInterface::DGen::GetWindowXPosition()
{
	return ::GetDGenWindowXPosition();
}

int	DGenInterface::DGen::GetWindowYPosition()
{
	return ::GetDGenWindowYPosition();
}

int DGenInterface::DGen::Reset()
{
	return ::Reset();
}

void DGenInterface::DGen::BringToFront()
{
	::BringToFront();
}

int DGenInterface::DGen::LoadRom(String^ path)
{
	marshal_context^ context = gcnew marshal_context();
	const char* result = context->marshal_as<const char*>(path);
	return ::LoadRom(result);
	delete context;
}

int DGenInterface::DGen::Update()
{
	return ::UpdateDGen();
}

int DGenInterface::DGen::AddBreakpoint(int addr)
{
	return ::AddBreakpoint(addr);
}

void	DGenInterface::DGen::ClearBreakpoints()
{
	::ClearBreakpoints();
}

int DGenInterface::DGen::AddWatchPoint(int fromAddr, int toAddr)
{
	return ::AddWatchPoint(fromAddr, toAddr);
}

void DGenInterface::DGen::ClearWatchpoints()
{
	::ClearWatchpoints();
}

int DGenInterface::DGen::StepInto()
{
	return ::StepInto();
}

bool DGenInterface::DGen::IsDebugging()
{
	return ::IsDebugging() == 1 ? true : false;
}

unsigned int* DGenInterface::DGen::GetProfilerResults(int* instructionCount)
{
	return ::GetProfilerResults(instructionCount);
}

unsigned int DGenInterface::DGen::GetInstructionCycleCount(unsigned int address)
{
	return ::GetInstructionCycleCount(address);
}

int DGenInterface::DGen::GetDReg(int index)
{
	return ::GetDReg(index);
}

int DGenInterface::DGen::GetAReg(int index)
{
	return ::GetAReg(index);
}

int DGenInterface::DGen::GetSR()
{
	return ::GetSR();
}

int DGenInterface::DGen::GetCurrentPC()
{
	return ::GetCurrentPC();
}

int DGenInterface::DGen::Resume()
{
	return ::Resume();
}

int DGenInterface::DGen::Break()
{
	return ::Break();
}

int DGenInterface::DGen::GetRegisters()
{
	return 0;
}

void DGenInterface::DGen::ReadMemory(unsigned int address, unsigned int size, BYTE* memory)
{
	::ReadMemory(address, size, memory);
}

void DGenInterface::DGen::SetInputMapping(SDLInputs input, int mapping)
{
	::SetInputMapping((int)input, mapping);
}

int DGenInterface::DGen::GetInputMapping(SDLInputs input)
{
	return ::GetInputMapping((int)input);
}

void	DGenInterface::DGen::Show()
{
	ShowSDLWindow();
}

void	DGenInterface::DGen::Hide()
{
	HideSDLWindow();
}

void	DGenInterface::DGen::KeyPressed(int vkCode, int keyDown)
{
	::KeyPressed(vkCode, keyDown);
}

int DGenInterface::DGen::GetColor(int i)
{
	return ::GetPaletteEntry(i);
}

unsigned char DGenInterface::DGen::GetVDPRegisterValue(int index)
{
	return ::GetVDPRegisterValue(index);
}