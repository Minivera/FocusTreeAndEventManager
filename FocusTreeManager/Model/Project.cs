using FocusTreeManager.Containers;
using GalaSoft.MvvmLight;
using ProtoBuf;
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
    [ProtoContract]
    public class Project : ObservableObject
    {
        [ProtoMember(1)]
        public string filename { get; private set; }

        [ProtoMember(2)]
        public ObservableCollection<FociGridContainer> fociContainerList { get; set; }

        [ProtoMember(3)]
        public ObservableCollection<LocalisationContainer> localisationList { get; set; }

        //[ProtoMember(4)]
        //public ObservableCollection<Event> eventList { get; set; }

        public Project()
        {
            fociContainerList = new ObservableCollection<FociGridContainer>();
            localisationList = new ObservableCollection<LocalisationContainer>();
            //eventList = new ObservableCollection<Event>();
        }

        public FociGridContainer getSpecificFociList(Guid containerID)
        {
            FociGridContainer container = fociContainerList.SingleOrDefault((c) => c.IdentifierID == containerID);
            return container;
        }

        public LocalisationContainer getSpecificLocalisationMap(Guid containerID)
        {
            LocalisationContainer container = localisationList.SingleOrDefault((c) => c.IdentifierID == containerID);
            return container;
        }

        public void SaveToFile(string filename)
        {
            this.filename = filename;
            using (var fs = File.Create(filename))
            {
                Serializer.Serialize(fs, this);
            }
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
