using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDStudio
{
    class Endian
    {
        public static int Swap(int value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            return BitConverter.ToInt32(temp, 0);
        }

        public static uint Swap(uint value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            return BitConverter.ToUInt32(temp, 0);
        }

        public static short Swap(short value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            return BitConverter.ToInt16(temp, 0);
        }

        public static ushort Swap(ushort value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            return BitConverter.ToUInt16(temp, 0);
        }
    }
}
