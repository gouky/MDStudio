using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DGenInterface;

namespace MDStudio
{
    class DGenThread
    {
        private static DGen m_DGen = null;
        private static DGenThread m_Instance;

        private Thread myThread = null;

        public void Init(int windowWidth, int windowHeight)
        {
            m_Instance = this;

            m_DGen = new DGen();
            m_DGen.Init(windowWidth, windowHeight);

            myThread = new Thread(new ThreadStart(ThreadLoop));
            myThread.Start();
        }

        public void LoadRom(string path)
        {
            if (myThread != null)
            {
                myThread.Abort();
                while (myThread.IsAlive)
                {
                    Thread.Sleep(1);
                }
            }

            m_DGen.LoadRom(path);
        }

        public void Start()
        {
            myThread = new Thread(new ThreadStart(ThreadLoop));
            myThread.Start();
        }

        public void Stop()
        {
            if (myThread != null)
            {
                myThread.Abort();
                while (myThread.IsAlive)
                {
                    Thread.Sleep(1);
                }
            }
            m_DGen.Reset();
            myThread = null;
        }

        public void Destroy()
        {
            m_DGen.Dispose();
            m_DGen = null;
        }
        
        public void AddBreakpoint(int addr)
        {
            m_DGen.AddBreakpoint(addr);
        }

        public void ClearBreakpoints()
        {
            m_DGen.ClearBreakpoints();
        }

        static public DGen GetDGen()
        {
            return m_DGen;
        }

        private static void ThreadLoop()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                m_DGen.Update();
                Thread.Sleep(1);
            }
        }
    }
}
