using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ProtoBuf;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using FocusTreeManager.CodeStructures.LegacySerialization;

namespace FocusTreeManager.Model.LegacySerialization
{
    [ProtoContract(AsReferenceDefault = true)]
    public class Focus : ObservableObject
    {
        const string IMAGE_PATH = "pack://application:,,,/FocusTreeManager;component/GFX/Focus/";

        [ProtoMember(1)]
        private string image;

        public string Image
        {
            get
            {
                return IMAGE_PATH + image + ".png";
            }
            set
            {
                Set<string>(() => this.Image, ref this.image, value);
            }
        }

        public string Icon
        {
            get
            {
                return image;
            }
        }

        [ProtoMember(2)]
        private string uniquename;

        public string UniqueName
        { 
            get
            {
                if (uniquename == null)
                {
                    return "unknown";
                }
                return uniquename;
            }
            set
            {
                Set<string>(() => this.UniqueName, ref this.uniquename, value);
            }
        }

        [ProtoMember(3)]
        private int x;

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                Set<int>(() => this.X, ref this.x, value);
            }
        }

        [ProtoMember(4)]
        private int y;

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                Set<int>(() => this.Y, ref this.y, value);
            }
        }

        [ProtoMember(7)]
        private double cost;

        public double Cost
        {
            get
            {
                return cost;
            }
            set
            {
                Set<double>(() => this.Cost, ref this.cost, value);
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
                Set<Script>(() => this.InternalScript, ref this.internalScript, value);
            }
        }

        [ProtoMember(5, AsReference = true)]
        public List<PrerequisitesSet> Prerequisite { get; set; }

        [ProtoMember(6, AsReference = true)]
        public List<MutuallyExclusiveSet> MutualyExclusive { get; set; }

        public Focus()
        {
            MutualyExclusive = new List<MutuallyExclusiveSet>();
            Prerequisite = new List<PrerequisitesSet>();
        }
    }
}
