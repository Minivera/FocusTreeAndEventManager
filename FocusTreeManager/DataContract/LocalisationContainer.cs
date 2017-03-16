using FocusTreeManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FocusTreeManager.Model.TabModels;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(LocaleContent))]
    [KnownType(typeof(FileInfo))]
    [DataContract(Name = "locale_container")]
    public class LocalisationContainer
    {
        [DataMember(Name = "guid_id", Order = 1)]
        public Guid IdentifierID { get; private set; }

        [DataMember(Name = "text_id", Order = 2)]
        public string ContainerID { get; set; }

        [DataMember(Name = "name", Order = 3)]
        public string LanguageName { get; set; }

        [DataMember(Name = "locales", Order = 4)]
        public List<LocaleContent> LocalisationMap { get; set; }

        [DataMember(Name = "file", Order = 5)]
        public FileInfo FileInfo { get; set; }

        public LocalisationContainer()
        {
            IdentifierID = Guid.NewGuid();
            LocalisationMap = new List<LocaleContent>();
        }

        public LocalisationContainer(string filename)
        {
            ContainerID = filename;
            LocalisationMap = new List<LocaleContent>();
            IdentifierID = Guid.NewGuid();
        }

        public LocalisationContainer(LocalisationModel item)
        {
            IdentifierID = item.UniqueID;
            ContainerID = item.VisibleName;
            LanguageName = item.LanguageName;
            FileInfo = item.FileInfo;
            LocalisationMap = new List<LocaleContent>();
            foreach (LocaleModel model in item.LocalisationMap)
            {
                LocalisationMap.Add(new LocaleContent()
                {
                    Key = model.Key,
                    Value = model.Value
                });
            }
        }
    }
}
