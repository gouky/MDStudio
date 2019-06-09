using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if TARGET_TITAN

using TiTAN;

namespace MDStudio
{
    class TargetTitan : HardwareTarget
    {
        private TiTANInterface m_titan;

        public TargetTitan()
        {
            m_titan = new TiTANInterface();
        }

        public override bool LoadBinary(string filename)
        {
            if(!m_titan.Initialise())
            {
                return false;
            }

            return m_titan.LoadBinary(filename);
        }

        public override void Run()
        {
        }

        public override void Reset()
        {
        }

        public override uint Break()
        {
            m_titan.Break();
            return GetPC();
        }

        public override void Resume()
        {
            m_titan.Resume();
        }

        public override uint Step()
        {
            m_titan.Step();
            return GetPC();
        }

        public override bool IsHalted()
        {
            return m_titan.IsHalted();
        }

        public override uint GetAReg(int index)
        {
            return (uint)m_titan.GetReg((TiTANInterface.TTN_REGS)(TiTANInterface.TTN_REGS.A0 + (int)index));
        }

        public override uint GetDReg(int index)
        {
            return (uint)m_titan.GetReg((TiTANInterface.TTN_REGS)(TiTANInterface.TTN_REGS.D0 + (int)index));
        }

        public override uint GetPC()
        {
            return (uint)m_titan.GetReg(TiTANInterface.TTN_REGS.PC);
        }

        public override uint GetSR()
        {
            return (uint)m_titan.GetReg(TiTANInterface.TTN_REGS.SR);
        }

        public override byte ReadByte(uint address)
        {
            byte[] memory = new byte[1];
            ReadMemory(address, 1, memory);
            return memory[0];
        }

        public override uint ReadLong(uint address)
        {
            byte[] memory = new byte[4];
            ReadMemory(address, 4, memory);
            return Endian.Swap(System.BitConverter.ToUInt32(memory, 0));
        }

        public override ushort ReadWord(uint address)
        {
            byte[] memory = new byte[2];
            ReadMemory(address, 2, memory);
            return Endian.Swap(System.BitConverter.ToUInt16(memory, 0));
        }

        public override void ReadMemory(uint address, uint size, byte[] memory)
        {
            unsafe
            {
                fixed (byte* ptr = memory)
                {
                    m_titan.ReadMemory(address, size, ptr);
                }
            }
        }

        public override uint GetZ80Reg(Z80Regs reg)
        {
            return 0;
        }

        public override byte ReadZ80Byte(uint address)
        {
            return 0;
        }

        public override bool AddBreakpoint(uint addr)
        {
            return m_titan.SetBreakpoint((int)addr);
        }

        public override bool AddWatchpoint(uint fromAddr, uint toAddr)
        {
            return false;
        }

        public override void RemoveBreakpoint(uint addr)
        {
            m_titan.ClearBreakpoint((int)addr);
        }

        public override void RemoveWatchpoint(uint addr)
        {
        }

        public override void RemoveAllBreakpoints()
        {
        }

        public override void RemoveAllWatchPoints()
        {
        }
    }
}

#endif