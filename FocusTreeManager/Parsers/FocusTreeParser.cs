using FocusTreeManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FocusTreeManager.CodeStructures;
using System.IO;
using System.Globalization;
using FocusTreeManager.DataContract;
using FocusTreeManager.CodeStructures.CodeExceptions;
using FocusTreeManager.Model.TabModels;

namespace FocusTreeManager.Parsers
{
    public static class FocusTreeParser
    {
        private static readonly string[] CORE_FOCUS_SCRIPTS_ELEMENTS =
        {
            "ai_will_do", "completion_reward", "available", "bypass", "cancel", "complete_tooltip"
        };

        private static readonly string[] ALL_PASED_ELEMENTS = 
        {
            "id", "x", "y", "icon", "prerequisite", "relative_position_id", "cost", "mutually_exclusive"
        };


        public static string ParseTreeForCompare(FocusGridModel model)
        {
            FociGridContainer container = new FociGridContainer(model);
            string focusTreeId = container.ContainerID.Replace(" ", "_");
            return Parse(container.FociList.ToList(), focusTreeId, 
                container.TAG, container.AdditionnalMods);
        }

        public static string ParseTreeScriptForCompare(string filename)
        {
            if (!File.Exists(filename))
            {
                return "";
            }
            Script script = new Script();
            script.Analyse(File.ReadAllText(filename));
            return ParseTreeForCompare(CreateTreeFromScript(filename, script));
        }

        public static Dictionary<string, string> ParseAllTrees(List<FociGridContainer> Containers)
        {
            Dictionary<string, string> fileList = new Dictionary<string, string>();
            foreach (FociGridContainer container in Containers)
            {
                string ID = container.ContainerID.Replace(" ", "_");
                fileList[ID] = Parse(container.FociList.ToList(), ID, 
                    container.TAG, container.AdditionnalMods);
            }
            return fileList;
        }

        public static string Parse(List<Focus> listFoci, string FocusTreeId, string TAG,
                                   string AdditionnalMods)
        {
            StringBuilder text = new StringBuilder();
            listFoci = prepareParse(listFoci);
            text.AppendLine("focus_tree = {");
            text.AppendLine("\tid = " + FocusTreeId);
            text.AppendLine("\tcountry = {");
            if (!string.IsNullOrEmpty(TAG))
            {
                text.AppendLine("\t\tfactor = 0");
                text.AppendLine("\t\tmodifier = {");
                text.AppendLine("\t\t\tadd = 10");
                text.AppendLine("\t\t\ttag = " + TAG);
                if (!string.IsNullOrEmpty(AdditionnalMods))
                {
                    foreach (string line in AdditionnalMods.Split('\n'))
                    {
                        text.AppendLine("\t\t\t" + line);
                    }
                }
                text.AppendLine("\t\t}");
                text.AppendLine("\t}");
                text.AppendLine("\tdefault = no");
            }
            //It is generic, make it default
            else
            {
                text.AppendLine("\t\tfactor = 1");
                text.AppendLine("\t}");
                text.AppendLine("\tdefault = yes");
            }
            foreach (Focus focus in listFoci)
            {
                text.AppendLine("\tfocus = {");
                text.AppendLine("\t\tid = " + focus.UniqueName);
                text.AppendLine("\t\ticon = GFX_" + focus.Image);
                text.AppendLine("\t\tcost = " + $"{focus.Cost:0.00}");
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
                    text.AppendLine("\t\tmutually_exclusive = {");
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
                text.AppendLine(focus.InternalScript.Parse(3));
                text.AppendLine("\t}");
            }
            text.AppendLine("}");
            return text.ToString();
        }
        
