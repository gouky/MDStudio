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
        public uint GetAddress(string filename, int lineNumber)
        {
            //TODO: Slow
            int sectionIdx = m_Filenames.FindIndex(element => element.filename.ToUpper() == filename.ToUpper());
            if(sectionIdx >= 0)
            {
                int addressIdx = m_Filenames[sectionIdx].addresses.FindIndex(element => (lineNumber >= element.lineFrom && lineNumber <= element.lineTo));
                if (addressIdx >= 0)
                {
                    return m_Filenames[sectionIdx].addresses[addressIdx].address;
                }
            }
            
            return 0;
        }

        public Tuple<string, int> GetFileLine(uint address)
        {
            if (m_Addr2FileLine.ContainsKey(address))
            {
                return m_Addr2FileLine[address];
            }
            else
            {
                return new Tuple<string, int>("", 0);
            }
        }

        private struct AddressEntry
        {
            public uint address;
            public byte flags;
            public int lineFrom;
            public int lineTo;
        }

        private struct SymbolEntry
        {
            public uint address;
            public string name;
        }

        private struct FilenameSection
        {
            public string filename;
            public List<AddressEntry> addresses;
        }

        private List<SymbolEntry> m_Symbols;
        private List<FilenameSection> m_Filenames;
        private Dictionary<uint, Tuple<string, int>> m_Addr2FileLine;
        private string m_AssembledFile;

        public enum ChunkId : byte
        {
            Filename = 0x88,            // A filename with start address and line count
            Address = 0x80,             // An address of next line
            AddressWithCount = 0x82,    // An address with line count
            EndOfSection = 0x8A,        // End of section
            Symbol = 0x2                // Symbol table entry
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
                                        //If filename already exists, continue adding data to it
                                        int sectionIdx = m_Filenames.FindIndex(element => element.filename == readString);
                                        if (sectionIdx >= 0)
                                        {
                                            //Continue
                                            filenameSection = m_Filenames[sectionIdx];

                                            //Fetch line counter
                                            currentLine = filenameSection.addresses[filenameSection.addresses.Count - 1].lineTo;
                                        }
                                        else
                                        {
                                            //This is the first address in a filename chunk
                                            filenameSection = new FilenameSection();
                                            filenameSection.addresses = new List<AddressEntry>();
                                            filenameSection.filename = readString;

                                            //Reset line counter
                                            currentLine = 0;
                                        }

                                        //Chunk payload contains address
                                        addressEntry.address = chunkHeader.payload;
                                        addressEntry.lineFrom = currentLine;
                                        addressEntry.lineTo = filenameHeader.firstLine;
                                        currentLine = filenameHeader.firstLine;
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
                                    currentLine++;
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

                            case ChunkId.Symbol:
                                {
                                    //Read symbol string length
                                    byte stringLength = 0;
                                    bytesRead += Serialise(ref stream, out stringLength);

                                    //Read string
                                    bytesRead += Serialise(ref stream, stringLength, out symbolEntry.name);

                                    //Payload contains address
                                    symbolEntry.address = chunkHeader.payload;

                                    m_Symbols.Add(symbolEntry);

                                    break;
                                }

                            case ChunkId.EndOfSection:
                                //Nothing of interest
                                break;
                        }
                    }

                    pinnedData.Free();

                    //Build address to file/line map
                    m_Addr2FileLine = new Dictionary<uint, Tuple<string, int>>();

                    foreach (FilenameSection section in m_Filenames)
                    {
                        foreach(AddressEntry address in section.addresses)
                        {
                            if(!m_Addr2FileLine.ContainsKey(address.address))
                            {
                                m_Addr2FileLine[address.address] = new Tuple<string, int>(section.filename, address.lineTo);
                            }
                        }
                    }

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
