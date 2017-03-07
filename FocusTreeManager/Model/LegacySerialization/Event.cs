using FocusTreeManager.CodeStructures.LegacySerialization;
using FocusTreeManager.ViewModel;
using FocusTreeManager.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ProtoBuf;
using System.Collections.Generic;

namespace FocusTreeManager.Model.LegacySerialization
{
    [ProtoContract]
    public class Event : ObservableObject
    {
        const string IMAGE_PATH = "/FocusTreeManager;component/GFX/Events/";

        public enum EventType
        {
            news_event,
            country_event
        }

        [ProtoMember(1)]
        private string id;

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                Set<string>(() => Id, ref id, value);
            }
        }

        [ProtoMember(2)]
        private EventType type;

        public EventType Type
        {
            get
            {
                return type;
            }
            set
            {
                Set<EventType>(() => Type, ref type, value);
            }
        }

        [ProtoMember(3)]
        private string picture;

        public string Picture
        {
            get
            {
                return picture;
            }
            set
            {
                Set<string>(() => Picture, ref picture, value);
                RaisePropertyChanged(() => ImagePath);
            }
        }

        public string ImagePath
        {
            get
            {
                return IMAGE_PATH + picture + ".png";
            }
        }

        [ProtoMember(4)]
        private bool isMajor;

        public bool IsMajor
        {
            get
            {
                return isMajor;
            }
            set
            {
                Set<bool>(() => IsMajor, ref isMajor, value);
            }
        }

        [ProtoMember(5)]
        private bool isTriggeredOnly;

        public bool IsTriggeredOnly
        {
            get
            {
                return isTriggeredOnly;
            }
            set
            {
                Set<bool>(() => IsTriggeredOnly, ref isTriggeredOnly, value);
            }
        }

        [ProtoMember(6)]
        private bool isHidden;

        public bool IsHidden
        {
            get
            {
                return isHidden;
            }
            set
            {
                Set<bool>(() => IsHidden, ref isHidden, value);
            }
        }
        
        [ProtoMember(7)]
        private bool isFiredOnce;

        public bool IsFiredOnce
        {
            get
            {
                return isFiredOnce;
            }
            set
            {
                Set<bool>(() => IsFiredOnce, ref isFiredOnce, value);
            }
        }

        [ProtoMember(8)]
        private Script internalScript;

        public Script InternalScript
        {
            get
            {
                return internalScript;
            }
            set
            {
                Set<Script>(() => InternalScript, ref internalScript, value);
            }
        }

        [ProtoMember(9, AsReference = true)]
        public List<EventDescription> Descriptions { get; set;}

        [ProtoMember(10, AsReference = true)]
        public List<EventOption> Options { get; set; }

        public Event()
        {
            Descriptions = new List<EventDescription>();
            Options = new List<EventOption>();
        }
    }
}
