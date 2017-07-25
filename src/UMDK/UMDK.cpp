/*
 *			UMDK V2 MDStudio Interface
 *
 *	Mostly based on all the code made available by Chris McClelland
 *
 */

#include "stdafx.h"

#include <stdio.h>
#include <stdlib.h>

#include <libfpgalink.h>

#include "UMDK.h"

using namespace System::Runtime::InteropServices;

static struct FLContext* s_DeviceHandle = NULL;
static const char *g_error = NULL;

#define	MAXIMUM_BREAKPOINTS		32

struct BreakPoint
{
	uint32	m_Address;
	uint16	m_Save;
	bool	m_Used;
};

static BreakPoint	s_Breakpoints[32];

// 68000 registers:
struct Registers {
	uint32 d0;
	uint32 d1;
	uint32 d2;
	uint32 d3;
	uint32 d4;
	uint32 d5;
	uint32 d6;
	uint32 d7;

	uint32 a0;
	uint32 a1;
	uint32 a2;
	uint32 a3;
	uint32 a4;
	uint32 a5;
	uint32 fp;
	uint32 sp;

	uint32 sr;
	uint32 pc;
};

typedef enum {
	D0, D1, D2, D3, D4, D5, D6, D7,
	A0, A1, A2, A3, A4, A5, FP, SP,
	SR, PC
} Register;

typedef enum {
	CF_RUNNING,
	CF_READY,
	CF_CMD
} CmdFlag;

typedef enum {
	CMD_STEP,
	CMD_CONT,
	CMD_READ,
	CMD_WRITE,
	CMD_RESET
} Command;

#define ILLEGAL  0x4AFC
#define IL_VEC   0x000010
#define TR_VEC   0x000024
#define VB_VEC   0x000078
#define MONITOR  0x400000
#define CB_FLAG  (MONITOR + 0x400)
#define CB_INDEX (MONITOR + 0x402)
#define CB_ADDR  (MONITOR + 0x404)
#define CB_LEN   (MONITOR + 0x408)
#define CB_REGS  (MONITOR + 0x40C)
#define CB_MEM   (MONITOR + 0x454)


UMDK::UMDKInterface::UMDKInterface()
{

}

UMDK::UMDKInterface::~UMDKInterface()
{

}

int UMDK::UMDKInterface::Open()
{
	if (s_DeviceHandle)
		return 1;

	const char*		vp = "1d50:602b";
	const char*		error = NULL;
	unsigned char	flag = 0;
	FLStatus		status;

	status = flInitialise(0, &error);
	status = flIsDeviceAvailable(vp, &flag, &error);
	status = flOpen(vp, &s_DeviceHandle, &error);
	status = flSelectConduit(s_DeviceHandle, 0x01, &error);

	return (status == FL_SUCCESS);
}

int	UMDK::UMDKInterface::Reset()
{
	FLStatus status;
	unsigned char	byte = 0x01;
	const char*		error = NULL;

	if (!s_DeviceHandle)
	{
		return 0;
	}

	status = flWriteChannel(s_DeviceHandle, 0x01, 1, &byte, &error);

	// Write CF_RUNNING (0x0000) to CB_FLAG, to show that MD is running
	unsigned char	command[10];

	uint32 flagAddr = 0xF80400 / 2;
	command[0] = 0x00; // set mem-pipe pointer
	command[3] = (uint8)flagAddr;
	flagAddr >>= 8;
	command[2] = (uint8)flagAddr;
	flagAddr >>= 8;
	command[1] = (uint8)flagAddr;
	command[4] = 0x80; // write words to SDRAM
	command[5] = 0x00;
	command[6] = 0x00;
	command[7] = 0x01; // one 16-bit word
	command[8] = 0x00; // CF_RUNNING
	command[9] = 0x00;
	
	status = flWriteChannelAsync(s_DeviceHandle, 0x00, 10, command, &error);
	
	return 1;
}

int	UMDK::UMDKInterface::Close()
{
	if(s_DeviceHandle)
		flClose(s_DeviceHandle);

	s_DeviceHandle = NULL;

	return 1;
}

int UMDK::UMDKInterface::HoldReset()
{
	unsigned char	byte = 0x01;
	const char*		error = NULL;
	FLStatus status;

	if (!s_DeviceHandle)
		return 0;

	status = flWriteChannel(s_DeviceHandle, 0x01, 1, &byte, &error);

	return (status == FL_SUCCESS);
}

