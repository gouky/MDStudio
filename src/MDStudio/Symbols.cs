using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MDStudio
{
    class Symbols
    {
        public struct Symbol
        {
            public uint address;
            public string name;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FileHeader
        {
            public uint unknown1;
            public uint unknown2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SymbolHeader
        {
            public uint address;
            public byte flags;
            public byte stringLen;
        }

        private List<Symbol> m_symbols;

        private int Serialise<T>(ref IntPtr bytes, out T value) where T : struct
        {
            value = (T)Marshal.PtrToStructure(bytes, typeof(T));
            bytes += Marshal.SizeOf<T>();
            return Marshal.SizeOf<T>();
        }

        private int Serialise(ref IntPtr bytes, int length, out string value)
        {
            var byteArray = new byte[length];
            System.Runtime.InteropServices.Marshal.Copy(bytes, byteArray, 0, length);
            value = System.Text.Encoding.Default.GetString(byteArray, 0, length);
            bytes += length;
            return length;
        }

        public bool Read(string filename)
        {
            try
            {
                m_symbols = new List<Symbol>();
                byte[] data = System.IO.File.ReadAllBytes(filename);

                if (data.Length > 0)
                {
                    GCHandle pinnedData = GCHandle.Alloc(data, GCHandleType.Pinned);
                    IntPtr stream = pinnedData.AddrOfPinnedObject();

                    int bytesRead = 0;

                    //Read file header
                    FileHeader fileHeader = new FileHeader();
                    bytesRead += Serialise(ref stream, out fileHeader);

                    //Read all chunks
                    SymbolHeader symbolHeader = new SymbolHeader();

                    while (bytesRead < data.Length)
                    {
                        Symbol symbol = new Symbol();

                        //Read chunk header
                        bytesRead += Serialise(ref stream, out symbolHeader);
                        symbol.address = symbolHeader.address;

                        //Read string
                        bytesRead += Serialise(ref stream, symbolHeader.stringLen, out symbol.name);

                        m_symbols.Add(symbol);
                    }

                    pinnedData.Free();

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }
    }
}
