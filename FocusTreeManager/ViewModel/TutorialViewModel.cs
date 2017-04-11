using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using FocusTreeManager.Adorners;
using FocusTreeManager.Helper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FocusTreeManager.ViewModel
{
    public struct TutorialStep
    {
        public string TextKey;
        public string ComponentToHighlight;
        public bool IsCircle;
        public bool IsSquare;
        public string WaitForMessage;
        public bool hasArrow;
        public bool RightClickOnComponent;
        public string RunThisCommand;
        public string SendThisMessage;
    }

    public class TutorialViewModel : ViewModelBase
    {
        private FrameworkElement managedElement;

        private TutorialAdorner managedAdorner;

        private AdornerLayer layer;

        private Dictionary<string, List<TutorialStep>> AllControls;

        public List<TutorialStep> Steps { get; set; }

        private int currentStep;

        public TutorialStep CurrentStep => Steps[currentStep];

        public bool InTutorial { get; private set; }

        public RelayCommand ContinueCommand { get; private set; }

        public RelayCommand<FrameworkElement> StartCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TutorialViewModel class.
        /// </summary>
        public TutorialViewModel()
        {
            AllControls = TutorialHelper.getTutorials();
            Steps = new List<TutorialStep>();
            currentStep = 0;
            InTutorial = false;
            ContinueCommand = new RelayCommand(Continue);
            StartCommand = new RelayCommand<FrameworkElement>(Start);
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        public void Continue()
        {
            //Continue the tutorial
            if (currentStep + 1 <= Steps.Count)
            {
                currentStep++;
            }
            else
            {
                InTutorial = false;
                managedAdorner = null;
                managedElement = null;
                layer.Remove(managedAdorner);
            }
            //Update adorner
            layer.Update();
            Messenger.Default.Send(new NotificationMessage("ContinueTutorial"));
        }

        public void Start(FrameworkElement element)
        {
            if (InTutorial) return;
            //Check if we have a tutorial for this element
            if (!AllControls.ContainsKey(element.Name)) return;
            InTutorial = true;
            //If yes, continue
            Steps = AllControls[element.Name];
            currentStep = 0;
            //Get element's windows parent
            managedElement = element;
            Window parent = UiHelper.FindVisualParent<Window>(managedElement, null);
            //Get the second grid, available just after the base adorner decorator.
            Grid secondGrid = UiHelper.FindVisualChildren<Grid>
                (UiHelper.FindVisualChildren<AdornerDecorator>(parent).First()).First();
            layer = AdornerLayer.GetAdornerLayer(secondGrid);
            //Create a new Adorner
            managedAdorner = new TutorialAdorner(managedElement, secondGrid);
            layer.Add(managedAdorner);
            layer.IsHitTestVisible = true;
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            //If this is not the intended target
            if (msg.Target != null && msg.Target != this) return;
            //If in tutorial and waiting for a command
            if (InTutorial && !string.IsNullOrEmpty(CurrentStep.WaitForMessage))
            {
                //If the message is the same as the one we are waiting for
                if (msg.Notification == CurrentStep.WaitForMessage)
                {
                    //Go to next step
                    Continue();
                }
            }
        }
    }
}