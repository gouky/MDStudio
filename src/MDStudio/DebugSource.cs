using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace MDStudio
{
    struct Include
    {
        public string Name;
        public int LineNumber;
        public string[] Content;
    }

    struct Source
    {
        public int[] m_Address;
        public int[] m_SourceToList; // Source line -> List line
        public int[] m_ListToSource; // List line -> Source line
    }

    class DebugSource
    {
        private String[] m_SplittedSource;
        private String[] m_SplittedSourceList;

        private List<Include> m_Includes;

        private Source m_Source;

        private string[] LoadFile(string path)
        {
            string[] file = System.IO.File.ReadAllLines(path);

            // make sure it doesn't end with a LF
            FileStream fs = new FileStream(path, FileMode.Open);
            fs.Seek(-1, SeekOrigin.End);
            int n = fs.ReadByte();
            if (n == '\n')
            {
                List<string> listString = new List<string>(file);
                listString.Add("");
                file = listString.ToArray();
            }
            fs.Close();

            return file;
        }

        public int GetSourceLine(int currentPC)
        {
            int line = 0;

            // brute force
            foreach (int addr in m_Source.m_Address)
            {
                if (currentPC == addr)
                {
                    return line;
                }
                line++;
            }

            return 0;
        }

        public int GetLineAddress(int line)
        {
            return m_Source.m_Address[line];
        }

        public void Init(string sourcePath, string sourceListPath, string pathToSource)
        {
            m_Includes = new List<Include>();

            m_SplittedSource = LoadFile(sourcePath);
            m_SplittedSourceList = LoadFile(sourceListPath);

            m_Source = new Source();
            m_Source.m_SourceToList = new int[m_SplittedSource.Length];
            m_Source.m_ListToSource = new int[m_SplittedSourceList.Length];
            m_Source.m_Address = new int[m_SplittedSource.Length];

            int lineNumber = 0;
            int sourceLine = 0;
            int lineToSkip = 0;
            string previousLine = null;
            foreach (string line in m_SplittedSourceList)
            {
                Match matchAddr = Regex.Match(line, @"(\d|[a-fA-F]){8}");

                if (!matchAddr.Success)
                {
                    previousLine = line;
                    lineNumber++;
                    continue;
                }

                if (lineToSkip > 0)
                {
                    previousLine = line;
                    lineToSkip--;
                    continue;
                }
                if (previousLine != null && previousLine == line)
                {
                    //  same?
                    previousLine = line;
                    lineNumber++;
                    continue;
                }

                previousLine = line;
                try
                {
                    string patternInclude = @"INCLUDE\W+(\w+\.\w+)";
                    string patternAddress = @"([0-9A-F]+)"; // we just want for valid code //(\d|[a-fA-F]){8}";
                    Match matchInclude = Regex.Match(line, patternInclude);
                    Match codeLine = Regex.Match(line, @"[0-9A-F]{8} ([0-9A-F]{4} )+\W+");
                    MatchCollection matchAddress = Regex.Matches(line, patternAddress);
                    int address = 0;

                    if (/*matchAddress.Count > 0 &&*/ codeLine.Success)
                    {
                        int.TryParse(matchAddress[0].Groups[1].Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out address);
                        m_Source.m_Address[sourceLine] = address;
                    }

                    if (matchInclude.Success)
                    {
                        Include include = new Include();
                        string includePath = pathToSource + matchInclude.Groups[1].Value;

                        include.LineNumber = lineNumber;
                        include.Name = matchInclude.Groups[1].Value;
                        include.Content = LoadFile(includePath);

                        m_Includes.Add(include);

                        m_Source.m_ListToSource[lineNumber] = sourceLine;
 
                        m_Source.m_SourceToList[sourceLine++] = lineNumber;

                        lineToSkip = include.Content.Length;
//                        lineNumber++;
                        lineNumber += lineToSkip+1;

                        continue;
                    }
                    else
                    {
                        m_Source.m_ListToSource[lineNumber] = sourceLine;

                        //Console.WriteLine("line " + sourceLine + " - " + line);

                        m_Source.m_SourceToList[sourceLine++] = lineNumber;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                lineNumber++;
            }

            //
//             int bkp = m_Source.m_Address[298];
//             Console.WriteLine("bkp address: " + bkp);
// 
//             int currentPC = 0x0002A91A;
//             int currentSourceLine = GetSourceLine(currentPC);
//             Console.WriteLine("bkp address: " + bkp);
        }
    }
}
