using Dragablz;
using FocusTreeManager.Containers;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FocusTreeManager.Model
{
    public class FocusGridModel : ObservableObject
    {
        private ObservableCollection<CanvasLine> canvasLines;

        private Focus selectedFocus;

        private int rowCount;

        private int columnCount;

        private string treeId;

        public RelayCommand AddFocusCommand { get; private set; }

        public RelayCommand<object> RightClickCommand { get; private set; }

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

        public string Filename
        {
            get
            {
                return treeId;
            }
            set
            {
                treeId = value;
                RaisePropertyChanged("Filename");
            }
        }

        public ObservableCollection<Focus> FociList
        {
            get
            {
                return (new ViewModelLocator()).Main.Project.getSpecificFociList(treeId);
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

        public FocusGridModel()
        {
            canvasLines = new ObservableCollection<CanvasLine>();
            //Commands
            AddFocusCommand = new RelayCommand(AddFocus);
            RightClickCommand = new RelayCommand<object>(RightClick);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
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

        public void AddFocus()
        {
            System.Windows.Application.Current.Properties["Mode"] = "Add";
            Messenger.Default.Send(new NotificationMessage("ShowAddFocus"));
        }

        public void RightClick(object sender)
        {
            System.Windows.Point Position = Mouse.GetPosition((Grid)sender);
            List<CanvasLine> clickedElements = CanvasLines.Where((line) =>
                                            inRange(line.X1, line.X2, (int)Position.X) &&
                                            inRange(line.Y1, line.Y2, (int)Position.Y)).ToList();
            List<CanvasLine> NewClickedElements = new List<CanvasLine>();
            foreach (CanvasLine line in clickedElements)
            {
                line.InternalSet.DeleteSetRelations();
                //Make sur to add all lines linked to that set
                NewClickedElements.AddRange(CanvasLines.Where((l) => l.InternalSet == line.InternalSet));
                //But remove our from it
                NewClickedElements.Remove(line);
            }
            //Delete added relations
            foreach (CanvasLine line in NewClickedElements)
            {
                line.InternalSet.DeleteSetRelations();
            }
            CanvasLines = new ObservableCollection<CanvasLine>( CanvasLines.Except(clickedElements).ToList());
            DrawOnCanvas();
        }

        private bool inRange(int Range1, int Range2, int Value)
        {
            int smallest = Math.Min(Range1, Range2);
            int highest = Math.Max(Range1, Range2);
            return ((smallest - 2 <= Value && Value <= highest - 2) ||
                    (smallest - 1 <= Value && Value <= highest - 1) ||
                    (smallest <= Value && Value <= highest) ||
                    (smallest + 1 <= Value && Value <= highest + 1) ||
                    (smallest + 2 <= Value && Value <= highest + 2));
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (this.FociList == null)
            {
                //meant to be dead, return
                return;
            }
            if (msg.Notification == "RenamedContainer")
            {
                FociGridContainer Model = msg.Sender as FociGridContainer;
                if (Model == null)
                {
                    return;
                }
                Filename = Model.ContainerID;
                DrawOnCanvas();
            }
            if (msg.Notification == "RedrawGrid")
            {
                EditGridDefinition();
                DrawOnCanvas();
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
                if (selectedFocus != null && selectedFocus != Model)
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
                if (selectedFocus != null && selectedFocus != Model)
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
        }

        const int FOCUS_WIDTH = 90;

        const int FOCUS_HEIGHT = 122;

        const int TRUE_FOCUS_HEIGHT = 60;

        const int PRE_LINE_HEIGHT = 20;

        public void DrawOnCanvas()
        {
            CanvasLines.Clear();
            foreach (Focus focus in FociList)
            {
                //Draw Prerequisites
                foreach (PrerequisitesSet set in focus.Prerequisite)
                {
                    //Draw line from top of fist Focus 
                    CanvasLine newline = new CanvasLine(
                        ((set.Focus.X) * FOCUS_WIDTH) + (FOCUS_WIDTH / 2),
                        (set.Focus.Y + 1) * FOCUS_HEIGHT - TRUE_FOCUS_HEIGHT,
                        ((set.Focus.X) * FOCUS_WIDTH) + (FOCUS_WIDTH / 2),
                        ((set.Focus.Y + 1) * FOCUS_HEIGHT) - PRE_LINE_HEIGHT - TRUE_FOCUS_HEIGHT,
                        System.Windows.Media.Brushes.Teal, set.isRequired(), set);
                    CanvasLines.Add(newline);
                    foreach (Focus Prerequisite in set.FociList)
                    {
                        //Draw horizontal lines to prerequisite pos
                        newline = new CanvasLine(
                            ((set.Focus.X) * FOCUS_WIDTH) + (FOCUS_WIDTH / 2),
                            (set.Focus.Y + 1) * FOCUS_HEIGHT - PRE_LINE_HEIGHT - TRUE_FOCUS_HEIGHT,
                            ((Prerequisite.X) * FOCUS_WIDTH) + (FOCUS_WIDTH / 2),
                            ((set.Focus.Y + 1) * FOCUS_HEIGHT) - PRE_LINE_HEIGHT - TRUE_FOCUS_HEIGHT,
                            System.Windows.Media.Brushes.Teal, set.isRequired(), set);
                        CanvasLines.Add(newline);
                        //Draw line to prerequisite bottom
                        newline = new CanvasLine(
                            ((Prerequisite.X) * FOCUS_WIDTH) + (FOCUS_WIDTH / 2),
                            (set.Focus.Y + 1) * FOCUS_HEIGHT - PRE_LINE_HEIGHT - TRUE_FOCUS_HEIGHT,
                            ((Prerequisite.X) * FOCUS_WIDTH) + (FOCUS_WIDTH / 2),
                            ((Prerequisite.Y + 1) * FOCUS_HEIGHT) + PRE_LINE_HEIGHT,
                            System.Windows.Media.Brushes.Teal, set.isRequired(), set);
                        CanvasLines.Add(newline);
                    }
                }
                //Draw Mutually exclusives
                foreach (MutuallyExclusiveSet set in focus.MutualyExclusive)
                {
                    CanvasLine newline = new CanvasLine(
                        (set.Focus1.X + 1) * FOCUS_WIDTH,
                        ((set.Focus1.Y + 1) * FOCUS_HEIGHT) - (FOCUS_HEIGHT / 2),
                        (set.Focus2.X) * FOCUS_WIDTH,
                        ((set.Focus2.Y + 1) * FOCUS_HEIGHT) - (FOCUS_HEIGHT / 2),
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
