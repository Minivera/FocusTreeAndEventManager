using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.CodeStructures.LegacySerialization
{
    [ProtoContract]
    [ProtoInclude(1, typeof(Assignation))]
    [ProtoInclude(2, typeof(CodeBlock))]
    [ProtoInclude(3, typeof(CodeValue))]
    public interface ICodeStruct
    {
        string Parse(int StartLevel = -1);
        void Analyse(string code, int Line = -1);
        ICodeStruct FindValue(string TagToFind);
        ICodeStruct FindAssignation(string TagToFind);
        List<ICodeStruct> FindAllValuesOfType<T>(string TagToFind);
        Script GetContentAsScript(string[] except);
    }
}
