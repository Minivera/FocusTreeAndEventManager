using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Level = 0;
            Code = new List<ICodeStruct>();
        }

        public CodeBlock(int level)
        {
            Level = level;
            Code = new List<ICodeStruct>();
        }

        internal void Analyse(List<SyntaxGroup> code)
        {
            foreach (SyntaxGroup group in code)
            {
                Assignation tempo = new Assignation(Level + 1);
                tempo.Analyse(group);
                //If tempo has a value
                if (!string.IsNullOrEmpty(tempo.Assignee))
                {
                    Code.Add(tempo);
                }
            }
        }

        internal void Analyse(List<Token> code)
        {
            //Cut the code in parts and add them as code values
            foreach (Token group in code)
            {
                Code.Add(new CodeValue(group.text));
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

        public Script GetContentAsScript(string[] except)
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
            return newScript;
        }
    }
}
