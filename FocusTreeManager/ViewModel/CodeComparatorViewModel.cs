using DiffPlex.DiffBuilder.Model;
using GalaSoft.MvvmLight;

namespace FocusTreeManager.ViewModel
{
    public class CodeComparatorViewModel : ViewModelBase
    {
        private SideBySideDiffModel diffModel = new SideBySideDiffModel();

        public SideBySideDiffModel DiffModel
        {
            get
            {
                return diffModel;
            }
            set
            {
                if (value == diffModel)
                {
                    return;
                }
                diffModel = value;
                RaisePropertyChanged(() => DiffModel);
                RaisePropertyChanged(() => OldFileDiff);
                RaisePropertyChanged(() => NewFileDiff);
            }
        }

        public DiffPaneModel OldFileDiff
        {
            get
            {
                return diffModel.OldText;
            }
        }

        public DiffPaneModel NewFileDiff
        {
            get
            {
                return diffModel.NewText;
            }
        }

        /// <summary>
        /// Initializes a new instance of the CodeComparatorViewModel class.
        /// </summary>
        public CodeComparatorViewModel()
        {
        }
    }
}