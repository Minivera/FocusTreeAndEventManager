using GalaSoft.MvvmLight;
using ProtoBuf;

namespace FocusTreeManager.Model.LegacySerialization
{
    [ProtoContract]
    public class LocaleContent : ObservableObject
    {
        [ProtoMember(1)]
        private string key;

        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                Set<string>(() => Key, ref key, value);
            }
        }

        [ProtoMember(2)]
        private string value;

        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                Set<string>(() => Value, ref this.value, value);
            }
        }
    }
}
