using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MDStudio
{
    class SymbolsFull
    {
        //Reverse engineering notes:
        // - Chunks have 5 byte footers, last byte = flags
        // - 0x88 = end of file header / end of filename/address chunk
        // - 0x80 = end of address chunk
        // - 0x8A = end of filename/address table
        // - 0x82 = 1 byte extra data follows subchunk, currently unknown

        //  ----------------------------------------------------------
        //  |                  ** FILE HEADER **                     |
        //  |              13 bytes : last byte = 0x88               |
        //  |--------------------------------------------------------|
        //  |                                                        |
        //  |   --------------------------------------------------   |
        //  |   |      ** FILENAME/ADDRESS TABLE HEADER **       |   |
        //  |   |   5 bytes : 3x byte flags, 2 byte string len   |   |
        //  |   |------------------------------------------------|   |
        //  |   |             ** FILENAME STRING  **             |   |
        //  |   |------------------------------------------------|   |
        //  |   |                                                |   |
        //  |   |   ------------------------------------------   |   |
        //  |   |   |             ** ADDRESS **              |   |   |
        //  |   |   | 5 bytes : 4 byte address, 1 byte flags |   |   |
        //  |   |   | 0x80 = end of address                  |   |   |
        //  |   |   | 0x82 = read 1 extra byte (unknown)     |   |   |
        //  |   |   | 0x88 = end of filename/address chunk   |   |   |
        //  |   |   | 0x8A = end of filename/address table   |   |   |
        //  |   |   ------------------------------------------   |   |
        //  |   |                                                |   |
        //  |   --------------------------------------------------   |
        //  |                                                        |
        //  |   --------------------------------------------------   |
        //  |   |              ** SYMBOL TABLE **                |   |
        //  |   |       ...                                      |   |
        //  |   --------------------------------------------------   |
        //  |                                                        |
        //  ----------------------------------------------------------

        public struct Symbol
        {
            public uint address;
            public string name;
        }

        public struct FilenameSection
        {
            public string filename;
            public List<AddressEntry> addresses;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FileHeader
        {
            public uint unknown1;
            public uint unknown2;
            public uint unknown3;
            public byte flags;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FilenameHeader
        {
            public byte flags1;
            public byte flags2;
            public byte flags3;
            public short length;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct AddressEntry
        {
            public uint address;
            public byte flags;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SymbolHeader
        {
            public uint address;
            public byte flags;
            public byte stringLen;
        }

        private List<Symbol> m_symbols;
        private List<FilenameSection> m_Filenames;

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

        private void EndianSwap(ref int value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            value = BitConverter.ToInt32(temp, 0);
        }

        private void EndianSwap(ref uint value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            value = BitConverter.ToUInt32(temp, 0);
        }

        private void EndianSwap(ref short value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            value = BitConverter.ToInt16(temp, 0);
        }

        private void EndianSwap(ref ushort value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            value = BitConverter.ToUInt16(temp, 0);
        }

        public bool Read(string filename)
        {
            try
            {
                m_symbols = new List<Symbol>();
                m_Filenames = new List<FilenameSection>();
                byte[] data = System.IO.File.ReadAllBytes(filename);

                if (data.Length > 0)
                {
                    GCHandle pinnedData = GCHandle.Alloc(data, GCHandleType.Pinned);
                    IntPtr stream = pinnedData.AddrOfPinnedObject();

                    int bytesRead = 0;

                    //Read file header
                    FileHeader fileHeader = new FileHeader();
                    bytesRead += Serialise(ref stream, out fileHeader);

                    //Read filename+address chunks
                    FilenameHeader filenameHeader = new FilenameHeader();
                    AddressEntry addressEntry = new AddressEntry();

                    byte flags = 0x0;

                    //0x8A == end of filename table 
                    while (bytesRead < data.Length && flags != 0x8A)
                    {
                        flags = 0x0;

                        //Symbol symbol = new Symbol();
                        FilenameSection filenameSection = new FilenameSection();
                        filenameSection.addresses = new List<AddressEntry>();

                        //Read filename chunk header
                        bytesRead += Serialise(ref stream, out filenameHeader);
                        EndianSwap(ref filenameHeader.length);
                        //symbol.address = symbolHeader.address;

                        //Read string
                        bytesRead += Serialise(ref stream, filenameHeader.length, out filenameSection.filename);

                        //Read all addresses
                        //0x88 == end of filename/address section
                        while(flags != 0x88 && flags != 0x8A)
                        {
                            bytesRead += Serialise(ref stream, out addressEntry);
                            //EndianSwap(ref addressEntry.address);
                            filenameSection.addresses.Add(addressEntry);
                            flags = addressEntry.flags;

                            if(flags == 0x82)
                            {
                                byte extraByte = 0;
                                bytesRead += Serialise(ref stream, out extraByte);
                            }
                        }

                        //Filename passed for assembly, not an address table entry
                        if (filenameHeader.flags2 != 0x1)
                        {
                            //m_symbols.Add(symbol);
                            m_Filenames.Add(filenameSection);
                        }
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
