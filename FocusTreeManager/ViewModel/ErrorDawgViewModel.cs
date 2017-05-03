using System;
using System.Collections.Generic;
using System.Linq;
using FocusTreeManager.CodeStructures;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ErrorDawgViewModel : ViewModelBase
    {
        public Visibility DawgVisible => ErrorsList.Any() ? 
            Visibility.Visible : Visibility.Hidden;

        public string NumOfErrors => ErrorsList.Count + " Errors";

        public List<string> ErrorsList { get; set; }

        public string Errors => string.Join("\n", ErrorsList);

        public RelayCommand ResetCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ErrorDawgViewModel class.
        /// </summary>
        public ErrorDawgViewModel()
        {
            ErrorsList = new List<string>();
            ResetCommand = new RelayCommand(ResetLog);
        }

        public void AddError(string error)
        {
            ErrorsList.Add(error);
            RaisePropertyChanged(() => DawgVisible);
            RaisePropertyChanged(() => NumOfErrors);
            RaisePropertyChanged(() => Errors);
        }

        private void ResetLog()
        {
            ErrorsList.Clear();
            RaisePropertyChanged(() => DawgVisible);
            RaisePropertyChanged(() => NumOfErrors);
            RaisePropertyChanged(() => Errors);
        }
    }
}