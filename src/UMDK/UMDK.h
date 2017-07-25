// UMDK.h

#pragma once

using namespace System;

namespace UMDK {

	public ref class UMDKInterface
	{
	public:
		UMDKInterface();
		~UMDKInterface();

		int		Open();
		int		Close();
		int		Reset();

		int		WriteFile(String^ pathToBinary);
		int		Write(void* data, size_t dataSize);
		
		//	UMDK Command functions
		int		cmdCreateBreakpoint(int type, int addr, int kind);

	private:
		int		HoldReset();
		int		Start();

		int		umdkReadWord(const uint32 address, uint16 *const pValue, const char **error);
		int		umdkWriteWord(const uint32 address, uint16 value, const char **error);
	};
}
