using FocusTreeManager.CodeStructures.CodeExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        [DataMember(Name = "endline", Order = 5)]
        public int EndLine { get; set; }

        public Assignation()
        {
            Level = 0;
        }

        public Assignation(int level)
        {
            Level = level;
        }

        internal RecursiveCodeLog Analyse(SyntaxGroup code)
        {
            Assignee = code.Component.text;
            Operator = code.Operator.text;
            Line = code.Component.line;
            //If we can detect at least one code block
            List<SyntaxGroup> list = code.Operand as List<SyntaxGroup>;
            if (list != null)
            {
                try
                {
                    CodeBlock block = new CodeBlock(Level + 1);
                    RecursiveCodeLog log = block.Analyse(list);
                    if (log != null)
                    {
                        log.AddToRecursiveChain("Error during analysis chain",
                            Assignee, Line.ToString());
                        return log;
                    }
                    Value = block;
                }
                catch (Exception)
                {
                    //TODO: Add language support
                    RecursiveCodeLog log = new RecursiveCodeLog();
                    log.AddToRecursiveChain("Impossible to analyze associated code",
                        Assignee, Line.ToString());
                    return log; 
                }
            }
            //If we get pure text
            else if (code.Operand is Token)
            {
                Value = new CodeValue(((Token)code.Operand).text);
            }
            //If we got a list of tokens, a chain of pure text
            else if (code.Operand is List<Token>)
            {
                Value = new CodeBlock();
                ((CodeBlock)Value).Analyse((List<Token>)code.Operand);
            }
            return null;
        }

        public string Parse(Dictionary<int, string> comments = null, int StartLevel = -1)
        {
            int BasicLevel = StartLevel == -1 ? Level : StartLevel + 1;
            string tabulations = "";
            for (int i = 1; i < BasicLevel; i++)
            {
                tabulations += "\t";
            }
            StringBuilder content = new StringBuilder();
            string endComment = "";
            //Check if comments is not null
            if (comments != null)
            {
                //Make a copy for the loop
                Dictionary<int, string> localCopy = comments.ToDictionary(t => t.Key, t => t.Value);
                //Parse all comments before going further
                foreach (KeyValuePair<int, string> comment in localCopy.TakeWhile(i => i.Key < Line))
                {
                    content.Append(tabulations + comment.Value + "\n");
                    comments.Remove(comment.Key);
                }
                //Get this lines comments if any
                endComment = comments.ContainsKey(Line) ? comments[Line] : "";
                //Remove all added
                comments.Remove(Line);
            }
            // If the value is nothing but it has an operator
            if ((Value == null || Value is CodeBlock
                && !((CodeBlock)Value).Code.Any())
                && Operator != null)
            {
                //Check if there are comments in there
                string comment = "";
                if (comments != null)
                {
                    comment = comments.Where(c => c.Key > Line && c.Key < EndLine)
                        .Aggregate(comment, (current, item) => tabulations + current + item.Value);
                }
                //Empty block
                content.Append(tabulations + Assignee + " " + Operator + " {\n" + 
                    comment + "\n}" + endComment);
            }
            //Otherwise, print as usual
            else if (Value != null)
            {
                content.Append(tabulations + Assignee + " " + Operator + " " + 
                    Value.Parse(comments, BasicLevel) + endComment);
            }
            return content.ToString();
        }

        public CodeValue FindValue(string TagToFind)
        {
            if (Assignee == TagToFind)
            {
                return Value as CodeValue;
            }
            //If we can,t run through the value, return unfound
            if (!(Value is CodeBlock)) return null;
            CodeValue found = Value.FindValue(TagToFind);
            return found;
        }

        public ICodeStruct Extract(string TagToFind)
        {
            if (Assignee == TagToFind)
            {
                return this;
            }
            //If value cannot be ran through, return unfound
            if (!(Value is CodeBlock)) return null;
            ICodeStruct found = Value.Extract(TagToFind);
            //Return what was found, cannot extract from an assignation, 
            //should extract in parent container
            return found;
        }

        public Assignation FindAssignation(string TagToFind)
        {
            if (Assignee == TagToFind)
            {
                return this;
            }
            //If value cannot be ran through, return unfound.
            if (!(Value is CodeBlock)) return null;
            Assignation found = Value.FindAssignation(TagToFind);
            return found;
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
                Value is T))
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

        public Script GetContentAsScript(string[] except, Dictionary<int, string> Comments = null)
        {
            Script newScript = new Script();
            if (Value is CodeBlock)
            {
                foreach (ICodeStruct codeStruct in ((CodeBlock)Value).Code)
                {
                    Assignation item = (Assignation)codeStruct;
                    if (!except.Contains(item.Assignee))
                    {
                        newScript.Code.Add(item);
                    }
                }
            }
            else
            {
                //TODO: Add language support
                RecursiveCodeLog log = new RecursiveCodeLog();
                log.AddToRecursiveChain("Impossible to obtain content, assigned value is not code", 
                                             Assignee, Line.ToString());
                newScript.Logger.Errors.Add(new SyntaxError(log.Message));
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
