using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(FociGridContainer))]
    [KnownType(typeof(LocalisationContainer))]
    [KnownType(typeof(EventContainer))]
    [KnownType(typeof(ScriptContainer))]
    [DataContract(Name = "project")]
    public sealed class Project
    {
        private static Project instance = new Project();

        public static Project Instance
        {
            get
            {
                return instance;
            }
        }

        public string filename { get; set; }

        [DataMember(Name = "foci_containers", Order = 0)]
        public List<FociGridContainer> fociContainerList { get; set; }

        [DataMember(Name = "locale_containers", Order = 1)]
        public List<LocalisationContainer> localisationList { get; set; }

        [DataMember(Name = "event_containers", Order = 2)]
        public List<EventContainer> eventList { get; set; }

        [DataMember(Name = "script_containers", Order = 3)]
        public List<ScriptContainer> scriptList { get; set; }

        private Project()
        {
            fociContainerList = new List<FociGridContainer>();
            localisationList = new List<LocalisationContainer>();
            eventList = new List<EventContainer>();
            scriptList = new List<ScriptContainer>();
        }

        public Project(Model.LegacySerialization.Project legacyProject)
        {
            fociContainerList = FociGridContainer.PopulateFromLegacy(legacyProject.fociContainerList.ToList());
            localisationList = LocalisationContainer.PopulateFromLegacy(legacyProject.localisationList.ToList());
            eventList = EventContainer.PopulateFromLegacy(legacyProject.eventList.ToList());
            scriptList = ScriptContainer.PopulateFromLegacy(legacyProject.scriptList.ToList());
        }

        static public void ResetInstance()
        {
            instance = new Project();
        }

        static public void SetInstance(Project project)
        {
            instance = project;
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
