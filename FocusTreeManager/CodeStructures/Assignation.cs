using FocusTreeManager.CodeStructures.CodeExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace FocusTreeManager.CodeStructures
{

    [KnownType(typeof(Assignation)), 
     KnownType(typeof(CodeBlock)), 
     KnownType(typeof(CodeValue)), 
     DataContract(Name = "assignation")]
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

        internal void Analyse(SyntaxGroup code)
        {
            this.Assignee = code.Component.text;
            this.Operator = code.Operator.text;
            this.Line = code.Component.line;
            if (code.Operand is List<SyntaxGroup>)
            {
                try
                {
                    CodeBlock block = new CodeBlock(this.Level + 1);
                    block.Analyse(code.Operand as List<SyntaxGroup>);
                    this.Value = block;
                    return;
                }
                catch (RecursiveCodeException exception1)
                {
                    throw exception1.AddToRecursiveChain("Error during analysis chain", 
                        this.Assignee, this.Line.ToString());
                }
                catch (Exception)
                {
                    throw new RecursiveCodeException().
                        AddToRecursiveChain("Impossible to analyze associated code", 
                        this.Assignee, this.Line.ToString());
                }
            }
            if (code.Operand is Token)
            {
                this.Value = new CodeValue(((Token)code.Operand).text);
            }
            else if (code.Operand is List<Token>)
            {
                this.Value = new CodeBlock();
                ((CodeBlock)this.Value).Analyse(code.Operand as List<Token>);
            }
        }

        public ICodeStruct Extract(string TagToFind)
        {
            if (this.Assignee == TagToFind)
            {
                return this;
            }
            if (this.Value is CodeBlock)
            {
                ICodeStruct item = this.Value.FindAssignation(TagToFind);
                if (item != null)
                {
                    ((CodeBlock)this.Value).Code.Remove(item);
                    return item;
                }
            }
            return null;
        }

        public List<ICodeStruct> FindAllValuesOfType<T>(string TagToFind)
        {
            List<ICodeStruct> list = new List<ICodeStruct>();
            if (this.Value != null)
            {
                if ((this.Assignee == TagToFind) && ((this.Value.GetType() == typeof(T)) 
                    || typeof(T).IsAssignableFrom(this.Value.GetType())))
                {
                    list.Add(this.Value);
                    return list;
                }
                if (this.Value is CodeBlock)
                {
                    list.AddRange(this.Value.FindAllValuesOfType<T>(TagToFind));
                }
            }
            return list;
        }

        public Assignation FindAssignation(string TagToFind)
        {
            if (this.Assignee == TagToFind)
            {
                return this;
            }
            if (this.Value is CodeBlock)
            {
                Assignation assignation = this.Value.FindAssignation(TagToFind);
                if (assignation != null)
                {
                    return assignation;
                }
            }
            return null;
        }

        public CodeValue FindValue(string TagToFind)
        {
            if (this.Assignee == TagToFind)
            {
                return (this.Value as CodeValue);
            }
            if (this.Value is CodeBlock)
            {
                CodeValue value2 = this.Value.FindValue(TagToFind);
                if (value2 != null)
                {
                    return value2;
                }
            }
            return null;
        }

        public Script GetContentAsScript(string[] except)
        {
            Script script = new Script();
            if (!(this.Value is CodeBlock))
            {
                throw new RecursiveCodeException().
                    AddToRecursiveChain("Impossible to obtain content, assigned value is not code", 
                    this.Assignee, this.Line.ToString());
            }
            foreach (Assignation assignation in ((CodeBlock)this.Value).Code)
            {
                if (!except.Contains<string>(assignation.Assignee))
                {
                    script.Code.Add(assignation);
                }
            }
            return script;
        }

        public string Parse(int StartLevel = -1)
        {
            int startLevel = (StartLevel == -1) ? this.Level : (StartLevel + 1);
            string str = "";
            for (int i = 1; i < startLevel; i++)
            {
                str = str + "\t";
            }
            StringBuilder builder = new StringBuilder();
            try
            {
                if (((this.Value == null) || ((this.Value is CodeBlock) 
                    && !((CodeBlock)this.Value).Code.Any<ICodeStruct>())) 
                    && (this.Operator != null))
                {
                    builder.Append(str + this.Assignee + " " + this.Operator + " {\n\n}");
                }
                else
                {
                    builder.Append(str + this.Assignee + " " + this.Operator + " " +
                        this.Value.Parse(startLevel));
                }
            }
            catch (RecursiveCodeException exception1)
            {
                throw exception1.AddToRecursiveChain("Error during parsing chain", 
                    this.Assignee, this.Line.ToString());
            }
            catch (Exception)
            {
                throw new RecursiveCodeException()
                    .AddToRecursiveChain("Impossible to parse associated code", 
                    this.Assignee, this.Line.ToString());
            }
            return builder.ToString();
        }
    }
}