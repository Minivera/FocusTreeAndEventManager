using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FocusTreeManager.CodeStructures.CodeExceptions;

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
            Level = 0;
            Code = new List<ICodeStruct>();
        }

        public CodeBlock(int level)
        {
            Level = level;
            Code = new List<ICodeStruct>();
        }

        internal RecursiveCodeLog Analyse(List<SyntaxGroup> code)
        {
            if (!code.Any()) return null;
            foreach (SyntaxGroup group in code)
            {
                //Check if there is an assignation or code value at the end of the code
                Assignation last = Code.LastOrDefault() as Assignation;
                if (last != null)
                {
                    //set its end line as this line's end - 1
                    last.EndLine = group.Component.line - 1;
                }
                Assignation tempo = new Assignation(Level + 1);
                RecursiveCodeLog log = tempo.Analyse(group);
                if (log != null)
                {
                    return log;
                }
                //If tempo has a value
                if (!string.IsNullOrEmpty(tempo.Assignee))
                {
                    Code.Add(tempo);
                }
            }
            //Check if there is an assignation or code value at the end of the code
            Assignation Verylast = Code.LastOrDefault() as Assignation;
            if (Verylast != null)
            {
                //set its end line as this line's end - 1
                Verylast.EndLine = code.Last().Component.line - 1;
            }
            return null;
        }

        internal RecursiveCodeLog Analyse(List<Token> code)
        {
            //Cut the code in parts and add them as code values
            foreach (Token group in code)
            {
                Code.Add(new CodeValue(group.text));
            }
            return null;
        }

        public string Parse(Dictionary<int, string> Comments = null, int StartLevel = -1)
        {
            if (!Code.Any())
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
                    content.Append("\n" + item.Parse(Comments, BasicLevel));
                }
                else if (item is CodeValue)
                {
                    //Parse all value spaced by one space
                    content.Append(" " + item.Parse(Comments, BasicLevel) + " ");
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

        public CodeValue FindValue(string TagToFind)
        {
            //Find the first occurrence or return null
            return Code.Select(item => item.FindValue(TagToFind)).
                FirstOrDefault(found => found != null);
        }

        public ICodeStruct Extract(string TagToFind)
        {
            foreach (ICodeStruct item in Code)
            {
                ICodeStruct found = item.Extract(TagToFind);
                //If it is not found, continue the loop
                if (found == null) continue;
                //Otherwise, extract
                if (Code.Contains(found))
                {
                    Code.Remove(found);
                }
                return found;
            }
            return null;
        }

        public Assignation FindAssignation(string TagToFind)
        {
            //Return the first found value or no value
            return Code.Select(item => item.FindAssignation(TagToFind)).
                FirstOrDefault(found => found != null);
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

        public Script GetContentAsScript(string[] except,
            Dictionary<int, string> Comments = null)
        {
            Script newScript = new Script();
            foreach (ICodeStruct codeStruct in Code)
            {
                Assignation item = (Assignation) codeStruct;
                if (!except.Contains(item.Assignee))
                {
                    newScript.Code.Add(item);
                }
            }
            //If no comments are given
            if (Comments == null) return newScript;
            Assignation firstLine = newScript.Code.Where(i => i is Assignation)
                .OrderBy(i => ((Assignation)i).Line).FirstOrDefault() as Assignation;
            Assignation lastLine = newScript.Code.Where(i => i is Assignation)
                .OrderBy(i => ((Assignation)i).Line).LastOrDefault() as Assignation;
            //If no lines were found
            if (firstLine == null || lastLine == null) return newScript;
            //Try to get the comments
            newScript.Comments = Comments.SkipWhile(c => c.Key < firstLine.Line)
                                         .TakeWhile(c => c.Key <= lastLine.Line)
                                         .ToDictionary(c => c.Key, c => c.Value);
            return newScript;
        }
    }
}
