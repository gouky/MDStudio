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
        //  |                       8 bytes                          |
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
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FilenameHeader
        {
            //public AddressEntry firstAddress;
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

                    FilenameHeader filenameHeader = new FilenameHeader();
                    AddressEntry addressEntry = new AddressEntry();
                    FilenameSection filenameSection = new FilenameSection();
                    byte flags = 0x0;
                    int bytesRead = 0;

                    //Read file header
                    FileHeader fileHeader = new FileHeader();
                    bytesRead += Serialise(ref stream, out fileHeader);

                    //Read all addresses in filename table
                    //0x8A == end of table 
                    while (bytesRead < data.Length && flags != 0x8A)
                    {
                        //Read address entry
                        bytesRead += Serialise(ref stream, out addressEntry);
                            
                        //Get chunk flags
                        flags = addressEntry.flags;

                        if (flags == 0x88)
                        {
                            //This is the first address in a filename chunk, so also read the filename
                            filenameSection = new FilenameSection();
                            filenameSection.addresses = new List<AddressEntry>();

                            //Read filename chunk header
                            bytesRead += Serialise(ref stream, out filenameHeader);
                            EndianSwap(ref filenameHeader.length);

                            //Read string
                            bytesRead += Serialise(ref stream, filenameHeader.length, out filenameSection.filename);

                            if (filenameHeader.flags2 == 0x1)
                            {
                                //This is the filename passed for assembly, not an address table entry
                                break;
                            }
                            else
                            {
                                //Valid filename chunk, add and continue
                                m_Filenames.Add(filenameSection);
                            }
                        }
                        else if (flags == 0x82)
                        {
                            //Read one extra byte, unknown yet
                            byte extraByte = 0;
                            bytesRead += Serialise(ref stream, out extraByte);
                        }
                        
                        //Add address to filename
                        filenameSection.addresses.Add(addressEntry);
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
