using FocusTreeManager.CodeStructures.CodeExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace FocusTreeManager.CodeStructures
{
    /// <summary>
    /// Hold the whole script in any text file, a text file contains one or multiple assignations.
    /// </summary>
    [KnownType(typeof(Assignation))]
    [KnownType(typeof(CodeBlock))]
    [KnownType(typeof(CodeValue))]
    [DataContract(Name = "script_struct")]
    public class Script : ICodeStruct
    {
        [DataMember(Name = "code", Order = 0)]
        public List<ICodeStruct> Code { get; set; }

        public Script()
        {
            Code = new List<ICodeStruct>();
        }

        public void Analyse(string code, int line = -1)
        {
            Code.Clear();
            List<SyntaxGroup> list = 
                Tokenizer.GroupTokensByBlocks(Tokenizer.Tokenize(code)) 
                as List<SyntaxGroup>;
            foreach (SyntaxGroup group in list)
            {
                try
                {
                    Assignation tempo = new Assignation(1);
                    tempo.Analyse(group);
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

        public string Parse(int StartLevel = -1)
        {
            string content = "";
            if (Code == null)
            {
                return content;
            }
            foreach (ICodeStruct item in Code)
            {
                try
                {
                    content += item.Parse(StartLevel) + "\n";
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

        public CodeValue FindValue(string TagToFind)
        {
            CodeValue found;
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

        public Assignation FindAssignation(string TagToFind)
        {
            Assignation found;
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

        static public string TryParse(ICodeStruct block, string tag)
        {
            Assignation found = block.FindAssignation(tag);
            if (found != null)
            {
                try
                {
                    return found.Value.Parse();
                }
                catch (Exception)
                {
                    throw new SyntaxException(tag, found.Line);
                }
            }
            else
            {
                throw new SyntaxException(tag);
            }
        }
    }
}
