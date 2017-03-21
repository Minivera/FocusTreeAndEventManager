using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FocusTreeManager.DataContract;
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
    public class FocusGridModel : ObservableObject, ISupportsUndo
    {
        private const int MIN_ROW_COUNT = 7;
        private const int MIN_COLUMN_COUNT = 20;

        private ObservableCollection<CanvasLine> canvasLines;

        private FocusModel selectedFocus;

        public CanvasLine selectedLine { get; set; }

        private int rowCount;

        private int columnCount;

        private bool DrawPositionLines;

        private string visibleName;

        private string tag;

        private string additionnalMods = "";

        private DataContract.FileInfo fileInfo;

        public RelayCommand<object> AddFocusCommand { get; private set; }

        public RelayCommand<object> RightClickCommand { get; private set; }

        public RelayCommand<object> HoverCommand { get; private set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public RelayCommand EditElementCommand { get; private set; }

        public RelayCommand ShowHidePositionLinesCommand { get; private set; }

        public bool isShown { get; set; }

        public int RowCount
        {
            get
            {
                return rowCount;
            }
            set
            {
                if (value == rowCount)
                {
                    return;
                }
                rowCount = value;
                RaisePropertyChanged(() => RowCount);
            }
        }

        public int ColumnCount
        {
            get
            {
                return columnCount;
            }
            set
            {
                if (value == columnCount)
                {
                    return;
                }
                columnCount = value;
                RaisePropertyChanged(() => ColumnCount);
            }
        }

        public Guid UniqueID { get; }

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

        public string TAG
        {
            get
            {
                return tag;
            }
            set
            {
                if (value == tag)
                {
                    return;
                }
                tag = value;
                RaisePropertyChanged(() => TAG);
            }
        }

        public string AdditionnalMods
        {
            get
            {
                return additionnalMods;
            }
            set
            {
                if (value == additionnalMods)
                {
                    return;
                }
                additionnalMods = value;
                RaisePropertyChanged(() => AdditionnalMods);
            }
        }

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

        public ObservableCollection<FocusModel> FociList { get; private set; }

        public ObservableCollection<CanvasLine> CanvasLines
        {
            get
            {
                return canvasLines;
            }
            set
            {
                canvasLines = value;
                RaisePropertyChanged(() => CanvasLines);
            }
        }

        public FocusGridModel(string Filename)
        {
            visibleName = Filename;
            UniqueID = Guid.NewGuid();
            FociList = new ObservableCollection<FocusModel>();
            FociList.CollectionChanged += FociList_CollectionChanged;
            //Min Row & column Count
            rowCount = MIN_ROW_COUNT;
            columnCount = MIN_COLUMN_COUNT;
            canvasLines = new ObservableCollection<CanvasLine>();
            SetupCommands();
        }

        public FocusGridModel(FociGridContainer container)
        {
            //Transfer data
            UniqueID = container.IdentifierID;
            visibleName = container.ContainerID;
            tag = container.TAG;
            additionnalMods = container.AdditionnalMods;
            FileInfo = container.FileInfo;
            FociList = new ObservableCollection<FocusModel>();
            foreach (Focus focus in container.FociList)
            {
                FociList.Add(new FocusModel(focus));
            }
            //Rerun to create sets
            foreach (FocusModel model in FociList)
            {
                model.RepairSets(
                    container.FociList.FirstOrDefault(f => f.UniqueName == model.UniqueName),
                    FociList.ToList());
            }
            //Create the remaining stuff
            FociList.CollectionChanged += FociList_CollectionChanged;
            //Min Row & column Count
            EditGridDefinition();
            canvasLines = new ObservableCollection<CanvasLine>();
            SetupCommands();
        }

        internal void SetupCommands()
        {
            //Commands
            AddFocusCommand = new RelayCommand<object>(AddFocus);
            RightClickCommand = new RelayCommand<object>(RightClick);
            HoverCommand = new RelayCommand<object>(Hover);
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            EditElementCommand = new RelayCommand(SendEditSignal);
            ShowHidePositionLinesCommand = new RelayCommand(ShowHidePositionLines);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        internal void ChangePosition(object draggedElement, Point currentPoint)
        {
            FocusModel element = draggedElement as FocusModel;
            if (element != null)
            {
                int X = (int)Math.Floor(currentPoint.X / 89);
                int Y = (int)Math.Floor(currentPoint.Y / 140);
                element.DisplayX = X;
                element.DisplayY = Y;
                //Make all relative to focuses changed their positions
                updateRelatives(element);
                EditGridDefinition();
                DrawOnCanvas();
            }
        }

        internal void updateRelatives(FocusModel RelativeTo)
        {
            if (FociList.All(f => f.CoordinatesRelativeTo != RelativeTo))
            {
                return;
            }
            foreach (FocusModel relative in
                    FociList.Where(f => f.CoordinatesRelativeTo == RelativeTo))
            {
                relative.RaisePropertyChanged(() => relative.DisplayX);
                relative.RaisePropertyChanged(() => relative.DisplayY);
                updateRelatives(relative);
            }
        }

        public void EditGridDefinition()
        {
            if (!FociList.Any())
            {
                return;
            }
            FocusModel biggestY = FociList.Aggregate((i1, i2) => i1.DisplayY > 
                    i2.DisplayY ? i1 : i2);
            FocusModel biggestX = FociList.Aggregate((i1, i2) => i1.DisplayX > 
                    i2.DisplayX ? i1 : i2);
            RowCount = biggestY.DisplayY >= RowCount ? biggestY.DisplayY + 1 : RowCount;
            ColumnCount = biggestX.DisplayX >= ColumnCount ? biggestX.DisplayX + 1 : ColumnCount;
        }

        public void AddGridCells(int RowsToAdd, int ColumnsToAddt)
        {
            RowCount += RowsToAdd;
            ColumnCount += ColumnsToAddt;
        }

        public void AddFocus(object sender)
        {
            Application.Current.Properties["Mode"] = "Add";
            UndoService.Current[GetUndoRoot()].BeginChangeSetBatch("AddFocus", false);
            Messenger.Default.Send(new NotificationMessage(sender, "ShowAddFocus"));
        }

        public void RightClick(object sender)
        {
            Point Position = Mouse.GetPosition((Grid)sender);
            List<CanvasLine> clickedElements = CanvasLines.Where(line => 
                                               line.ContainsPoint(Position)).ToList();
            if (clickedElements.Any())
            {
                UndoService.Current[GetUndoRoot()].BeginChangeSetBatch("DeleteSetAny", false);
                foreach (CanvasLine line in clickedElements)
                {
                    if (line.InternalSet != null)
                    {
                        line.InternalSet.DeleteSetRelations();
                    }
                    else
                    {
                        //Hit a relative to
                        line.Relative.X = line.Relative.DisplayX;
                        line.Relative.Y = line.Relative.DisplayY;
                        line.Relative.CoordinatesRelativeTo = null;
                    }
                }
                UndoService.Current[GetUndoRoot()].EndChangeSetBatch();
                CanvasLines = new ObservableCollection<CanvasLine>(CanvasLines.Except(clickedElements).ToList());
                DrawOnCanvas();
            }
        }

        public void Hover(object sender)
        {
            Point Position = Mouse.GetPosition((Grid)sender);
            List<CanvasLine> clickedElements = CanvasLines.Where(line =>
                                               line.ContainsPoint(Position)).ToList();
            if (clickedElements.Any())
            {
                selectedLine = clickedElements.FirstOrDefault();
                Messenger.Default.Send(new NotificationMessage("DrawOnCanvas"));
            }
            else
            {
                //If a line is selected
                if (selectedLine == null) return;
                selectedLine = null;
                Messenger.Default.Send(new NotificationMessage("DrawOnCanvas"));
            }
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

        private void ShowHidePositionLines()
        {
            DrawPositionLines = !DrawPositionLines;
            DrawOnCanvas();
        }

        public void RedrawGrid()
        {
            EditGridDefinition();
            DrawOnCanvas();
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (VisibleName == null)
            {
                return;
            }
            //Always manage container renamed
            if (msg.Notification == "ContainerRenamed")
            {
                RaisePropertyChanged(() => VisibleName);
            }
            if (!isShown)
            {
                //is not shown, do not manage
                return;
            }
            FocusModel Model = msg.Sender as FocusModel;
            switch (msg.Notification)
            {
                case "CloseEditFocus":
                    switch ((string) Application.Current.Properties["Mode"])
                    {
                        case "Add":
                            ManageFocusViewModel viewModel = msg.Sender as ManageFocusViewModel;
                            if (viewModel != null) addFocusToList(viewModel.Focus);
                            DrawOnCanvas();
                            break;
                        case "Edit":
                            EditGridDefinition();
                            DrawOnCanvas();
                            break;
                    }
                    Application.Current.Properties["Mode"] = null;
                    break;
                case "DeleteFocus":
                    DeleteFocus(Model);
                    break;
                case "AddFocusMutually":
                    Application.Current.Properties["Mode"] = "Mutually";
                    if (Model != null)
                    {
                        selectedFocus = Model;
                        Model.IsSelected = true;
                    }
                    break;
                case "FinishAddFocusMutually":
                    if (selectedFocus != null && selectedFocus != Model &&
                        FociList.Any(f => f == Model))
                    {
                        UndoService.Current[GetUndoRoot()]
                            .BeginChangeSetBatch("AddMutuallyExclusive", false);
                        Application.Current.Properties["Mode"] = null;
                        selectedFocus.IsSelected = false;
                        MutuallyExclusiveSetModel tempo = 
                            new MutuallyExclusiveSetModel(selectedFocus, Model);
                        selectedFocus.MutualyExclusive.Add(tempo);
                        Model?.MutualyExclusive.Add(tempo);
                        UndoService.Current[GetUndoRoot()].EndChangeSetBatch();
                        RaisePropertyChanged(() => FociList);
                        DrawOnCanvas();
                    }
                    break;
                case "AddFocusPrerequisite":
                    Application.Current.Properties["Mode"] = "Prerequisite";
                    if (Model != null)
                    {
                        selectedFocus = Model;
                        Model.IsSelected = true;
                    }
                    break;
                case "FinishAddFocusPrerequisite":
                    if (selectedFocus != null && selectedFocus != Model &&
                        FociList.Any(f => f == Model))
                    {
                        UndoService.Current[GetUndoRoot()]
                            .BeginChangeSetBatch("AddPrerequisite", false);
                        Application.Current.Properties["Mode"] = null;
                        string Type = (string) Application.Current.Properties["ModeParam"];
                        Application.Current.Properties["ModeParam"] = null;
                        selectedFocus.IsSelected = false;
                        if (Type == "Required")
                        {
                            //Create new set
                            PrerequisitesSetModel set = new PrerequisitesSetModel(selectedFocus);
                            set.FociList.Add(Model);
                            selectedFocus.Prerequisite.Add(set);
                        }
                        else
                        {
                            //Create new set if no exist
                            if (!selectedFocus.Prerequisite.Any())
                            {
                                PrerequisitesSetModel set = new PrerequisitesSetModel(selectedFocus);
                                selectedFocus.Prerequisite.Add(set);
                            }
                            //Add Model to last Set
                            selectedFocus.Prerequisite.Last().FociList.Add(Model);
                        }
                        UndoService.Current[GetUndoRoot()].EndChangeSetBatch();
                        RaisePropertyChanged(() => FociList);
                        DrawOnCanvas();
                    }
                    break;
                case "MakeRelativeTo":
                    Application.Current.Properties["Mode"] = "RelativeTo";
                    if (Model != null)
                    {
                        selectedFocus = Model;
                        Model.IsSelected = true;
                    }
                    break;
                case "FinishMakeRelativeTo":
                    if (selectedFocus != null && selectedFocus != Model &&
                        FociList.Any(f => f == Model))
                    {
                        if (Model != null)
                        {
                            UndoService.Current[GetUndoRoot()]
                                .BeginChangeSetBatch("MakeRelativeTo", false);
                            Application.Current.Properties["Mode"] = null;
                            selectedFocus.IsSelected = false;
                            selectedFocus.X = selectedFocus.DisplayX - Model.DisplayX;
                            selectedFocus.Y = selectedFocus.DisplayY - Model.DisplayY;
                            selectedFocus.CoordinatesRelativeTo = Model;
                            UndoService.Current[GetUndoRoot()].EndChangeSetBatch();
                            RaisePropertyChanged(() => FociList);
                            DrawOnCanvas();
                        }
                    }
                    break;
            }
        }

        public void UpdateFocus(FocusModel sender)
        {
            if (!isShown) return;
            EditGridDefinition();
            DrawOnCanvas();
        }

        private void DeleteFocus(FocusModel Model)
        {
            //Kill the set that might have this focus as parent
            foreach (FocusModel focus in FociList)
            {
                foreach (PrerequisitesSetModel set in focus.Prerequisite.ToList())
                {
                    if (set.FociList.Contains(Model))
                    {
                        set.DeleteSetRelations();
                    }
                }
                foreach (MutuallyExclusiveSetModel set in focus.MutualyExclusive.ToList())
                {
                    if (set.Focus2 == Model || set.Focus1 == Model)
                    {
                        set.DeleteSetRelations();
                    }
                }
            }
            //Remove the focus
            FociList.Remove(Model);
            UndoService.Current[GetUndoRoot()].EndChangeSetBatch();
            EditGridDefinition();
            DrawOnCanvas();
        }

        public void addFocusToList(FocusModel FocusToAdd)
        {
            FociList.Add(FocusToAdd);
            UndoService.Current[GetUndoRoot()].EndChangeSetBatch();
            RowCount = FocusToAdd.Y >= RowCount ? FocusToAdd.Y + 1 : RowCount;
            ColumnCount = FocusToAdd.X >= ColumnCount ? FocusToAdd.X + 1 : ColumnCount;
            DrawOnCanvas();
        }

        private const double PRE_LINE_HEIGHT = 20;

        public void DrawOnCanvas()
        {
            if (FociList == null)
            {
                return;
            }
            CanvasLines.Clear();
            foreach (FocusModel focus in FociList)
            {
                //Draw relatives
                if (DrawPositionLines && focus.CoordinatesRelativeTo != null)
                {
                    CanvasLine newline = new CanvasLine(
                            focus.FocusTop.X,
                            focus.FocusTop.Y,
                            focus.CoordinatesRelativeTo.FocusBottom.X,
                            focus.CoordinatesRelativeTo.FocusBottom.Y,
                            System.Windows.Media.Brushes.Gold, false, null, focus);
                    CanvasLines.Add(newline);
                }
                //Draw Prerequisites
                foreach (PrerequisitesSetModel set in focus.Prerequisite)
                {
                    //Draw line from top of first Focus 
                    CanvasLine newline = new CanvasLine(
                        set.Focus.FocusTop.X,
                        set.Focus.FocusTop.Y,
                        set.Focus.FocusTop.X,
                        set.Focus.FocusTop.Y - PRE_LINE_HEIGHT,
                        System.Windows.Media.Brushes.Teal, set.isRequired(), set);
                    CanvasLines.Add(newline);
                    foreach (FocusModel model in set.FociList)
                    {
                        FocusModel Prerequisite = model;
                        if (Prerequisite == null) continue;
                        //Draw horizontal lines to prerequisite pos
                        newline = new CanvasLine(set.Focus.FocusTop.X, 
                            set.Focus.FocusTop.Y - PRE_LINE_HEIGHT, Prerequisite.FocusBottom.X, 
                            set.Focus.FocusTop.Y - PRE_LINE_HEIGHT, System.Windows.Media.Brushes.Teal,
                            set.isRequired(), set);
                        CanvasLines.Add(newline);
                        //Draw line to prerequisite bottom
                        newline = new CanvasLine(Prerequisite.FocusBottom.X, 
                            set.Focus.FocusTop.Y - PRE_LINE_HEIGHT, Prerequisite.FocusBottom.X, 
                            Prerequisite.FocusBottom.Y, System.Windows.Media.Brushes.Teal, 
                            set.isRequired(), set);
                        CanvasLines.Add(newline);
                    }
                }
                //Draw Mutually exclusives
                foreach (MutuallyExclusiveSetModel set in focus.MutualyExclusive)
                {
                    CanvasLine newline = new CanvasLine(
                        set.Focus1.FocusRight.X,
                        set.Focus1.FocusRight.Y,
                        set.Focus2.FocusLeft.X,
                        set.Focus2.FocusLeft.Y,
                        System.Windows.Media.Brushes.Red, false, set);
                    if (!CanvasLines.Any(line => line.X1 == newline.X1 &&
                                                 line.X2 == newline.X2 &&
                                                 line.Y1 == newline.Y1 &&
                                                 line.Y2 == newline.Y2))
                    {
                        CanvasLines.Add(newline);
                    }
                }
            }
            RaisePropertyChanged(() => CanvasLines);
            Messenger.Default.Send(new NotificationMessage("DrawOnCanvas"));
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
            MessageDialogResult Result = await new ViewModelLocator()
                .Main.ShowFileChangedDialog();
            if (Result == MessageDialogResult.Affirmative)
            {
                string oldText = FocusTreeParser.ParseTreeForCompare(this);
                string newText = FocusTreeParser.ParseTreeScriptForCompare(info.Filename);
                SideBySideDiffModel model = new SideBySideDiffBuilder(
                    new Differ()).BuildDiffModel(oldText, newText);
                new ViewModelLocator().CodeComparator.DiffModel = model;
                new CompareCode().ShowDialog();
            }
        }

        #region Undo/Redo

        private void FociList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, 
                "FociList", FociList, e, "FociList Changed");
        }
        
        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion
    }
}
