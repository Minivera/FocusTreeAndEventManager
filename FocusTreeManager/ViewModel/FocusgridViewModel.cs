using FocusTreeManager.Model;
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

namespace FocusTreeManager.ViewModel
{
    public class FocusGridViewModel : ViewModelBase
    {
        private ObservableCollection<Focus> fociList;

        private ObservableCollection<CanvasLine> canvasLines;

        private Focus selectedFocus;

        private int rowCount;

        private int columnCount;

        public RelayCommand AddFocusCommand { get; private set; }

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

        public ObservableCollection<Focus> FociList
        {
            get
            {
                return fociList;
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

        public void addFocusToList(Focus FocusToAdd)
        {
            fociList.Add(FocusToAdd);
            RowCount = FocusToAdd.Y >= RowCount ? FocusToAdd.Y + 1 : RowCount;
            ColumnCount = FocusToAdd.X >= ColumnCount ? FocusToAdd.X + 1 : ColumnCount;
            DrawOnCanvas();
        }

        public void EditGridDefinition()
        {
            Focus biggestY = fociList.Aggregate((i1, i2) => i1.Y > i2.Y ? i1 : i2);
            Focus biggestX = fociList.Aggregate((i1, i2) => i1.X > i2.X ? i1 : i2);
            RowCount = biggestY.Y >= RowCount ? biggestY.Y + 1 : RowCount;
            ColumnCount = biggestX.X >= ColumnCount ? biggestX.X + 1 : ColumnCount;
        }

        public FocusGridViewModel()
        {
            fociList = new ObservableCollection<Focus>();
            canvasLines = new ObservableCollection<CanvasLine>();
            //Commands
            AddFocusCommand = new RelayCommand(AddFocus);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        public void AddFocus()
        {
            System.Windows.Application.Current.Properties["Mode"] = "Add";
            Messenger.Default.Send(new NotificationMessage("ShowAddFocus"));
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "HideAddFocus")
            {
                System.Windows.Application.Current.Properties["Mode"] = null;
                AddFocusViewModel viewModel = (AddFocusViewModel)msg.Sender;
                addFocusToList(viewModel.AddedFocus);
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
                fociList.Remove(Model);
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
            foreach (Focus focus in fociList)
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
