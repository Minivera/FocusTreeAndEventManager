using System.Collections.Generic;

namespace FocusTreeManager.CodeStructures
{
    public interface ICodeStruct
    {
        string Parse(Dictionary<int, string> comments = null, int StartLevel = -1);
        CodeValue FindValue(string TagToFind);
        ICodeStruct Extract(string TagToFind);
        Assignation FindAssignation(string TagToFind);
        List<ICodeStruct> FindAllValuesOfType<T>(string TagToFind);
        Script GetContentAsScript(string[] except, Dictionary<int, string> comments = null);
    }
}
