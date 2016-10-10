using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.CodeStructures
{
    interface ICodeStruct
    {
        string Parse();
        void Analyse(string code);
        ICodeStruct Find(string TagToFind);
        List<ICodeStruct> FindAll<T>(string TagToFind);
    }
}
