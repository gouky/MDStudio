using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDStudio
{
    class TargetDGen : EmulatorTarget
    {
        private DGenThread m_DGenThread;

        public TargetDGen()
        {
            try
            {
                m_DGenThread = new DGenThread();
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public override void Initialise(int width, int height, IntPtr parent, bool pal, char region)
        {
            m_DGenThread.Init(width, height, parent, pal, region);
        }

        public override void Shutdown()
        {
            m_DGenThread.Stop();
            m_DGenThread.Destroy();
        }

        public override void BringToFront()
        {
            DGenThread.GetDGen().BringToFront();
        }

        public override void SetInputMapping(SDLInputs input, int mapping)
        {
            DGenThread.GetDGen().SetInputMapping((DGenInterface.DGen.SDLInputs)input, mapping);
        }

        public override void SendKeyPress(int vkCode, int keyDown)
        {
            DGenThread.GetDGen().KeyPressed(vkCode, keyDown);
        }

        public override bool LoadBinary(string filename)
        {
            m_DGenThread.LoadRom(filename);
            return true;
        }

        public override void Run()
        {
            m_DGenThread.Start();
        }

        public override void Reset()
        {
            DGenThread.GetDGen().Reset();
        }

        public override void SoftReset()
        {
            DGenThread.GetDGen().SoftReset();
        }

        public override uint Break()
        {
            DGenThread.GetDGen().Break();
            return GetPC();
        }

        public override void Resume()
        {
            DGenThread.GetDGen().Resume();
        }

        public override uint Step()
        {
            DGenThread.GetDGen().StepInto();
            return GetPC();
        }

        public override bool IsHalted()
        {
            return (DGenThread.GetDGen() != null) ? DGenThread.GetDGen().IsDebugging() : false;
        }

        public override uint GetAReg(int index)
        {
            return (uint)DGenThread.GetDGen().GetAReg(index);
        }

        public override uint GetDReg(int index)
        {
            return (uint)DGenThread.GetDGen().GetDReg(index);
        }

        public override uint GetPC()
        {
            return (uint)DGenThread.GetDGen().GetCurrentPC();
        }

        public override uint GetSR()
        {
            return (uint)DGenThread.GetDGen().GetSR();
        }

        public override byte ReadByte(uint address)
        {
            return DGenThread.GetDGen().ReadByte(address);
        }

        public override uint ReadLong(uint address)
        {
            return DGenThread.GetDGen().ReadLong(address);
        }

        public override ushort ReadWord(uint address)
        {
            return DGenThread.GetDGen().ReadWord(address);
        }

        public override void ReadMemory(uint address, uint size, byte[] memory)
        {
            unsafe
            {
                fixed(byte* ptr = memory)
                {
                    DGenThread.GetDGen().ReadMemory(address, size, ptr);
                }
            }
        }

        public override uint GetZ80Reg(Z80Regs reg)
        {
            return (uint)DGenThread.GetDGen().GetZ80Reg((DGenInterface.DGen.Z80Regs)reg);
        }

        public override byte ReadZ80Byte(uint address)
        {
            return DGenThread.GetDGen().ReadZ80Byte(address);
        }

        public override bool AddBreakpoint(uint addr)
        {
            m_DGenThread.AddBreakpoint((int)addr);
            return true;
        }

        public override bool AddWatchPoint(uint fromAddr, uint toAddr)
        {
            return DGenThread.GetDGen().AddWatchPoint((int)fromAddr, (int)toAddr) != 0;
        }

        public override void RemoveBreakpoint(uint addr)
        {
            //TODO
            m_DGenThread.ClearBreakpoints();
        }

        public override void RemoveWatchPoint(uint addr)
        {
            //TODO
            m_DGenThread.ClearBreakpoints();
        }

        public override void RemoveAllBreakpoints()
        {
            m_DGenThread.ClearBreakpoints();
        }

        public override void RemoveAllWatchPoints()
        {
            //TODO
            m_DGenThread.ClearBreakpoints();
        }

        public override uint GetColor(int index)
        {
            return (uint)DGenThread.GetDGen().GetColor(index);
        }
    }
}
