using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using FocusTreeManager.CodeStructures;
using System;
using System.Collections.ObjectModel;
using FocusTreeManager.DataContract;

namespace FocusTreeManager.Model
{
    public class FocusModel : ObservableObject
    {
        public Focus DataContract { get; set; }

        public string Image
        {
            get
            {
                return DataContract.Image;
            }
            set
            {
                DataContract.Icon = value;
                RaisePropertyChanged(() => Image);
            }
        }

        public string Icon
        {
            get
            {
                return DataContract.Image;
            }
        }
        
        public string UniqueName
        { 
            get
            {
                if (DataContract.UniqueName == null)
                {
                    return "unknown";
                }
                return DataContract.UniqueName;
            }
            set
            {
                DataContract.UniqueName = value;
                RaisePropertyChanged(() => UniqueName);
                RaisePropertyChanged(() => VisibleName);
                RaisePropertyChanged(() => Description);
            }
        }

        public string VisibleName
        {
            get
            {
                var locales = Project.Instance.getLocalisationWithKey(UniqueName);
                string translation = locales != null ? locales.translateKey(UniqueName) : null;
                return translation != null ? translation : UniqueName;
            }
        }

        public string Description
        {
            get
            {
                var locales = Project.Instance.getLocalisationWithKey(UniqueName + "_desc");
                string translation = locales != null ? locales.translateKey(UniqueName + "_desc") : null;
                return translation != null ? translation : UniqueName + "_desc";
            }
        }

        public int X
        {
            get
            {
                return DataContract.X;
            }
            set
            {
                DataContract.X = value;
                RaisePropertyChanged(() => X);
            }
        }

        public int Y
        {
            get
            {
                return DataContract.Y;
            }
            set
            {
                DataContract.Y = value;
                RaisePropertyChanged(() => Y);
            }
        }

        public double Cost
        {
            get
            {
                return DataContract.Cost;
            }
            set
            {
                DataContract.Cost = value;
                RaisePropertyChanged(() => Cost);
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

        public Point FocusTop { get; set; }

        public Point FocusBottom { get; set; }

        public Point FocusLeft { get; set; }

        public Point FocusRight { get; set; }

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

        public List<PrerequisitesSetModel> Prerequisite
        {
            get
            {
                return DataContract.getPrerequisitesModels();
            }
        }

        public List<MutuallyExclusiveSetModel> MutualyExclusive
        {
            get
            {
                return DataContract.getMutuallyExclusivesModels();
            }
        }

        public RelayCommand EditFocusCommand { get; private set; }

        public RelayCommand DeleteFocusCommand { get; private set; }

        public RelayCommand MutuallyFocusCommand { get; private set; }

        public RelayCommand<string> PrerequisiteFocusCommand { get; private set; }
        
        public RelayCommand TestFinishCommand { get; private set; }

        public FocusModel(Focus linkedContract)
        {
            DataContract = linkedContract;
            EditFocusCommand = new RelayCommand(Edit);
            DeleteFocusCommand = new RelayCommand(Delete);
            MutuallyFocusCommand = new RelayCommand(AddMutuallyExclusive);
            PrerequisiteFocusCommand = new RelayCommand<string>(AddPrerequisite);
            TestFinishCommand = new RelayCommand(FinishSetCommands);
        }

        static public FocusModel createNewModel()
        {
            Focus contract = new Focus();
            return contract.Model;
        }

        public void setPoints(Point Top, Point Bottom, Point Left, Point Right)
        {
            FocusTop = Top;
            FocusBottom = Bottom;
            FocusLeft = Left;
            FocusRight = Right;
        }

        public void Edit()
        {
            System.Windows.Application.Current.Properties["Mode"] = "Edit";
            Messenger.Default.Send(new NotificationMessage(this, "ShowEditFocus"));
        }

        public void Delete()
        {
            //Kill the focus sets
            foreach (MutuallyExclusiveSetModel set in MutualyExclusive.ToList())
            {
                set.DataContract.DeleteSetRelations();
            }
            foreach (PrerequisitesSetModel set in Prerequisite.ToList())
            {
                set.DataContract.DeleteSetRelations();
            }
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
