using FocusTreeManager.Containers;
using FocusTreeManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FocusTreeManager.CodeStructures;
using System.IO;

namespace FocusTreeManager.Parsers
{
    static class FocusTreeParser
    {
        public static Dictionary<string, string> ParseAllTrees(ObservableCollection<FociGridContainer> Containers)
        {
            Dictionary<string, string> fileList = new Dictionary<string, string>();
            foreach (FociGridContainer container in Containers)
            {
                string ID = container.ContainerID.Replace(" ", "_");
                fileList[ID] = Parse(container.FociList.ToList(), ID, container.TAG);
            }
            return fileList;
        }

        public static string Parse(List<Focus> listFoci, string FocusTreeId, string TAG)
        {
            StringBuilder text = new StringBuilder();
            listFoci = prepareParse(listFoci);
            text.AppendLine("focus_tree = {");
            text.AppendLine("\tid = " + FocusTreeId);
            text.AppendLine("\tcountry = {");
            text.AppendLine("\t\tfactor = 0");
            text.AppendLine("\t\tmodifier = {");
            text.AppendLine("\t\t\tadd = 10");
            text.AppendLine("\t\t\ttag = " + TAG);
            text.AppendLine("\t\t}");
            text.AppendLine("\t}");
            text.AppendLine("\tdefault = no");
            foreach (var focus in listFoci)
            {
                text.AppendLine("\tfocus = {");
                text.AppendLine("\t\tid = " + focus.UniqueName);
                text.AppendLine("\t\ticon = GFX_" + focus.Icon);
                text.AppendLine("\t\tcost = " + focus.Cost);
                if (focus.Prerequisite.Any())
                {
                    foreach (PrerequisitesSet prereqisite in focus.Prerequisite)
                    {
                        text.AppendLine("\t\tprerequisite = {");
                        foreach (Focus PreFoci in prereqisite.FociList)
                        {
                            text.AppendLine("\t\t\tfocus = " + PreFoci.UniqueName);
                        }
                        text.AppendLine("\t\t}");
                    }
                }
                if (focus.MutualyExclusive.Any())
                {
                    text.AppendLine("\t\tmutually_exclusive = ");
                    foreach (MutuallyExclusiveSet ExclusiveSet in focus.MutualyExclusive)
                    {
                        if (ExclusiveSet.Focus1.UniqueName != focus.UniqueName)
                        {
                            text.AppendLine("\t\t\tfocus = " + ExclusiveSet.Focus1.UniqueName);
                        }
                        else
                        {
                            text.AppendLine("\t\t\tfocus = " + ExclusiveSet.Focus2.UniqueName);
                        }
                    }
                    text.AppendLine("\t\t}");
                }
                text.AppendLine("\t\tx = " + focus.X);
                text.AppendLine("\t\ty = " + focus.Y);
                //TODO: Write the focus code
                text.AppendLine("\t}");
            }
            text.AppendLine("}");
            return text.ToString();
        }

        private static List<Focus> prepareParse(List<Focus> listFoci)
        {
            List<Focus> SortedList = new List<Focus>();
            List<Focus> HoldedList = new List<Focus>();
            //Loop inside the list sorted by X then Y
            foreach (Focus focus in listFoci.OrderBy(f => f.X)
                                    .ThenBy(f => f.Y)
                                    .ToList())
            {
                //If there is any prerequisites
                if (focus.Prerequisite.Any())
                {
                    bool isValid = true;
                    //Loop inside all prerequisites
                    foreach (PrerequisitesSet set in focus.Prerequisite)
                    {
                        //Check if all the prerequisites are added
                        if (!set.FociList.All(value => SortedList.Contains(value)))
                        {
                            //Add the current focus to the holded list, break
                            HoldedList.Add(focus);
                            isValid = false;
                            break;
                        }
                    }
                    if (isValid)
                    {
                        //If all prerequisites were okay, add it to the list
                        SortedList.Add(focus);
                    }
                }
                else
                {
                    //Otherwise, add it to the list
                    SortedList.Add(focus);
                }
                //Check if we can add some of the holded focus
                CheckPrerequisite(HoldedList, SortedList);
            }
            return SortedList;
        }

