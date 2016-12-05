using FocusTreeManager.Containers.LegacySerialization;
using GalaSoft.MvvmLight;
using ProtoBuf;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FocusTreeManager.Model.LegacySerialization
{
    [ProtoContract]
    public class Project : ObservableObject
    {
        [ProtoMember(1)]
        public string filename { get; set; }

        [ProtoMember(2)]
        public ObservableCollection<FociGridContainer> fociContainerList { get; set; }

        [ProtoMember(3)]
        public ObservableCollection<LocalisationContainer> localisationList { get; set; }

        [ProtoMember(4)]
        public ObservableCollection<EventContainer> eventList { get; set; }

        [ProtoMember(5)]
        public ObservableCollection<ScriptContainer> scriptList { get; set; }

        public Project()
        {
            fociContainerList = new ObservableCollection<FociGridContainer>();
            localisationList = new ObservableCollection<LocalisationContainer>();
            eventList = new ObservableCollection<EventContainer>();
            scriptList = new ObservableCollection<ScriptContainer>();
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

        public EventContainer getSpecificEventList(Guid containerID)
        {
            EventContainer container = eventList.SingleOrDefault((c) => c.IdentifierID == containerID);
            return container;
        }

        public ScriptContainer getSpecificScriptList(Guid containerID)
        {
            ScriptContainer container = scriptList.SingleOrDefault((c) => c.IdentifierID == containerID);
            return container;
        }
    }
}