        private static List<Focus> prepareParse(List<Focus> listFoci)
        {
            List<Focus> SortedList = new List<Focus>();
            List<Focus> HoldedList = new List<Focus>();
            //Add the roots, the nodes without any perequisites. Helps with performance
            SortedList.AddRange(listFoci.Where((f) => !f.Prerequisite.Any()));
            int MaxY = listFoci.Max(i => i.Y);
            int MaxX = listFoci.Max(i => i.X);
            //For each X in the grid
            for (int i = 0; i < MaxX; i++)
            {
                //For each Y in the grid
                for (int j = 0; j < MaxY; j++)
                {
                    //If there is a focus with the current X and Y
                    Focus focus = listFoci.FirstOrDefault((f) => f.X == i && f.Y == j);
                    if (focus == null)
                    {
                        continue;
                    }
                    //If the prerequisites are not present
                    if (!CheckPrerequisite(focus, SortedList))
                    {
                        foreach (PrerequisitesSet set in focus.Prerequisite)
                        {
                            //check if any of the prerequisite can be added immediatly
                            foreach (Focus setFocus in set.FociList)
                            {
                                //If that focus has no prerequisites or the prerequisites are present
                                if ((CheckPrerequisite(setFocus, SortedList) || !setFocus.Prerequisite.Any())
                                    && !SortedList.Contains(setFocus))
                                {
                                    //Add it if it is not there already
                                    SortedList.Add(setFocus);
                                }
                            }
                        }
                        //Recheck prerequisite again
                        if (CheckPrerequisite(focus, SortedList) && !SortedList.Contains(focus))
                        {
                            //Add the focus to the sorted list
                            SortedList.Add(focus);
                        }
                        else
                        {
                            //Add the current focus to the holded list
                            HoldedList.Add(focus);
                            break;
                        }
                    }
                    else if (!SortedList.Contains(focus))
                    {
                        //Otherwise, add it to the list
                        SortedList.Add(focus);
                    }
                    //Check if we can add some of the holded focus
                    AddBackwardsPrerequisite(HoldedList, SortedList);
                }
                //Check if we can add some of the holded focus
                AddBackwardsPrerequisite(HoldedList, SortedList);
            }
            //Check if we can add some of the holded focus
            AddBackwardsPrerequisite(HoldedList, SortedList);
            //Just to be sure, add any prerequisite that are not in the list, but in the original
            SortedList.AddRange(listFoci.Except(SortedList));
            return SortedList;
        }

        private static bool CheckPrerequisite(Focus focus, ICollection<Focus> SortedList)
        {
            return focus.Prerequisite.All(set => set.FociList.All(
                SortedList.Contains));
        }

        private static void AddBackwardsPrerequisite(ICollection<Focus> HoldedList, 
                                                     ICollection<Focus> SortedList)
        {
            bool wasAdded = false;
            List<Focus> TempoList = HoldedList.ToList();
            //Run through all focus holded
            foreach (Focus focus in TempoList)
            {
                if (CheckPrerequisite(focus, SortedList) && !SortedList.Contains(focus))
                {
                    //If all prerequisites were okay, add it to the list
                    SortedList.Add(focus);
                    HoldedList.Remove(focus);
                    wasAdded = true;
                }
                //If one focus was added
                if (wasAdded)
                {
                    //Recheck prerequisites
                    AddBackwardsPrerequisite(HoldedList, SortedList);
                }
            }
        }

