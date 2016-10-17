using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FocusTreeManager.CodeStructures
{
    public class CodeValue : ICodeStruct
    {
        public string Value { get; set; }

        public CodeValue(string value)
        {
            this.Value = value;
        }

        public void Analyse(string code)
        {
            //Can't analyse
            return;
        }

        public ICodeStruct Find(string TagToFind)
        {
            //End of structure
            return null;
        }

        public ICodeStruct FindExternal(string TagToFind)
        {
            //End of structure
            return null;
        }

        public List<ICodeStruct> FindAll<T>(string TagToFind)
        {
            //End of structure
            return new List<ICodeStruct>();
        }

        public string Parse()
        {
            return Regex.Replace(Value, @"\t|\n|\r|\s", "");
        }
    }
}
