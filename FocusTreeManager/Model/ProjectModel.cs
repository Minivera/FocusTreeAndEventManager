using FocusTreeManager.DataContract;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using MonitoredUndo;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using FocusTreeManager.Model.TabModels;

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

        public string ProjectName => string.IsNullOrEmpty(Filename) ? 
            "New Project" : Path.GetFileNameWithoutExtension(Filename);

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
                c => c.UniqueID == containerID);
            return container;
        }

        public LocalisationModel getSpecificLocalisationMap(Guid containerID)
        {
            LocalisationModel container = localisationList.SingleOrDefault(
                c => c.UniqueID == containerID);
            return container;
        }

        public EventTabModel getSpecificEventList(Guid containerID)
        {
            EventTabModel container = eventList.SingleOrDefault(
                c => c.UniqueID == containerID);
            return container;
        }

        public ScriptModel getSpecificScriptList(Guid containerID)
        {
            ScriptModel container = scriptList.SingleOrDefault(
                c => c.UniqueID == containerID);
            return container;
        }

        public static ProjectModel createFromDataContract(Project dataContract)
        {
            ProjectModel newproject = new ProjectModel
            {
                ListModFolders = new ObservableCollection<string>(dataContract.modFolderList),
                PreloadGameContent = dataContract.preloadGameContent
            };
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
                    VisibleName = container.ContainerID,
                    InternalScript = container.InternalScript,
                    FileInfo = container.FileInfo
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

        private void fociList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "fociList",
                fociList, e, "fociList Changed");
        }

        private void localisationList_CollectionChanged(object sender, 
            NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "localisationList",
                localisationList, e, "localisationList Changed");
        }

        private void eventList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "eventList",
                eventList, e, "eventList Changed");
        }

        private void scriptList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "scriptList",
                scriptList, e, "scriptList Changed");
        }

        public object GetUndoRoot()
        {
            return new ViewModelLocator().Main;
        }

        #endregion
    }
}
