using FocusTreeManager.CodeStructures;
using FocusTreeManager.Helper;
using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static FocusTreeManager.ViewModel.ScripterControlsViewModel;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ScripterViewModel : ViewModelBase, IDropTarget
    {
        private readonly ObservableCollection<AssignationModel> codeBlocks 
            = new ObservableCollection<AssignationModel>();

        public ObservableCollection<AssignationModel> CodeBlocks
        {
            get
            {
                return codeBlocks;
            }
        }

        private Script managedScript = new Script();

        public Script ManagedScript
        {
            get
            {
                return managedScript;
            }
            set
            {
                managedScript = value;
                setCode(managedScript);
            }
        }

        private string editorScript;

        public string EditorScript
        {
            get
            {
                return editorScript;
            }
            set
            {
                editorScript = value;
                ManagedScript.Analyse(value);
                RaisePropertyChanged(() => EditorScript);
            }
        }

        public ScripterType ScriptType { get; set; }

        public string SelectedTabIndex { get; set; }

        public RelayCommand SaveScriptCommand { get; private set; }

        public RelayCommand CancelCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ScripterViewModel class.
        /// </summary>
        public ScripterViewModel()
        {
            SaveScriptCommand = new RelayCommand(SaveScript);
            CancelCommand = new RelayCommand(CancelScript);
        }
        
        private void setCode(Script internalScript)
        {
            codeBlocks.Clear();
            EditorScript = internalScript == null? "" : internalScript.Parse(0);
            List<AssignationModel> listBlock = ModelsToScriptHelper.
                TransformScriptToModels(managedScript, new RelayCommand<object>(DeleteNode));
            foreach (AssignationModel item in listBlock)
            {
                codeBlocks.Add(item);
            }
        }

        public void ScriptToScripter()
        {
            ManagedScript.Analyse(editorScript);
            codeBlocks.Clear();
            List<AssignationModel> listBlock = ModelsToScriptHelper.
                TransformScriptToModels(ManagedScript, new RelayCommand<object>(DeleteNode));
            foreach (AssignationModel item in listBlock)
            {
                codeBlocks.Add(item);
            }
            RaisePropertyChanged(() => CodeBlocks);
        }

        public void ScripterToScript()
        {
            managedScript = ModelsToScriptHelper.TransformModelsToScript(CodeBlocks.ToList());
            EditorScript = managedScript.Parse();
        }

        public void SaveScript()
        {
            if (SelectedTabIndex == "Scripter")
            {
                managedScript = ModelsToScriptHelper.TransformModelsToScript(CodeBlocks.ToList());
            }
            else if (SelectedTabIndex == "Editor")
            {
                ManagedScript.Analyse(editorScript);
            }
            codeBlocks.Clear();
            Messenger.Default.Send(new NotificationMessage("HideScripter"));
        }

        public void CancelScript()
        {
            codeBlocks.Clear();
            Messenger.Default.Send(new NotificationMessage("HideScripter"));
        }

        public void DragOver(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo == null || dropInfo.DragInfo == null)
            {
                return;
            }
            int insertIndex = dropInfo.InsertIndex != dropInfo.UnfilteredInsertIndex 
                ? dropInfo.UnfilteredInsertIndex : dropInfo.InsertIndex;
            var destinationList = dropInfo.TargetCollection.TryGetList();
            AssignationModel data = dropInfo.Data as AssignationModel;
            AssignationModel currentItem = dropInfo.VisualTargetItem != null ?
                                                ((FrameworkElement)dropInfo.VisualTargetItem)
                                                .DataContext as AssignationModel : null;
            // If the sourcve and destination are in the same control, delete at the current index
            if (Equals(dropInfo.DragInfo.VisualSource, dropInfo.VisualTarget))
            {
                var sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();
                if (currentItem == null || currentItem.CanHaveChild)
                {
                    var index = sourceList.IndexOf(data);
                    if (index != -1)
                    {
                        sourceList.RemoveAt(index);
                        if (Equals(sourceList, destinationList) && index < insertIndex)
                        {
                            --insertIndex;
                        }
                    }
                }
            }
            TreeView treeViewItem = dropInfo.VisualTarget as TreeView;
            // Add data at the new index
            if (currentItem == null || currentItem.CanHaveChild)
            {
                var obj2Insert = data;
                if (!Equals(dropInfo.DragInfo.VisualSource, dropInfo.VisualTarget))
                {
                    var cloneable = data as ICloneable;
                    if (cloneable != null)
                    {
                        obj2Insert = (AssignationModel)cloneable.Clone();
                        obj2Insert.DeleteNodeCommand = new RelayCommand<object>(DeleteNode);
                    }
                }
                destinationList.Insert(insertIndex++, obj2Insert);
            }
        }

        public void DeleteNode(object sender)
        {
            if (!(sender is AssignationModel))
            {
                return;
            }
            AssignationModel model = sender as AssignationModel;
            foreach (AssignationModel child in CodeBlocks.ToList())
            {
                //If it is the same as the searched one
                if (child == sender)
                {
                    //Remove
                    CodeBlocks.Remove(child);
                }
                else
                {
                    //loop inside
                    DeleteInChilds(child, model);
                }
            }
        }

        public void DeleteInChilds(AssignationModel model, AssignationModel sender)
        {
            //If there are childs in the model
            if (model.Childrens.Any())
            {
                //Loop in all the childs
                foreach (AssignationModel child in model.Childrens.ToList())
                {
                    //If it is the same as the searched one
                    if (child == sender)
                    {
                        //Remove
                        model.Childrens.Remove(child);
                    }
                    else
                    {
                        //loop inside
                        DeleteInChilds(child, sender);
                    }
                }
            }
        }
    }
}