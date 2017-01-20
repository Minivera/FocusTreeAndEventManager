using FocusTreeManager.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Focus))]
    [DataContract(Name = "prerequisite", Namespace = "focusesNs")]
    public class PrerequisitesSet
    {
        [DataMember(Name = "focus", Order = 0)]
        public Focus Focus { get; set; }
        
        [DataMember(Name = "linked_foci", Order = 1)]
        public List<Focus> FociList { get; set; }

        public PrerequisitesSet(Focus focus)
        {
            this.Focus = focus;
            FociList = new List<Focus>();
        }

        public bool isRequired()
        {
            return (FociList.Count > 1);
        }
    }
}
