using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DGenInterface;
using MDStudio.Properties;

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

            if(Settings.Default.DGenWindowLocation != null)
            {
                m_DGen.SetWindowPosition(Settings.Default.DGenWindowLocation.X, Settings.Default.DGenWindowLocation.Y);
            }
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
            
            myThread = null;

            if(m_DGen != null)
            {
                Settings.Default.DGenWindowLocation = new System.Drawing.Point(m_DGen.GetWindowXPosition(), m_DGen.GetWindowYPosition());
                m_DGen.Reset();
            }
        }

        public void BringToFront()
        {
            if(m_DGen != null)
            {
                m_DGen.BringToFront();
            }
        }
        public void Destroy()
        {
            if(m_DGen != null)
            {
                m_DGen.Dispose();
                m_DGen = null;
            }
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
