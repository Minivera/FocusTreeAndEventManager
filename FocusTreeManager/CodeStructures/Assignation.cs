using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FocusTreeManager.CodeStructures
{
    public class Assignation : ICodeStruct
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
            //If we can detect at leats one code block
            if(!String.IsNullOrWhiteSpace(match.Groups[2].Value))
            {
                string text2 = match.Groups[2].Value;
                //Kill comments if possible
                text2 = Regex.Replace(text2, @"(#.*\n)", String.Empty);
                CodeBlock block = new CodeBlock(Level + 1);
                block.Analyse(text2);
                Value = block;
            }
            else if (!String.IsNullOrWhiteSpace(match.Groups[3].Value))
            {
                string text2 = match.Groups[3].Value;
                //Kill comments if possible
                text2 = Regex.Replace(text2, @"(#.*\n)", String.Empty);
                Value = new CodeValue(text2);
            }
            else
            {
                //Empty, kill
                return;
            }
            string text = match.Groups[1].Value;
            //Kill comments if possible
            text = Regex.Replace(text, @"(#.*\n)", String.Empty);
            Assignee = text;
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

        public ICodeStruct FindExternal(string TagToFind)
        {
            if (Regex.Replace(Assignee, @"\t|\n|\r|\s", "") == TagToFind)
            {
                return this;
            }
            ICodeStruct found;
            if (Value is CodeBlock)
            {
                found = Value.FindExternal(TagToFind);
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
