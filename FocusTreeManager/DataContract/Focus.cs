using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using FocusTreeManager.CodeStructures;
using FocusTreeManager.Model;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Script))]
    [KnownType(typeof(PrerequisitesSet))]
    [KnownType(typeof(MutuallyExclusiveSet))]
    [DataContract(Name = "focus", Namespace = "focusesNs")]
    public class Focus
    {
        const string IMAGE_PATH = "pack://application:,,,/FocusTreeManager;component/GFX/Focus/";

        [DataMember(Name = "image", Order = 0)]
        public string Icon { get; set; }

        public string Image
        {
            get
            {
                return IMAGE_PATH + Icon + ".png";
            }
        }

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

        public FocusModel Model { get; private set; }

        public Focus()
        {
            MutualyExclusive = new List<MutuallyExclusiveSet>();
            Prerequisite = new List<PrerequisitesSet>();
            Model = new FocusModel(this);
        }

        public Focus(Model.LegacySerialization.Focus legacyItem)
        {
            Icon = legacyItem.Icon;
            UniqueName = legacyItem.UniqueName;
            X = legacyItem.X;
            Y = legacyItem.Y;
            Cost = legacyItem.Cost;
            InternalScript = new Script();
            InternalScript.Analyse(legacyItem.InternalScript.Parse());
            MutualyExclusive = new List<MutuallyExclusiveSet>();
            Prerequisite = new List<PrerequisitesSet>();
            Model = new FocusModel(this);
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext c)
        {
            Model = new FocusModel(this);
        }

        public List<PrerequisitesSetModel> getPrerequisitesModels()
        {
            List<PrerequisitesSetModel> list = new List<PrerequisitesSetModel>();
            foreach (PrerequisitesSet item in Prerequisite)
            {
                list.Add(item.Model);
            }
            return list;
        }

        public List<MutuallyExclusiveSetModel> getMutuallyExclusivesModels()
        {
            List<MutuallyExclusiveSetModel> list = new List<MutuallyExclusiveSetModel>();
            foreach (MutuallyExclusiveSet item in MutualyExclusive)
            {
                list.Add(item.Model);
            }
            return list;
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
                item.RepairMutuallyExclusive(fociList.SingleOrDefault(
                                                (i) => i.X == item.X && i.Y == item.Y).MutualyExclusive,
                                             list);
                item.RepairPrerequisites(fociList.SingleOrDefault(
                                            (i) => i.X == item.X && i.Y == item.Y).Prerequisite,
                                         list);
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
                this.Prerequisite.Add(set);
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
                this.MutualyExclusive.Add(set);
            }
        }

        public void setDefaults(int FocusNumber)
        {
            InternalScript = new Script();
            Icon = "goal_unknown";
            UniqueName = "newfocus_" + FocusNumber;
            X = 0;
            Y = 0;
        }
    }
}
