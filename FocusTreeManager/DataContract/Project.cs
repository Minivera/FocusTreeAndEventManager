using FocusTreeManager.Model;
using FocusTreeManager.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
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
        const string FOCUS_TREE_PATH = @"\common\national_focus\";

        const string LOCALISATION_PATH = @"\localisation\";

        const string EVENTS_PATH = @"\events\";

        public string filename { get; set; }

        [DataMember(Name = "foci_containers", Order = 0)]
        public List<FociGridContainer> fociContainerList { get; set; }

        [DataMember(Name = "locale_containers", Order = 1)]
        public List<LocalisationContainer> localisationList { get; set; }

        [DataMember(Name = "event_containers", Order = 2)]
        public List<EventContainer> eventList { get; set; }

        [DataMember(Name = "script_containers", Order = 3)]
        public List<ScriptContainer> scriptList { get; set; }

        [DataMember(Name = "mod_folder_list", Order = 4)]
        public List<string> modFolderList { get; set; }

        [DataMember(Name = "pre_load_content", Order = 5)]
        public bool preloadGameContent { get; set; }

        public Project()
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

        public void ExportProject(string filename)
        {
            string path = filename + FOCUS_TREE_PATH;
            Directory.CreateDirectory(path);
            //For each parsed focus trees
            foreach (KeyValuePair<string, string> item in
                FocusTreeParser.ParseAllTrees(fociContainerList))
            {
                using (TextWriter tw = new StreamWriter(path + item.Key + ".txt"))
                {
                    tw.Write(item.Value);
                }
            }
            path = filename + LOCALISATION_PATH;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            //For each parsed localisation files
            foreach (KeyValuePair<string, string> item in
                LocalisationParser.ParseEverything(localisationList))
            {
                using (TextWriter tw = new StreamWriter(path + item.Key + ".yaml"))
                {
                    tw.Write(item.Value);
                }
            }
            path = filename + EVENTS_PATH;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            //For each parsed event file
            foreach (KeyValuePair<string, string> item in
                EventParser.ParseAllEvents(eventList))
            {
                using (TextWriter tw = new StreamWriter(path + item.Key + ".txt"))
                {
                    tw.Write(item.Value);
                }
            }
            //For each parsed script file
            foreach (KeyValuePair<string, string> item in
                ScriptParser.ParseEverything(scriptList))
            {
                using (TextWriter tw = new StreamWriter(filename + "\\" + item.Key + ".txt"))
                {
                    tw.Write(item.Value);
                }
            }
        }

        public void UpdateDataContract(ProjectModel model)
        {
            filename = model.Filename;
            modFolderList = model.ListModFolders.ToList();
            preloadGameContent = model.PreloadGameContent;
            //Build foci list
            fociContainerList.Clear();
            foreach (FocusGridModel item in model.fociList)
            {
                fociContainerList.Add(new FociGridContainer(item));
            }
            //Build localization list
            localisationList.Clear();
            foreach (LocalisationModel item in model.localisationList)
            {
                localisationList.Add(new LocalisationContainer(item));
            }
            //Build events list
            eventList.Clear();
            foreach (EventTabModel item in model.eventList)
            {
                eventList.Add(new EventContainer(item));
            }
            //Build scripts list
            scriptList.Clear();
            foreach (ScriptModel item in model.scriptList)
            {
                scriptList.Add(new ScriptContainer()
                {
                    IdentifierID = item.UniqueID,
                    ContainerID = item.Filename,
                    InternalScript = item.InternalScript
                });
            }
        }
    }
}
