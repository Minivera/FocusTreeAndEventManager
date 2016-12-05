using FocusTreeManager.Containers;
using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace FocusTreeManager.Model
{
    public sealed class Project : ObservableObject
    {
        private static readonly Lazy<Project> lazy =
        new Lazy<Project>(() => new Project());

        public static Project Instance { get { return lazy.Value; } }

        public string filename { get; set; }
        
        public ObservableCollection<FociGridContainer> fociContainerList { get; set; }
        
        public ObservableCollection<LocalisationContainer> localisationList { get; set; }
        
        public ObservableCollection<EventContainer> eventList { get; set; }
        
        public ObservableCollection<ScriptContainer> scriptList { get; set; }

        public Project()
        {
            fociContainerList = new ObservableCollection<FociGridContainer>();
            localisationList = new ObservableCollection<LocalisationContainer>();
            eventList = new ObservableCollection<EventContainer>();
            scriptList = new ObservableCollection<ScriptContainer>();
        }

        public Project(LegacySerialization.Project legacyProject)
        {
            fociContainerList = FociGridContainer.PopulateFromLegacy(legacyProject.fociContainerList);
            localisationList = LocalisationContainer.PopulateFromLegacy(legacyProject.localisationList);
            eventList = EventContainer.PopulateFromLegacy(legacyProject.eventList);
            scriptList = ScriptContainer.PopulateFromLegacy(legacyProject.scriptList);
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

        public LocalisationContainer getLocalisationWithKey(string key)
        {
            if (key == null)
            {
                return null;
            }
            return localisationList.FirstOrDefault((l) => l.LocalisationMap.
                    Where((x) => x.Key != null && x.Key.ToLower() == key.ToLower()).Any());
        }
    }
}
