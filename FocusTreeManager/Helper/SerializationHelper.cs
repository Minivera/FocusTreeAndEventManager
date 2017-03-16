using FocusTreeManager.DataContract;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace FocusTreeManager.Helper
{
    public static class SerializationHelper
    {
        public static Project Deserialize(string filename)
        {
            //If we are loading a legacy version
            try
            {
                if (Path.GetExtension(filename) == ".xh4prj")
                {
                    FileStream fs = new FileStream(filename, FileMode.Open);
                    XmlReader reader = XmlReader.Create(fs);
                    DataContractSerializer ser = new DataContractSerializer(typeof(Project));
                    Project project = (Project)ser.ReadObject(reader, true);
                    reader.Close();
                    fs.Close();
                    return project;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Serialize(string filename, Project project)
        {
            if (filename == null || project == null)
            {
                return;
            }
            DataContractSerializer serializer = new DataContractSerializer(project.GetType(), 
                null, int.MaxValue, false, true, null);
            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (XmlDictionaryWriter writer =
                    XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8))
                {
                    writer.WriteStartDocument();
                    serializer.WriteObject(writer, project);
                }
            }
        }

    }
}
