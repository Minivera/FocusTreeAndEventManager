using FocusTreeManager.DataContract;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MonitoredUndo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FocusTreeManager.Model
{
    public class FocusGridModel : ObservableObject, ISupportsUndo
    {
        const int MIN_ROW_COUNT = 7;
        const int MIN_COLUMN_COUNT = 20;

        private ObservableCollection<CanvasLine> canvasLines;

        private FocusModel selectedFocus;

        public CanvasLine selectedLine { get; set; }

        private int rowCount;

        private int columnCount;

        private Guid ID;

        private string visibleName;

        private string tag;

        private string additionnalMods = "";

        private FileInfo fileInfo;

        public RelayCommand<object> AddFocusCommand { get; private set; }

        public RelayCommand<object> RightClickCommand { get; private set; }

        public RelayCommand<object> HoverCommand { get; private set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public RelayCommand EditElementCommand { get; private set; }

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

        public Guid UniqueID
        {
            get
            {
                return ID;
            }
        }

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

        public FileInfo FileInfo
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
                RaisePropertyChanged("CanvasLines");
            }
        }

        public FocusGridModel(string Filename)
        {
            visibleName = Filename;
            ID = Guid.NewGuid();
            FociList = new ObservableCollection<FocusModel>();
            FociList.CollectionChanged += FociList_CollectionChanged;
            //Min Row & column Count
            rowCount = MIN_ROW_COUNT;
            columnCount = MIN_COLUMN_COUNT;
            canvasLines = new ObservableCollection<CanvasLine>();
            //Commands
            AddFocusCommand = new RelayCommand<object>(AddFocus);
            RightClickCommand = new RelayCommand<object>(RightClick);
            HoverCommand = new RelayCommand<object>(Hover);
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            EditElementCommand = new RelayCommand(SendEditSignal);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        public FocusGridModel(FociGridContainer container)
        {
            //Transfer data
            ID = container.IdentifierID;
            visibleName = container.ContainerID;
            tag = container.TAG;
            additionnalMods = container.AdditionnalMods;
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
            rowCount = MIN_ROW_COUNT;
            columnCount = MIN_COLUMN_COUNT;
            canvasLines = new ObservableCollection<CanvasLine>();
            //Commands
            AddFocusCommand = new RelayCommand<object>(AddFocus);
            RightClickCommand = new RelayCommand<object>(RightClick);
            HoverCommand = new RelayCommand<object>(Hover);
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            EditElementCommand = new RelayCommand(SendEditSignal);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        internal void ChangePosition(object draggedElement, Point currentPoint)
        {
            if (!(draggedElement is FocusModel))
            {
                return;
            }
            else
            {
                int X = (int)Math.Floor(currentPoint.X / 89);
                int Y = (int)Math.Floor(currentPoint.Y / 140);
                ((FocusModel)draggedElement).X = X;
                ((FocusModel)draggedElement).Y = Y;
                EditGridDefinition();
                DrawOnCanvas();
            }
        }

        public void EditGridDefinition()
        {
            if (!FociList.Any())
            {
                return;
            }
            FocusModel biggestY = FociList.Aggregate((i1, i2) => i1.Y > i2.Y ? i1 : i2);
            FocusModel biggestX = FociList.Aggregate((i1, i2) => i1.X > i2.X ? i1 : i2);
            RowCount = biggestY.Y >= RowCount ? biggestY.Y + 1 : RowCount;
            ColumnCount = biggestX.X >= ColumnCount ? biggestX.X + 1 : ColumnCount;
        }

        public void AddGridCells(int RowsToAdd, int ColumnsToAddt)
        {
            RowCount += RowsToAdd;
            ColumnCount += ColumnsToAddt;
        }

        public void AddFocus(object sender)
        {
            System.Windows.Application.Current.Properties["Mode"] = "Add";
            UndoService.Current[GetUndoRoot()].BeginChangeSetBatch("AddFocus", false);
            Messenger.Default.Send(new NotificationMessage(sender, "ShowAddFocus"));
        }

        public void RightClick(object sender)
        {
            System.Windows.Point Position = Mouse.GetPosition((Grid)sender);
            List<CanvasLine> clickedElements = CanvasLines.Where((line) =>
                                            inRange((int)line.X1, (int)line.X2, (int)Position.X) &&
                                            inRange((int)line.Y1, (int)line.Y2, (int)Position.Y)).ToList();
            if (clickedElements.Any())
            {
                UndoService.Current[GetUndoRoot()].BeginChangeSetBatch("DeleteSetAny", false);
                foreach (CanvasLine line in clickedElements)
                {
                    line.InternalSet.DeleteSetRelations();
                }
                UndoService.Current[GetUndoRoot()].EndChangeSetBatch();
                CanvasLines = new ObservableCollection<CanvasLine>(CanvasLines.Except(clickedElements).ToList());
                DrawOnCanvas();
            }
        }

        public void Hover(object sender)
        {
            System.Windows.Point Position = Mouse.GetPosition((Grid)sender);
            List<CanvasLine> clickedElements = CanvasLines.Where((line) =>
                                            inRange((int)line.X1, (int)line.X2, (int)Position.X) &&
                                            inRange((int)line.Y1, (int)line.Y2, (int)Position.Y)).ToList();
            if (clickedElements.Any())
            {
                selectedLine = clickedElements.FirstOrDefault();
                Messenger.Default.Send(new NotificationMessage("DrawOnCanvas"));
            }
            else
            {
                if (selectedLine != null)
                {
                    selectedLine = null;
                    Messenger.Default.Send(new NotificationMessage("DrawOnCanvas"));
                }
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

        public bool inRange(int Range1, int Range2, int Value)
        {
            int smallest = Math.Min(Range1, Range2);
            int highest = Math.Max(Range1, Range2);
            return ((smallest - 2 <= Value && Value <= highest - 2) ||
                    (smallest - 1 <= Value && Value <= highest - 1) ||
                    (smallest <= Value && Value <= highest) ||
                    (smallest + 1 <= Value && Value <= highest + 1) ||
                    (smallest + 2 <= Value && Value <= highest + 2));
        }

        public void RedrawGrid()
        {
            EditGridDefinition();
            DrawOnCanvas();
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
            if (!this.isShown)
            {
                //is not shown, do not manage
                return;
            }
            FocusModel Model = msg.Sender as FocusModel;
            switch (msg.Notification)
            {
                case "CloseEditFocus":
                    if ((string)System.Windows.Application.Current.Properties["Mode"] == "Add")
                    {
                        ManageFocusViewModel viewModel = msg.Sender as ManageFocusViewModel;
                        addFocusToList(viewModel.Focus);
                        DrawOnCanvas();
                    }
                    else if((string)System.Windows.Application.Current.Properties["Mode"] == "Edit")
                    { 
                        EditGridDefinition();
                        DrawOnCanvas();
                    }
                    System.Windows.Application.Current.Properties["Mode"] = null;
                    break;
                case "DeleteFocus":
                    DeleteFocus(Model);
                    break;
                case "AddFocusMutually":
                    System.Windows.Application.Current.Properties["Mode"] = "Mutually";
                    selectedFocus = Model;
                    Model.IsSelected = true;
                    break;
                case "FinishAddFocusMutually":
                    if (selectedFocus != null && selectedFocus != Model &&
                        FociList.Where((f) => f == Model).Any())
                    {
                        UndoService.Current[GetUndoRoot()].BeginChangeSetBatch("AddMutuallyExclusive", false);
                        System.Windows.Application.Current.Properties["Mode"] = null;
                        selectedFocus.IsSelected = false;
                        var tempo = new MutuallyExclusiveSetModel(selectedFocus, Model);
                        selectedFocus.MutualyExclusive.Add(tempo);
                        Model.MutualyExclusive.Add(tempo);
                        UndoService.Current[GetUndoRoot()].EndChangeSetBatch();
                        RaisePropertyChanged(() => FociList);
                        DrawOnCanvas();
                    }
                    break;
                case "AddFocusPrerequisite":
                    System.Windows.Application.Current.Properties["Mode"] = "Prerequisite";
                    selectedFocus = Model;
                    Model.IsSelected = true;
                    break;
                case "FinishAddFocusPrerequisite":
                    if (selectedFocus != null && selectedFocus != Model &&
                        FociList.Where((f) => f == Model).Any())
                    {
                        UndoService.Current[GetUndoRoot()].BeginChangeSetBatch("AddPrerequisite", false);
                        System.Windows.Application.Current.Properties["Mode"] = null;
                        string Type = (string)System.Windows.Application.Current.Properties["ModeParam"];
                        System.Windows.Application.Current.Properties["ModeParam"] = null;
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
            }
        }

        public void UpdateFocus(FocusModel sender)
        {
            if (this.isShown)
            {
                EditGridDefinition();
                DrawOnCanvas();
            }
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

        const double PRE_LINE_HEIGHT = 20;

        public void DrawOnCanvas()
        {
            if (FociList == null)
            {
                return;
            }
            CanvasLines.Clear();
            foreach (FocusModel focus in FociList)
            {
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
                    foreach (FocusModel Prerequisite in set.FociList.OfType<FocusModel>())
                    {
                        //Draw horizontal lines to prerequisite pos
                        newline = new CanvasLine(
                            set.Focus.FocusTop.X,
                            set.Focus.FocusTop.Y - PRE_LINE_HEIGHT,
                            Prerequisite.FocusBottom.X,
                            set.Focus.FocusTop.Y - PRE_LINE_HEIGHT,
                            System.Windows.Media.Brushes.Teal, set.isRequired(), set);
                        CanvasLines.Add(newline);
                        //Draw line to prerequisite bottom
                        newline = new CanvasLine(
                            Prerequisite.FocusBottom.X,
                            set.Focus.FocusTop.Y - PRE_LINE_HEIGHT,
                            Prerequisite.FocusBottom.X,
                            Prerequisite.FocusBottom.Y,
                            System.Windows.Media.Brushes.Teal, set.isRequired(), set);
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
                    if (!CanvasLines.Where((line) => (line.X1 == newline.X1 &&
                                                    line.X2 == newline.X2 &&
                                                    line.Y1 == newline.Y1 &&
                                                    line.Y2 == newline.Y2)).Any())
                    {
                        CanvasLines.Add(newline);
                    }
                }
            }
            RaisePropertyChanged(() => CanvasLines);
            Messenger.Default.Send(new NotificationMessage("DrawOnCanvas"));
        }

        #region Undo/Redo

        void FociList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultChangeFactory.Current.OnCollectionChanged(this, 
                "FociList", this.FociList, e, "FociList Changed");
        }
        
        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion
    }
}
