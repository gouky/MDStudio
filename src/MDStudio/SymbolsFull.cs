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
        public struct AddressEntry
        {
            public uint address;
            public byte flags;
            public int lineFrom;
            public int lineTo;
        }

        public struct SymbolEntry
        {
            public uint address;
            public string name;
        }

        public struct FilenameSection
        {
            public string filename;
            public List<AddressEntry> addresses;
        }

        private List<SymbolEntry> m_Symbols;
        private List<FilenameSection> m_Filenames;
        private string m_AssembledFile;

        //Reverse engineering notes:
        // - Address chunks are 5 bytes: 4 bytes address + 1 byte flags
        // - Filename chunks have 5 byte headers, last word = string length, string follows
        // - 0x88 = address chunk + filename chunk packed
        // - 0x80 = address chunk
        // - 0x82 = address chunk + 1 byte line counter - number of lines this address spans
        // - 0x8A = address chunk + end of filename/address table


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
        //  |   |   | 0x82 = read 1 extra byte (# lines)     |   |   |
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

        public enum ChunkId : byte
        {
            Filename = 0x88,
            Address = 0x80,
            AddressWithCount = 0x82,
            SymbolTable = 0x8A
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
            public byte firstLine;
            public byte flags;
            public byte unknown;
            public short length;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ChunkHeader
        {
            public uint payload;
            public ChunkId chunkId;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SymbolChunk
        {
            public uint address;
            public byte flags;
            public byte stringLen;
        }

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
                m_Symbols = new List<SymbolEntry>();
                m_Filenames = new List<FilenameSection>();
                byte[] data = System.IO.File.ReadAllBytes(filename);

                if (data.Length > 0)
                {
                    GCHandle pinnedData = GCHandle.Alloc(data, GCHandleType.Pinned);
                    IntPtr stream = pinnedData.AddrOfPinnedObject();

                    FilenameHeader filenameHeader = new FilenameHeader();
                    ChunkHeader chunkHeader = new ChunkHeader();
                    FilenameSection filenameSection = new FilenameSection();
                    AddressEntry addressEntry = new AddressEntry();
                    SymbolChunk symbolChunk = new SymbolChunk();
                    SymbolEntry symbolEntry = new SymbolEntry();
                    string readString;

                    int bytesRead = 0;
                    int currentLine = 0;

                    //Read file header
                    FileHeader fileHeader = new FileHeader();
                    bytesRead += Serialise(ref stream, out fileHeader);

                    //Iterate over chunks
                    while (bytesRead < data.Length)
                    {
                        //Read chunk header
                        bytesRead += Serialise(ref stream, out chunkHeader);

                        //What is it?
                        switch (chunkHeader.chunkId)
                        {
                            case ChunkId.Filename:
                                {
                                    //Read filename header
                                    bytesRead += Serialise(ref stream, out filenameHeader);
                                    EndianSwap(ref filenameHeader.length);

                                    //Read string
                                    bytesRead += Serialise(ref stream, filenameHeader.length, out readString);

                                    if (filenameHeader.flags == 0x1)
                                    {
                                        //This is the filename passed for assembly
                                        m_AssembledFile = readString;
                                    }
                                    else
                                    {
                                        //This is the first address in a filename chunk
                                        filenameSection = new FilenameSection();
                                        filenameSection.addresses = new List<AddressEntry>();
                                        filenameSection.filename = readString;

                                        //Reset line counter
                                        currentLine = filenameHeader.firstLine;

                                        //Chunk payload contains address
                                        addressEntry.address = chunkHeader.payload;
                                        addressEntry.lineFrom = 0;
                                        addressEntry.lineTo = currentLine;
                                        filenameSection.addresses.Add(addressEntry);

                                        //Add to filename list
                                        m_Filenames.Add(filenameSection);
                                    }

                                    break;
                                }

                            case ChunkId.Address:
                                {
                                    //Chunk payload contains address
                                    addressEntry.address = chunkHeader.payload;

                                    //Set line range
                                    addressEntry.lineFrom = currentLine;
                                    addressEntry.lineTo = currentLine;

                                    //Add
                                    filenameSection.addresses.Add(addressEntry);

                                    break;
                                }

                            case ChunkId.AddressWithCount:
                                {
                                    //Read line count
                                    byte lineCount = 0;
                                    bytesRead += Serialise(ref stream, out lineCount);

                                    //Chunk payload contains address
                                    addressEntry.address = chunkHeader.payload;

                                    //Set line range
                                    addressEntry.lineFrom = currentLine;
                                    currentLine += lineCount;
                                    addressEntry.lineTo = currentLine;

                                    //Add
                                    filenameSection.addresses.Add(addressEntry);
                                    
                                    break;
                                }

                            case ChunkId.SymbolTable:
                                {
                                    //Welcome to the symbol table
                                    while (bytesRead < data.Length)
                                    {
                                        //Read chunk header
                                        bytesRead += Serialise(ref stream, out symbolChunk);
                                        symbolEntry.address = symbolChunk.address;

                                        //Read string
                                        bytesRead += Serialise(ref stream, symbolChunk.stringLen, out symbolEntry.name);

                                        m_Symbols.Add(symbolEntry);
                                    }

                                    break;
                                }
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
