using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private List<LanguageSelector> availableLanguages;

        private LanguageSelector selectedLanguage;

        public List<LanguageSelector> AvailableLanguages
        {
            get
            {
                return availableLanguages;
            }
        }

        public LanguageSelector SelectedLanguage
        {
            get
            {
                return selectedLanguage;
            }
            set
            {
                selectedLanguage = value;
                Configurator.setLanguage(value.FileName);
                Messenger.Default.Send(new NotificationMessage(this, "ChangeLanguage"));
                RaisePropertyChanged("SelectedLanguage");
            }
        }

        public string GamePath
        {
            get
            {
                return Configurator.getGamePath();
            }
            set
            {
                Configurator.setGamePath(value);
                RaisePropertyChanged("GamePath");
            }
        }
        
        public RelayCommand FindCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the SettingsViewModel class.
        /// </summary>
        public SettingsViewModel()
        {
            availableLanguages = Configurator.returnAllLanguages();
            selectedLanguage = availableLanguages.SingleOrDefault((l) => l.FileName == Configurator.getLanguage());
            FindCommand = new RelayCommand(SelectGameFolder);
        }

        public void SelectGameFolder()
        {
            var dialog = new CommonOpenFileDialog();
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            dialog.Title = resourceLocalization["Game_Path_Title"] as string;
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = "C:";
            dialog.AddToMostRecentlyUsedList = false;
            dialog.AllowNonFileSystemItems = false;
            dialog.DefaultDirectory = "C:";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                GamePath = dialog.FileName;
            }
        }
    }
}