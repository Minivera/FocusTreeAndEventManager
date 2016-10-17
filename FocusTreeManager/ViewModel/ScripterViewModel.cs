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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        private ObservableCollection<AssignationModel> codeBlocks;

        public ObservableCollection<AssignationModel> CodeBlocks
        {
            get
            {
                return codeBlocks;
            }
        }

        public string ScriptText
        {
            get
            {
                return ModelsToScriptHelper.TransformModelsToScript(CodeBlocks.ToList()).Parse();
            }
        }

        private Focus AssociatedFocus;

        public RelayCommand SaveScriptCommand { get; private set; }

        public RelayCommand CancelCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ScripterViewModel class.
        /// </summary>
        public ScripterViewModel()
        {
            codeBlocks = new ObservableCollection<AssignationModel>();
            SaveScriptCommand = new RelayCommand(SaveScript);
            CancelCommand = new RelayCommand(CancelScript);
        }
        
        public void setCode(Script internalScript, Focus focus)
        {
            codeBlocks = new ObservableCollection<AssignationModel>(ModelsToScriptHelper.
                TransformScriptToModels(internalScript, new RelayCommand<object>(DeleteNode)));
            AssociatedFocus = focus;
        }

        public void SaveScript()
        {
            AssociatedFocus.InternalScript = ScriptText;
            AssociatedFocus = null;
            codeBlocks = new ObservableCollection<AssignationModel>();
            Messenger.Default.Send(new NotificationMessage("HideScripter"));
        }

        public void CancelScript()
        {
            AssociatedFocus = null;
            codeBlocks = new ObservableCollection<AssignationModel>();
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