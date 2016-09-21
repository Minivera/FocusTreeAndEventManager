using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Model
{
    [Serializable]
    public class LocalisationModel : ObservableObject
    {
        private string filename;

        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                Set<string>(() => this.Filename, ref this.filename, value);
            }
        }

        private string shortName;

        public string ShortName
        {
            get
            {
                return shortName;
            }
            set
            {
                Set<string>(() => this.ShortName, ref this.shortName, value);
            }
        }

        public ObservableCollection<FocusTreeManager.Model.LocaleContent> LocalisationMap
        {
            get
            {
                return (new ViewModelLocator()).Main.Project.getSpecificLocalisationMap(Filename);
            }
        }

        public LocalisationModel(string filename)
        {
            this.Filename = filename;
        }
    }
}
