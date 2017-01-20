using FocusTreeManager.DataContract;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using MonitoredUndo;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace FocusTreeManager.Model
{
    public class ProjectModel : ObservableObject, ISupportsUndo
    {
        public ObservableCollection<FocusGridModel> fociList { get; set; }
        
        public ObservableCollection<LocalisationModel> localisationList { get; set; }
        
        public ObservableCollection<EventTabModel> eventList { get; set; }
        
        public ObservableCollection<ScriptModel> scriptList { get; set; }

        public ProjectModel()
        {
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

        public LocalisationModel getLocalisationWithKey(string key)
        {
            if (key == null)
            {
                return null;
            }
            return localisationList.FirstOrDefault((l) => l.LocalisationMap.
                    Where((x) => x.Key != null && x.Key.ToLower() == key.ToLower()).Any());
        }

        public void createFromDataContract(Project dataContract)
        {
            //Build foci list
            fociList.Clear();
            foreach (FociGridContainer container in dataContract.fociContainerList)
            {
                fociList.Add(new FocusGridModel(container));
            }
            //Build localization list
            localisationList.Clear();
            foreach (LocalisationContainer container in dataContract.localisationList)
            {
                localisationList.Add(new LocalisationModel(container));
            }
            //Build events list
            eventList.Clear();
            foreach (EventContainer container in dataContract.eventList)
            {
                eventList.Add(new EventTabModel(container));
            }
            //Build scripts list
            scriptList.Clear();
            foreach (ScriptContainer container in dataContract.scriptList)
            {
                scriptList.Add(new ScriptModel(container.ContainerID)
                {
                    Filename = container.ContainerID,
                    InternalScript = container.InternalScript
                });
            }
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
