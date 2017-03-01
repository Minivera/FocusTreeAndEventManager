using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FocusTreeManager.CodeStructures;
using FocusTreeManager.DataContract;
using FocusTreeManager.Parsers;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using MonitoredUndo;
using System;
using System.IO;
using System.Windows;

namespace FocusTreeManager.Model
{
    public class ScriptModel : ObservableObject, ISupportsUndo
    {
        private Guid ID;

        public Guid UniqueID
        {
            get
            {
                return ID;
            }
        }

        private string visibleName;

        public string VisibleName
        {
            get
            {
                return visibleName;
            }
            set
            {
                if (value == visibleName)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "VisibleName", visibleName, value, "VisibleName Changed");
                visibleName = value;
                RaisePropertyChanged(() => VisibleName);
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
                if (internalScript == value)
                {
                    return;
                }
                internalScript = value;
                RaisePropertyChanged(() => InternalScript);
            }
        }

        private DataContract.FileInfo fileInfo;

        public DataContract.FileInfo FileInfo
        {
            get
            {
                return fileInfo;
            }
            set
            {
                if (value == fileInfo)
                {
                    return;
                }
                fileInfo = value;
                RaisePropertyChanged(() => FileInfo);
            }
        }

        public RelayCommand<FrameworkElement> SaveScriptCommand { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public RelayCommand EditElementCommand { get; private set; }

        public ScriptModel(string Filename)
        {
            visibleName = Filename;
            this.ID = Guid.NewGuid();
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            //Commands
            SaveScriptCommand = new RelayCommand<FrameworkElement>(SaveScript);
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            EditElementCommand = new RelayCommand(SendEditSignal);
        }

        private void SaveScript(FrameworkElement obj)
        {
            (new ViewModelLocator()).Scripter.SaveScript();
            InternalScript = (new ViewModelLocator()).Scripter.ManagedScript;
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (this.VisibleName == null)
            {
                return;
            }
            //Always manage container renamed
            if (msg.Notification == "ContainerRenamed")
            {
                RaisePropertyChanged(() => VisibleName);
            }
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                (new ViewModelLocator()).ProjectView, "SendDeleteItemSignal"));
        }

        private void SendEditSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                (new ViewModelLocator()).ProjectView, "SendEditItemSignal"));
        }

        async public void CheckForChanges()
        {
            DataContract.FileInfo info = this.FileInfo;
            //check the fileinfo data
            if (info != null)
            {
                //If the file exists
                if (File.Exists(info.Filename))
                {
                    //If the file was modified after the last modification date
                    if (File.GetLastWriteTime(info.Filename) > info.LastModifiedDate)
                    {
                        //Then we can show a message
                        MessageDialogResult Result = await (new ViewModelLocator())
                            .Main.ShowFileChangedDialog();
                        if (Result == MessageDialogResult.Affirmative)
                        {
                            string oldText = ScriptParser.ParseScriptForCompare(this);
                            string newText = ScriptParser.ParseScriptFileForCompare(info.Filename);
                            SideBySideDiffModel model = new SideBySideDiffBuilder(
                                new Differ()).BuildDiffModel(oldText, newText);
                            (new ViewModelLocator()).CodeComparator.DiffModel = model;
                            new CompareCode().ShowDialog();
                        }
                    }
                }
            }
        }

        #region Undo/Redo

        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion

    }
}
