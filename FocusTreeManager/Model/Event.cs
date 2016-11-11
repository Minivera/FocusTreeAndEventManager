using FocusTreeManager.CodeStructures;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FocusTreeManager.Model
{
    [ProtoContract]
    public class Event : ObservableObject
    {
        const string IMAGE_PATH = "/FocusTreeManager;component/GFX/Events/";

        public enum EventType
        {
            news_event,
            country_event
        }

        [ProtoMember(1)]
        private string id;

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                Set<string>(() => this.Id, ref this.id, value);
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => Description);
            }
        }

        public string Title
        {
            get
            {
                var locales = (new ViewModelLocator()).Main.Project.getLocalisationWithKey(Id + ".t");
                string translation = locales != null ? locales.translateKey(Id + ".t") : null;
                return translation != null ? translation : Id + ".t";
            }
        }

        public string Description
        {
            get
            {
                var locales = (new ViewModelLocator()).Main.Project.getLocalisationWithKey(Id + ".d");
                string translation = locales != null ? locales.translateKey(Id + ".d") : null;
                return translation != null ? translation : Id + ".d";
            }
        }

        [ProtoMember(2)]
        private EventType type;

        public EventType Type
        {
            get
            {
                return type;
            }
            set
            {
                Set<EventType>(() => this.Type, ref this.type, value);
            }
        }

        [ProtoMember(3)]
        private string picture;

        public string Picture
        {
            get
            {
                return picture;
            }
            set
            {
                Set<string>(() => this.Picture, ref this.picture, value);
                RaisePropertyChanged(() => ImagePath);
            }
        }

        public string ImagePath
        {
            get
            {
                return IMAGE_PATH + picture + ".png";
            }
        }

        [ProtoMember(4)]
        private bool isMajor;

        public bool IsMajor
        {
            get
            {
                return isMajor;
            }
            set
            {
                Set<bool>(() => this.IsMajor, ref this.isMajor, value);
            }
        }

        [ProtoMember(5)]
        private bool isTriggeredOnly;

        public bool IsTriggeredOnly
        {
            get
            {
                return isTriggeredOnly;
            }
            set
            {
                Set<bool>(() => this.IsTriggeredOnly, ref this.isTriggeredOnly, value);
            }
        }

        [ProtoMember(6)]
        private bool isHidden;

        public bool IsHidden
        {
            get
            {
                return isHidden;
            }
            set
            {
                Set<bool>(() => this.IsHidden, ref this.isHidden, value);
            }
        }
        
        [ProtoMember(7)]
        private bool isFiredOnce;

        public bool IsFiredOnce
        {
            get
            {
                return isFiredOnce;
            }
            set
            {
                Set<bool>(() => this.IsFiredOnce, ref this.isFiredOnce, value);
            }
        }

        [ProtoMember(8)]
        private Script internalScript;

        public Script InternalScript
        {
            get
            {
                return internalScript;
            }
            set
            {
                Set<Script>(() => this.InternalScript, ref this.internalScript, value);
            }
        }

        [ProtoMember(9, AsReference = true)]
        public List<EventDescription> Descriptions { get; set;}

        [ProtoMember(10, AsReference = true)]
        public List<EventOption> Options { get; set; }
        
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
                    Set<EventType>(() => this.Type, ref this.type, EventType.news_event);
                }
                else
                {
                    Set<EventType>(() => this.Type, ref this.type, EventType.country_event);
                }
                RaisePropertyChanged(() => SwitchIsChecked);
            }
        }

        public RelayCommand EditScriptCommand { get; private set; }

        public RelayCommand DeleteEventCommand { get; private set; }

        public RelayCommand<object> EditOptionScriptCommand { get; private set; }

        public RelayCommand<object> EditDescScriptCommand { get; private set; }

        public RelayCommand ChangeImageCommand { get; private set; }

        public Event()
        {
            Descriptions = new List<EventDescription>();
            Options = new List<EventOption>();
            //Commands
            EditScriptCommand = new RelayCommand(EditScript);
            DeleteEventCommand = new RelayCommand(DeleteElement);
            EditOptionScriptCommand = new RelayCommand<object>(EditOptionScript, CanExecuteEdit);
            EditDescScriptCommand = new RelayCommand<object>(EditDescriptionScript, CanExecuteEdit);
            ChangeImageCommand = new RelayCommand(ChangeImage);
        }

        public void setDefaults(string EventNamespace)
        {
            if (string.IsNullOrEmpty(EventNamespace))
            {
                EventNamespace = "namespace";
            }
            Id = EventNamespace + ".0";
            Picture = "event_test";
            Type = EventType.country_event;
            InternalScript = new Script();
            EventDescription newDesc = new EventDescription();
            newDesc.setDefaults();
            Descriptions.Add(newDesc);
            RaisePropertyChanged(() => Descriptions);
            EventOption newOptions = new EventOption();
            newOptions.setDefaults();
            Options.Add(newOptions);
            RaisePropertyChanged(() => Options);
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
            if (option is EventOption)
            {
                ((EventOption)option).EditOptionScript();
            }
        }

        private void EditDescriptionScript(object description)
        {
            if (description is EventDescription)
            {
                ((EventDescription)description).EditDescScript();
            }
        }

        private bool CanExecuteEdit(object arg)
        {
            return arg is EventDescription || arg is EventOption;
        }

        public void DeleteElement()
        {
            Messenger.Default.Send(new NotificationMessage(this, "DeleteEvent"));
        }

        public void ChangeImage()
        {
            ChangeImage view = new ChangeImage();
            (new ViewModelLocator()).ChangeImage.LoadImages("Events");
            view.ShowDialog();
            Picture = (new ViewModelLocator()).ChangeImage.FocusImage;
        }
    }
}
