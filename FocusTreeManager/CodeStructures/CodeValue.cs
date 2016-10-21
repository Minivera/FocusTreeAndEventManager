using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FocusTreeManager.CodeStructures
{
    [ProtoContract(SkipConstructor = true)]
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

        public void Analyse(string code)
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

        public string Parse()
        {
            return Value;
        }
    }
}
