using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FocusTreeManager.CodeStructures
{
    /// <summary>
    /// Code block containing one or multiple assignations. It contains everything
    /// that is between one level of brackets.
    /// </summary>
    class CodeBlock : ICodeStruct
    {
        private int Level;

        public List<ICodeStruct> Code { get; set; }

        public CodeBlock(int level)
        {
            this.Level = level;
            Code = new List<ICodeStruct>();
        }

        public void Analyse(string code)
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
                        tempo.Analyse(inlines.Value);
                        Code.Add(tempo);
                    }
                } 
                else
                {
                    Assignation tempo = new Assignation(Level + 1);
                    tempo.Analyse(ItemMatch.Value);
                    Code.Add(tempo);
                }
            }
        }

        public string Parse()
        {
            StringBuilder content = new StringBuilder();
            if (Code.Count == 1)
            {
                content.Append("{ " + Code.FirstOrDefault().Parse() + " }");
            }
            else if(Code.Count > 1)
            {
                //Get the right amount of tabulations for the level
                string tabulations = "";
                for (int i = 1; i < Level; i++)
                {
                    tabulations += "\t";
                }
                content.Append("{");
                foreach (Assignation item in Code)
                {
                    //Parse each internal assignations
                    content.AppendLine(item.Parse());
                }
                content.AppendLine(tabulations + "}");
            }
            return content.ToString();
        }

        public ICodeStruct Find(string TagToFind)
        {
            ICodeStruct found;
            foreach (ICodeStruct item in Code)
            {
                found = item.Find(TagToFind);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public List<ICodeStruct> FindAll<T>(string TagToFind)
        {
            List<ICodeStruct> founds = new List<ICodeStruct>();
            foreach (ICodeStruct item in Code)
            {
                founds.AddRange(item.FindAll<T>(TagToFind));
            }
            return founds;
        }
    }
}
