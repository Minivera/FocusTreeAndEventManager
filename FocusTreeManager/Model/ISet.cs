using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Model
{
    [ProtoContract]
    public interface ISet
    {
        void DeleteSetRelations();
        void assertInternalFocus(IEnumerable<Focus> fociList);
    }
}
