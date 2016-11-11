using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;

namespace FocusTreeManager.Model
{
    public class AssignationModel : ObservableObject, ICloneable
    {
        public string Text
        {
            get
            {
                ResourceDictionary resourceLocalization = new ResourceDictionary();
                resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                return resourceLocalization[LocalizationKey] as string;
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
                Set<string>(() => this.LocalizationKey, ref this.localizationKey, value);
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
                Set<Brush>(() => this.Color, ref this.color, value);
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
                Set<Brush>(() => this.BorderColor, ref this.borderColor, value);
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
                Set<Brush>(() => this.BackgroundColor, ref this.backgroundColor, value);
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
                Set<bool>(() => this.IsNotEditable, ref this.isNotEditable, value);
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
                Set<bool>(() => this.CanHaveChild, ref this.canHaveChild, value);
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
                Set<bool>(() => this.IsCloned, ref this.isCloned, value);
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
                Set<bool>(() => this.IsExpanded, ref this.isExpanded, value);
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
                Set<string>(() => this.Code, ref this.code, value);
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
                    return;
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
            Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).Scripter, "ExpanderClick"));
        }

        public object Clone()
        {
            AssignationModel model = new AssignationModel(this) { IsCloned = true };
            foreach (var child in Childrens)
            {
                model.Childrens.Add((AssignationModel)child.Clone());
            }
            return model;
        }
    }
}
