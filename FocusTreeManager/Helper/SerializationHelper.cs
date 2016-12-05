using FocusTreeManager.DataContract;
using FocusTreeManager.Model;
using ProtoBuf;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace FocusTreeManager.Helper
{
    static class SerializationHelper
    {
        static public Project Deserialize(string filename)
        {
            //If we are loading a legacy version
            try
            {
                if (Path.GetExtension(filename) == ".h4prj")
                {
                    Model.LegacySerialization.Project project;
                    using (var fs = File.OpenRead(filename))
                    {
                        project = Serializer.Deserialize<Model.LegacySerialization.Project>(fs);
                        project.filename = filename;
                    }
                    //Repair references
                    foreach (Containers.LegacySerialization.FociGridContainer container in project.fociContainerList)
                    {
                        container.RepairInternalReferences();
                    }
                    return new Project(project);
                }
                //Loading the new xml format
                else
                {
                    Project project;
                    FileStream fs = new FileStream(filename, FileMode.Open);
                    XmlReader reader = XmlReader.Create(fs);
                    DataContractSerializer ser = new DataContractSerializer(typeof(Project));
                    project = (Project)ser.ReadObject(reader, true);
                    reader.Close();
                    fs.Close();
                    return project;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        static public void Serialize(string filename, Project project)
        {
            var serializer = new DataContractSerializer(project.GetType(), null, int.MaxValue, false, true, null);
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
