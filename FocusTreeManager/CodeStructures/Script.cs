using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FocusTreeManager.CodeStructures
{
    /// <summary>
    /// Hold the whole script in any text file, a text file contains one or multiple assignations.
    /// </summary>
    class Script : ICodeStruct
    {
        /// <summary>
        /// Regex that check for eathier an assignation with a code block (text = {...})
        /// or a simple assignation (text = text)
        /// </summary>
        internal const string REGEX_BRACKETS = @"([^=]+)[ \t]+=[ \t]+(?:\{((?:[^\{\}]|(?<o>\{)|(?<-o>\}))+(?(o)(?!)))\}|(.*))";

        internal const string REGEX_INLINE_ASSIGNATIONS = @"[^=\{]+[ \t]+=[ \t]+[^=\s\}]+";

        public List<ICodeStruct> Code { get; set; }

        public Script()
        {
            Code = new List<ICodeStruct>();
        }

        public void Analyse(string code)
        {
            Code.Clear();
            Regex regex = new Regex(REGEX_BRACKETS);
            //For each block of text = brackets
            foreach (Match ItemMatch in regex.Matches(code))
            {
                Assignation tempo = new Assignation(1);
                tempo.Analyse(ItemMatch.Value);
                Code.Add(tempo);
            }
        }

        public string Parse()
        {
            string content = "";
            foreach (ICodeStruct item in Code)
            {
                content += item.Parse();
            }
            return content;
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
