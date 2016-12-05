using ProtoBuf;
using System.Collections.Generic;

namespace FocusTreeManager.Model.LegacySerialization
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
