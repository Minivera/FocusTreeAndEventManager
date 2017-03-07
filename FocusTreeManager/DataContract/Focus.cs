using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using FocusTreeManager.CodeStructures;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Script))]
    [KnownType(typeof(PrerequisitesSet))]
    [KnownType(typeof(MutuallyExclusiveSet))]
    [DataContract(Name = "focus", Namespace = "focusesNs")]
    public class Focus
    {

        [DataMember(Name = "image", Order = 0)]
        public string Image { get; set; }

        [DataMember(Name = "name", Order = 1)]
        public string UniqueName { get; set; }

        [DataMember(Name = "x", Order = 2)]
        public int X { get; set; }

        [DataMember(Name = "y", Order = 3)]
        public int Y { get; set; }

        [DataMember(Name = "cost", Order = 4)]
        public double Cost { get; set; }

        [DataMember(Name = "script", Order = 5)]
        public Script InternalScript { get; set; }

        [DataMember(Name = "prerequisites", Order = 6)]
        public List<PrerequisitesSet> Prerequisite { get; set; }

        [DataMember(Name = "mutually_exclusives", Order = 7)]
        public List<MutuallyExclusiveSet> MutualyExclusive { get; set; }

        public Focus()
        {
            MutualyExclusive = new List<MutuallyExclusiveSet>();
            Prerequisite = new List<PrerequisitesSet>();
        }

        public Focus(Model.LegacySerialization.Focus legacyItem)
        {
            Image = legacyItem.Icon;
            UniqueName = legacyItem.UniqueName;
            X = legacyItem.X;
            Y = legacyItem.Y;
            Cost = legacyItem.Cost;
            InternalScript = new Script();
            InternalScript.Analyse(legacyItem.InternalScript.Parse());
            MutualyExclusive = new List<MutuallyExclusiveSet>();
            Prerequisite = new List<PrerequisitesSet>();
        }

        internal static List<Focus> PopulateFromLegacy(
            List<Model.LegacySerialization.Focus> fociList)
        {
            List<Focus> list = new List<Focus>();
            foreach (Model.LegacySerialization.Focus legacyItem in fociList)
            {
                list.Add(new Focus(legacyItem));
            }
            foreach (Focus item in list)
            {
                Model.LegacySerialization.Focus focus = fociList.SingleOrDefault(
                    i => i.X == item.X && i.Y == item.Y);
                if (focus != null)
                {
                    item.RepairMutuallyExclusive(focus.MutualyExclusive,
                        list);
                    item.RepairPrerequisites(focus.Prerequisite,
                        list);
                }
            }
            return list;
        }

        private void RepairPrerequisites(List<Model.LegacySerialization.PrerequisitesSet> prerequisites,
                                         List<Focus> fociList)
        {
            foreach (Model.LegacySerialization.PrerequisitesSet legacyItem in prerequisites)
            {
                PrerequisitesSet set = new PrerequisitesSet(this);
                foreach (Model.LegacySerialization.Focus legacyFocus in legacyItem.FociList)
                {
                    set.FociList.Add(fociList.SingleOrDefault((i) => i.X == legacyFocus.X &&
                                                               i.Y == legacyFocus.Y));
                }
                Prerequisite.Add(set);
            }
        }

        private void RepairMutuallyExclusive(List<Model.LegacySerialization.MutuallyExclusiveSet> mutualyExclusives,
                                             List<Focus> fociList)
        {
            foreach (Model.LegacySerialization.MutuallyExclusiveSet legacyItem in mutualyExclusives)
            {
                MutuallyExclusiveSet set = new MutuallyExclusiveSet(fociList.SingleOrDefault((i) => 
                                                                        i.X == legacyItem.Focus1.X &&
                                                                        i.Y == legacyItem.Focus1.Y),
                                                                    fociList.SingleOrDefault((i) =>
                                                                        i.X == legacyItem.Focus2.X &&
                                                                        i.Y == legacyItem.Focus2.Y));
                MutualyExclusive.Add(set);
            }
        }
    }
}