        public static FocusGridModel CreateTreeFromScript(string fileName, Script script)
        {
            FocusGridModel container = new FocusGridModel(Script.TryParse(script, "id"));
            //Get content of Modifier block
            Assignation modifier = script.FindAssignation("modifier");
            container.TAG = Script.TryParse(modifier, "tag", false);
            if (container.TAG != null)
            {
                container.AdditionnalMods = modifier.GetContentAsScript(new[] { "add", "tag" })
                                                    .Parse(0);
            }
            //Run through all foci
            foreach (ICodeStruct codeStruct in script.FindAllValuesOfType<CodeBlock>("focus"))
            {
                CodeBlock block = (CodeBlock)codeStruct;
                try
                {
                    //Create the focus
                    FocusModel newFocus = new FocusModel
                    {
                        UniqueName = Script.TryParse(block, "id"),
                        Image = Script.TryParse(block, "icon").Replace("GFX_", ""),
                        X = int.Parse(Script.TryParse(block, "x")),
                        Y = int.Parse(Script.TryParse(block, "y")),
                        Cost = GetDouble(Script.TryParse(block, "cost"), 10)
                    };
                    //Get all core scripting elements
                    Script InternalFocusScript = new Script();
                    foreach (string element in CORE_FOCUS_SCRIPTS_ELEMENTS)
                    {
                        ICodeStruct found = block.FindAssignation(element);
                        if (found != null)
                        {
                            InternalFocusScript.Code.Add(found);
                        }
                    }
                    InternalFocusScript.Code.AddRange(block.
                        GetContentAsScript(CORE_FOCUS_SCRIPTS_ELEMENTS.
                            Concat(ALL_PASED_ELEMENTS).ToArray()).Code);
                    newFocus.InternalScript = InternalFocusScript;
                    container.FociList.Add(newFocus);
                }
                catch (SyntaxException e)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Invalid syntax for focus "
                        + Script.TryParse(block, "id") + ", please double-check the syntax.");
                    ErrorLogger.Instance.AddLogLine("\t" + e.Message);
                }
                catch (Exception)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Invalid syntax for focus " 
                        + Script.TryParse(block, "id") + ", please double-check the syntax.");
                }
            }
            //Run through all foci again for mutually exclusives and prerequisites
            foreach (ICodeStruct codeStruct in script.FindAllValuesOfType<CodeBlock>("focus"))
            {
                CodeBlock block = (CodeBlock)codeStruct;
                string id = block.FindValue("id") != null ? block.FindValue("id").Parse() : "";
                FocusModel newFocus = container.FociList.FirstOrDefault(f => f.UniqueName == id);
                if (newFocus == null)
                {
                    //Check if we removed this focus because of a syntax error
                    continue;
                } 
                foreach (ICodeStruct exclusives in block.FindAllValuesOfType<ICodeStruct>
                    ("mutually_exclusive"))
                {
                    foreach (ICodeStruct focuses in exclusives
                        .FindAllValuesOfType<ICodeStruct>("focus"))
                    {
                        //Check if focus exists in list
                        FocusModel found = container.FociList.FirstOrDefault((f) =>
                            f.UniqueName == focuses.Parse());
                        //If we have found something
                        if (found == null) continue;
                        MutuallyExclusiveSetModel set = 
                            new MutuallyExclusiveSetModel(newFocus, found);
                        //Check if the set already exists in this focus
                        if (newFocus.MutualyExclusive.Contains(set) ||
                            found.MutualyExclusive.Contains(set))
                        {
                            continue;
                        }
                        newFocus.MutualyExclusive.Add(set);
                        found.MutualyExclusive.Add(set);
                    }
                }
                foreach (ICodeStruct prerequistes in block
                    .FindAllValuesOfType<ICodeStruct>("prerequisite"))
                {
                    if (!prerequistes.FindAllValuesOfType<ICodeStruct>("focus").Any())
                    {
                        break;
                    }
                    PrerequisitesSetModel set = new PrerequisitesSetModel(newFocus);
                    foreach (ICodeStruct focuses in prerequistes.FindAllValuesOfType<ICodeStruct>
                        ("focus"))
                    {
                        //Add the focus as a prerequisites in the current existing focuses
                        FocusModel search = container.FociList.FirstOrDefault((f) =>
                            f.UniqueName == focuses.Parse());
                        if (search != null)
                        {
                            set.FociList.Add(search);
                        }
                    }
                    //If any prerequisite was added (Poland has empty prerequisite blocks...)
                    if (set.FociList.Any())
                    { 
                        newFocus.Prerequisite.Add(set);
                    }
                }
            }
            return container;
        }

        public static double GetDouble(string value, double defaultValue)
        {
            double result;
            //Try parsing in the current culture
            if (!double.TryParse(value, NumberStyles.Any, 
                    CultureInfo.CurrentCulture, out result) &&
                //Then try in US english
                !double.TryParse(value, NumberStyles.Any, 
                    CultureInfo.GetCultureInfo("en-US"), out result) &&
                //Then in neutral language
                !double.TryParse(value, NumberStyles.Any, 
                    CultureInfo.InvariantCulture, out result))
            {
                result = defaultValue;
            }
            return result;
        }
    }
}
