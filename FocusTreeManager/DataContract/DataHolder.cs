using FocusTreeManager.Helper;
using GalaSoft.MvvmLight;
using FocusTreeManager.Model;

namespace FocusTreeManager.DataContract
{
    public sealed class DataHolder
    {
        private static DataHolder instance = new DataHolder();

        public static DataHolder Instance
        {
            get
            {
                return instance;
            }
        }

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

        static public bool LoadContract(string filename)
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
