using FocusTreeManager.CodeStructures;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using static FocusTreeManager.DataContract.Event;
using FocusTreeManager.DataContract;
using MonitoredUndo;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Media;
using FocusTreeManager.Helper;

namespace FocusTreeManager.Model
{
    public class EventModel : ObservableObject, ISupportsUndo
    {
        const string IMAGE_PATH = "/FocusTreeManager;component/GFX/Events/";

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
                
                var locales = (new ViewModelLocator()).Main.Project.DefaultLocale;
                string translation = locales != null ? locales.translateKey(Id + ".t") : null;
                return translation != null ? translation : Id + ".t";
            }
        }

        public string Description
        {
            get
            {
                var locales = (new ViewModelLocator()).Main.Project.DefaultLocale;
                string translation = locales != null ? locales.translateKey(Id + ".d") : null;
                return translation != null ? translation : Id + ".d";
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

        public ImageSource ImagePath
        {
            get
            {
                return ImageHelper.getImageFromGame(picture, ImageType.Event);
            }
        }

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

        public bool SwitchIsChecked
        {
            get
            {
                if (Type == EventType.country_event)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                if (value)
                {
                    Type = EventType.news_event;
                }
                else
                {
                    Type = EventType.country_event;
                }
                RaisePropertyChanged(() => SwitchIsChecked);
            }
        }

        public RelayCommand EditScriptCommand { get; private set; }

        public RelayCommand DeleteEventCommand { get; private set; }

        public RelayCommand<object> EditOptionScriptCommand { get; private set; }

        public RelayCommand<object> EditDescScriptCommand { get; private set; }

        public RelayCommand ChangeImageCommand { get; private set; }

        public EventModel()
        {
            Descriptions = new ObservableCollection<EventDescriptionModel>();
            Descriptions.CollectionChanged += Descriptions_CollectionChanged;
            Options = new ObservableCollection<EventOptionModel>();
            Options.CollectionChanged += Options_CollectionChanged;
            //Commands
            EditScriptCommand = new RelayCommand(EditScript);
            DeleteEventCommand = new RelayCommand(DeleteElement);
            EditOptionScriptCommand = new RelayCommand<object>(EditOptionScript, CanExecuteEdit);
            EditDescScriptCommand = new RelayCommand<object>(EditDescriptionScript, CanExecuteEdit);
            ChangeImageCommand = new RelayCommand(ChangeImage);
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
            internalScript = item.InternalScript;
            Descriptions = new ObservableCollection<EventDescriptionModel>();
            foreach (EventDescription description in item.Descriptions)
            {
                Descriptions.Add(new EventDescriptionModel()
                    {
                        InternalScript = description.InternalScript
                    });
            }
            Descriptions.CollectionChanged += Descriptions_CollectionChanged;
            Options = new ObservableCollection<EventOptionModel>();
            foreach (EventOption option in item.Options)
            {
                Options.Add(new EventOptionModel()
                {
                    Name = option.Name,
                    InternalScript = option.InternalScript
                });
            }
            Options.CollectionChanged += Options_CollectionChanged;
            //Commands
            EditScriptCommand = new RelayCommand(EditScript);
            DeleteEventCommand = new RelayCommand(DeleteElement);
            EditOptionScriptCommand = new RelayCommand<object>(EditOptionScript, CanExecuteEdit);
            EditDescScriptCommand = new RelayCommand<object>(EditDescriptionScript, CanExecuteEdit);
            ChangeImageCommand = new RelayCommand(ChangeImage);
        }

        public void EditScript()
        {
            ScripterViewModel ViewModel = (new ViewModelLocator()).Scripter;
            EditScript dialog = new EditScript(InternalScript, 
                ScripterControlsViewModel.ScripterType.Event);
            dialog.ShowDialog();
            InternalScript = ViewModel.ManagedScript;
        }

        private void EditOptionScript(object option)
        {
            if (option is EventOptionModel)
            {
                ((EventOptionModel)option).EditOptionScript();
            }
        }

        private void EditDescriptionScript(object description)
        {
            if (description is EventDescriptionModel)
            {
                ((EventDescriptionModel)description).EditDescScript();
            }
        }

        private bool CanExecuteEdit(object arg)
        {
            return arg is EventDescriptionModel || arg is EventOptionModel;
        }

        public void DeleteElement()
        {
            Messenger.Default.Send(new NotificationMessage(this, "DeleteEvent"));
        }

        public void ChangeImage()
        {
            ChangeImage view = new ChangeImage();
            (new ViewModelLocator()).ChangeImage.LoadImages("Events", Picture);
            view.ShowDialog();
            Picture = (new ViewModelLocator()).ChangeImage.FocusImage;
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

        void Descriptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "Descriptions",
                this.Descriptions, e, "Descriptions Changed");
        }

        void Options_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "Options",
                this.Options, e, "Options Changed");
        }

        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion

    }
}
