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
            string str = "";
            if (this.Code != null)
            {
                foreach (ICodeStruct struct2 in this.Code)
                {
                    try
                    {
                        str = str + struct2.Parse(StartLevel) + "\n";
                    }
                    catch (RecursiveCodeException exception)
                    {
                        ErrorLogger.Instance.AddLogLine("Error during script Parsing");
                        ErrorLogger.Instance.AddLogLine("\t" + exception.Message);
                    }
                    catch (Exception)
                    {
                        ErrorLogger.Instance.AddLogLine("Unknown error in script");
                    }
                }
            }
            return str;
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

        public static string TryParse(ICodeStruct block, string tag, bool isMandatory = true)
        {
            Assignation assignation = block.FindAssignation(tag);
            if (assignation != null)
            {
                try
                {
                    return assignation.Value.Parse(-1);
                }
                catch (Exception)
                {
                    throw new SyntaxException(tag, new int?(assignation.Line), null);
                }
            }
            if (isMandatory)
            {
                int? line = null;
                throw new SyntaxException(tag, line, null);
            }
            return null;
        }
    }
}
