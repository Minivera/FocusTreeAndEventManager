using System;
using System.Runtime.Serialization;

namespace FocusTreeManager.DataContract
{
    [DataContract(Name = "file_info", Namespace = "finfo")]
    public class FileInfo
    {
        [DataMember(Name = "visbleName", Order = 0)]
        public string Filename { get; set; }

        [DataMember(Name = "modified_date", Order = 1)]
        public DateTime LastModifiedDate { get; set; }
    }
}
