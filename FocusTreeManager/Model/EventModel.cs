using FocusTreeManager.CodeStructures;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using static FocusTreeManager.DataContract.Event;
using FocusTreeManager.DataContract;
using MonitoredUndo;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Media;
using FocusTreeManager.Helper;
using FocusTreeManager.Model.TabModels;

namespace FocusTreeManager.Model
{
    public class EventModel : ObservableObject, ISupportsUndo
    {
        private string id;

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                if (value == id)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Id", id, value, "Id Changed");
                id = value;
                RaisePropertyChanged(() => Id);
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => Description);
            }
        }

        public string Title
        {
            get
            {
                
                LocalisationModel locales = new ViewModelLocator().Main.Project.DefaultLocale;
                string translation = locales?.translateKey(Id + ".t");
                return translation ?? Id + ".t";
            }
        }

        public string Description
        {
            get
            {
                LocalisationModel locales = new ViewModelLocator().Main.Project.DefaultLocale;
                string translation = locales?.translateKey(Id + ".d");
                return translation ?? Id + ".d";
            }
        }

        private EventType type;

        public EventType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (value == type)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Type", type, value, "Type Changed");
                type = value;
                RaisePropertyChanged(() => Type);
            }
        }

        private string picture;

        public string Picture
        {
            get
            {
                return picture;
            }
            set
            {
                if (value == picture)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Picture", picture, value, "Picture Changed");
                picture = value;
                RaisePropertyChanged(() => Picture);
            }
        }

        public ImageSource ImagePath => ImageHelper.getImageFromGame(picture, ImageType.Event);

        private bool isMajor;

        public bool IsMajor
        {
            get
            {
                return isMajor;
            }
            set
            {
                if (value == isMajor)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "IsMajor", isMajor, value, "IsMajor Changed");
                isMajor = value;
                RaisePropertyChanged(() => IsMajor);
            }
        }

        private bool isTriggeredOnly;

        public bool IsTriggeredOnly
        {
            get
            {
                return isTriggeredOnly;
            }
            set
            {
                if (value == isTriggeredOnly)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "IsTriggeredOnly", isTriggeredOnly, value, "IsTriggeredOnly Changed");
                isTriggeredOnly = value;
                RaisePropertyChanged(() => IsTriggeredOnly);
            }
        }

        private bool isHidden;

        public bool IsHidden
        {
            get
            {
                return isHidden;
            }
            set
            {
                if (value == isHidden)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "IsHidden", isHidden, value, "IsHidden Changed");
                isHidden = value;
                RaisePropertyChanged(() => IsHidden);
            }
        }

        private bool isFiredOnce;

        public bool IsFiredOnce
        {
            get
            {
                return isFiredOnce;
            }
            set
            {
                if (value == isHidden)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "IsFiredOnce", isFiredOnce, value, "IsFiredOnce Changed");
                isFiredOnce = value;
                RaisePropertyChanged(() => IsFiredOnce);
            }
        }

        private Script internalScript;

        public Script InternalScript
        {
            get
            {
                return internalScript;
            }
            set
            {
                if (value == internalScript)
                {
                    return;
                }
                internalScript = value;
                RaisePropertyChanged(() => InternalScript);
            }
        }

        public ObservableCollection<EventDescriptionModel> Descriptions { get; set; }

        public ObservableCollection<EventOptionModel> Options { get; set; }

        private string note;

        public string Note
        {
            get
            {
                return note;
            }
            set
            {
                if (value == note)
                {
                    return;
                }
                note = value;
                RaisePropertyChanged(() => Note);
            }
        }

        public RelayCommand EditScriptCommand { get; private set; }

        public RelayCommand DeleteEventCommand { get; private set; }

        public RelayCommand<object> EditOptionScriptCommand { get; private set; }

        public RelayCommand<object> EditDescScriptCommand { get; private set; }

        public RelayCommand ChangeImageCommand { get; private set; }

        public RelayCommand<string> EditLocaleCommand { get; private set; }

        public EventModel()
        {
            Descriptions = new ObservableCollection<EventDescriptionModel>();
            Descriptions.CollectionChanged += Descriptions_CollectionChanged;
            Options = new ObservableCollection<EventOptionModel>();
            Options.CollectionChanged += Options_CollectionChanged;
            //Commands
            EditScriptCommand = new RelayCommand(EditScript);
            DeleteEventCommand = new RelayCommand(DeleteElement);
            EditOptionScriptCommand = new RelayCommand<object>(EditOptionScript);
            EditDescScriptCommand = new RelayCommand<object>(EditDescriptionScript);
            ChangeImageCommand = new RelayCommand(ChangeImage);
            EditLocaleCommand = new RelayCommand<string>(EditLocale, CanEditLocale);
        }

        public EventModel(Event item)
        {
            id = item.Id;
            type = item.Type;
            picture = item.Picture;
            isFiredOnce = item.IsFiredOnce;
            isHidden = item.IsHidden;
            isMajor = item.IsMajor;
            isTriggeredOnly = item.IsTriggeredOnly;
            Script newScript = new Script();
            newScript.Analyse(item.InternalScript.Parse());
            internalScript = newScript;
            note = item.Note;
            Descriptions = new ObservableCollection<EventDescriptionModel>();
            foreach (EventDescription description in item.Descriptions)
            {
                newScript = new Script();
                newScript.Analyse(description.InternalScript.Parse());
                Descriptions.Add(new EventDescriptionModel
                    {
                        InternalScript = newScript
                });
            }
            Descriptions.CollectionChanged += Descriptions_CollectionChanged;
            Options = new ObservableCollection<EventOptionModel>();
            foreach (EventOption option in item.Options)
            {
                newScript = new Script();
                newScript.Analyse(option.InternalScript.Parse());
                Options.Add(new EventOptionModel
                {
                    Name = option.Name,
                    InternalScript = newScript
                });
            }
            Options.CollectionChanged += Options_CollectionChanged;
            //Commands
            EditScriptCommand = new RelayCommand(EditScript);
            DeleteEventCommand = new RelayCommand(DeleteElement);
            EditOptionScriptCommand = new RelayCommand<object>(EditOptionScript);
            EditDescScriptCommand = new RelayCommand<object>(EditDescriptionScript);
            ChangeImageCommand = new RelayCommand(ChangeImage);
            EditLocaleCommand = new RelayCommand<string>(EditLocale, CanEditLocale);
        }

        public EventModel(EventModel item)
        {
            id = item.Id;
            type = item.Type;
            picture = item.Picture;
            isFiredOnce = item.IsFiredOnce;
            isHidden = item.IsHidden;
            isMajor = item.IsMajor;
            isTriggeredOnly = item.IsTriggeredOnly;
            internalScript = item.InternalScript;
            note = item.Note;
            Descriptions = new ObservableCollection<EventDescriptionModel>();
            foreach (EventDescriptionModel description in item.Descriptions)
            {
                Descriptions.Add(new EventDescriptionModel
                {
                    InternalScript = description.InternalScript
                });
            }
            Descriptions.CollectionChanged += Descriptions_CollectionChanged;
            Options = new ObservableCollection<EventOptionModel>();
            foreach (EventOptionModel option in item.Options)
            {
                Options.Add(new EventOptionModel
                {
                    Name = option.Name,
                    InternalScript = option.InternalScript
                });
            }
            Options.CollectionChanged += Options_CollectionChanged;
            //Commands
            EditScriptCommand = new RelayCommand(EditScript);
            DeleteEventCommand = new RelayCommand(DeleteElement);
            EditOptionScriptCommand = new RelayCommand<object>(EditOptionScript);
            EditDescScriptCommand = new RelayCommand<object>(EditDescriptionScript);
            ChangeImageCommand = new RelayCommand(ChangeImage);
        }

        public void EditScript()
        {
            ScripterViewModel ViewModel = new ViewModelLocator().Scripter;
            ViewModel.ScriptType = ScripterType.Event;
            ViewModel.ManagedScript = internalScript;
            EditScript dialog = new EditScript();
            dialog.ShowDialog();
            InternalScript = ViewModel.ManagedScript;
        }

        private static void EditOptionScript(object option)
        {
            EventOptionModel model = option as EventOptionModel;
            model?.EditOptionScript();
        }

        private static void EditDescriptionScript(object description)
        {
            EventDescriptionModel model = description as EventDescriptionModel;
            model?.EditDescScript();
        }

        public void DeleteElement()
        {
            Messenger.Default.Send(new NotificationMessage(this, 
                new ViewModelLocator().Main.SelectedTab, "DeleteEvent"));
        }

        public void ChangeImage()
        {
            ChangeImage view = new ChangeImage();
            new ViewModelLocator().ChangeImage.LoadImages("Events", Picture);
            view.ShowDialog();
            Picture = new ViewModelLocator().ChangeImage.FocusImage;
        }

        public void EditLocale(string param)
        {
            if (!CanEditLocale(param))
            {
                return;
            }
            switch (param)
            {
                case "Title":
                    LocalizatorViewModel vm = new ViewModelLocator().Localizator;
                    LocalisationModel locales = new ViewModelLocator().Main.Project.DefaultLocale;
                    LocaleModel model = locales.LocalisationMap.FirstOrDefault(
                        l => l.Key == Id + ".t");
                    if (model != null)
                    {
                        vm.Locale = model;
                    }
                    else
                    {
                        vm.Locale = new LocaleModel
                        {
                            Key = Id + ".t",
                            Value = Title
                        };
                    }
                    vm.AddOrUpdateCommand = AddOrUpdateLocale;
                    vm.RaisePropertyChanged(() => vm.Locale);
                    break;
                case "Description":
                    vm = new ViewModelLocator().Localizator;
                    locales = new ViewModelLocator().Main.Project.DefaultLocale;
                    model = locales.LocalisationMap.FirstOrDefault(
                        l => l.Key == Id + ".d");
                    if (model != null)
                    {
                        vm.Locale = model;
                    }
                    else
                    {
                        vm.Locale = new LocaleModel
                        {
                            Key = Id + ".d",
                            Value = Description
                        };
                    }
                    vm.AddOrUpdateCommand = AddOrUpdateLocale;
                    vm.RaisePropertyChanged(() => vm.Locale);
                    break;
            }
        }

        public bool CanEditLocale(string param)
        {
            LocalisationModel locales = new ViewModelLocator().Main.Project.DefaultLocale;
            return locales != null;
        }

        public void AddOrUpdateLocale()
        {
            LocalizatorViewModel vm = new ViewModelLocator().Localizator;
            LocalisationModel locales = new ViewModelLocator().Main.Project.DefaultLocale;
            LocaleModel model = locales.LocalisationMap.FirstOrDefault(
                        l => l.Key == vm.Locale.Key);
            if (model == null)
            {
                locales.LocalisationMap.Add(vm.Locale);
            }
            locales.RaisePropertyChanged(() => locales.LocalisationMap);
            RaisePropertyChanged(() => Title);
            RaisePropertyChanged(() => Description);
        }

        public void setDefaults(string EventNamespace)
        {
            if (string.IsNullOrEmpty(EventNamespace))
            {
                EventNamespace = "namespace";
            }
            id = EventNamespace + ".0";
            picture = "event_test";
            type = EventType.country_event;
            internalScript = new Script();
            EventDescriptionModel newDesc = new EventDescriptionModel();
            newDesc.setDefaults();
            Descriptions.Add(newDesc);
            EventOptionModel newOptions = new EventOptionModel();
            newOptions.setDefaults();
            Options.Add(newOptions);
        }

        #region Undo/Redo

        private void Descriptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "Descriptions",
                Descriptions, e, "Descriptions Changed");
        }

        private void Options_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "Options",
                Options, e, "Options Changed");
        }

        public object GetUndoRoot()
        {
            return new ViewModelLocator().Main;
        }

        #endregion

    }
}
