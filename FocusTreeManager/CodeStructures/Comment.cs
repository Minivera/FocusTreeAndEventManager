using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.CodeStructures
{
    class Comment : ICodeStruct
    {
        public void Analyse(string Code)
        {
            throw new NotImplementedException();
        }

        public ICodeStruct Find(string TagToFind)
        {
            throw new NotImplementedException();
        }

        public List<ICodeStruct> FindAll<T>(string TagToFind)
        {
            throw new NotImplementedException();
        }

        public string Parse()
        {
            throw new NotImplementedException();
        }
    }
}
