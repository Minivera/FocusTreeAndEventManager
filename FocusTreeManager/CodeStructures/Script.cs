using FocusTreeManager.CodeStructures.CodeExceptions;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FocusTreeManager.CodeStructures
{
    /// <summary>
    /// Hold the whole script in any text file, a text file contains one or multiple assignations.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(500, typeof(ICodeStruct))]
    public class Script : ICodeStruct
    {
        /// <summary>
        /// Regex that check for either an assignation with a code block (text = {...})
        /// or a simple assignation (text = text)
        /// </summary>
        internal const string REGEX_BRACKETS =
            @"([^=<>]+)[ \t]+([=<>])[ \t]+(?:\{((?:[^\{\}]|(?<o>\{)|(?<-o>\}))+(?(o)(?!)))\}|(.*))";

        //Check for inline assignation (Paradox loves these)
        internal const string REGEX_INLINE_ASSIGNATIONS = @"[^=<>\{]+[ \t]+[=<>][ \t]+[^=<>\s\}]+";

        [ProtoMember(1, AsReference = true)]
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
                try
                {
                    Assignation tempo = new Assignation(1);
                    tempo.Analyse(ItemMatch.Value);
                    Code.Add(tempo);
                }
                catch (RecursiveCodeException e)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Error during script Loading");
                    ErrorLogger.Instance.AddLogLine("\t" + e.Message);
                }
                catch (Exception)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Unknown error in script");
                    continue;
                }
            }
        }

        public string Parse()
        {
            string content = "";
            foreach (ICodeStruct item in Code)
            {
                try
                {
                    content += item.Parse();
                }
                catch (RecursiveCodeException e)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Error during script Parsing");
                    ErrorLogger.Instance.AddLogLine("\t" + e.Message);
                }
                catch (Exception)
                {
                    //TODO: Add language support
                    ErrorLogger.Instance.AddLogLine("Unknown error in script");
                    continue;
                }
            }
            return content;
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
    }
}
