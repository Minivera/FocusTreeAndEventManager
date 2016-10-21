using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Model
{
    [ProtoContract]
    [ProtoInclude(1, typeof(MutuallyExclusiveSet))]
    [ProtoInclude(2, typeof(PrerequisitesSet))]
    public interface ISet
    {
        void DeleteSetRelations();
        void assertInternalFocus(IEnumerable<Focus> fociList);
    }
}
