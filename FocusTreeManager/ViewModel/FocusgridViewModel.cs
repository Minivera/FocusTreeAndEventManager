using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.ViewModel
{
    public class FocusGridViewModel : ViewModelBase
    {
        private ObservableCollection<Focus> fociList;
        private Focus selectedFocus;

        public int RowCount { get; set; }

        public int ColumnCount { get; set; }

        public ObservableCollection<Focus> FociList
        {
            get
            {
                return fociList;
            }
        }

        public Focus SelectedFocus
        {
            get
            {
                return selectedFocus;
            }
            set
            {
                selectedFocus = value;
                RaisePropertyChanged("SelectedFocus");
            }
        }

        public void addFocusToList(Focus FocusToAdd)
        {
            fociList.Add(FocusToAdd);
            RowCount = FocusToAdd.Y >= RowCount ? FocusToAdd.Y + 1 : RowCount;
            ColumnCount = FocusToAdd.X >= ColumnCount ? FocusToAdd.X + 1 : ColumnCount;
        }

        public FocusGridViewModel()
        {
            fociList = new ObservableCollection<Focus>();
            SelectedFocus = null;

            //TEST
            Focus focus = new Focus();
            focus.X = 0;
            focus.Y = 0;
            addFocusToList(focus);
            Focus focus2 = new Focus();
            focus2.X = 1;
            focus2.Y = 2;
            addFocusToList(focus2);
        }
    }
}
