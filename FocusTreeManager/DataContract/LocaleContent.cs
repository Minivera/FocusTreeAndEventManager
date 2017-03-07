using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace FocusTreeManager.DataContract
{
    [DataContract(Name = "locale")]
    public class LocaleContent
    {
        [DataMember(Name = "key", Order = 0)]
        public string Key { get; set; }

        [DataMember(Name = "value", Order = 1)]
        public string Value { get; set; }

        public LocaleContent() { }

        public LocaleContent(Model.LegacySerialization.LocaleContent legacyItem)
        {
            Key = legacyItem.Key;
            Value = legacyItem.Value;
        }

        internal static List<LocaleContent> PopulateFromLegacy(
            List<Model.LegacySerialization.LocaleContent> localisationMap)
        {
            return localisationMap.Select(legacyItem => new LocaleContent(legacyItem)).ToList();
        }
    }
}
