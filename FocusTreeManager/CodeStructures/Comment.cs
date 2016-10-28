using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.CodeStructures
{
    [ProtoContract(SkipConstructor = true)]
    public class Comment : ICodeStruct
    {
        public Comment()
        { }

        public void Analyse(string Code)
        {
            throw new NotImplementedException();
        }

        public ICodeStruct FindValue(string TagToFind)
        {
            throw new NotImplementedException();
        }

        public ICodeStruct FindAssignation(string TagToFind)
        {
            throw new NotImplementedException();
        }

        public List<ICodeStruct> FindAllValuesOfType<T>(string TagToFind)
        {
            throw new NotImplementedException();
        }

        public string Parse(int StartLevel = -1)
        {
            throw new NotImplementedException();
        }
    }
}
