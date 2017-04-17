using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows;
using FocusTreeManager.Helper;

namespace FocusTreeManager.Model
{
    public class AssignationModel : ObservableObject, ICloneable
    {
        public string Text
        {
            get
            {
                return LocalizationHelper.getValueForKey(LocalizationKey);
            }
        }

        private string localizationKey;

        public string LocalizationKey
        {
            get
            {
                return localizationKey;
            }
            set
            {
                Set(() => LocalizationKey, ref localizationKey, value);
            }
        }

        private Brush color;

        public Brush Color
        {
            get
            {
                return color;
            }
            set
            {
                Set(() => Color, ref color, value);
            }
        }

        private Brush borderColor;

        public Brush BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                Set(() => BorderColor, ref borderColor, value);
            }
        }

        private Brush backgroundColor;

        public Brush BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                Set(() => BackgroundColor, ref backgroundColor, value);
            }
        }

        private bool isNotEditable;

        public bool IsNotEditable
        {
            get
            {
                return isNotEditable;
            }
            set
            {
                Set(() => IsNotEditable, ref isNotEditable, value);
            }
        }

        private bool canHaveChild;

        public bool CanHaveChild
        {
            get
            {
                return canHaveChild;
            }
            set
            {
                Set(() => CanHaveChild, ref canHaveChild, value);
            }
        }

        private bool isCloned;

        public bool IsCloned
        {
            get
            {
                return isCloned;
            }
            set
            {
                Set(() => IsCloned, ref isCloned, value);
            }
        }

        private bool isExpanded;

        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                Set(() => IsExpanded, ref isExpanded, value);
            }
        }

        private string code;

        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                Set(() => Code, ref code, value);
            }
        }

        private ObservableCollection<AssignationModel> childrens;

        public ObservableCollection<AssignationModel> Childrens
        {
            get
            {
                return childrens;
            }
            set
            {
                if (Equals(value, childrens))
                {
                    return;
                }
                childrens = value;
                RaisePropertyChanged(() => Childrens);
            }
        }

        public RelayCommand<object> DeleteNodeCommand { get; set; }

        public AssignationModel()
        {
            childrens = new ObservableCollection<AssignationModel>();
        }

        public AssignationModel(AssignationModel source)
        {
            LocalizationKey = source.LocalizationKey;
            Color = source.Color;
            BorderColor = source.BorderColor;
            BackgroundColor = source.BackgroundColor;
            IsNotEditable = source.IsNotEditable;
            CanHaveChild = source.CanHaveChild;
            Code = source.Code;
            childrens = new ObservableCollection<AssignationModel>();
        }

        private void ExpanderClick()
        {
            Messenger.Default.Send(new NotificationMessage(this, 
                new ViewModelLocator().Scripter, "ExpanderClick"));
        }

        public object Clone()
        {
            AssignationModel model = new AssignationModel(this) { IsCloned = true };
            foreach (AssignationModel child in Childrens)
            {
                model.Childrens.Add((AssignationModel)child.Clone());
            }
            return model;
        }
    }
}
