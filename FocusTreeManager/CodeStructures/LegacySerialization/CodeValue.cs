using FocusTreeManager.CodeStructures.CodeExceptions;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FocusTreeManager.CodeStructures.LegacySerialization
{
    [ProtoContract]
    public class CodeValue : ICodeStruct
    {
        [ProtoMember(1)]
        public string Value { get; set; }

        public CodeValue()
        { }

        public CodeValue(string value)
        {
            this.Value = Regex.Replace(value, @"\t|\n|\r|\s", "");
        }

        public void Analyse(string code, int line = -1)
        {
            //Can't analyse
            return;
        }

        public ICodeStruct FindValue(string TagToFind)
        {
            //End of structure
            return null;
        }

        public ICodeStruct FindAssignation(string TagToFind)
        {
            //End of structure
            return null;
        }

        public List<ICodeStruct> FindAllValuesOfType<T>(string TagToFind)
        {
            //End of structure
            return new List<ICodeStruct>();
        }

        public string Parse(int StartLevel = -1)
        {
            return Value;
        }

        public Script GetContentAsScript(string[] except)
        {
            //TODO: Add language support
            RecursiveCodeException e = new RecursiveCodeException();
            throw e.AddToRecursiveChain("Impossible to obtain content, value has not code",
                                            Value, "-1");
        }
    }
}
