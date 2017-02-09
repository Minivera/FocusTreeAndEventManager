using System.Collections.Generic;

namespace FocusTreeManager.CodeStructures
{
    public interface ICodeStruct
    {
        string Parse(int StartLevel = -1);
        void Analyse(string code, int Line = -1);
        ICodeStruct FindValue(string TagToFind);
        ICodeStruct Extract(string TagToFind);
        ICodeStruct FindAssignation(string TagToFind);
        List<ICodeStruct> FindAllValuesOfType<T>(string TagToFind);
        Script GetContentAsScript(string[] except);
    }
}
