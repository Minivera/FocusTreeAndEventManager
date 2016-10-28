using FocusTreeManager.CodeStructures;
using FocusTreeManager.CodeStructures.CodeExceptions;
using FocusTreeManager.Model;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FocusTreeManager.Helper
{
    public static class ModelsToScriptHelper
    {
        private static readonly Brush[] CONDITIONS_COLORS =
        {
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#b71c1c")),
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#f44336")),
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffff"))
        };

        private static readonly Brush[] BLOCKS_COLORS =
        {
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#3f51b5")),
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#1a237e")),
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffff"))
        };

        private static readonly Brush[] ASSIGNATIONS_COLORS =
        {
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffeb3b")),
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#f57f17")),
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"))
        };

        private static readonly string[] COMMON_CONTROLS =
        {
            "if", "AND", "OR", "NOT", "limit", "ai_will_do",
            "completion_reward", "available", "bypass", "cancel"
        };

        private static readonly string[] COMMON_CONTROLS_NAMES =
        {
            "Scripter_Condition_If", "Scripter_Condition_AND", "Scripter_Condition_OR",
            "Scripter_Condition_NOT", "Scripter_Condition_Limit", "Scripter_Block_AI",
            "Scripter_Block_Reward", "Scripter_Block_Available", "Scripter_Block_Bypass",
            "Scripter_Block_Cancel"
        };

        public static List<AssignationModel> TransformScriptToModels(Script script, RelayCommand<object> deleteCommand)
        {
            List<AssignationModel> listModels = new List<AssignationModel>();
            if (script.Code == null)
            {
                return listModels;
            }
            foreach (ICodeStruct code in script.Code)
            {
                try
                {
                    listModels.Add(BlockToModel(code, deleteCommand));
                }
                catch (RecursiveCodeException e)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Error when parsing the script to scripter controls");
                    ErrorLogger.Instance.AddLogLine("\t" + e.Message);
                }
                catch (Exception)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Unknown error in script");
                    continue;
                }
            }
            return listModels;
        }

        private static AssignationModel BlockToModel(ICodeStruct currentBlock, RelayCommand<object> deleteCommand)
        {
            //Should never loop inside something other than an assignation, check type
            if (currentBlock is Assignation)
            {
                Assignation block = currentBlock as Assignation;
                //check children
                if (block.Value is CodeValue)
                {
                    ResourceDictionary resourceLocalization = new ResourceDictionary();
                    resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
                    //Simple assignation, return corresponding model
                    return new AssignationModel()
                    {
                        BackgroundColor = ASSIGNATIONS_COLORS[0],
                        BorderColor = ASSIGNATIONS_COLORS[1],
                        Color = ASSIGNATIONS_COLORS[2],
                        IsNotEditable = false,
                        CanHaveChild = false,
                        Code = Regex.Replace(block.Assignee, @"\t|\n|\r|\s", "") + " " + block.Operator
                                + " " + Regex.Replace(((CodeValue)block.Value).Value, @"\t|\n|\r|\s", ""),
                        Text = resourceLocalization["Scripter_Assignation_Custom"] as string,
                        DeleteNodeCommand = deleteCommand
                    };
                    //Ignore childs even if there is some
                }
                //Otherwise, the child is a codeblock
                else if (block.Value is CodeBlock)
                {
                    AssignationModel newAssignation = new AssignationModel()
                    {
                        BackgroundColor = getColorArray(block.Assignee)[0],
                        BorderColor = getColorArray(block.Assignee)[1],
                        Color = getColorArray(block.Assignee)[2],
                        IsNotEditable = false,
                        CanHaveChild = true,
                        Code = Regex.Replace(block.Assignee, @"\t|\n|\r|\s", "") + 
                            " " + block.Operator + " {}",
                        Text = getAssignationName(block.Assignee),
                        DeleteNodeCommand = deleteCommand
                    };
                    foreach (ICodeStruct code in ((CodeBlock)block.Value).Code)
                    {
                        newAssignation.Childrens.Add(BlockToModel(code, deleteCommand));
                    }
                    return newAssignation;
                }
                else
                {
                    //An error in the code, log it
                    RecursiveCodeException e = new RecursiveCodeException();
                    throw e.AddToRecursiveChain("Invalid assignation", block.Assignee);
                }
            }
            return null;
        }
        
        private static string getAssignationName(string part)
        {
            part = Regex.Replace(part, @"\t|\n|\r", String.Empty);
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative);
            int index = Array.IndexOf(COMMON_CONTROLS, part);
            //If a common control
            if (index >= 0)
            {
                //Return its control name
                return resourceLocalization[COMMON_CONTROLS_NAMES[index]] as string;
            }
            else
            {
                //Otherwise, random control
                return resourceLocalization["Scripter_Block_Custom"] as string;
            }
        }

        private static Brush[] getColorArray(string part)
        {
            part = Regex.Replace(part, @"\t|\n|\r", String.Empty);
            int index = Array.IndexOf(COMMON_CONTROLS, part);
            //If a common control
            if (index >= 0 && index <= 4)
            {
                //Return condition brush
                return CONDITIONS_COLORS;
            }
            else
            {
                //Return block brush
                return BLOCKS_COLORS;
            }
        }

        public static Script TransformModelsToScript(List<AssignationModel> models)
        {
            Script newScript = new Script();
            foreach (AssignationModel assignation in models)
            {
                try
                {
                    newScript.Code.Add(ModelToBlock(assignation));
                }
                catch (RecursiveCodeException e)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Error when parsing the scripter controls to a script");
                    ErrorLogger.Instance.AddLogLine("\t" + e.Message);
                }
                catch (Exception)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Unknown error in script");
                    continue;
                }
            }
            return newScript;
        }

        /// <summary>
        /// Recursive method that loop inside the given model childrens an create the required block
        /// </summary>
        /// <param name="model">The model to loop into</param>
        /// <param name="level">The current level of the script</param>
        /// <returns>a well formed ICodeStruct made from the model's data.</returns>
        private static ICodeStruct ModelToBlock(AssignationModel model, int level = 0)
        {
            if (model.Childrens.Any())
            {
                Assignation newBlock;
                //If contains equals
                if (model.Code.Contains("="))
                {
                    //Empty code block, create as such
                    newBlock = new Assignation(level)
                    {
                        Assignee = model.Code.Split('=')[0].Trim(),
                        Operator = "=",
                        Value = new CodeBlock(level + 1)
                    };
                }
                //If contains lesser than, but also does not contains a block
                else if (model.Code.Contains("<") &&
                    (!model.Code.Contains("{") && !model.Code.Contains("}")))
                {
                    //If yes, assignation
                    return new Assignation(level)
                    {
                        Assignee = model.Code.Split('<')[0].Trim(),
                        Operator = "<",
                        Value = new CodeValue(model.Code.Split('<')[1].Trim())
                    };
                }
                //If contains bigger than, but also does not contains a block
                else if (model.Code.Contains(">") &&
                    (!model.Code.Contains("{") && !model.Code.Contains("}")))
                {
                    //If yes, assignation
                    return new Assignation(level)
                    {
                        Assignee = model.Code.Split('>')[0].Trim(),
                        Operator = ">",
                        Value = new CodeValue(model.Code.Split('>')[1].Trim())
                    };
                }
                else
                {
                    //Weird block, just return its code
                    newBlock = new Assignation(level)
                    {
                        Assignee = model.Code,
                        Operator = "=",
                        Value = new CodeBlock(level + 1)
                    };
                }
                foreach (AssignationModel child in model.Childrens)
                {
                    ((CodeBlock)newBlock.Value).Code.Add(ModelToBlock(child, level + 1));
                }
                return newBlock;
            }
            else
            {
                //Should be a code value, since there is no children
                //Check if it contains an equal, but no code block
                if (model.Code.Contains("=") && 
                    (!model.Code.Contains("{") && !model.Code.Contains("}")))
                {
                    //If yes, assignation
                    return new Assignation(level)
                    {
                        Assignee = model.Code.Split('=')[0].Trim(),
                        Operator = "=",
                        Value = new CodeValue(model.Code.Split('=')[1].Trim())
                    };
                }
                //If contains lesser than, but also does not contains a block
                else if (model.Code.Contains("<") &&
                    (!model.Code.Contains("{") && !model.Code.Contains("}")))
                {
                    //If yes, assignation
                    return new Assignation(level)
                    {
                        Assignee = model.Code.Split('<')[0].Trim(),
                        Operator = "<",
                        Value = new CodeValue(model.Code.Split('<')[1].Trim())
                    };
                }
                //If contains bigger than, but also does not contains a block
                else if (model.Code.Contains(">") &&
                    (!model.Code.Contains("{") && !model.Code.Contains("}")))
                {
                    //If yes, assignation
                    return new Assignation(level)
                    {
                        Assignee = model.Code.Split('>')[0].Trim(),
                        Operator = ">",
                        Value = new CodeValue(model.Code.Split('>')[1].Trim())
                    };
                }
                //If contains equals, but also contains a block
                else if (model.Code.Contains("="))
                {
                    //Empty code block, create as such
                    return new Assignation(level)
                    {
                        Assignee = model.Code.Split('=')[0].Trim()
                    };
                }
                else
                {
                    //Weird block, just return its code
                    return new Assignation(level)
                    {
                        Assignee = model.Code
                    };
                }
            }
        }
    }
}
