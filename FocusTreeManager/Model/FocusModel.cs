using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Linq;
using FocusTreeManager.CodeStructures;
using System.Collections.ObjectModel;
using FocusTreeManager.DataContract;
using MonitoredUndo;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using FocusTreeManager.Helper;

namespace FocusTreeManager.Model
{
    public class FocusModel : ObservableObject, ISupportsUndo
    {
        private string image;

        public string Image
        {
            get
            {
                return image;
            }
            set
            {
                if (value == image)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Image", image, value, "Image Changed");
                image = value;
                RaisePropertyChanged(() => Image);
                RaisePropertyChanged(() => Icon);
            }
        }

        public ImageSource Icon
        {
            get
            {
                return ImageHelper.getImageFromGame(image, ImageType.Goal);
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
                if (value == uniquename)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "UniqueName", uniquename, value, "UniqueName Changed");
                uniquename = value;
                RaisePropertyChanged(() => UniqueName);
                RaisePropertyChanged(() => VisibleName);
                RaisePropertyChanged(() => Description);
            }
        }

        public string VisibleName
        {
            get
            {
                var locales = (new ViewModelLocator()).Main.Project.DefaultLocale;
                string translation = locales != null ? locales.translateKey(UniqueName) : null;
                return translation != null ? translation : UniqueName;
            }
        }

