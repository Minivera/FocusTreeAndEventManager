using FocusTreeManager.Containers;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Model
{
    [Serializable]
    public class Project : ObservableObject
    {
        public ObservableCollection<FociGridContainer> fociContainerList { get; private set; }

        public ObservableCollection<LocalisationContainer> localisationList { get; private set; }

        //public ObservableCollection<Event> eventList { get; private set; }

        public Project()
        {
            fociContainerList = new ObservableCollection<FociGridContainer>();
            localisationList = new ObservableCollection<LocalisationContainer>();
            //eventList = new ObservableCollection<Event>();
        }

        public ObservableCollection<Focus> getSpecificFociList(string containerID)
        {
            FociGridContainer container = fociContainerList.SingleOrDefault((c) => c.ContainerID == containerID);
            return container != null ? container.FociList : null;
        }

        public ObservableCollection<LocaleContent> getSpecificLocalisationMap(string containerID)
        {
            LocalisationContainer container = localisationList.SingleOrDefault((c) => c.ContainerID == containerID);
            return container != null ? container.LocalisationMap : null;
        }

        public void SaveToFile(string filename)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();
        }

        public LocalisationContainer getLocalisationWithKey(string key)
        {
            if (key == null)
            {
                return null;
            }
            return localisationList.FirstOrDefault((l) => l.LocalisationMap.
                    Where((x) => x.Key.ToLower() == key.ToLower()).Any());
        }
    }
}
