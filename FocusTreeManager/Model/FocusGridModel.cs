using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FocusTreeManager.Model
{
    public class FocusGridModel : ObservableObject
    {
        const int MIN_ROW_COUNT = 7;
        const int MIN_COLUMN_COUNT = 20;

        private ObservableCollection<CanvasLine> canvasLines;

        private Focus selectedFocus;

        public CanvasLine selectedLine { get; set; }

        private int rowCount;

        private int columnCount;

        private Guid ID;

        public RelayCommand<object> AddFocusCommand { get; private set; }

        public RelayCommand<object> RightClickCommand { get; private set; }

        public RelayCommand<object> HoverCommand { get; private set; }

        public bool isShown { get; set; }

        public int RowCount
        {
            get
            {
                return rowCount;
            }
            set
            {
                rowCount = value;
                RaisePropertyChanged("RowCount");
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
                columnCount = value;
                RaisePropertyChanged("ColumnCount");
            }
        }

        public Guid UniqueID
        {
            get
            {
                return ID;
            }
        }

        public string Filename
        {
            get
            {
                var element = (new ViewModelLocator()).Main.Project.getSpecificFociList(ID);
                return element != null ? element.ContainerID : null;
            }
        }

        public string TAG
        {
            get
            {
                var element = (new ViewModelLocator()).Main.Project.getSpecificFociList(ID);
                return element != null ? element.TAG : null;
            }
            set
            {
                var element = (new ViewModelLocator()).Main.Project.getSpecificFociList(ID);
                if (element != null)
                {
                    element.TAG = value;
                }
            }
        }

        public ObservableCollection<Focus> FociList
        {
            get
            {
                var element = (new ViewModelLocator()).Main.Project.getSpecificFociList(ID);
                return element != null ? element.FociList : null;
            }
        }

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

        public FocusGridModel(Guid ID)
        {
            //Min Row & column Count
            RowCount = MIN_ROW_COUNT;
            ColumnCount = MIN_COLUMN_COUNT;
            this.ID = ID;
            canvasLines = new ObservableCollection<CanvasLine>();
            //Commands
            AddFocusCommand = new RelayCommand<object>(AddFocus);
            RightClickCommand = new RelayCommand<object>(RightClick);
            HoverCommand = new RelayCommand<object>(Hover);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        internal void ChangePosition(object draggedElement, Point currentPoint)
        {
            if (!(draggedElement is Focus))
            {
                return;
            }
            else
            {
                int X = (int)Math.Floor(currentPoint.X / 89);
                int Y = (int)Math.Floor(currentPoint.Y / 140);
                ((Focus)draggedElement).X = X;
                ((Focus)draggedElement).Y = Y;
                RaisePropertyChanged(() => FociList);
                EditGridDefinition();
                DrawOnCanvas();
            }
        }

        public void addFocusToList(Focus FocusToAdd)
        {
            FociList.Add(FocusToAdd);
            RowCount = FocusToAdd.Y >= RowCount ? FocusToAdd.Y + 1 : RowCount;
            ColumnCount = FocusToAdd.X >= ColumnCount ? FocusToAdd.X + 1 : ColumnCount;
            DrawOnCanvas();
        }

        public void EditGridDefinition()
        {
            if (!FociList.Any())
            {
                return;
            }
            Focus biggestY = FociList.Aggregate((i1, i2) => i1.Y > i2.Y ? i1 : i2);
            Focus biggestX = FociList.Aggregate((i1, i2) => i1.X > i2.X ? i1 : i2);
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
                foreach (CanvasLine line in clickedElements)
                {
                    line.InternalSet.DeleteSetRelations();
                }
                RaisePropertyChanged("FociList");
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
            if (!this.isShown || this.Filename == null)
            {
                //is not shown, do not manage
                return;
            }
            if (msg.Notification == "HideAddFocus")
            {
                System.Windows.Application.Current.Properties["Mode"] = null;
                AddFocusViewModel viewModel = msg.Sender as AddFocusViewModel;
                addFocusToList(viewModel.Focus);
                RaisePropertyChanged(() => FociList);
                DrawOnCanvas();
            }
            if (msg.Notification == "HideEditFocus")
            {
                System.Windows.Application.Current.Properties["Mode"] = null;
                EditGridDefinition();
                DrawOnCanvas();
            }
            if (msg.Notification == "DeleteFocus")
            {
                Focus Model = (Focus)msg.Sender;
                //Kill the set that might have this focus as parent
                foreach (Focus focus in FociList)
                {
                    foreach (PrerequisitesSet set in focus.Prerequisite.ToList())
                    {
                        if (set.FociList.Contains(Model))
                        {
                            set.DeleteSetRelations();
                        }
                    }
                }
                FociList.Remove(Model);
                RaisePropertyChanged(() => FociList);
                EditGridDefinition();
                DrawOnCanvas();
            }
            if (msg.Notification == "AddFocusMutually")
            {
                System.Windows.Application.Current.Properties["Mode"] = "Mutually";
                Focus Model = (Focus)msg.Sender;
                selectedFocus = Model;
                Model.IsSelected = true;
            }
            if (msg.Notification == "FinishAddFocusMutually")
            {
                Focus Model = (Focus)msg.Sender;
                if (selectedFocus != null && selectedFocus != Model && 
                    FociList.Where((f) => f == Model).Any())
                {
                    System.Windows.Application.Current.Properties["Mode"] = null;
                    selectedFocus.IsSelected = false;
                    var tempo = new MutuallyExclusiveSet(selectedFocus, Model);
                    selectedFocus.MutualyExclusive.Add(tempo);
                    Model.MutualyExclusive.Add(tempo);
                    DrawOnCanvas();
                }
            }
            if (msg.Notification == "AddFocusPrerequisite")
            {
                System.Windows.Application.Current.Properties["Mode"] = "Prerequisite";
                Focus Model = (Focus)msg.Sender;
                selectedFocus = Model;
                Model.IsSelected = true;
            }
            if (msg.Notification == "FinishAddFocusPrerequisite")
            {
                Focus Model = (Focus)msg.Sender;
                if (selectedFocus != null && selectedFocus != Model &&
                    FociList.Where((f) => f == Model).Any())
                {
                    System.Windows.Application.Current.Properties["Mode"] = null;
                    string Type = (string)System.Windows.Application.Current.Properties["ModeParam"];
                    System.Windows.Application.Current.Properties["ModeParam"] = null;
                    selectedFocus.IsSelected = false;
                    if (Type == "Required")
                    {
                        //Create new set
                        PrerequisitesSet set = new PrerequisitesSet(selectedFocus);
                        set.FociList.Add(Model);
                        selectedFocus.Prerequisite.Add(set);
                    }
                    else
                    {
                        //Create new set if no exist
                        if (!selectedFocus.Prerequisite.Any())
                        {
                            PrerequisitesSet set = new PrerequisitesSet(selectedFocus);
                            selectedFocus.Prerequisite.Add(set);
                        }
                        //Add Model to first Set
                        selectedFocus.Prerequisite.First().FociList.Add(Model);
                    }
                    RaisePropertyChanged(() => FociList);
                    DrawOnCanvas();
                }
            }
            if (msg.Notification == "ContainerRenamed")
            {
                RaisePropertyChanged(() => Filename);
            }
        }

        public void UpdateFocus(Focus sender)
        {
            if (this.isShown)
            {
                EditGridDefinition();
                DrawOnCanvas();
            }
        }

        const double PRE_LINE_HEIGHT = 20;

        public void DrawOnCanvas()
        {
            if (FociList == null)
            {
                return;
            }
            CanvasLines.Clear();
            foreach (Focus focus in FociList)
            {
                //Draw Prerequisites
                foreach (PrerequisitesSet set in focus.Prerequisite)
                {
                    //Draw line from top of first Focus 
                    CanvasLine newline = new CanvasLine(
                        set.Focus.FocusTop.X,
                        set.Focus.FocusTop.Y,
                        set.Focus.FocusTop.X,
                        set.Focus.FocusTop.Y - PRE_LINE_HEIGHT,
                        System.Windows.Media.Brushes.Teal, set.isRequired(), set);
                    CanvasLines.Add(newline);
                    foreach (Focus Prerequisite in set.FociList)
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
                foreach (MutuallyExclusiveSet set in focus.MutualyExclusive)
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
    }
}
