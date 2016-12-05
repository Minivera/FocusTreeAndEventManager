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

namespace FocusTreeManager.Model
{
    public class EventModel : ObservableObject
    {
        public Event DataContract { get; set; }
        
        public string Id
        {
            get
            {
                return DataContract.Id;
            }
            set
            {
                DataContract.Id = value;
                RaisePropertyChanged(() => Id);
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => Description);
            }
        }

        public string Title
        {
            get
            {
                var locales = Project.Instance.getLocalisationWithKey(Id + ".t");
                string translation = locales != null ? locales.translateKey(Id + ".t") : null;
                return translation != null ? translation : Id + ".t";
            }
        }

        public string Description
        {
            get
            {
                var locales = Project.Instance.getLocalisationWithKey(Id + ".d");
                string translation = locales != null ? locales.translateKey(Id + ".d") : null;
                return translation != null ? translation : Id + ".d";
            }
        }

        public EventType Type
        {
            get
            {
                return DataContract.Type;
            }
            set
            {
                DataContract.Type = value;
                RaisePropertyChanged(() => Type);
            }
        }

        public string Picture
        {
            get
            {
                return DataContract.Picture;
            }
            set
            {
                DataContract.Picture = value;
                RaisePropertyChanged(() => Picture);
            }
        }

        public string ImagePath
        {
            get
            {
                return DataContract.ImagePath;
            }
        }

        public bool IsMajor
        {
            get
            {
                return DataContract.IsMajor;
            }
            set
            {
                DataContract.IsMajor = value;
                RaisePropertyChanged(() => IsMajor);
            }
        }

        public bool IsTriggeredOnly
        {
            get
            {
                return DataContract.IsTriggeredOnly;
            }
            set
            {
                DataContract.IsTriggeredOnly = value;
                RaisePropertyChanged(() => IsTriggeredOnly);
            }
        }

        public bool IsHidden
        {
            get
            {
                return DataContract.IsHidden;
            }
            set
            {
                DataContract.IsHidden = value;
                RaisePropertyChanged(() => IsHidden);
            }
        }

        public bool IsFiredOnce
        {
            get
            {
                return DataContract.IsFiredOnce;
            }
            set
            {
                DataContract.IsFiredOnce = value;
                RaisePropertyChanged(() => IsFiredOnce);
            }
        }

        public Script InternalScript
        {
            get
            {
                return DataContract.InternalScript;
            }
            set
            {
                DataContract.InternalScript = value;
                RaisePropertyChanged(() => InternalScript);
            }
        }

        public List<EventDescriptionModel> Descriptions
        {
            get
            {
                return DataContract.getDescriptionModels();
            }
        }

        public List<EventOptionModel> Options
        {
            get
            {
                return DataContract.getOptionModels();
            }
        }

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

        public EventModel(Event linkedContract)
        {
            DataContract = linkedContract;
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
    }
}
