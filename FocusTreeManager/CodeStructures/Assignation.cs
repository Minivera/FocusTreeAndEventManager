using FocusTreeManager.CodeStructures.CodeExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace FocusTreeManager.CodeStructures
{
    [KnownType(typeof(Assignation))]
    [KnownType(typeof(CodeBlock))]
    [KnownType(typeof(CodeValue))]
    [DataContract(Name = "assignation")]
    public class Assignation : ICodeStruct
    {
        [DataMember(Name = "assignee", Order = 0)]
        public string Assignee { get; set; }

        [DataMember(Name = "value", Order = 1)]
        public ICodeStruct Value { get; set; }

        [DataMember(Name = "level", Order = 2)]
        private int Level;

        [DataMember(Name = "operator", Order = 3)]
        public string Operator { get; set; }

        [DataMember(Name = "line", Order = 4)]
        public int Line { get; set; }

        public Assignation()
        {
            this.Level = 0;
        }

        public Assignation(int level)
        {
            this.Level = level;
        }

        public void Analyse(string code, int line = -1)
        {
            Regex regex = new Regex(Script.REGEX_BRACKETS);
            Match match = regex.Match(code);
            string text = match.Groups[1].Value;
            //Kill comments if possible
            text = Regex.Replace(text, @"(#.*)", String.Empty);
            Assignee = Regex.Replace(text, @"\t|\n|\r|\s", "");
            Operator = Regex.Replace(match.Groups[2].Value, @"\t|\n|\r|\s", "");
            Line = line;
            //If we can detect at least one code block
            if (!String.IsNullOrWhiteSpace(match.Groups[3].Value))
            {
                try
                {
                    string text2 = match.Groups[3].Value;
                    //Kill comments if possible
                    text2 = Regex.Replace(text2, @"(#.*)", String.Empty);
                    CodeBlock block = new CodeBlock(Level + 1);
                    block.Analyse(text2, Line);
                    Value = block;
                }
                catch (RecursiveCodeException e)
                {
                    //TODO: Add language support
                    throw e.AddToRecursiveChain("Error during analysis chain", Assignee, Line.ToString());
                }
                catch (Exception)
                {
                    //TODO: Add language support
                    RecursiveCodeException e = new RecursiveCodeException();
                    throw e.AddToRecursiveChain("Impossible to analyse associated code", Assignee, Line.ToString());
                }
            }
            else if (!String.IsNullOrWhiteSpace(match.Groups[4].Value))
            {
                string text2 = match.Groups[4].Value;
                //Kill comments if possible
                text2 = Regex.Replace(text2, @"(#.*)", String.Empty);
                Value = new CodeValue(text2);
            }
            else
            {
                //Empty, kill
                return;
            }
        }

        public string Parse(int StartLevel = -1)
        {
            int BasicLevel = StartLevel == -1 ? Level : StartLevel + 1;
            string tabulations = "";
            for (int i = 1; i < BasicLevel; i++)
            {
                tabulations += "\t";
            }
            StringBuilder content = new StringBuilder();
            try
            {
                // If the value is nothing but it has an operator
                if (Value == null && Operator != null)
                {
                    //Empty block
                    content.Append(tabulations + Assignee + " " + Operator + " {\n\n}");
                }
                //Otherwise, print as usual
                else
                {
                    content.Append(tabulations + Assignee + " " + Operator + " " + Value.Parse(BasicLevel));
                }
            }
            catch (RecursiveCodeException e)
            {
                //TODO: Add language support
                throw e.AddToRecursiveChain("Error during parsing chain", Assignee, Line.ToString());
            }
            catch (Exception)
            {
                //TODO: Add language support
                RecursiveCodeException e = new RecursiveCodeException();
                throw e.AddToRecursiveChain("Impossible to parse associated code", Assignee, Line.ToString());
            }
            return content.ToString();
        }

        public ICodeStruct FindValue(string TagToFind)
        {
            if (Assignee == TagToFind)
            {
                return Value;
            }
            ICodeStruct found;
            if (Value is CodeBlock)
            {
                found = Value.FindValue(TagToFind);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public ICodeStruct FindAssignation(string TagToFind)
        {
            if (Assignee == TagToFind)
            {
                return this;
            }
            ICodeStruct found;
            if (Value is CodeBlock)
            {
                found = Value.FindAssignation(TagToFind);
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
            if (Value == null)
            {
                //Empty block (Paradox loves these....)
                return founds;
            }
            if (Assignee == TagToFind 
                && (Value.GetType() == typeof(T) ||
                typeof(T).IsAssignableFrom(Value.GetType())))
            {
                founds.Add(Value);
                return founds;
            }
            //If we haven't found this element as our tag, search in childs
            if (Value is CodeBlock)
            {
                founds.AddRange(Value.FindAllValuesOfType<T>(TagToFind));
            }
            return founds;
        }

        public Script GetContentAsScript(string[] except)
        {
            Script newScript = new Script();
            if (Value is CodeBlock)
            {
                foreach (Assignation item in ((CodeBlock)Value).Code)
                {
                    if (!except.Contains(item.Assignee))
                    {
                        newScript.Code.Add(item);
                    }
                }
            }
            else
            {
                //TODO: Add language support
                RecursiveCodeException e = new RecursiveCodeException();
                throw e.AddToRecursiveChain("Impossible to obtain content, assigned value is not code", 
                                             Assignee, Line.ToString());
            }
            return newScript;
        }
    }
}
