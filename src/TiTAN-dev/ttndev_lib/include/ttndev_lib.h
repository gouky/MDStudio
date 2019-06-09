#pragma once

enum class TTN_RESULT
{
	SUCCESS,
	INVALID_STATE,
	NOT_INITIALISED,
	DEVICE_ERROR,
	FILE_ERROR,
	SEND_ERROR,
	RECV_ERROR,
	HANDSHAKE_ERROR,
	NO_FREE_BREAKPOINTS,
	INVALID_BREAKPOINT,
	DUPLICATE_BREAKPOINT,
	INVALID_MEMORY_RANGE
};

enum class TTN_STATE
{
	UNINITIALISED,
	BUSY,
	IDLE,
	RUNNING,
	BREAK
};

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

TTN_RESULT ttn_initialise();
TTN_RESULT ttn_shutdown();
TTN_RESULT ttn_open(const char* filename);
TTN_RESULT ttn_break();
TTN_RESULT ttn_resume();
TTN_RESULT ttn_step();
TTN_RESULT ttn_get_reg(TTN_REGS reg, int& value);
TTN_RESULT ttn_get_memory(int address, int size, char* buffer);
TTN_RESULT ttn_set_breakpoint(int address);
TTN_RESULT ttn_clear_breakpoint(int address);
TTN_RESULT ttn_clear_all_breakpoints();
TTN_STATE ttn_get_state();
