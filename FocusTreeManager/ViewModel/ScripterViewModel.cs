using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FocusTreeManager.CodeStructures;
using FocusTreeManager.CodeStructures.CodeExceptions;
using FocusTreeManager.Helper;
using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

namespace FocusTreeManager.ViewModel
{
    public enum ScripterType
    {
        FocusTree,
        Event,
        EventOption,
        EventDescription,
        Generic
    }

    public enum ControlType
    {
        Assignation,
        Block,
        Condition
    }

    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ScripterViewModel : ViewModelBase, IDropTarget
    {
        public ObservableCollection<AssignationModel> CodeBlocks { get; } = 
            new ObservableCollection<AssignationModel>();

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
                RaisePropertyChanged(() => EditorScript);
            }
        }

        private ScripterType scriptType = ScripterType.Generic;

        public ScripterType ScriptType
        {
            get
            {
                return scriptType;
            }

            set
            {
                scriptType = value;
                RaisePropertyChanged(() => ScriptType);
            }
        }

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
        
        private void setCode(ICodeStruct internalScript)
        {
            CodeBlocks.Clear();
            EditorScript = internalScript == null? "" : internalScript.Parse(null, 0);
            List<AssignationModel> listBlock = ModelsToScriptHelper.
                TransformScriptToModels(managedScript, new RelayCommand<object>(DeleteNode));
            foreach (AssignationModel item in listBlock)
            {
                CodeBlocks.Add(item);
            }
        }

        public void SaveScript()
        {
            //Make sure all syntax exceptions are ignored, we'll show them later
            switch (Configurator.getScripterPreference())
            {
                case "Scripter":
                    managedScript = ModelsToScriptHelper.TransformModelsToScript(CodeBlocks.ToList());
                    break;
                case "Editor":
                    ManagedScript.Analyse(editorScript);
                    break;
            }
            CodeBlocks.Clear();
            Close();
        }

        public void CancelScript()
        {
            CodeBlocks.Clear();
            Close();
        }

        public void DragOver(IDropInfo dropInfo)
        {
            DragDrop.DefaultDropHandler.DragOver(dropInfo);
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo?.DragInfo == null)
            {
                return;
            }
            int insertIndex = dropInfo.InsertIndex != dropInfo.UnfilteredInsertIndex 
                ? dropInfo.UnfilteredInsertIndex : dropInfo.InsertIndex;
            IList destinationList = dropInfo.TargetCollection.TryGetList();
            AssignationModel data = dropInfo.Data as AssignationModel;
            AssignationModel currentItem = ((FrameworkElement) dropInfo.VisualTargetItem)?.
                DataContext as AssignationModel;
            // If the source and destination are in the same control, delete at the current index
            if (Equals(dropInfo.DragInfo.VisualSource, dropInfo.VisualTarget))
            {
                IList sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();
                if (currentItem == null || currentItem.CanHaveChild)
                {
                    int index = sourceList.IndexOf(data);
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
            // Add data at the new index
            if (currentItem != null && !currentItem.CanHaveChild) return;
            AssignationModel obj2Insert = data;
            if (!Equals(dropInfo.DragInfo.VisualSource, dropInfo.VisualTarget))
            {
                ICloneable cloneable = data;
                if (cloneable != null)
                {
                    obj2Insert = (AssignationModel)cloneable.Clone();
                    obj2Insert.DeleteNodeCommand = new RelayCommand<object>(DeleteNode);
                }
            }
            destinationList.Insert(insertIndex++, obj2Insert);
        }

        public void DeleteNode(object sender)
        {
            if (!(sender is AssignationModel))
            {
                return;
            }
            AssignationModel model = (AssignationModel) sender;
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
            if (!model.Childrens.Any()) return;
            //Loop in all the children
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
    }
}