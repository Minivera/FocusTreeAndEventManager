using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using FocusTreeManager.ViewModel;

namespace FocusTreeManager.Helper
{
    public static class TutorialHelper
    {
        public static Dictionary<string, List<TutorialStep>> getTutorials()
        {
            Dictionary<string, List<TutorialStep>> dictionnary = new Dictionary<string, List<TutorialStep>>();
            foreach (string file in Directory.EnumerateFiles("Common\\Tutorials", "*.xml"))
            {
                XDocument doc = XDocument.Load(file);
                //Skip a fil if it has no roots
                if (doc.Root == null) continue;
                IEnumerable<XElement> tutorials = doc.Root.Elements("tutorial");
                //Run through each tutorial
                foreach (XElement tutorial in tutorials)
                {
                    List<TutorialStep> list = new List<TutorialStep>();
                    XAttribute control = tutorial.Attribute("control");
                    //Make sure a control is set
                    if (control == null) continue;
                    //Else, we start running through steps
                    foreach (XElement step in tutorial.Elements("step"))
                    {
                        TutorialStep stepObj = new TutorialStep
                        {
                            TextKey = step.Attribute("Text")?.Value,
                            ComponentToHighlight = step.Attribute("Highlight")?.Value,
                            WaitForMessage = step.Attribute("WaitForMessage")?.Value,
                            RunThisCommand = step.Attribute("CommandToRun")?.Value,
                            SendThisMessage = step.Attribute("MessageToSend")?.Value,
                            hasArrow = step.Attribute("Arrow") != null,
                            RightClickOnComponent = step.Attribute("RightClick") != null
                        };
                        if (step.Attribute("Circle") != null)
                        {
                            stepObj.IsSquare = false;
                            stepObj.IsCircle = true;
                        }
                        else
                        {
                            stepObj.IsSquare = true;
                            stepObj.IsCircle = false;
                        }
                        list.Add(stepObj);
                    }
                    dictionnary[control.Value] = list;
                }
            }
            return dictionnary;
        }
    }
}