int UMDK::UMDKInterface::Start()
{
	const char*		error = NULL;
	FLStatus status;

	if (!s_DeviceHandle)
		return 0;

	unsigned char command[10];

	// Write CF_RUNNING (0x0000) to CB_FLAG, to show that MD is running
	uint32 flagAddr = 0xF80400 / 2;
	command[0] = 0x00; // set mem-pipe pointer
	command[3] = (uint8)flagAddr;
	flagAddr >>= 8;
	command[2] = (uint8)flagAddr;
	flagAddr >>= 8;
	command[1] = (uint8)flagAddr;
	command[4] = 0x80; // write words to SDRAM
	command[5] = 0x00;
	command[6] = 0x00;
	command[7] = 0x01; // one 16-bit word
	command[8] = 0x00; // CF_RUNNING
	command[9] = 0x00;
	status = flWriteChannelAsync(s_DeviceHandle, 0x00, 10, command, &error);
	if (status == FL_SUCCESS)
	{
		unsigned char byte = 0x00;

		status = flWriteChannelAsync(s_DeviceHandle, 0x01, 1, &byte, &error);

		return (status == FL_SUCCESS);
	}

	return 0;
}

int UMDK::UMDKInterface::WriteFile(String^ pathToBinary)
{
	if (!s_DeviceHandle)
		return 0;

	void*	buffer;
	size_t	dataSize;
	FILE*	hFile = NULL;
	const char* str = (const char*)(Marshal::StringToHGlobalAnsi(pathToBinary)).ToPointer();

	buffer = flLoadFile(/*"D:\\sonic1.gen"*/str, &dataSize);
	if (buffer)
	{
		HoldReset();
		Write(buffer, dataSize);
		Start();
	}

/*
	fopen_s(&hFile, "D:\\sonic1.gen", "rb");
	if(hFile)
	{
		fseek(hFile, 0, SEEK_END);
		dataSize = ftell(hFile);
		fseek(hFile, 0, SEEK_SET);
		buffer = malloc(dataSize);
		fread(buffer, 1, dataSize, hFile);
		fclose(hFile);

 		HoldReset();
		Write(buffer, dataSize);
		Start();

		free(buffer);

		return 1;
	}*/

	return 0;
}

int UMDK::UMDKInterface::Write(void* data, size_t dataSize)
{
	const char*		error = NULL;
	unsigned char	command[10];
	FLStatus		status;

	// Now send actual data
	unsigned int	address = 0;
	size_t			numWords = dataSize / 2;

	if (!s_DeviceHandle)
	{
		return 0;
	}

	command[0] = 0x00; // set mem-pipe pointer
	command[3] = (unsigned char)address;
	address >>= 8;
	command[2] = (unsigned char)address;
	address >>= 8;
	command[1] = (unsigned char)address;
	command[4] = 0x80; // write words to SDRAM
	command[7] = (unsigned char)numWords;
	numWords >>= 8;
	command[6] = (unsigned char)numWords;
	numWords >>= 8;
	command[5] = (unsigned char)numWords;

	status = flWriteChannelAsync(s_DeviceHandle, 0x00, 8, command, &error);
	status = flWriteChannel(s_DeviceHandle, 0x00, dataSize, (unsigned char*)data, &error);

	return 1;
}

int	UMDK::UMDKInterface::cmdCreateBreakpoint(int type, int addr, int kind)
{
	//	find a slot
	uint32	freebkp = (uint32)-1;

	for (uint32 i = 0; i < MAXIMUM_BREAKPOINTS; ++i)
	{
		if (!s_Breakpoints[i].m_Used)
		{
			freebkp = i;
			break;
		}
	}

	if (freebkp == (uint32)-1)
	{
		return 0;
	}

	s_Breakpoints[freebkp].m_Used = true;
	s_Breakpoints[freebkp].m_Address = addr;

 	umdkReadWord(addr, &s_Breakpoints[freebkp].m_Save, &g_error);
 	umdkWriteWord(addr, ILLEGAL, &g_error);

	return 1;
}

int	UMDK::UMDKInterface::umdkReadWord(const uint32 address, uint16 *const pValue, const char **error)
{
	return 1;
}

int	UMDK::UMDKInterface::umdkWriteWord(const uint32 address, uint16 value, const char **error)
{
	return 1;
}

