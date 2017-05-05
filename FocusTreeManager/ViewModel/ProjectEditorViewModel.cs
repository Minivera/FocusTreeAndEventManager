using System;
using System.Windows;
using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAPICodePack.Dialogs;
using FocusTreeManager.Helper;

namespace FocusTreeManager.ViewModel
{
    public class ProjectEditorViewModel : ViewModelBase
    {

        private ProjectModel project;

        public ProjectModel Project
        {
            get
            {
                return project;
            }
            set
            {
                if (value == project)
                {
                    return;
                }
                project = value;
                RaisePropertyChanged(() => Project);
            }
        }

        private string selectedString;

        public string SelectedString
        {
            get
            {
                return selectedString;
            }
            set
            {
                selectedString = value;
                DeleteModFolderCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AcceptCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public RelayCommand AddModFolderCommand { get; set; }

        public RelayCommand DeleteModFolderCommand { get; set; }

        public RelayCommand WindowClosingCommand { get; set; }

        public bool Accepted { get; set; }      

        public ProjectEditorViewModel()
        {
            Accepted = false;
            AcceptCommand = new RelayCommand(Accept);
            CancelCommand = new RelayCommand(Cancel);
            AddModFolderCommand = new RelayCommand(AddModFolder);
            DeleteModFolderCommand = new RelayCommand(DeleteModFolder, CanDeleteModFolder);
            WindowClosingCommand = new RelayCommand(WindowClosing);
        }

        public void Accept()
        {
            Accepted = true;
            Close();
        }

        public void Cancel()
        {
            Accepted = false;
            Project = null;
            Close();
        }

        public void WindowClosing()
        {
            if (Accepted) return;
            Project = null;
        }

        public void AddModFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Title = LocalizationHelper.getValueForKey("Game_Path_Title"),
                IsFolderPicker = true,
                InitialDirectory = Environment.GetFolderPath(Environment.
                                   SpecialFolder.MyDocuments),
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                DefaultDirectory = "C:",
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Project.ListModFolders.Add(dialog.FileName);
                AsyncImageLoader.AsyncImageLoader.Worker.RefreshFromMods();
            }
            Activate();
        }

        public void DeleteModFolder()
        {
            if (string.IsNullOrEmpty(selectedString))
            {
                return;
            }
            Project.ListModFolders.Remove(selectedString);
            AsyncImageLoader.AsyncImageLoader.Worker.RefreshFromMods();
        }

        public bool CanDeleteModFolder()
        {
            return !string.IsNullOrEmpty(selectedString);
        }

        private void Close()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                }
            }
        }

        private void Activate()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Activate();
                }
            }
        }
    }
}