        public string Description
        {
            get
            {
                var locales = (new ViewModelLocator()).Main.Project.DefaultLocale;
                string translation = locales != null ? locales.translateKey(UniqueName + "_desc") : null;
                return translation != null ? translation : UniqueName + "_desc";
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
                if (value == x)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "X", x, value, "X Changed");
                x = value;
                RaisePropertyChanged(() => X);
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
                if (value == y)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Y", y, value, "Y Changed");
                y = value;
                RaisePropertyChanged(() => Y);
            }
        }

        private double cost;

        public double Cost
        {
            get
            {
                return cost;
            }
            set
            {
                if (value == cost)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Cost", cost, value, "Cost Changed");
                cost = value;
                RaisePropertyChanged(() => Cost);
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
                //Cannot undo the script changes, do it in the scripter
                internalScript = value;
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
                //Don,t care about undo here
                Set<bool>(() => this.IsSelected, ref this.isSelected, value);
            }
        }
        
        public ObservableCollection<PrerequisitesSetModel> Prerequisite { get; set; }
        
        public ObservableCollection<MutuallyExclusiveSetModel> MutualyExclusive { get; set; }

        public RelayCommand EditFocusCommand { get; private set; }

        public RelayCommand DeleteFocusCommand { get; private set; }

        public RelayCommand MutuallyFocusCommand { get; private set; }

        public RelayCommand<string> PrerequisiteFocusCommand { get; private set; }
        
        public RelayCommand TestFinishCommand { get; private set; }

        public RelayCommand<string> EditLocaleCommand { get; private set; }

        public FocusModel()
        {
            Prerequisite = new ObservableCollection<PrerequisitesSetModel>();
            Prerequisite.CollectionChanged += Prerequisite_CollectionChanged;
            MutualyExclusive = new ObservableCollection<MutuallyExclusiveSetModel>();
            MutualyExclusive.CollectionChanged += MutualyExclusive_CollectionChanged;
            //Commands
            EditFocusCommand = new RelayCommand(Edit);
            DeleteFocusCommand = new RelayCommand(Delete);
            MutuallyFocusCommand = new RelayCommand(AddMutuallyExclusive);
            PrerequisiteFocusCommand = new RelayCommand<string>(AddPrerequisite);
            TestFinishCommand = new RelayCommand(FinishSetCommands);
            EditLocaleCommand = new RelayCommand<string>(EditLocale, CanEditLocale);
        }

        public FocusModel(Focus focus)
        {
            image = focus.Image;
            uniquename = focus.UniqueName;
            x = focus.X;
            y = focus.Y;
            cost = focus.Cost;
            internalScript = focus.InternalScript;
            Prerequisite = new ObservableCollection<PrerequisitesSetModel>();
            Prerequisite.CollectionChanged += Prerequisite_CollectionChanged;
            MutualyExclusive = new ObservableCollection<MutuallyExclusiveSetModel>();
            MutualyExclusive.CollectionChanged += MutualyExclusive_CollectionChanged;
            //Commands
            EditFocusCommand = new RelayCommand(Edit);
            DeleteFocusCommand = new RelayCommand(Delete);
            MutuallyFocusCommand = new RelayCommand(AddMutuallyExclusive);
            PrerequisiteFocusCommand = new RelayCommand<string>(AddPrerequisite);
            TestFinishCommand = new RelayCommand(FinishSetCommands);
            EditLocaleCommand = new RelayCommand<string>(EditLocale, CanEditLocale);
        }

        public void RepairSets(Focus focus, List<FocusModel> fociList)
        {
            foreach (PrerequisitesSet set in focus.Prerequisite)
            {
                //Add the linked focus
                PrerequisitesSetModel prerequiste = new PrerequisitesSetModel(
                    fociList.FirstOrDefault(f => f.UniqueName == set.Focus.UniqueName));
                //Run through all the parents and add them
                foreach (Focus parent in set.FociList)
                {
                    prerequiste.FociList.Add(
                        fociList.FirstOrDefault(f => f.UniqueName == parent.UniqueName));
                }
                Prerequisite.Add(prerequiste);
            }
            foreach (MutuallyExclusiveSet set in focus.MutualyExclusive)
            {
                //Create the set with both foci
                MutualyExclusive.Add(new MutuallyExclusiveSetModel(
                    fociList.FirstOrDefault(f => f.UniqueName == set.Focus1.UniqueName),
                    fociList.FirstOrDefault(f => f.UniqueName == set.Focus2.UniqueName)));
            }
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
            UndoService.Current[GetUndoRoot()].BeginChangeSetBatch("FullyDeleteFocus", false);
            //Kill the focus sets
            foreach (MutuallyExclusiveSetModel set in MutualyExclusive.ToList())
            {
                set.DeleteSetRelations();
            }
            foreach (PrerequisitesSetModel set in Prerequisite.ToList())
            {
                set.DeleteSetRelations();
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

        public void EditLocale(string param)
        {
            if (!CanEditLocale(param))
            {
                return;
            }
            switch (param)
            {
                case "VisibleName":
                    LocalizatorViewModel vm = (new ViewModelLocator()).Localizator;
                    var locales = (new ViewModelLocator()).Main.Project.DefaultLocale;
                    LocaleModel model = locales.LocalisationMap.FirstOrDefault(
                        l => l.Key == this.UniqueName);
                    if (model != null)
                    {
                        vm.Locale = model;
                    }
                    else
                    {
                        vm.Locale = new LocaleModel()
                        {
                            Key = this.UniqueName,
                            Value = this.VisibleName
                        };
                    }
                    vm.AddOrUpdateCommand = AddOrUpdateLocale;
                    vm.RaisePropertyChanged("Locale");
                    break;
                case "Description":
                    vm = (new ViewModelLocator()).Localizator;
                    locales = (new ViewModelLocator()).Main.Project.DefaultLocale;
                    model = locales.LocalisationMap.FirstOrDefault(
                        l => l.Key == this.UniqueName + "_desc");
                    if (model != null)
                    {
                        vm.Locale = model;
                    }
                    else
                    {
                        vm.Locale = new LocaleModel()
                        {
                            Key = this.UniqueName,
                            Value = this.Description
                        };
                    }
                    vm.AddOrUpdateCommand = AddOrUpdateLocale;
                    vm.RaisePropertyChanged("Locale");
                    break;
            }
        }

        public bool CanEditLocale(string param)
        {
            LocalizatorViewModel vm = (new ViewModelLocator()).Localizator;
            var locales = (new ViewModelLocator()).Main.Project.DefaultLocale;
            return locales != null;
        }

        public void AddOrUpdateLocale()
        {
            LocalizatorViewModel vm = (new ViewModelLocator()).Localizator;
            var locales = (new ViewModelLocator()).Main.Project.DefaultLocale;
            LocaleModel model = locales.LocalisationMap.FirstOrDefault(
                        l => l.Key == vm.Locale.Key);
            if (model == null)
            {
                locales.LocalisationMap.Add(vm.Locale);
            }
            RaisePropertyChanged("LocalisationMap");
            RaisePropertyChanged(() => VisibleName);
            RaisePropertyChanged(() => Description);
        }

        public void AddPrerequisite(string Type)
        {
            System.Windows.Application.Current.Properties["ModeParam"] = Type;
            Messenger.Default.Send(new NotificationMessage(this, "AddFocusPrerequisite"));
        }
        
        public void setDefaults(int FocusNumber)
        {
            internalScript = new Script();
            image = "goal_unknown";
            uniquename = "newfocus_" + FocusNumber;
            x = 0;
            y = 0;
        }

        #region Undo/Redo

        void Prerequisite_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "Prerequisite",
                Prerequisite, e, "Prerequisite Changed");
        }

        void MutualyExclusive_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, "MutualyExclusive",
                MutualyExclusive, e, "MutualyExclusive Changed");
        }

        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion
    }
}
