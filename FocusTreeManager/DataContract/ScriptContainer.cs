using FocusTreeManager.CodeStructures;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Script))]
    [KnownType(typeof(FileInfo))]
    [DataContract(Name = "script_container")]
    public class ScriptContainer
    {
        [DataMember(Name = "guid_id", Order = 0)]
        public Guid IdentifierID { get; set; }

        [DataMember(Name = "text_id", Order = 1)]
        public string ContainerID { get; set; }

        [DataMember(Name = "script", Order = 2)]
        public Script InternalScript { get; set; }

        [DataMember(Name = "file", Order = 4)]
        public FileInfo FileInfo { get; set; }

        public ScriptContainer()
        {
            IdentifierID = Guid.NewGuid();
        }

        public ScriptContainer(string filename)
        {
            ContainerID = filename;
            InternalScript = new Script();
            IdentifierID = Guid.NewGuid();
        }
    }
}
