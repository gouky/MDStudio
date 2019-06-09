using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//////////////////////////////////////////////////////////////////
// BIG FAT TODO - interim classes to support multiple targets,
// each target eventually needs to be a DLL with plugin interface.
//////////////////////////////////////////////////////////////////

namespace MDStudio
{
    public enum Z80Regs
    {
        Z80_REG_FA = 0,
        Z80_REG_CB = 1,
        Z80_REG_ED = 2,
        Z80_REG_LH = 3,
        Z80_REG_FA_ALT = 4,
        Z80_REG_CB_ALT = 5,
        Z80_REG_ED_ALT = 6,
        Z80_REG_LH_ALT = 7,
        Z80_REG_IX = 8,
        Z80_REG_IY = 9,
        Z80_REG_SP = 10,
        Z80_REG_PC = 11
    }

    public enum SDLInputs
    {
        eInputUp = 0,
        eInputDown = 1,
        eInputLeft = 2,
        eInputRight = 3,
        eInputB = 4,
        eInputC = 5,
        eInputA = 6,
        eInputStart = 7,
        eInputZ = 8,
        eInputY = 9,
        eInputX = 10,
        eInputMode = 11,
        eInput_COUNT = 12
    }

    public abstract class Target
    {
        //Initialisation
        public abstract bool LoadBinary(string filename);

        //Control
        public abstract void Run();
        public abstract void Reset();
        public abstract uint Break();
        public abstract void Resume();
        public abstract uint Step();
        public abstract bool IsHalted();

        //Registers
        public abstract uint GetAReg(int index);
        public abstract uint GetDReg(int index);
        public abstract uint GetPC();
        public abstract uint GetSR();

        //Memory
        public abstract byte ReadByte(uint address);
        public abstract uint ReadLong(uint address);
        public abstract ushort ReadWord(uint address);
        public abstract void ReadMemory(uint address, uint size, byte[] memory);

        //Z80
        public abstract uint GetZ80Reg(Z80Regs reg);
        public abstract byte ReadZ80Byte(uint address);

        //Breakpoints
        public abstract bool AddBreakpoint(uint addr);
        public abstract bool AddWatchpoint(uint fromAddr, uint toAddr);
        public abstract void RemoveBreakpoint(uint addr);
        public abstract void RemoveWatchpoint(uint addr);
        public abstract void RemoveAllBreakpoints();
        public abstract void RemoveAllWatchPoints();
    }

    abstract class EmulatorTarget : Target
    {
        //Window management
        public abstract void Initialise(int width, int height, IntPtr parent, bool pal, char region);
        public abstract void Shutdown();
        public abstract void BringToFront();

        //Input
        public abstract void SetInputMapping(SDLInputs input, int mapping);
        public abstract void SendKeyPress(int vkCode, int keyDown);

        //VDP
        public abstract byte GetVDPRegisterValue(int index);
        public abstract uint GetColor(int index);

        //Control
        public abstract void SoftReset();
    }

    abstract class HardwareTarget : Target
    {

    }
}
