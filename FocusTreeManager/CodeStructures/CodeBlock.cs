using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace FocusTreeManager.CodeStructures
{
    /// <summary>
    /// Code block containing one or multiple assignations. It contains everything
    /// that is between one level of brackets.
    /// </summary>
    [KnownType(typeof(Assignation))]
    [KnownType(typeof(CodeBlock))]
    [KnownType(typeof(CodeValue))]
    [DataContract(Name = "code_block")]
    public class CodeBlock : ICodeStruct
    {
        [DataMember(Name = "level", Order = 0)]
        private int Level;
        
        [DataMember(Name = "code", Order = 1)]
        public List<ICodeStruct> Code { get; set; }

        public CodeBlock()
        {
            this.Level = 0;
            Code = new List<ICodeStruct>();
        }

        public CodeBlock(int level)
        {
            this.Level = level;
            Code = new List<ICodeStruct>();
        }

        public void Analyse(string code, int line = -1)
        {
            Regex regex = new Regex(Script.REGEX_BRACKETS);
            //For each block of text = brackets
            foreach (Match ItemMatch in regex.Matches(code))
            {
                int firstIndex = ItemMatch.Value.IndexOf('=');
                int lastIndex = ItemMatch.Value.LastIndexOf('=');
                //If it was an inline with more than one assignations
                if (firstIndex >= 0 && firstIndex != lastIndex &&
                    ItemMatch.Value.IndexOf('\n') <= 0 &&
                    ItemMatch.Value.IndexOf('{') <= 0)
                {
                    //We got more than one equal in one line and there was
                    //no brackets, it is a inline multi assignation
                    Regex regex2 = new Regex(Script.REGEX_INLINE_ASSIGNATIONS);
                    foreach (Match inlines in regex2.Matches(ItemMatch.Value))
                    {
                        Assignation tempo = new Assignation(Level + 1);
                        tempo.Analyse(inlines.Value, line);
                        //If tempo has a value
                        if (!String.IsNullOrEmpty(tempo.Assignee))
                        {
                            Code.Add(tempo);
                        }
                    }
                } 
                else
                {
                    Assignation tempo = new Assignation(Level + 1);
                    tempo.Analyse(ItemMatch.Value, line + 1);
                    //If tempo has a value
                    if (!String.IsNullOrEmpty(tempo.Assignee))
                    {
                        Code.Add(tempo);
                    }
                }
                line += ItemMatch.Value.Count(s => s == '\n');
            }
            //Test if code is empty, but the text is not
            if (!Code.Any() && !string.IsNullOrWhiteSpace(code))
            {
                //Cut the code in parts and add them as code values
                foreach (string item in code.Split(new string[] { " ", "\r\n", "\n" }, StringSplitOptions.None))
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        Code.Add(new CodeValue(item));
                    }
                }
            }
        }

        public string Parse(int StartLevel = -1)
        {
            if(Code.Count == 0)
            {
                return "";
            }
            int BasicLevel = StartLevel == -1 ? Level : StartLevel + 1;
            StringBuilder content = new StringBuilder();
            //Get the right amount of tabulations for the level
            string tabulations = "";
            for (int i = 1; i < BasicLevel; i++)
            {
                tabulations += "\t";
            }
            content.Append("{");
            foreach (ICodeStruct item in Code)
            {
                if (item is Assignation)
                {
                    //Parse each internal assignations
                    content.Append("\n" + item.Parse(BasicLevel));
                }
                else if (item is CodeValue)
                {
                    //Parse all value spaced by one space
                    content.Append(" " + item.Parse(BasicLevel) + " ");
                }
            }
            if (Code.Last() is Assignation)
            {
                //Add the last breakline and tabulations if needed
                content.Append("\n" + tabulations);
            }
            content.Append("}");
            return content.ToString();
        }

        public ICodeStruct FindValue(string TagToFind)
        {
            ICodeStruct found;
            foreach (ICodeStruct item in Code)
            {
                found = item.FindValue(TagToFind);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public ICodeStruct Extract(string TagToFind)
        {
            ICodeStruct found;
            foreach (ICodeStruct item in Code)
            {
                found = item.FindAssignation(TagToFind);
                if (found != null)
                {
                    ((CodeBlock)item).Code.Remove(found);
                    return found;
                }
            }
            return null;
        }

        public ICodeStruct FindAssignation(string TagToFind)
        {
            ICodeStruct found;
            foreach (ICodeStruct item in Code)
            {
                found = item.FindAssignation(TagToFind);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public List<ICodeStruct> FindAllValuesOfType<T>(string TagToFind)
        {
            List<ICodeStruct> founds = new List<ICodeStruct>();
            foreach (ICodeStruct item in Code)
            {
                founds.AddRange(item.FindAllValuesOfType<T>(TagToFind));
            }
            return founds;
        }

        public Script GetContentAsScript(string[] except)
        {
            Script newScript = new Script();
            foreach (Assignation item in Code)
            {
                if (!except.Contains(item.Assignee))
                {
                    newScript.Code.Add(item);
                }
            }
            return newScript;
        }
    }
}
