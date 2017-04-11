/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:FocusTreeManager"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System.Windows;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ProjectViewViewModel>();
            SimpleIoc.Default.Register<ManageFocusViewModel>();
            SimpleIoc.Default.Register<ChangeImageViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<FileManagerViewModel>();
            SimpleIoc.Default.Register<ProjectEditorViewModel>();
            SimpleIoc.Default.Register<LocalizatorViewModel>();
            SimpleIoc.Default.Register<ScripterViewModel>();
            SimpleIoc.Default.Register<ScripterControlsViewModel>();
            SimpleIoc.Default.Register<CodeComparatorViewModel>();
            SimpleIoc.Default.Register<ErrorDawgViewModel>();
            SimpleIoc.Default.Register<StatusBarViewModel>();
            SimpleIoc.Default.Register<TutorialViewModel>();
            Messenger.Default.Register<NotificationMessage>(this, NotifyUserMethod);
        }

        private void NotifyUserMethod(NotificationMessage obj)
        {
            //Notify
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ManageFocusViewModel EditFocus
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ManageFocusViewModel>();
            }
        }

        public ChangeImageViewModel ChangeImage
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ChangeImageViewModel>();
            }
        }

        public ProjectViewViewModel ProjectView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ProjectViewViewModel>();
            }
        }

        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        public FileManagerViewModel FileManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<FileManagerViewModel>();
            }
        }

        public ProjectEditorViewModel ProjectEditor
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ProjectEditorViewModel>();
            }
        }

        public LocalizatorViewModel Localizator
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LocalizatorViewModel>();
            }
        }

        public ScripterViewModel Scripter
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ScripterViewModel>();
            }
        }

        public ScripterControlsViewModel ScripterControls
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ScripterControlsViewModel>();
            }
        }

        public CodeComparatorViewModel CodeComparator
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CodeComparatorViewModel>();
            }
        }

        public ErrorDawgViewModel ErrorDawg
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ErrorDawgViewModel>();
            }
        }

        public StatusBarViewModel StatusBar
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StatusBarViewModel>();
            }
        }

        public TutorialViewModel Tutorial
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TutorialViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}