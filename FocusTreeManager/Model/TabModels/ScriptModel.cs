using System;
using System.IO;
using System.Windows;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FocusTreeManager.CodeStructures;
using FocusTreeManager.Parsers;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using MonitoredUndo;

namespace FocusTreeManager.Model.TabModels
{
    public class ScriptModel : ObservableObject, ISupportsUndo
    {
        public Guid UniqueID { get; }

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

        public RelayCommand CopyElementCommand { get; private set; }

        public ScriptModel(string Filename)
        {
            visibleName = Filename;
            UniqueID = Guid.NewGuid();
            SetupCommons();
        }

        public ScriptModel(ScriptModel model)
        {
            visibleName = model.VisibleName;
            UniqueID = Guid.NewGuid();
            Script newScript = new Script();
            newScript.Analyse(model.InternalScript.Parse());
            InternalScript = newScript;
            SetupCommons();
        }

        private void SetupCommons()
        {
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            //Commands
            SaveScriptCommand = new RelayCommand<FrameworkElement>(SaveScript);
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            EditElementCommand = new RelayCommand(SendEditSignal);
            CopyElementCommand = new RelayCommand(SendCopySignal);
        }

        private void SaveScript(FrameworkElement obj)
        {
            new ViewModelLocator().Scripter.SaveScript();
            InternalScript = new ViewModelLocator().Scripter.ManagedScript;
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            //If this is not the intended target
            if (msg.Target != null && msg.Target != this) return;
            //If this is a dead tab waiting to be destroyed
            if (VisibleName == null) return;
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                new ViewModelLocator().ProjectView, "SendDeleteItemSignal"));
        }

        private void SendEditSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                new ViewModelLocator().ProjectView, "SendEditItemSignal"));
        }

        private void SendCopySignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                new ViewModelLocator().ProjectView, "SendCopyItemSignal"));
        }

        public async void CheckForChanges()
        {
            DataContract.FileInfo info = FileInfo;
            //check the fileinfo data
            if (info == null) return;
            //If the file exists
            if (!File.Exists(info.Filename)) return;
            //If the file was modified after the last modification date
            if (File.GetLastWriteTime(info.Filename) <= info.LastModifiedDate) return;
            //Then we can show a message
            MessageDialogResult Result = await (new ViewModelLocator())
                .Main.ShowFileChangedDialog();
            if (Result == MessageDialogResult.Affirmative)
            {
                string oldText = ScriptParser.ParseScriptForCompare(this);
                string newText = ScriptParser.ParseScriptFileForCompare(info.Filename);
                SideBySideDiffModel model = new SideBySideDiffBuilder(
                    new Differ()).BuildDiffModel(oldText, newText);
                new ViewModelLocator().CodeComparator.DiffModel = model;
                new CompareCode().ShowDialog();
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
