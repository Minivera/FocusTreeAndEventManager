using FocusTreeManager.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Focus))]
    [DataContract(Name = "prerequisite", Namespace = "focusesNs")]
    public class PrerequisitesSet : ISet
    {
        [DataMember(Name = "focus", Order = 0)]
        public Focus Focus { get; set; }
        
        [DataMember(Name = "linked_foci", Order = 1)]
        public List<Focus> FociList { get; set; }

        public PrerequisitesSetModel Model { get; private set; }

        public PrerequisitesSet(Focus focus)
        {
            this.Focus = focus;
            FociList = new List<Focus>();
            Model = new PrerequisitesSetModel(this);
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext c)
        {
            Model = new PrerequisitesSetModel(this);
        }

        public bool isRequired()
        {
            return (FociList.Count > 1);
        }

        public void DeleteSetRelations()
        {
            if (Focus == null)
            {
                //Already deleted, click on more than one line
                return;
            }
            Focus.Prerequisite.Remove(this);
            Focus = null;
            FociList.Clear();
        }

        public List<FocusModel> getFocusModelList()
        {
            List<FocusModel> list = new List<FocusModel>();
            foreach (Focus item in FociList)
            {
                list.Add(item.Model);
            }
            return list;
        }
    }
}
