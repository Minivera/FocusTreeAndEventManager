using FocusTreeManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FocusTreeManager.Model.TabModels;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Focus))]
    [KnownType(typeof(FileInfo))]
    [DataContract(Name = "foci_container", Namespace = "focusesNs")]
    public class FociGridContainer
    {
        [DataMember(Name = "guid_id", Order = 0)]
        public Guid IdentifierID { get; private set; }

        [DataMember(Name = "text_id", Order = 1)]
        public string ContainerID { get; set; }

        [DataMember(Name = "tag", Order = 2)]
        public string TAG { get; set; }

        [DataMember(Name = "foci", Order = 3)]
        public List<Focus> FociList { get; set; }

        [DataMember(Name = "mods", Order = 4)]
        public string AdditionnalMods { get; set; }

        [DataMember(Name = "file", Order = 5)]
        public FileInfo FileInfo { get; set; }

        public FociGridContainer()
        {
            IdentifierID = Guid.NewGuid();
            FociList = new List<Focus>();
        }

        public FociGridContainer(string filename)
        {
            ContainerID = filename;
            FociList = new List<Focus>();
            IdentifierID = Guid.NewGuid();
        }

        public FociGridContainer(FocusGridModel item)
        {
            IdentifierID = item.UniqueID;
            ContainerID = item.VisibleName;
            TAG = item.TAG;
            AdditionnalMods = item.AdditionnalMods;
            FileInfo = item.FileInfo;
            FociList = new List<Focus>();
            foreach (FocusModel model in item.FociList)
            {
                FociList.Add(new Focus()
                {
                    UniqueName = model.UniqueName,
                    Image = model.Image,
                    X = model.X,
                    Y = model.Y,
                    Cost = model.Cost,
                    InternalScript = model.InternalScript
                });
            }
            //Repair sets
            foreach (Focus focus in FociList)
            {
                FocusModel associatedModel = 
                    item.FociList.FirstOrDefault(f => f.UniqueName == focus.UniqueName);
                if (associatedModel == null) continue;
                {
                    foreach (PrerequisitesSetModel set in associatedModel.Prerequisite)
                    {
                        PrerequisitesSet newset = new PrerequisitesSet(focus);
                        foreach (FocusModel model in set.FociList)
                        {
                            newset.FociList.Add(FociList.FirstOrDefault(f => f.UniqueName == model.UniqueName));
                        }
                        focus.Prerequisite.Add(newset);
                    }
                    foreach (MutuallyExclusiveSetModel set in associatedModel.MutualyExclusive)
                    {
                        MutuallyExclusiveSet newset = new MutuallyExclusiveSet(
                            FociList.FirstOrDefault(f => f.UniqueName == set.Focus1.UniqueName),
                            FociList.FirstOrDefault(f => f.UniqueName == set.Focus2.UniqueName));
                        focus.MutualyExclusive.Add(newset);
                    }
                }
            }
        }
    }
}
