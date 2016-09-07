using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Model
{
    public class Focus : ObservableObject
    {
        private string image;

        public string Image
        {
            get
            {
                return image;
            }
            set
            {
                Set<string>(() => this.Image, ref this.image, value);
            }
        }

        private string uniquename;

        public string UniqueName
        { 
            get
            {
                return uniquename;
            }
            set
            {
                Set<string>(() => this.UniqueName, ref this.uniquename, value);
            }
        }

        private string visiblename;

        public string VisibleName
        {
            get
            {
                return visiblename;
            }
            set
            {
                Set<string>(() => this.VisibleName, ref this.visiblename, value);
            }
        }

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

        public List<Focus> Parents { get; set; }

        public List<Focus> Childrens { get; set; }

        public List<Focus> RequiredParents { get; set; }

        public List<Focus> MutualyExclusiveSiblings { get; set; }

        public Focus()
        {

        }
    }
}
