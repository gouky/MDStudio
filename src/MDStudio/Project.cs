using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace MDStudio
{
    [XmlRoot("Root")]
    class Project
    {
        [XmlElement("SourceFile")]
        public string SourceFile { get; set; }

        public string PathToProject { get; set; }

        public Project(string pathToProject)
        {
            PathToProject = pathToProject;

            Read();
        }

        public bool Read()
        {
            XmlSerializer xs = new XmlSerializer(typeof(Project));
            
            if (File.Exists(PathToProject))
            {
                StreamReader sr = new StreamReader(PathToProject);
                try
                {
                    Project project;

                    project = (Project)xs.Deserialize(sr);

                    SourceFile = project.SourceFile;

                    sr.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
