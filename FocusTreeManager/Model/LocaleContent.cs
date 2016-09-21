using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Model
{
    [Serializable]
    public class LocaleContent : ObservableObject
    {
        private string key;

        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                Set<string>(() => this.Key, ref this.key, value);
            }
        }

        private string value;

        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                Set<string>(() => this.Value, ref this.value, value);
            }
        }
    }
}
