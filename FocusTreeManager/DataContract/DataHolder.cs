using FocusTreeManager.Helper;
using FocusTreeManager.Model;

namespace FocusTreeManager.DataContract
{
    public sealed class DataHolder
    {
        public static DataHolder Instance { get; } = new DataHolder();

        public Project Project;

        public DataHolder()
        {
            Project = new Project();
        }

        public void SaveContract(ProjectModel model)
        {
            Project.UpdateDataContract(model);
            SerializationHelper.Serialize(Project.filename, Project);
        }

        public static bool LoadContract(string filename)
        {
            Project returnVal = SerializationHelper.Deserialize(filename);
            if (returnVal == null)
            {
                return false;
            }
            else
            {
                Instance.Project = returnVal;
                Instance.Project.filename = filename;
                return true;
            }
        }
    }
}
