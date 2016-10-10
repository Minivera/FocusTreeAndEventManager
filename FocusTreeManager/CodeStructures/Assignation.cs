using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FocusTreeManager.CodeStructures
{
    class Assignation : ICodeStruct
    {
        public string Assignee { get; set; }

        public ICodeStruct Value { get; set; }

        private int Level;

        public Assignation(int level)
        {
            this.Level = level;
        }

        public void Analyse(string code)
        {
            Regex regex = new Regex(Script.REGEX_BRACKETS);
            Match match = regex.Match(code);
            Assignee = match.Groups[1].Value;
            //If we can detect at leats one code block
            if(match.Groups[2].Length > 0)
            {
                CodeBlock block = new CodeBlock(Level + 1);
                block.Analyse(match.Groups[2].Value);
                Value = block;
            }
            else
            {
                //Trim remaining brackets
                Value = new CodeValue(match.Groups[3].Value);
            }
        }

        public string Parse()
        {
            string tabulations = "";
            for (int i = 1; i < Level; i++)
            {
                tabulations += "\t";
            }
            StringBuilder content = new StringBuilder();
            content.Append(tabulations + Assignee + " = " + Value.Parse());
            return content.ToString();
        }

        public ICodeStruct Find(string TagToFind)
        {
            if (Regex.Replace(Assignee, @"\t|\n|\r|\s", "") == TagToFind)
            {
                return Value;
            }
            ICodeStruct found;
            if (Value is CodeBlock)
            {
                found = Value.Find(TagToFind);
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
            if (Regex.Replace(Assignee, @"\t|\n|\r|\s", "") == TagToFind 
                && (Value.GetType() == typeof(T) ||
                typeof(T).IsAssignableFrom(Value.GetType())))
            {
                founds.Add(Value);
                return founds;
            }
            //If we haven't found this element as our tag, search in childs
            if (Value is CodeBlock)
            {
                founds.AddRange(Value.FindAll<T>(TagToFind));
            }
            return founds;
        }
    }
}