        public static FociGridContainer CreateTreeFromScript(string fileName, Script script)
        {
            Dictionary<string, PrerequisitesSet> waitingList = new Dictionary<string, PrerequisitesSet>();
            FociGridContainer container = new FociGridContainer(Path.GetFileNameWithoutExtension(fileName));
            container.TAG = script.Find("tag") != null ? script.Find("tag").Parse() : "";
            foreach (CodeBlock block in script.FindAll<CodeBlock>("focus"))
            {
                Focus newFocus = new Focus();
                newFocus.UniqueName = block.Find("id").Parse();
                newFocus.Image = block.Find("icon").Parse().Replace("GFX_", "");
                newFocus.X = int.Parse(block.Find("x").Parse());
                newFocus.Y = int.Parse(block.Find("y").Parse());
                newFocus.Cost = int.Parse(block.Find("cost").Parse());
                foreach (ICodeStruct exclusives in block.FindAll<ICodeStruct>("mutually_exclusive"))
                {
                    foreach (ICodeStruct focuses in exclusives.FindAll<ICodeStruct>("focus"))
                    {
                        //Check if focus exists in list
                        Focus found = container.FociList.FirstOrDefault((f) =>
                            f.UniqueName == focuses.Parse());
                        if (found != null)
                        {
                            MutuallyExclusiveSet set = new MutuallyExclusiveSet(newFocus, found);
                            newFocus.MutualyExclusive.Add(set);
                            found.MutualyExclusive.Add(set);
                        }
                    }
                }
                foreach (ICodeStruct prerequistes in block.FindAll<ICodeStruct>("prerequisite"))
                {
                    if (!prerequistes.FindAll<ICodeStruct>("focus").Any())
                    {
                        break;
                    }
                    PrerequisitesSet set = new PrerequisitesSet(newFocus);
                    foreach (ICodeStruct focuses in prerequistes.FindAll<ICodeStruct>("focus"))
                    {
                        //Add the focus as a prerequisites in the current existing focuses
                        //or put into wait
                        Focus search = container.FociList.FirstOrDefault((f) =>
                            f.UniqueName == focuses.Parse());
                        if (search != null)
                        {
                            set.FociList.Add(search);
                        }
                        else
                        {
                            waitingList[focuses.Parse()] = set;
                        }
                    }
                    newFocus.Prerequisite.Add(set);
                }
                //TODO: Load script
                container.FociList.Add(newFocus);
            }
            //Repair lost sets, shouldn't happen, but in case
            foreach (KeyValuePair<string, PrerequisitesSet> item in waitingList)
            {
                Focus search = container.FociList.FirstOrDefault((f) =>
                        f.UniqueName == item.Key);
                if (search != null)
                {
                    item.Value.FociList.Add(search);
                }
            }
            return container;
        }

        private static void CheckPrerequisite(List<Focus> HoldedList, List<Focus> CurrentList)
        {
            bool wasAdded = false;
            List<Focus> TempoList = HoldedList.ToList();
            //Run through all focus holded
            foreach (Focus focus in TempoList)
            {
                bool isValid = true;
                //Loop inside all prerequisites
                foreach (PrerequisitesSet set in focus.Prerequisite)
                {
                    //Check if all the prerequisites are added
                    if (set.FociList.Except(CurrentList).Any())
                    {
                        isValid = false;
                        break;
                    }
                }
                if (isValid)
                {
                    //If all prerequisites were okay, add it to the list
                    CurrentList.Add(focus);
                    HoldedList.Remove(focus);
                    wasAdded = true;
                }
                //If one focus was added
                if (wasAdded)
                {
                    //Recheck prerequisites
                    CheckPrerequisite(HoldedList, CurrentList);
                }
            }
        }
    }
}
