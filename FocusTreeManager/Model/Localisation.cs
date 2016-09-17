using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Model
{
    [Serializable]
    public class Localisation : ObservableObject
    {
        [Serializable]
        public class LocaleContent
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private string filename;

        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                Set<string>(() => this.Filename, ref this.filename, value);
            }
        }

        public List<LocaleContent> LocalisationMap { get; set; }

        public Localisation(string filename)
        {
            this.Filename = filename;
            LocalisationMap = new List<LocaleContent>();
        }

        public void addLocalisationPair(string key, string value)
        {
            if (null != LocalisationMap.Where((l) => l.Key == key))
            {
                LocalisationMap.Where((l) => l.Key == key).ToList().ForEach(cc => cc.Value = value);
            }
            else
            {
                LocaleContent newTempo = new LocaleContent();
                newTempo.Key = key;
                newTempo.Value = value;
                LocalisationMap.Add(newTempo);
            }
        }
        
        public string translateKey(string key)
        {
            return LocalisationMap != null ? LocalisationMap.Where((l) => l.Key == key)
                .SingleOrDefault().Value : null;
        }
    }
}
