using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FocusTreeManager.ViewModel
{
    public class LocalizatorViewModel : ViewModelBase
    {
        private LocaleModel locale;

        public LocaleModel Locale
        {
            get
            {
                return locale;
            }
            set
            {
                if (value == locale)
                {
                    return;
                }
                locale = value;
                RaisePropertyChanged(() => Locale);
            }
        }

        public RelayCommand OkCommand { get; set; }

        public delegate void AddOrUpdate();

        public AddOrUpdate AddOrUpdateCommand;

        public LocalizatorViewModel()
        {
            OkCommand = new RelayCommand(Accept);
        }

        public void Accept()
        {
            AddOrUpdateCommand?.Invoke();
            Messenger.Default.Send(new NotificationMessage(this, "CloseLocalizator"));
        }
    }
}