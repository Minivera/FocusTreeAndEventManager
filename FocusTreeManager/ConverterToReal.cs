using System.IO;
using FocusTreeManager.Model;

namespace FocusTreeManager
{
    public class ConverterToReal
    {
        public ConverterToReal(Project project)
        {
            _project = project;
        }

        public void SetPath(string path)
        {
            _path = path;
        }

        public void WriteToFiles()
        {
            System.IO.Directory.CreateDirectory(_path);
            foreach (var focuses in _project.fociContainerList)
            {
                string filePath = _path + "\\" + focuses.ContainerID + ".txt";
                TextWriter tw = new StreamWriter(filePath);
                tw.WriteLine("focus_tree = {");
                string id = focuses.ContainerID;
                id = id.Replace(" ", "_");
                tw.WriteLine("\tid = " + id);
                tw.WriteLine("\tdefault = no");
                foreach (var focus in focuses.FociList)
                {
                    tw.WriteLine("\tfocus = {");
                    id = focus.UniqueName;
                    tw.WriteLine("\t\tid = " + id);
                    tw.WriteLine("\t\ticon = " + focus.Image);
                    if (focus.Prerequisite.Count > 0)
                    {
                        foreach (var prereq1 in focus.Prerequisite)
                        {
                            tw.WriteLine("\t\tprerequisite = {");
                            foreach (var prereq2 in prereq1.FociList)
                            {
                                tw.WriteLine("\t\t\tfocus = " + prereq2.UniqueName);
                            }
                            tw.WriteLine("\t\t}");
                        }
                    }
                    if (focus.MutualyExclusive.Count > 0)
                    {
                        tw.WriteLine("\t\tmutually_exclusive = ");
                        foreach (var prereq1 in focus.MutualyExclusive)
                        {
                            if (prereq1.Focus1.UniqueName != focus.UniqueName)
                            {
                                tw.WriteLine("\t\t\tfocus = " + prereq1.Focus1.UniqueName);
                            }
                            else
                            {
                                tw.WriteLine("\t\t\tfocus = " + prereq1.Focus2.UniqueName);
                            }
                        }
                        tw.WriteLine("\t\t}");
                    }
                    tw.WriteLine("\t\tx = " + focus.X);
                    tw.WriteLine("\t\ty = " + focus.Y);

                    tw.WriteLine("\t}");
                }
                tw.WriteLine("}");
                tw.Close();
            }
        }
        private Project _project;
        private string _path;
    }
}