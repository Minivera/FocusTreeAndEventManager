using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static FocusTreeManager.Model.Localisation;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LocalisationViewModel : ViewModelBase
    {
        private string currentLocaleName;

        public string CurrentLocaleName
        {
            get
            {
                return currentLocaleName;
            }
            set
            {
                currentLocaleName = value;
                RaisePropertyChanged("CurrentLocaleName");
            }
        }

        public Localisation ContentLocale
        {
            get
            {
                return (new ViewModelLocator()).Main.Project.localisationList
                    .SingleOrDefault((l) => l.Filename == CurrentLocaleName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the LocalisationViewModel class.
        /// </summary>
        public LocalisationViewModel()
        {
        }
    }
}