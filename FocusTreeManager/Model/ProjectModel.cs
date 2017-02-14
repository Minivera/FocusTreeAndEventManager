using FocusTreeManager.DataContract;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using MonitoredUndo;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace FocusTreeManager.Model
{
    public class ProjectModel : ObservableObject, ISupportsUndo
    {
        private string filename;

        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                if (value == filename)
                {
                    return;
                }
                filename = value;
                RaisePropertyChanged(() => Filename);
                RaisePropertyChanged(() => ProjectName);
            }
        }

        public string ProjectName
        {
            get
            {
                if (string.IsNullOrEmpty(Filename))
                {
                    return "New Project";
                }
                return Path.GetFileNameWithoutExtension(Filename);
            }
        }

        private bool preloadGameContent;

        public bool PreloadGameContent
        {
            get
            {
                return preloadGameContent;
            }
            set
            {
                if (value == preloadGameContent)
                {
                    return;
                }
                preloadGameContent = value;
                RaisePropertyChanged(() => PreloadGameContent);
            }
        }

        private LocalisationModel defaultLocale;

        public LocalisationModel DefaultLocale
        {
            get
            {
                return defaultLocale;
            }
            set
            {
                if (value == defaultLocale)
                {
                    return;
                }
                defaultLocale = value;
                RaisePropertyChanged(() => DefaultLocale);
            }
        }

        public ObservableCollection<string> ListModFolders { get; set; }

        public ObservableCollection<FocusGridModel> fociList { get; set; }
        
        public ObservableCollection<LocalisationModel> localisationList { get; set; }
        
        public ObservableCollection<EventTabModel> eventList { get; set; }
        
        public ObservableCollection<ScriptModel> scriptList { get; set; }

        public ProjectModel()
        {
            ListModFolders = new ObservableCollection<string>();
            fociList = new ObservableCollection<FocusGridModel>();
            fociList.CollectionChanged += fociList_CollectionChanged;
            localisationList = new ObservableCollection<LocalisationModel>();
            localisationList.CollectionChanged += localisationList_CollectionChanged;
            eventList = new ObservableCollection<EventTabModel>();
            eventList.CollectionChanged += eventList_CollectionChanged;
            scriptList = new ObservableCollection<ScriptModel>();
            scriptList.CollectionChanged += scriptList_CollectionChanged;
        }

        public FocusGridModel getSpecificFociList(Guid containerID)
        {
            FocusGridModel container = fociList.SingleOrDefault(
                (c) => c.UniqueID == containerID);
            return container;
        }

        public LocalisationModel getSpecificLocalisationMap(Guid containerID)
        {
            LocalisationModel container = localisationList.SingleOrDefault(
                (c) => c.UniqueID == containerID);
            return container;
        }

        public EventTabModel getSpecificEventList(Guid containerID)
        {
            EventTabModel container = eventList.SingleOrDefault(
                (c) => c.UniqueID == containerID);
            return container;
        }

        public ScriptModel getSpecificScriptList(Guid containerID)
        {
            ScriptModel container = scriptList.SingleOrDefault(
                (c) => c.UniqueID == containerID);
            return container;
        }

        static public ProjectModel createFromDataContract(Project dataContract)
        {
            ProjectModel newproject = new ProjectModel();
            //Build foci list
            foreach (FociGridContainer container in dataContract.fociContainerList)
            {
                newproject.fociList.Add(new FocusGridModel(container));
            }
            //Build localization list
            foreach (LocalisationContainer container in dataContract.localisationList)
            {
                newproject.localisationList.Add(new LocalisationModel(container));
            }
            //Build events list
            foreach (EventContainer container in dataContract.eventList)
            {
                newproject.eventList.Add(new EventTabModel(container));
            }
            //Build scripts list
            foreach (ScriptContainer container in dataContract.scriptList)
            {
                newproject.scriptList.Add(new ScriptModel(container.ContainerID)
                {
                    Filename = container.ContainerID,
                    InternalScript = container.InternalScript
                });
            }
            if (newproject.localisationList.Any() && dataContract.defaultLocale != null)
            {
                newproject.DefaultLocale = newproject.localisationList.FirstOrDefault(
                    l => l.UniqueID == dataContract.defaultLocale.IdentifierID);
            }
            return newproject;
        }

        #region Undo/Redo

        void fociList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "fociList", 
                this.fociList, e, "fociList Changed");
        }

        void localisationList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "localisationList",
                this.localisationList, e, "localisationList Changed");
        }

        void eventList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "eventList",
                this.eventList, e, "eventList Changed");
        }

        void scriptList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "scriptList",
                this.scriptList, e, "scriptList Changed");
        }

        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion
    }
}
