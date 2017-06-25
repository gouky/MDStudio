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

int DGenInterface::DGen::Init()
{
	return ::InitDGen();
}

int DGenInterface::DGen::Reset()
{
	return ::Reset();
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

int DGenInterface::DGen::StepInto()
{
	return ::StepInto();
}

bool DGenInterface::DGen::IsDebugging()
{
	return ::IsDebugging() == 1 ? true : false;
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

void	DGenInterface::DGen::Show()
{
	ShowSDLWindow();
}

void	DGenInterface::DGen::Hide()
{
	HideSDLWindow();
}