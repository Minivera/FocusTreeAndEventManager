using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FocusTreeManager.Model
{
    [Serializable]
    public class Focus : ObservableObject
    {
        const string IMAGE_PATH = "/FocusTreeManager;component/GFX/Focus/";

        private string image;

        public string Image
        {
            get
            {
                return IMAGE_PATH + image + ".png";
            }
            set
            {
                Set<string>(() => this.Image, ref this.image, value);
            }
        }

        private string uniquename;

        public string UniqueName
        { 
            get
            {
                return uniquename;
            }
            set
            {
                Set<string>(() => this.UniqueName, ref this.uniquename, value);
            }
        }

        public string VisibleName
        {
            get
            {
                var locales = (new ViewModelLocator()).Main.Project.localisationList.FirstOrDefault(); ;
                string translation = locales != null ? locales.translateKey(uniquename) : null;
                return translation != null ? translation : uniquename;
            }
        }

        public string Description
        {
            get
            {
                var locales = (new ViewModelLocator()).Main.Project.localisationList.FirstOrDefault();
                string translation = locales != null ? locales.translateKey(uniquename + "_desc") : null;
                return translation != null ? translation : uniquename + "_desc";
            }
        }

        private int x;

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                Set<int>(() => this.X, ref this.x, value);
            }
        }

        private int y;

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                Set<int>(() => this.Y, ref this.y, value);
            }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                Set<bool>(() => this.IsSelected, ref this.isSelected, value);
            }
        }

        public List<PrerequisitesSet> Prerequisite { get; set; }

        public List<MutuallyExclusiveSet> MutualyExclusive { get; set; }

        public RelayCommand EditFocusCommand { get; private set; }

        public RelayCommand DeleteFocusCommand { get; private set; }

        public RelayCommand MutuallyFocusCommand { get; private set; }

        public RelayCommand<string> PrerequisiteFocusCommand { get; private set; }
        
        public RelayCommand TestFinishCommand { get; private set; }
        

        public Focus()
        {
            MutualyExclusive = new List<MutuallyExclusiveSet>();
            Prerequisite = new List<PrerequisitesSet>();
            EditFocusCommand = new RelayCommand(Edit);
            DeleteFocusCommand = new RelayCommand(Delete);
            MutuallyFocusCommand = new RelayCommand(AddMutuallyExclusive);
            PrerequisiteFocusCommand = new RelayCommand<string>(AddPrerequisite);
            TestFinishCommand = new RelayCommand(FinishSetCommands);
        }

        public void setDefaults()
        {
            Image = "unknown";
            UniqueName = "Unkown";
            X = 0;
            Y = 0;
        }

        public void Edit()
        {
            System.Windows.Application.Current.Properties["Mode"] = "Edit";
            Messenger.Default.Send(new NotificationMessage(this, "ShowEditFocus"));
        }

        public void Delete()
        {
            Messenger.Default.Send(new NotificationMessage(this, "DeleteFocus"));
        }

        public void AddMutuallyExclusive()
        {
            Messenger.Default.Send(new NotificationMessage(this, "AddFocusMutually"));
        }

        public void FinishSetCommands()
        {
            if ((string)System.Windows.Application.Current.Properties["Mode"] == "Mutually")
            {
                Messenger.Default.Send(new NotificationMessage(this, "FinishAddFocusMutually"));
            }
            if ((string)System.Windows.Application.Current.Properties["Mode"] == "Prerequisite")
            {
                Messenger.Default.Send(new NotificationMessage(this, "FinishAddFocusPrerequisite"));
            }
        }

        public void AddPrerequisite(string Type)
        {
            System.Windows.Application.Current.Properties["ModeParam"] = Type;
            Messenger.Default.Send(new NotificationMessage(this, "AddFocusPrerequisite"));
        }
    }
}
